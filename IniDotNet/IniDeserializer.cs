using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace IniDotNet
{
    public static class IniDeserializer
    {
        private const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private static readonly IIniSectionReader sectionReader = new StandardIniSectionReader();
        private static readonly ISectionBindingLocator bindingLocator = new AttributeBasedSectionBindingLocator();

        #region POCO deserialization

        public static T Deserialize<T>(string iniFileContents)
        {
            object configModel = Activator.CreateInstance<T>();
            Type configModelType = typeof(T);


            // Locates the properties within the model type and maps them to an .INI section.
            IReadOnlyList<SectionBinding> bindings = bindingLocator.Locate(configModelType);

            using (var reader = new StringReader(iniFileContents))
            {
                foreach (IniSection section in sectionReader.Read(reader))
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

        private static object DeserializeSection(IniSection section, SectionBinding binding)
        {
            object configSectionModel = Activator.CreateInstance(binding.Type);

            foreach (var kvp in section.Contents)
            {
                PropertyInfo destinationProperty = binding.Type.GetProperty(kvp.Key, flags | BindingFlags.IgnoreCase);
                if (destinationProperty == null)
                {
                    Debug.WriteLine(
                        $"Type '{binding.Type.FullName}' has no suitable property for '{section.Name}'.'{kvp.Key}'");
                    continue;
                }

                IniListPropertyAttribute listAttr = destinationProperty.GetCustomAttribute<IniListPropertyAttribute>();

                // Get a type converter to try and convert our string representation
                // into the type of the destination property.
                TypeConverter tc;

                ListTypeBuilder listTypeBuilder = null;

                if (listAttr == null)
                {
                    tc = TypeDescriptor.GetConverter(destinationProperty.PropertyType);
                }
                else
                {
                    listTypeBuilder = new ListTypeBuilder(destinationProperty.PropertyType);

                    // Gets the T from IEnumerable<T>.
                    Type listType = listTypeBuilder.GetItemType();
                    if (listType == null)
                    {
                        throw new IniException(
                            $"Property '{section.Name}'.'{kvp.Key}' marked as list does not implement IEnumerable<T>.");
                    }

                    tc = TypeDescriptor.GetConverter(listType);
                }

                if (!tc.CanConvertFrom(typeof(string)))
                {
                    throw new IniException(
                        $"No suitable conversion exists to convert '{section.Name}'.'{kvp.Key}' " +
                        $"to '{destinationProperty.PropertyType.FullName}'.");
                }

                object convertedValue;
                if (listAttr == null)
                {
                    convertedValue = tc.ConvertFrom(kvp.Value);
                }
                else
                {
                    foreach (string listItem in kvp.Value.Split(new[] {listAttr.Separator}, StringSplitOptions.None))
                    {
                        listTypeBuilder.Add(tc.ConvertFrom(listItem));
                    }

                    convertedValue = listTypeBuilder.Build();
                }

                destinationProperty.SetValue(configSectionModel, convertedValue);
            }

            return configSectionModel;
        }

        #endregion

        #region Dictionary deserialization

        public static IDictionary<string, IDictionary<string, string>> Deserialize(string iniFileContents)
        {
            var configModel = new Dictionary<string, IDictionary<string, string>>();

            using (var reader = new StringReader(iniFileContents))
            {
                foreach (IniSection section in sectionReader.Read(reader))
                {
                    configModel.Add(section.Name, section.Contents);
                }
            }

            return configModel;
        }

        public static IDictionary<string, string> DeserializeSection(string iniFileContents, string sectionName)
        {
            using (var reader = new StringReader(iniFileContents))
            {
                IniSection section = sectionReader.Read(reader)
                    .SingleOrDefault(s => sectionName.Equals(s.Name, StringComparison.InvariantCultureIgnoreCase));

                return section?.Contents ?? new Dictionary<string, string>();
            }
        }

        #endregion

    }

    internal sealed class ListTypeBuilder
    {
        private readonly Type type;
        private readonly Type itemType;
        private readonly IList tempHolderList;

        public ListTypeBuilder(Type type)
        {
            this.type = type;

            itemType = GetGenericEnumerableType(type);
            tempHolderList = new ArrayList();
        }

        public void Add(object item)
        {
            tempHolderList.Add(item);
        }

        public Type GetItemType()
        {
            return itemType;
        }

        public object Build()
        {
            Type genericListType = typeof(List<>).MakeGenericType(itemType);
            if (type.IsAssignableFrom(genericListType))
            {
                IList nonGenericList = (IList) Activator.CreateInstance(genericListType);
                foreach (object item in tempHolderList)
                {
                    nonGenericList.Add(item);
                }

                return nonGenericList;
            }

            if (type.IsArray)
            {
                Array array = Array.CreateInstance(GetItemType(), tempHolderList.Count);
                for (int i = 0; i < tempHolderList.Count; i++)
                {
                    array.SetValue(tempHolderList[i], i);
                }

                return array;
            }

            throw new NotSupportedException($"Cannot deserialize to list type: {type.FullName}");
        }

        private static Type GetGenericEnumerableType(Type t)
        {
            if (t.IsInterface && t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return t.GetGenericArguments()[0];
            }

            foreach (var i in t.GetInterfaces())
            {
                if (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    return i.GetGenericArguments()[0];
                }
            }

            return null;
        }
    }

    internal sealed class IniSection
    {
        public IniSection(string name, IDictionary<string, string> contents)
        {
            Name = name;
            Contents = contents;
        }

        public string Name { get; }
        public IDictionary<string, string> Contents { get; }
    }
}
