using System;

namespace IniDotNet
{
    public class IniException : Exception
    {
        public IniException(string message) : base(message)
        {
        }
    }
}
