using System;

namespace IniDotNet
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IniConverterAttribute : Attribute
    {
        public IniConverterAttribute(Type converterType) : this(converterType, null)
        {
        }

        public IniConverterAttribute(Type converterType, params object[] args)
        {
            ConverterType = converterType;
            Args = args;
        }

        public Type ConverterType { get; }
        public object[] Args { get; }
    }
}
