using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace IniDotNet
{
    internal interface IIniSectionReader
    {
        IEnumerable<IniSection> Read(TextReader reader);
    }

    internal class StandardIniSectionReader : IIniSectionReader
    {
        private readonly bool throwOnDuplicateKeys;

        public StandardIniSectionReader() : this(true)
        {
        }

        public StandardIniSectionReader(bool throwOnDuplicateKeys)
        {
            this.throwOnDuplicateKeys = throwOnDuplicateKeys;
        }

        public IEnumerable<IniSection> Read(TextReader reader)
        {
            int lineNumber = 0;
            string line;

            string name = null;
            var contents = new Dictionary<string, string>();

            while ((line = reader.ReadLine()) != null)
            {
                lineNumber++;

                // Skip whitespace and comments.
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (line.StartsWith(";") || line.StartsWith("#")) continue;

                // Section detected.
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    if (name != null)
                    {
                        yield return new IniSection(name, contents);
                    }

                    name = line.Substring(1, line.Length - 2);
                    contents = new Dictionary<string, string>();
                    continue;
                }

                int idx = line.IndexOf('=');
                if (idx == -1)
                {
                    throw new IniException($"Unexpected content on line {lineNumber}: '{line}'");
                }

                string key = line.Substring(0, idx);
                string value = line.Substring(idx + 1);

                if (contents.ContainsKey(key))
                {
                    if (throwOnDuplicateKeys)
                    {
                        throw new IniException($"Duplicate key '{key}' in section '{name}'.");
                    }

                    Debug.WriteLine($"Ignoring duplicate key '{key}' in section '{name}'.");
                    continue;
                }

                contents.Add(key, value);
            }

            if (name != null)
            {
                yield return new IniSection(name, contents);
            }
        }
    }
}