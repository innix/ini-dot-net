using System;
using System.Collections.Generic;
using System.Reflection;

namespace IniDotNet.Binders
{
    public sealed class AttributeBasedSectionBinder : ISectionBinder
    {
        private const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public IReadOnlyList<SectionBinding> Bind(Type modelType)
        {
            var bindings = new List<SectionBinding>();

            foreach (var prop in modelType.GetProperties(flags))
            {
                IniSectionAttribute attr = prop.GetCustomAttribute<IniSectionAttribute>();

                bindings.Add(new SectionBinding(attr?.Name ?? prop.Name, prop));
            }

            return bindings.AsReadOnly();
        }
    }
}
