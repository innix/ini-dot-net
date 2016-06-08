using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace IniDotNet.Converters
{
    public sealed class BooleanConverter : IConverter
    {
        private static readonly string[] TruthyValues =
        {
            "1", "yes"
        };

        private static readonly string[] FalseyValues =
        {
            "0", "no"
        };

        public bool TryConvertTo(Type type, string stringValue, out object converted)
        {
            if (type != typeof (bool))
            {
                converted = null;
                return false;
            }

            if (TruthyValues.Any(x => x.Equals(stringValue, StringComparison.OrdinalIgnoreCase)))
            {
                converted = true;
                return true;
            }
            if (FalseyValues.Any(x => x.Equals(stringValue, StringComparison.OrdinalIgnoreCase)))
            {
                converted = false;
                return true;
            }

            TypeConverter tc = TypeDescriptor.GetConverter(type);
            if (tc.CanConvertFrom(typeof (string)))
            {
                converted = tc.ConvertFrom(stringValue);
                return true;
            }

            converted = null;
            return false;
        }
    }
}
