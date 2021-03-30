using EntityQueryLanguage.Components.Services.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityQueryLanguage.Components.Services.DataAccess
{
    public interface ISqlServerDataTypeMap
    {
        Dictionary<string, Type> Elements { get; }
    }

    [EqlService(typeof(ISqlServerDataTypeMap))]
    public class SqlServerDataTypeMap: ISqlServerDataTypeMap
    {
        public Dictionary<string, Type> Elements => elements;

        private Dictionary<string, Type> elements;

        public SqlServerDataTypeMap()
        {
            elements = new Dictionary<string, Type>()
            {
                { "char", typeof(string)},
                { "varchar", typeof(string)},
                { "nvarchar", typeof(string)},
                { "decimal", typeof(decimal)},
                { "numeric", typeof(decimal)},
                { "money", typeof(decimal)},
                { "int", typeof(int)},
                { "long", typeof(long)},
                { "date", typeof(DateTime)},
                { "datetime", typeof(DateTime)},
                { "datetimeoffset", typeof(DateTimeOffset)},
                { "bit", typeof(bool)},
                { "varbinary", typeof(byte[])}
            };
        }
    }
}
