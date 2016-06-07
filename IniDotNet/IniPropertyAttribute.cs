using System;

namespace IniDotNet
{
    public class IniPropertyAttribute : Attribute
    {
        public IniPropertyAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
