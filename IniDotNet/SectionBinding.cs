using System;
using System.Reflection;

namespace IniDotNet
{
    /// <summary>
    /// Represents a binding between a .INI section and a property.
    /// </summary>
    public sealed class SectionBinding
    {
        public SectionBinding(string name, PropertyInfo property)
        {
            Name = name;
            Property = property;
        }

        public string Name { get; }
        public PropertyInfo Property { get; }

        public Type Type => Property.PropertyType;
    }
}
