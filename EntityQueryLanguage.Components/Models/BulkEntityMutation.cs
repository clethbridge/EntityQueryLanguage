using System;
using System.Collections.Generic;
using System.Text;

namespace EntityQueryLanguage.Components.Models
{
    public class BulkEntityMutation
    {
        public string EntityKey { get; set; }

        public BulkEntityMutationType Type { get; set; }

        public List<Dictionary<string, dynamic>> Records { get; set; } = new List<Dictionary<string, dynamic>>();
    }
}
