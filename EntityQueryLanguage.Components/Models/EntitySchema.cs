using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityQueryLanguage.Components.Models
{
    public class EntitySchema
    {
        public List<EntityType> EntityTypes { get; set; } = new List<EntityType>();

        public List<EntityLookup> Lookups { get; set; } = new List<EntityLookup>();

        public EntityType GetEntityType(string entityKey) =>
            EntityTypes.FirstOrDefault(e => e.EntityKey == entityKey);

        public string DecodeLookup(string key, object code)
        {
            if (code != null)
            {
                return
                     Lookups
                    .First(l => l.TermKey == key && l.Code == Convert.ToInt32(code))
                    .Value;
            }
            else
            {
                return null;
            }
        }

        public int EncodeLookup(string key, string value) =>
             Lookups
            .First(l => l.TermKey == key && l.Value == value)
            .Code;
    }
}
