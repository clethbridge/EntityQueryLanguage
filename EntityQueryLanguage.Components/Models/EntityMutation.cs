using System;
using System.Collections.Generic;
using System.Text;

namespace EntityQueryLanguage.Components.Models
{
    public class EntityMutation: IEntityId
    {
        public dynamic EntityId { get; set; }

        public string EntityKey { get; set; }
        
        public EntityMutationType Type { get; set; }

        public Dictionary<string, dynamic> Fields { get; set; } = new Dictionary<string, dynamic>();

        public EntityQuery Return { get; set; }

        public bool HasEntityId => EntityId != null;
    }
}
