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
        IReadOnlyList<SectionBinding> Bind(Type modelType);
    }
}
