using System;

namespace IniDotNet
{
    public abstract class IniConverter
    {
        public abstract object ConvertFromString(string stringValue);

        internal IConverter ToTypeConverter()
        {
            return new Converter(this);
        }

        private class Converter : IConverter
        {
            private readonly IniConverter parentConverter;

            public Converter(IniConverter parentConverter)
            {
                this.parentConverter = parentConverter;
            }

            public bool TryConvertTo(Type type, string stringValue, out object converted)
            {
                converted = parentConverter.ConvertFromString(stringValue);
                return true;
            }
        }
    }
}
