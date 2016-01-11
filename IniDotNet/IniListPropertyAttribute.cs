using System;

namespace IniDotNet
{
    public class IniListPropertyAttribute : Attribute
    {
        public IniListPropertyAttribute(string separator)
        {
            Separator = separator;
        }

        public string Separator { get; }
    }
}
