using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityQueryLanguage.Components.Models
{
    public class EntityQuery: IEntityId
    {
        /// <summary>
        /// The value of the identifying field for a given record (e.g 'Id'). 
        /// </summary>
        public dynamic EntityId { get; set; }

        /// <summary>
        /// A unique key used to identify and map the subject Entity Type to a database object. 
        /// </summary>
        public string EntityKey { get; set; }

        /// <summary>
        /// Indicates whether the query executor should join entities together or execute multiple queries to return the data.
        /// By default, EQL will join database objects together. To have it execute multiple queries set this to 'MultipleQueries'
        /// </summary>
        public QueryStrategy Strategy { get; set; } = QueryStrategy.Join;

        /// <summary>
        /// The list of terms to be selected from the subject Entity Type
        /// </summary>
        public List<string> TermKeys { get; set; } = new List<string>();

        /// <summary>
        /// Manages the columns to filter the search by, and whether it is an (Or / And) type of search. 
        /// </summary>
        public EntityFilter Filter { get; set; } = new EntityFilter();

        /// <summary>
        /// A collection of all fields to order by, where the position of the term key in the list indicates the precedence of the ordering.
        /// </summary>
        public List<EntitySort> Sortings { get; set; } = new List<EntitySort>();

        /// <summary>
        /// A collection of entity queries where the child entity has a one-to-one relationship with the subject entity.
        /// </summary>
        public List<EntitySubQuery> Projections { get; set; } = new List<EntitySubQuery>();

        /// <summary>
        /// A collection of entity queries where the child entity has a one-to-many relationship with the subject entity.
        /// </summary>
        public List<EntitySubQuery> Collections { get; set; } = new List<EntitySubQuery>();

        /// <summary>
        /// The union of projections and collections for the subject entity.
        /// </summary>
        public List<EntitySubQuery> SubQueries => Projections.Union(Collections).ToList();
    }
}
