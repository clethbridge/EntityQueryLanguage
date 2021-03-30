using EntityQueryLanguage.Components.Models;
using EntityQueryLanguage.Components.Services.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityQueryLanguage.Components.Services.Builders
{
    public interface IColumnBuilder
    {
        string Build(EntityType entityType, EntityField entityField);
    }

    [EqlService(typeof(IColumnBuilder))]
    public class ColumnBuilder: IColumnBuilder
    {
        public string Build(EntityType entityType, EntityField entityField) =>
            $"[{entityField.TermKey}] = {entityType.DatabaseName}.{entityField.ColumnName}";
    }
}
