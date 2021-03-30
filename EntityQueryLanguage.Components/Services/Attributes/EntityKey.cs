using System;

namespace EntityQueryLanguage.Components.Services.Attributes
{
    public class EntityKey : Attribute
    {
        public string Value { get; set; }

        public EntityKey(string value)
        {
            Value = value;
        }
    }


}
