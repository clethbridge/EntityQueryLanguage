using EntityQueryLanguage.Components.Models;
using EntityQueryLanguage.Components.Services.Attributes;
using EntityQueryLanguage.Components.Services.Builders;
using EntityQueryLanguage.Components.Services.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace EntityQueryLanguage.Components.Services.Executors
{
    public interface IBulkInsertExecutor
    {
        Task ExecuteAsync(BulkEntityMutation bulkEntityMutation);
    }

    [EqlService(typeof(IBulkInsertExecutor))]
    public class BulkInsertExecutor : IBulkInsertExecutor
    {
        private IDbContext dbContext;

        private IDataTableBuilder dataTableBuilder;

        public BulkInsertExecutor(
            IDbContext dbContext,
            IDataTableBuilder dataTableBuilder)
        {
            this.dbContext = dbContext;
            this.dataTableBuilder = dataTableBuilder;
        }

        public async Task ExecuteAsync(BulkEntityMutation bulkEntityMutation)
        {
            string tableName;
            DataTable dataTable = dataTableBuilder.Build(bulkEntityMutation, out tableName);

            await dbContext.BulkInsert(tableName, dataTable);
        }
    }
}
