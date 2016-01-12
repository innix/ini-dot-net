using System;
using System.Collections.Generic;
using System.IO;

namespace IniDotNet
{
    public interface IFileParser
    {
        IEnumerable<IniSection> Parse(TextReader reader);
    }
}
