using EntityQueryLanguage.Components.Models;
using EntityQueryLanguage.Components.Services.Attributes;
using EntityQueryLanguage.Components.Services.Validators;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityQueryLanguage.Components.Services.Executors
{
    public interface IEntityMutationExecutor
    {
        Task<dynamic> ExecuteAsync(EntityMutation mutation, Dictionary<string, dynamic> parameters = null);

        Task BulkInsertAsync(BulkEntityMutation bulkEntityMutation);
    }

    [EqlService(typeof(IEntityMutationExecutor))]
    public class EntityMutationExecutor : IEntityMutationExecutor
    {
        private IEntityMutationValidator mutationValidator;
        private IEntityQueryValidator queryValidator;
        private IMutationStatementExecutorFactory executorFactory;
        private IParameterParser parameterParser;
        private IBulkInsertExecutor bulkInsertExecutor;

        public EntityMutationExecutor(
            IEntityMutationValidator mutationValidator,
            IEntityQueryValidator queryValidator,
            IMutationStatementExecutorFactory executorFactory,
            IParameterParser parameterParser,
            IBulkInsertExecutor bulkInsertExecutor)
        {
            this.mutationValidator = mutationValidator;
            this.queryValidator = queryValidator;
            this.executorFactory = executorFactory;
            this.parameterParser = parameterParser;
            this.bulkInsertExecutor = bulkInsertExecutor;
        }

        public async Task<dynamic> ExecuteAsync(EntityMutation mutation, Dictionary<string, dynamic> parameters = null)
        {
            Dictionary<string, dynamic> model;

            parameterParser.Evaluate(mutation, parameters);

            EqlValidation mutationValidation = mutationValidator.Validate(mutation, out model);

            EqlValidation queryValidation = ValidateQuery(mutation);

            if (mutationValidation.IsValid && queryValidation.IsValid)
            {
                try
                {
                    IMutationStatementExecutor executor = executorFactory.Get(mutation.Type);

                    return await executor.ExecuteAsync(mutation, model);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                var errors = string.Join("\r\n", mutationValidation.Errors.Union(queryValidation?.Errors));

                throw new Exception(errors);
            }
        }

        public async Task BulkInsertAsync(BulkEntityMutation bulkEntityMutation)
        {
            try
            {
                await bulkInsertExecutor.ExecuteAsync(bulkEntityMutation);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private EqlValidation ValidateQuery(EntityMutation mutation) =>
            mutation.Return == null ? new EqlValidation() : queryValidator.Validate(mutation.Return);
    }
}
