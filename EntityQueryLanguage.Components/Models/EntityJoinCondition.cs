using System;
using System.Collections.Generic;
using System.Text;

namespace EntityQueryLanguage.Components.Models
{
    public class EntityJoinCondition
    {
        public string TermKey { get; set; }

        public dynamic Value { get; set; }

        public string ParentTermKey { get; set; }

        public bool HasTermKey => !string.IsNullOrEmpty(TermKey);

        public bool HasParentTermKey => !string.IsNullOrEmpty(ParentTermKey);

        public bool HasValue => Value != null;
    }
}
