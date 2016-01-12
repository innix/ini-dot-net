using System;

namespace IniDotNet
{
    public interface IConverter
    {
        bool TryConvertTo(Type type, string stringValue, out object converted);
    }
}
