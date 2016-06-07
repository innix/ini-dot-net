using System;
using System.ComponentModel;

namespace IniDotNet.Converters
{
    public sealed class StandardConverter : IConverter
    {
        public bool TryConvertTo(Type type, string stringValue, out object converted)
        {
            TypeConverter tc = TypeDescriptor.GetConverter(type);

            if (tc.CanConvertFrom(typeof(string)))
            {
                converted = tc.ConvertFrom(stringValue);
                return true;
            }

            converted = null;
            return false;
        }
    }
}
