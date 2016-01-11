using System;
using System.Collections.Generic;
using System.Reflection;

namespace IniDotNet
{
    internal interface ISectionBindingLocator
    {
        IReadOnlyList<SectionBinding> Locate(Type modelType);
    }

    internal class AttributeBasedSectionBindingLocator : ISectionBindingLocator
    {
        private const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public IReadOnlyList<SectionBinding> Locate(Type modelType)
        {
            var bindings = new List<SectionBinding>();

            foreach (var prop in modelType.GetProperties(flags))
            {
                IniSectionAttribute attr = prop.GetCustomAttribute<IniSectionAttribute>();

                string name = attr?.Name;
                bindings.Add(new SectionBinding(name ?? prop.Name, prop));
            }

            return bindings.AsReadOnly();
        }
    }
}
