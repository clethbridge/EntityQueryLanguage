using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityQueryLanguage.Components.Services.DataAccess
{
    public class SqlServerDbContext : IDbContext
    {
        private string connectionString;

        public SqlServerDbContext(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task ExecuteNonQueryAsync(string sql, Dictionary<string, dynamic> parameters)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection))
                {
                    try
                    {
                        sqlConnection.Open();

                        AddParameters(parameters, sqlCommand);

                        await sqlCommand.ExecuteNonQueryAsync();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    { 
                        sqlConnection.Close();
                    }
                }
            }
        }

        public async Task<DataTable> ExecuteProcedureAsync(string name, Dictionary<string, dynamic> parameters)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(name, sqlConnection))
                {
                    try
                    {
                        return await Task.Factory.StartNew(() =>
                        {
                            sqlDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                            
                            DataTable dataTable = new DataTable();

                            sqlConnection.Open();

                            AddParameters(parameters, sqlDataAdapter.SelectCommand);

                            sqlDataAdapter.Fill(dataTable);

                            return dataTable;
                        });
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        sqlConnection.Close();
                    }
                }
            }
        }

        public async Task<DataTable> ExecuteQueryAsync(string sql, Dictionary<string, dynamic> parameters)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sql, sqlConnection))
                {
                    try
                    {
                        return await Task.Factory.StartNew(() => 
                        { 
                            DataTable dataTable = new DataTable();

                            sqlConnection.Open();

                            AddParameters(parameters, sqlDataAdapter.SelectCommand);

                            sqlDataAdapter.Fill(dataTable);

                            return dataTable;
                        });

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        sqlConnection.Close();
                    }
                }
            }
        }

        public async Task BulkInsert(string tableName, DataTable dataTable)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(sqlConnection))
                {
                    try
                    {
                        sqlBulkCopy.DestinationTableName = tableName;

                        dataTable
                        .Columns
                        .Cast<DataColumn>()
                        .ToList()
                        .ForEach(column => sqlBulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName));

                        sqlConnection.Open();

                        await sqlBulkCopy.WriteToServerAsync(dataTable);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        sqlConnection.Close();
                    }
                }
            }
        }

        private void AddParameters(Dictionary<string, dynamic> parameters, SqlCommand sqlCommand)
        {
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    sqlCommand.Parameters.AddWithValue(parameter.Key, parameter.Value);
                }
            }
        }
    }
}
