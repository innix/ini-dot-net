using System;
using System.ComponentModel;

namespace IniDotNet
{
    internal interface IConverter
    {
        object ConvertTo(Type type, string stringValue);
    }

    internal sealed class StandardConverter : IConverter
    {
        public object ConvertTo(Type type, string stringValue)
        {
            TypeConverter tc = TypeDescriptor.GetConverter(type);

            if (tc.CanConvertFrom(typeof(string)))
            {
                return tc.ConvertFrom(stringValue);
            }

            throw new NotSupportedException($"No suitable conversion exists to convert to '{type.FullName}'.");
        }
    }

    internal sealed class CollectionConverter : IConverter
    {
        private readonly IConverter itemConverter;
        private readonly string seperator;

        public CollectionConverter(IConverter itemConverter, string seperator)
        {
            this.itemConverter = itemConverter;
            this.seperator = seperator;
        }

        public object ConvertTo(Type type, string stringValue)
        {
            var listTypeBuilder = new ListTypeBuilder(type);

            // Gets the T from IEnumerable<T>.
            Type listType = listTypeBuilder.GetItemType();
            if (listType == null)
            {
                throw new NotSupportedException("Property is marked as list but does not implement IEnumerable<T>.");
            }

            foreach (string listItem in stringValue.Split(new[] { seperator }, StringSplitOptions.None))
            {
                listTypeBuilder.Add(itemConverter.ConvertTo(listType, listItem));
            }

            return listTypeBuilder.Build();
        }
    }
}
