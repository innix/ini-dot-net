using System;

namespace IniDotNet.Converters
{
    public sealed class CompositeConverter : IConverter
    {
        private readonly IConverter[] converters;

        public CompositeConverter(params IConverter[] converters)
        {
            this.converters = converters;
        }

        public bool TryConvertTo(Type type, string stringValue, out object converted)
        {
            foreach (IConverter converter in converters)
            {
                if (converter.TryConvertTo(type, stringValue, out converted))
                {
                    return true;
                }
            }

            converted = null;
            return false;
        }
    }
}
