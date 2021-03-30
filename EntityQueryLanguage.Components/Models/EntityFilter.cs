using System;
using System.Collections.Generic;
using System.Text;

namespace EntityQueryLanguage.Components.Models
{
    public class EntityFilter
    {
        public ConjunctionType Conjunction { get; set; }

        public List<EntityFieldFilter> FieldFilters { get; set; } = new List<EntityFieldFilter>();
    }

    public class EntityFieldFilter
    { 
        public string TermKey { get; set; }

        public OperatorType Operator { get; set; }

        public dynamic Value { get; set; }

        public List<dynamic> Values { get; set; }
    }
}
