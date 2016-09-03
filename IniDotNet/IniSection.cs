using System.Collections.Generic;

namespace IniDotNet
{
    public sealed class IniSection
    {
        public IniSection(string name, IDictionary<string, string> contents)
        {
            Name = name;
            Contents = contents;
        }

        public string Name { get; }
        public IDictionary<string, string> Contents { get; }

        public bool IsTopLevel => Name == null;

        internal string InternalName => Name == "" ? "<top level>" : Name;
    }
}
