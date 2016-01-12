using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace IniDotNet
{
    public sealed class IniDeserializer
    {
        private const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        /// <summary>
        /// Reads the .INI file content and parses it into .NET objects.
        /// </summary>
        public IIniSectionReader Reader { get; set; } = new StandardIniSectionReader();

        /// <summary>
        /// Generates a set of bindings to determine what fields/properties the .INI sections will be deserialized into.
        /// </summary>
        public ISectionBindingLocator SectionBinder { get; set; } = new AttributeBasedSectionBindingLocator();

        /// <summary>
        /// Converts the string values from the .INI file into .NET types.
        /// </summary>
        public IConverter Converter { get; set; } = new StandardConverter();

        #region POCO deserialization

        public T Deserialize<T>(string iniFileContents)
        {
            object configModel = Activator.CreateInstance<T>();
            Type configModelType = typeof(T);


            // Locates the properties within the model type and maps them to an .INI section.
            IReadOnlyList<SectionBinding> bindings = SectionBinder.Locate(configModelType);

            using (var reader = new StringReader(iniFileContents))
            {
                foreach (IniSection section in Reader.Read(reader))
                {
                    // Finds the binding for the current INI section.
                    SectionBinding binding = bindings
                        .SingleOrDefault(b => section.Name.Equals(b.Name, StringComparison.OrdinalIgnoreCase));

                    if (binding == null)
                    {
                        Debug.WriteLine($"Section '{section.Name}' is not bound to a type, skipping...");
                        continue;
                    }


                    PropertyInfo configModelPropertyInfo = binding.Property;

                    // Deserialize section and set the property in our model object.
                    object configSectionModel = DeserializeSection(section, binding);
                    configModelPropertyInfo.SetValue(configModel, configSectionModel);
                }
            }

            return (T) configModel;
        }

        private object DeserializeSection(IniSection section, SectionBinding binding)
        {
            object configSectionModel = Activator.CreateInstance(binding.Type);

            foreach (var kvp in section.Contents)
            {
                PropertyInfo destinationProperty = binding.Type.GetProperty(kvp.Key, flags | BindingFlags.IgnoreCase);
                if (destinationProperty == null)
                {
                    Debug.WriteLine($"Type '{binding.Type.FullName}' has no suitable property" +
                                    $"for '{section.Name}'.'{kvp.Key}'");

                    continue;
                }

                IniListPropertyAttribute listAttr = destinationProperty.GetCustomAttribute<IniListPropertyAttribute>();

                // Find correct converter.
                IConverter converter = Converter;
                if (listAttr != null)
                {
                    converter = new CollectionConverter(converter, listAttr.Separator);
                }

                // Do conversion.
                try
                {
                    object convertedValue = converter.ConvertTo(destinationProperty.PropertyType, kvp.Value);
                    destinationProperty.SetValue(configSectionModel, convertedValue);
                }
                catch (NotSupportedException ex)
                {
                    throw new IniException($"['{section.Name}'.'{kvp.Key}'] {ex.Message}");
                }
            }

            return configSectionModel;
        }

        #endregion

        #region Dictionary deserialization

        public IDictionary<string, IDictionary<string, string>> Deserialize(string iniFileContents)
        {
            var configModel = new Dictionary<string, IDictionary<string, string>>();

            using (var reader = new StringReader(iniFileContents))
            {
                foreach (IniSection section in Reader.Read(reader))
                {
                    configModel.Add(section.Name, section.Contents);
                }
            }

            return configModel;
        }

        public IDictionary<string, string> DeserializeSection(string iniFileContents, string sectionName)
        {
            using (var reader = new StringReader(iniFileContents))
            {
                IniSection section = Reader.Read(reader)
                    .SingleOrDefault(s => sectionName.Equals(s.Name, StringComparison.InvariantCultureIgnoreCase));

                return section?.Contents ?? new Dictionary<string, string>();
            }
        }

        #endregion

    }
}
