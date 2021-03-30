using EntityQueryLanguage.Components.Services.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace EntityQueryLanguage.Components.Services.Builders
{
    public class BaseEntityBuilder
    {
        protected string GetEntityKey(Type modelType) =>
             modelType.GetCustomAttribute<EntityKey>()?.Value;
    }
}
