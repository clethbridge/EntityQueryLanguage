using System;
using System.Collections.Generic;
using System.Linq;

namespace EntityQueryLanguage.Components.Models
{
    public class EntityType
    {
        /// <summary>
        /// A unique key used to identify and map the subject Entity Type to a database object.
        /// </summary>
        public string EntityKey { get; set; }

        /// <summary>
        /// The name of this object in the database
        /// </summary>
        public string DatabaseName { get; set; }

        public bool CanInsert { get; set; }

        public bool CanUpdate { get; set; }

        public bool CanDelete { get; set; }

        /// <summary>
        /// A collection of fields that correspond to the columns of this object within the database.
        /// </summary>
        public List<EntityField> EntityFields { get; set; } = new List<EntityField>();

        /// <summary>
        /// Returns the specified field from the subject entity.
        /// </summary>
        public EntityField GetEntityField(string termKey) => 
            EntityFields.FirstOrDefault(f => f.TermKey == termKey);

        public EntityField PrimaryKey => 
            EntityFields.FirstOrDefault(f => f.IsPrimaryKey);

        /// <summary>
        /// The set of fields that can not be null for the subject entity .
        /// </summary>
        public List<EntityField> RequiredFields =>
           EntityFields.Where(f => !f.IsNullable).ToList();

        /// <summary>
        /// The set of unqiue term keys for each field belonging to the subject entity.
        /// </summary>
        public List<string> TermKeys => 
            EntityFields.Select(f => f.TermKey).ToList();
    }
}
