using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IniDotNet.Binders
{
    public sealed class AttributeBasedSectionBinder : ISectionBinder
    {
        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public IList<SectionBinding> Bind(Type modelType, IList<IniSection> sections)
        {
            var properties = modelType.GetProperties(Flags);
            var bindings = sections
                .Select(kvp => FindBindingBySectionName(properties, kvp.Name))
                .Where(binding => binding != null)
                .ToList();

            return bindings.AsReadOnly();
        }

        private SectionBinding FindBindingBySectionName(PropertyInfo[] properties, string name)
        {
            foreach (PropertyInfo prop in properties)
            {
                IniSectionAttribute attr = prop.GetCustomAttribute<IniSectionAttribute>();
                if (attr != null && attr.Name == name)
                {
                    return new SectionBinding(attr.Name, prop);
                }
            }

            return properties
                .Where(prop => prop.Name == name)
                .Select(prop => new SectionBinding(prop.Name, prop))
                .FirstOrDefault();
        }
    }
}
