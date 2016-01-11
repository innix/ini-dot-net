using System;

namespace IniDotNet
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IniSectionAttribute : Attribute
    {
        public IniSectionAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
