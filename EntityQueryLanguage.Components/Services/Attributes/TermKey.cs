using System;

namespace EntityQueryLanguage.Components.Services.Attributes
{
    public class TermKey: Attribute
    { 
        public string Value { get; set; }

        public TermKey(string value)
        {
            Value = value;
        }
    }


}
