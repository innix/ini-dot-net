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
            // TODO: Allow custom type conversion.
            // TODO: Allow IniListProperty to deserialize to any collection type, not just List<T>.
            // TODO: Allow parsing of values where the equals sign is padded with a space (e.g. "Prop1 = Value1").

            string contents = File.ReadAllText(@"C:\Users\Phil\Desktop\config.ini");

            Config cfg = IniDeserializer.Deserialize<Config>(contents);

            Debug.Assert(cfg != null);
            Debug.Assert(cfg.General != null);
            Debug.Assert(cfg.Spe != null);

            Debug.Assert(cfg.General.NodeName == "foo");
            Debug.Assert(cfg.General.NodeIndex == 12);

            Debug.Assert(cfg.Spe.EndPoints != null);
            Debug.Assert(cfg.Spe.EndPoints.Count == 2);
            Debug.Assert(cfg.Spe.EndPoints.First() == "127.0.0.1:1234");

            Debug.Assert(cfg.Spe.Ids != null);
            Debug.Assert(cfg.Spe.Ids.Count == 3);
            Debug.Assert(cfg.Spe.Ids.Skip(1).First() == 456);
        }
    }

    class Config
    {
        [IniSection("General")]
        public GeneralConfig General { get; private set; }

        [IniSection("SignalProcessing")]
        public SpeConfig Spe { get; private set; }
    }

    class GeneralConfig
    {
        public string NodeName { get; private set; }
        public int NodeIndex { get; private set; }
    }

    class SpeConfig
    {
        [IniListProperty(",")]
        public IReadOnlyList<string> EndPoints { get; private set; }

        [IniListProperty(",")]
        public IReadOnlyList<int> Ids { get; private set; }
    }
}
