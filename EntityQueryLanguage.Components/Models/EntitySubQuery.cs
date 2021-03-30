using System.Collections.Generic;

namespace EntityQueryLanguage.Components.Models
{
    public class EntitySubQuery
    { 
        public string Name { get; set; }

        public EntityQuery EntityQuery { get; set; }

        public List<EntityJoinCondition> Conditions { get; set; } = new List<EntityJoinCondition>();
    }
}
