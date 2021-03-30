using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace EntityQueryLanguage.Components.Services.DataAccess
{
    public interface IDbContext
    {
        Task<DataTable> ExecuteQueryAsync(string sql, Dictionary<string, dynamic> parameters);

        Task ExecuteNonQueryAsync(string sql, Dictionary<string, dynamic> parameters);

        Task<DataTable> ExecuteProcedureAsync(string sql, Dictionary<string, dynamic> parameters);

        Task BulkInsert(string tableName, DataTable dataTable);
    }
}
