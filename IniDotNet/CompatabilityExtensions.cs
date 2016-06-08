using System;
using System.Reflection;

namespace IniDotNet
{
    internal static class CompatabilityExtensions
    {
        public static T GetCustomAttribute<T>(this PropertyInfo propertyInfo) where T : Attribute
        {
            return (T) Attribute.GetCustomAttribute(propertyInfo, typeof (T));
        }

        public static bool IsNullOrWhiteSpace(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return true;
            }

            for (int i = 0; i < value.Length; i++)
            {
                if (!char.IsWhiteSpace(value[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
