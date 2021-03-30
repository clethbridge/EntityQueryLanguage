using EntityQueryLanguage.Components.Models;
using EntityQueryLanguage.Components.Services.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityQueryLanguage.Components.Services
{
    public interface INullChecker
    {
        bool IsNotPopulated(EntityMutation mutation, EntityField field);
    }

    [EqlService(typeof(INullChecker))]
    public class NullChecker: INullChecker
    {
        public bool IsNotPopulated(EntityMutation mutation, EntityField field)
        {
            if (mutation.Type == EntityMutationType.Insert && field.IsPrimaryKey)
                return false;
            else
            {
                bool hasField = mutation.Fields != null && mutation.Fields.ContainsKey(field.TermKey);

                if (hasField)
                {
                    var value = mutation.Fields[field.TermKey];

                    return object.Equals(value, null) && string.IsNullOrEmpty(value?.ToString());
                }
                else
                    return true;
            }
        }
    }
}
