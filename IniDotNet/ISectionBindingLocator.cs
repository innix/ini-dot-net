using System;
using System.Collections.Generic;
using System.Reflection;

namespace IniDotNet
{
    /// <summary>
    /// A section binding locator creates a mapping to determine which .INI section
    /// is bound to which property in the model class.
    /// </summary>
    public interface ISectionBindingLocator
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

                bindings.Add(new SectionBinding(attr?.Name ?? prop.Name, prop));
            }

            return bindings.AsReadOnly();
        }
    }
}
