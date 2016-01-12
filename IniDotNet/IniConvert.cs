using System;
using System.Collections.Generic;
using IniDotNet.Converters;

namespace IniDotNet
{
    public static class IniConvert
    {
        public static T DeserializeObject<T>(string iniFileContents)
        {
            var deserializer = new IniDeserializer();

            if (typeof(T).IsAssignableFrom(typeof(IDictionary<string, IDictionary<string, string>>)))
            {
                return (T) deserializer.Deserialize(iniFileContents);
            }

            return deserializer.Deserialize<T>(iniFileContents);
        }

        public static T DeserializeObject<T>(string iniFileContents, string iniSection)
        {
            var deserializer = new IniDeserializer();

            if (typeof(T).IsAssignableFrom(typeof(IDictionary<string, string>)))
            {
                return (T)deserializer.DeserializeSection(iniFileContents, iniSection);
            }

            throw new NotSupportedException();
            //return deserializer.DeserializeSection<T>(iniFileContents, iniSection);
        }
    }
}
