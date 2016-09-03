# Ini.NET
[![Build status](https://ci.appveyor.com/api/projects/status/jpxdp8i8cwb957dj?svg=true)](https://ci.appveyor.com/project/innix/ini-dot-net)
[![NuGet](https://img.shields.io/nuget/v/IniDotNet.svg)](https://www.nuget.org/packages/IniDotNet/)

Ini.NET is a client library that provides an easy way to deserialize .ini files into .NET objects.

## Install
Install via NuGet: [https://www.nuget.org/packages/IniDotNet/](https://www.nuget.org/packages/IniDotNet/)

## Getting started
Ini.NET deserializes .ini files into plain old .NET objects using the `IniConvert.DeserializeObject<T>(string)` method. Here is a trivial example:

```csharp
    public class Program
    {
        public static void Main(string[] args)
        {
            string contents = @"
[General]
Key = qwerty
EnableThing = True
";

            Config cfg = IniConvert.DeserializeObject<Config>(contents);
            Console.WriteLine(cfg.General.Key);
        }
    }

    public class Config
    {
        public GeneralConfig General { get; private set; }
    }

    public class GeneralConfig
    {
        public string Key { get; private set; }
        public bool EnableThing { get; private set; }
    }
```

### Deserialize .ini section to a different property name
Use the `[IniSection]` attribute. If your .ini section is called `[FooBar]` but want it mapped to a .NET property called `General`:
```csharp
    public class Config
    {
        [IniSection("FooBar")]
        public GeneralConfig General { get; private set; }
    }

    public class GeneralConfig
    {
        public string Key { get; private set; }
        public bool EnableThing { get; private set; }
    }
```

### Deserialize .ini field to a different property name
Use the `[IniProperty]` attribute. If you have a field in your `[General]` section called `SpecialKey` but want it mapped to the `Key` property:
```csharp
    public class Config
    {
        public GeneralConfig General { get; private set; }
    }

    public class GeneralConfig
    {
        [IniProperty("SpecialKey")]
        public string Key { get; private set; }
        public bool EnableThing { get; private set; }
    }
```

### Deserialize .ini value to a .NET list
Use the `[IniListProperty]`. Specify the delimeter used to separate the values:
```csharp
    public class Program
    {
        public static void Main(string[] args)
        {
            string contents = @"
[General]
MenuItems = Milkshake,Fries,Burger,Ice cream,Cake
";

            Config cfg = IniConvert.DeserializeObject<Config>(contents);
            foreach (string menuItem in cfg.General.MenuItems)
            {
                Console.WriteLine(menuItem);
            }
        }
    }

    public class Config
    {
        public GeneralConfig General { get; private set; }
    }

    public class GeneralConfig
    {
        [IniListProperty(",")]
        public List<string> MenuItems { get; private set; }
    }
```

### Custom deserialization
You can write your own `IniConverter` for types not supported out-the-box with Ini.NET.
```csharp
    public class Program
    {
        public static void Main(string[] args)
        {
            string contents = @"
[General]
# seconds
Delay = 30
";

            Config cfg = IniConvert.DeserializeObject<Config>(contents);
            Console.WriteLine(cfg.General.Delay);
        }
    }

    public class Config
    {
        public GeneralConfig General { get; private set; }
    }

    public class GeneralConfig
    {
        [IniConverter(typeof(SecondsToTimeSpanConverter))]
        public TimeSpan Delay { get; private set; }
    }

    public class SecondsToTimeSpanConverter : IniConverter
    {
        public override object ConvertFromString(string stringValue)
        {
            return TimeSpan.FromSeconds(int.Parse(stringValue));
        }
    }
```

### I am getting an exception about "no parameterless constructor" / "missing set method"!
Ini.NET currently requires classes to have a parameterless constructor and properties with a `set`/`private set` (i.e. C# 6's getter-only properties are not supported). There is an open issue (#1) about this.
