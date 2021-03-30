using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace EntityQueryLanguage.Components.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum EntityMutationType
    { 
        [EnumMember(Value = "Insert")]
        Insert,
        [EnumMember(Value = "Update")]
        Update,
        [EnumMember(Value = "Delete")]
        Delete
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum EntitySortType
    {
        [EnumMember(Value = "Ascending")]
        Ascending,
        [EnumMember(Value = "Descending")]
        Descending
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum BulkEntityMutationType
    {
        [EnumMember(Value = "Bulk Insert")]
        BulkInsert,
        [EnumMember(Value = "Bulk Update")]
        BulkUpdate,
        [EnumMember(Value = "Bulk Delete")]
        BulkDelete
    }

    public enum EqlServiceType
    { 
        Singleton,
        Scoped,
        Transient
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ConjunctionType
    { 
        [EnumMember(Value = "Or")]
        Or,
        [EnumMember(Value = "And")]
        And
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum OperatorType
    { 
        [EnumMember(Value = "=")]
        Equals,
        [EnumMember(Value = "<>")]
        DoesNotEqual,
        [EnumMember(Value = ">")]
        GreaterThan,
        [EnumMember(Value = ">=")]
        GreaterThanOrEqualTo,
        [EnumMember(Value = "<")]
        LessThan,
        [EnumMember(Value = "<=")]
        LessThanOrEqualTo,
        [EnumMember(Value = "Is Null")]
        IsNull,
        [EnumMember(Value = "Is Not Null")]
        IsNotNull,
        [EnumMember(Value = "Starts With")]
        StartsWith,
        [EnumMember(Value = "Ends With")]
        EndsWith,
        [EnumMember(Value = "Contains")]
        Contains,
        [EnumMember(Value = "In")]
        In,
        [EnumMember(Value = "Not In")]
        NotIn
    }

    public enum QueryStrategy
    {
        [EnumMember(Value = "Join")]
        Join,
        [EnumMember(Value = "Multiple Queries")]
        MultipleQueries
    }
}
