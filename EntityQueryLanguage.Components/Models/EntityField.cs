using System;
using System.Linq;

namespace EntityQueryLanguage.Components.Models
{
    public class EntityField
    {
        /// <summary>
        /// A unique key that represents the subject Entity Field within an Entity Type. 
        /// </summary>
        public string TermKey { get; set; }

        /// <summary>
        /// The name of this field within the database.
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// The name of this field within the code.
        /// </summary>
        public string PropertyName => 
            string.IsNullOrEmpty(ColumnName) ? string.Empty : ColumnName.UnquoteName();

        /// <summary>
        /// The name of the data type of this field within the database.
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// The maximum length of characters this field can store within the database.
        /// </summary>
        public int MaxLength { get; set; }

        /// <summary>
        /// The maximum amount of precision this field can store within the database.
        /// </summary>
        public int Precision { get; set; }

        /// <summary>
        /// The maximum amount of scale this field can store within the database.
        /// </summary>
        public int Scale { get; set; }

        /// <summary>
        /// A flag indicating if this field corresponds to the primary key column for the subject Entity Type.
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        /// A flag indicating if this field can handle null values in the database.
        /// </summary>
        public bool IsNullable { get; set; }

        /// <summary>
        /// A flag indicating that this field has a defined set of allowable values.
        /// </summary>
        public bool IsLookup { get; set; }

        public bool IsIdentity { get; set; }

        public string DerivedDataType
        {
            get
            {
                var useMaxLength = CheckDataType("varchar", "nvarchar", "varbinary");

                var usePrecisionAndScale = CheckDataType("numeric", "decimal");

                if (useMaxLength)
                {
                    return MaxLength == -1 ? $"{DataType}(MAX)" : $"{DataType}({MaxLength})";
                }
                else if (usePrecisionAndScale)
                {
                    return $"{DataType}({Precision},{Scale})";
                }
                else
                {
                    return DataType;
                }
            }
        }

        private bool CheckDataType(params string[] dataTypes) =>
            dataTypes.Any(dataType => string.Equals(dataType, DataType, StringComparison.OrdinalIgnoreCase));
    }
}
