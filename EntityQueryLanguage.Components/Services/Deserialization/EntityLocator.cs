using EntityQueryLanguage.Components.Services.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace EntityQueryLanguage.Components.Services.Deserialization
{
    public interface IEntityLocator
    {
        List<DataRow> FilterById(IEnumerable<DataRow> rows, string columnName, dynamic entityId);

        IEnumerable<dynamic> GetUniqueIds(List<DataRow> rows, string columnName);
    }

    [EqlService(typeof(IEntityLocator))]
    public class EntityLocator: IEntityLocator
    {
        public List<DataRow> FilterById(IEnumerable<DataRow> rows, string columnName, dynamic entityId) =>
             rows
            .Where(row => object.Equals(row[columnName], entityId))
            .ToList();

        public IEnumerable<dynamic> GetUniqueIds(List<DataRow> rows, string columnName) =>
             rows
            .Select(row => row[columnName])
            .Where(id => !object.Equals(id, DBNull.Value))
            .Distinct();
    }
}
