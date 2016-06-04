# Ini.NET
[![Build status](https://ci.appveyor.com/api/projects/status/32r7s2skrgm9ubva?svg=true)](https://ci.appveyor.com/project/innix/ini-dot-net/branch/master)

Ini.NET is a client library that provides an easy way to deserialize .ini files into .NET objects.

## Example
```csharp
    public class Program
    {
        public static void Main(string[] args)
        {
            string contents = @"
[General]
Key = qwerty
EnableThing = True

[ABC]
EndPoints = 62.252.201.71:4000,127.0.0.1:4001
ConnectAttemptsPerEndPoint = 5
";

            Config cfg = IniConvert.DeserializeObject<Config>(contents);
            
            
            Console.WriteLine(cfg.General.Key); // => "qwerty"
            if (cfg.General.EnableThing)
            {
                Console.WriteLine("Thing enabled");
            }

            foreach (string endPoint in cfg.Other.EndPoints)
            {
                Console.WriteLine(endPoint);
            }

            Console.WriteLine(cfg.Other.ConnectAttemptsPerEndPoint);
        }
    }

    class Config
    {
        public GeneralConfig General { get; private set; }

        [IniSection("ABC")] // only required if property name doesn't match section name.
        public AbcConfig Other { get; private set; }
    }

    public class GeneralConfig
    {
        public string Key { get; private set; }
        public bool EnableThing { get; private set; }
    }

    public class AbcConfig
    {
        [IniListProperty(",")] // split by comma to make a list.
        public IReadOnlyList<string> EndPoints { get; private set; }

        public int ConnectAttemptsPerEndPoint { get; private set; }
    }
```
