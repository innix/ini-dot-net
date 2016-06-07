using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using IniDotNet.Binders;
using IniDotNet.Converters;
using IniDotNet.Parsers;

namespace IniDotNet
{
    public sealed class IniDeserializer
    {
        private const BindingFlags SearchFlags =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private static readonly IConverter[] DefaultConverters =
        {
            new BooleanConverter(), 
            new DnsEndPointConverter(),
            new IPEndPointConverter(),
            new StandardConverter()
        };

        /// <summary>
        /// Reads the .INI file content and parses it into .NET objects.
        /// </summary>
        public IFileParser Parser { get; set; } = new DefaultFileParser();

        /// <summary>
        /// Generates a set of bindings to determine what fields/properties the .INI sections
        /// will be deserialized into.
        /// </summary>
        public ISectionBinder SectionBinder { get; set; } = new AttributeBasedSectionBinder();

        /// <summary>
        /// Converts the string values from the .INI file into .NET types.
        /// </summary>
        public IConverter Converter { get; set; } = new CompositeConverter(DefaultConverters);

        #region POCO deserialization

        public T Deserialize<T>(string iniFileContents)
        {
            object configModel = Activator.CreateInstance<T>();
            Type configModelType = typeof(T);


            // Locates the properties within the model type and maps them to an .INI section.
            IReadOnlyList<SectionBinding> bindings = SectionBinder.Bind(configModelType);

            using (var reader = new StringReader(iniFileContents))
            {
                foreach (IniSection section in Parser.Parse(reader))
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
                // Gets all properties of the bound type with an IniPropertyAttribute
                // which has a name that matches the key we are working on.
                var candidateProperties = binding.Type.GetProperties(SearchFlags)
                    .Select(prop => new {prop, attr = prop.GetCustomAttribute<IniPropertyAttribute>()})
                    .Where(x => x.attr != null)
                    .Where(x => kvp.Key.Equals(x.attr.Name, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (candidateProperties.Count > 1)
                {
                    throw new IniException($"Multiple candidates for '{section.Name}'.'{kvp.Key}'.");
                }

                PropertyInfo destProperty;
                if (candidateProperties.Count == 1)
                {
                    destProperty = candidateProperties.Single().prop;
                }
                else
                {
                    // Else if there is no suitable IniPropertyAttributes, just
                    // search for a property with a matching name.
                    destProperty = binding.Type.GetProperty(kvp.Key, SearchFlags | BindingFlags.IgnoreCase);

                    if (destProperty == null)
                    {
                        Debug.WriteLine($"Type '{binding.Type.FullName}' has no suitable property" +
                                        $"for '{section.Name}'.'{kvp.Key}'");

                        continue;
                    }
                }


                IniListPropertyAttribute listAttr = destProperty.GetCustomAttribute<IniListPropertyAttribute>();
                IniConverterAttribute convertAttr = destProperty.GetCustomAttribute<IniConverterAttribute>();

                IConverter converter = Converter;

                // If property has custom converter attribute, create a new instance of it and wrap
                // it into a IConverter for easy use with existing code.
                if (convertAttr != null)
                {
                    if (!typeof(IniConverter).IsAssignableFrom(convertAttr.ConverterType))
                    {
                        throw new IniException("Provided custom converter is not a subclass of IniConverter.");
                    }

                    IniConverter customConverter =
                        (IniConverter) Activator.CreateInstance(convertAttr.ConverterType, convertAttr.Args);

                    converter = customConverter.ToTypeConverter();
                }

                if (listAttr != null)
                {
                    converter = new CollectionConverter(converter, listAttr.Separator);
                }


                // Do conversion.
                try
                {
                    object convertedValue;
                    if (!converter.TryConvertTo(destProperty.PropertyType, kvp.Value, out convertedValue))
                    {
                        throw new IniException($"['{section.Name}'.'{kvp.Key}'] No conversion for property.");
                    }

                    destProperty.SetValue(configSectionModel, convertedValue);
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
                foreach (IniSection section in Parser.Parse(reader))
                {
                    configModel.Add(section.Name, section.Contents);
                }
            }

            return configModel;
        }

        public IDictionary<string, string> DeserializeSection(string iniFileContents, string sectionName)
        {
            IEnumerable<IniSection> sections;
            using (var reader = new StringReader(iniFileContents))
            {
                sections = Parser.Parse(reader);
            }

            IniSection section = sections
                .SingleOrDefault(s => sectionName.Equals(s.Name, StringComparison.InvariantCultureIgnoreCase));

            return section?.Contents ?? new Dictionary<string, string>();
        }

        #endregion
    }
}
