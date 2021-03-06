using System;
using System.Collections.Generic;

namespace IniDotNet
{
    /// <summary>
    /// A section binding locator creates a mapping to determine which .INI section
    /// is bound to which property in the model class.
    /// </summary>
    public interface ISectionBinder
    {
        IList<SectionBinding> Bind(Type modelType, IList<IniSection> sections);
    }
}
