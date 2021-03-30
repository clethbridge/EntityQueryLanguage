using EntityQueryLanguage.Components.Models;
using EntityQueryLanguage.Components.Services.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityQueryLanguage.Components.Services.Parsers
{
    public interface IDataTypeParser
    {
        Dictionary<string, dynamic> Parse(
            EntityType entityType,
            EntityMutation mutation,
            EqlValidation validation
        );

        dynamic ParseScalar(EntityField field, dynamic value);
    }

    [EqlService(typeof(IDataTypeParser))]
    public class DataTypeParser : IDataTypeParser
    {
        public Dictionary<string, dynamic> Parse(
            EntityType entityType,
            EntityMutation mutation,
            EqlValidation validation)
        {
            var model = new Dictionary<string, dynamic>();

            foreach (var mutationField in mutation.Fields)
            {
                EntityField entityField = entityType.GetEntityField(mutationField.Key);

                if (mutationField.Value != null)
                {
                    try
                    {
                        var value = ParseScalar(entityField, mutationField.Value);

                        model.Add(entityField.ColumnName.UnquoteName(), value);
                    }
                    catch (Exception ex)
                    {
                        validation.Errors.Add(ex.Message);
                    }
                }
            }

            return model;
        }

        public dynamic ParseScalar(EntityField entityField, dynamic value)
        {
            try
            {
                string sValue = value.ToString();

                switch (entityField.DataType)
                {
                    case "char":
                    case "varchar":
                    case "nvarchar":
                        return ParseVarchar(entityField, sValue);
                    case "decimal":
                    case "numeric":
                    case "money":
                        return ParseDecimal(entityField, sValue);
                    case "int":
                        return ParseInt(entityField, sValue);
                    case "long":
                        return ParseLong(entityField, sValue);
                    case "date":
                    case "datetime":
                        return ParseDate(entityField, sValue);
                    case "datetimeoffset":
                        return ParseDateOffset(entityField, sValue);
                    case "bit":
                        return ParseBool(entityField, sValue);
                    case "varbinary":
                        return value;
                    default:
                        throw new Exception($"{entityField.DataType} is not currently supported");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private int ParseInt(EntityField entityField, string sValue)
        {
            int iValue;
            bool success = int.TryParse(sValue, out iValue);

            if (success)
                return iValue;
            else
                throw new Exception($"Unable to parse '{sValue}' into an integer for '{entityField.TermKey}'.");
        }

        private long ParseLong(EntityField entityField, string sValue)
        {
            long lValue;
            bool success = long.TryParse(sValue, out lValue);

            if (success)
                return lValue;
            else
                throw new Exception($"Unable to parse '{sValue}' into an integer for '{entityField.TermKey}'.");
        }

        private decimal ParseDecimal(EntityField entityField, string sValue)
        {
            decimal dValue;
            bool success = decimal.TryParse(sValue, out dValue);

            if (success)
                return dValue;
            else
                throw new Exception($"Unable to parse '{sValue}' into a decimal for '{entityField.TermKey}'.");
        }

        private bool ParseBool(EntityField entityField, string sValue)
        {
            bool bValue;
            bool success = bool.TryParse(sValue, out bValue);

            if (success)
                return bValue;
            else
                throw new Exception($"Unable to parse '{sValue}' into a bit for '{entityField.TermKey}'.");
        }

        private DateTime ParseDate(EntityField entityField, string sValue)
        {
            DateTime dValue;
            bool success = DateTime.TryParse(sValue, out dValue);

            if (success)
                return dValue;
            else
                throw new Exception($"Unable to parse '{sValue}' into a date for '{entityField.TermKey}'.");
        }

        private DateTimeOffset ParseDateOffset(EntityField entityField, string sValue)
        {
            DateTimeOffset dValue;
            bool success = DateTimeOffset.TryParse(sValue, out dValue);

            if (success)
                return dValue;
            else
                throw new Exception($"Unable to parse '{sValue}' into a date for '{entityField.TermKey}'.");
        }

        private string ParseVarchar(EntityField entityField, string sValue)
        {
            int maxLength = entityField.MaxLength;

            bool invalid = sValue.Length > maxLength && entityField.DataType != "nvarchar" && maxLength != -1;

            if (invalid)
                throw new Exception($"'{entityField.TermKey}' has a max length of {maxLength}.");
            else
                return sValue;
        }
    }
}
