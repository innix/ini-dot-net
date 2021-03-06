using System;
using System.Collections;
using System.Collections.Generic;

namespace IniDotNet.Converters
{
    public sealed class CollectionConverter : IConverter
    {
        private readonly IConverter itemConverter;
        private readonly string seperator;

        public CollectionConverter(IConverter itemConverter, string seperator)
        {
            this.itemConverter = itemConverter;
            this.seperator = seperator;
        }

        public bool TryConvertTo(Type type, string stringValue, out object converted)
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
                object convertedItem;
                if (!itemConverter.TryConvertTo(listType, listItem, out convertedItem))
                {
                    throw new NotSupportedException($"No suitable conversion exists to convert to '{listType.FullName}'.");
                }

                listTypeBuilder.Add(convertedItem);
            }

            converted = listTypeBuilder.Build();
            return true;
        }
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
}
