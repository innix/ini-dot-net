using System;
using System.ComponentModel;

namespace IniDotNet.Converters
{
    public sealed class StandardConverter : IConverter
    {
        public bool TryConvertTo(Type type, string stringValue, out object converted)
        {
            // .NET has a built-in converter for string-to-TimeSpan, but it is very lenient on which
            // formats it will accept and will almost never parse it correctly. So we will disable
            // built-in TimeSpan conversion and allow users to write their own converters.
            if (type == typeof(TimeSpan))
            {
                converted = null;
                return false;
            }

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
