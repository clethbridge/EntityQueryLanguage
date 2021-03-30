using System;
using System.Collections.Generic;
using System.Text;

namespace EntityQueryLanguage.Components.Services.Attributes
{
    /// <summary>
    /// Used to indicate that this field is set by the database and won't be included in mutations
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DbDefault: Attribute {}
}
