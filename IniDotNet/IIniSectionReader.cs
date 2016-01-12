using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace IniDotNet
{
    public interface IIniSectionReader
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
                if (idx == 0)
                {
                    throw new IniException($"Empty key on line {lineNumber}: '{line}'");
                }

                // If equals sign is padded with a space on either side (e.g. "key = value"),
                // then make sure we ignore it when extracting key and value.
                int offset = 0;
                if (char.IsWhiteSpace(line[idx - 1]) && char.IsWhiteSpace(line.ElementAtOrDefault(idx + 1)))
                {
                    offset = 1;
                }

                string key = line.Substring(0, idx - offset);
                string value = line.Substring(idx + 1 + offset);

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