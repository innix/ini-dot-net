using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IniDotNet.Examples
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // TODO: Inject properties through constructor if available.

            string contents = @"
[Generalz]
Key = qwerty
EnableThing = True

[ABC]
EndPoints = 62.252.201.71:4000,127.0.0.1:4001
ConnectAttemptsPerEndPoint = 5
DelayBetweenConnects = 10
";

            Config cfg = IniConvert.DeserializeObject<Config>(contents);

            Debug.Assert(cfg != null);
            Debug.Assert(cfg.General != null);

            Debug.Assert(cfg.General.EnableThing);
            Debug.Assert(cfg.General.Key == "qwerty");

            foreach (IPEndPoint endPoint in cfg.Abc.EndPoints)
            {
                Console.WriteLine(endPoint);
            }

            Console.WriteLine(cfg.Abc.DelayBetweenConnects);
        }
    }

    class Config
    {
        [IniSection("Generalz")]
        public GeneralConfig General { get; private set; }

        [IniSection("ABC")]
        public AbcConfig Abc { get; private set; }
    }

    public class GeneralConfig
    {
        public string Key { get; set; }
        public bool EnableThing { get; private set; }
    }


    public class AbcConfig
    {
        [IniListProperty(",")]
        public IReadOnlyList<IPEndPoint> EndPoints { get; private set; }

        [IniConverter(typeof(SecondsToTimeSpanConverter))]
        public TimeSpan DelayBetweenConnects { get; private set; }

        public int ConnectAttemptsPerEndPoint { get; private set; }
    }

    class SecondsToTimeSpanConverter : IniConverter
    {
        public override object ConvertFromString(string stringValue)
        {
            return TimeSpan.FromSeconds(int.Parse(stringValue));
        }
    }
}
