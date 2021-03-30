using System;
using System.Collections.Generic;
using System.Text;

namespace EntityQueryLanguage.Components.Models
{
    public class EntityLookup
    {
        public int Id { get; set; }

        public string TermKey { get; set; }

        public int Code { get; set; }

        public string Value { get; set; }

        public int DisplayOrder { get; set; }
    }
}
