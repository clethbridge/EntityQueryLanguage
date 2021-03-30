using EntityQueryLanguage.Components.Models;
using EntityQueryLanguage.Components.Services.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace EntityQueryLanguage.Components.Services.Builders
{
    public class EntityQueryBuilder<T> : BaseEntityBuilder, IDisposable 
    {
        private EntityQuery rootEntityQuery;

        private Type rootModelType;

        private Dictionary<ExpressionType, OperatorType> operatorLookup;

        public EntityQueryBuilder()
        {
            rootModelType = typeof(T);
            
            rootEntityQuery = new EntityQuery()
            {
                EntityKey = GetEntityKey(rootModelType)
            };

            operatorLookup = new Dictionary<ExpressionType, OperatorType>()
            {
                { ExpressionType.Equal, OperatorType.Equals },
                { ExpressionType.NotEqual, OperatorType.DoesNotEqual },
                { ExpressionType.GreaterThan, OperatorType.GreaterThan },
                { ExpressionType.GreaterThanOrEqual, OperatorType.GreaterThanOrEqualTo },
                { ExpressionType.LessThan, OperatorType.LessThan },
                { ExpressionType.LessThanOrEqual, OperatorType.LessThanOrEqualTo},
            };
        }

        public EntityQueryBuilder<T> SelectAll()
        {
            rootEntityQuery.TermKeys = GetTermKeys(rootModelType);

            return this;
        }

        public EntityQueryBuilder<T> Select(params Expression<Func<T, dynamic>>[] expressions)
        {
            foreach (var expression in expressions)
            {
                string termKey = string.Empty;

                if (expression.Body is UnaryExpression)
                {
                    UnaryExpression unaryExpression = (UnaryExpression)expression.Body;

                    termKey = GetTermKey((MemberExpression)unaryExpression.Operand);
                }
                else if (expression.Body is MemberExpression)
                {
                    termKey = GetTermKey((MemberExpression)expression.Body);
                }
                else
                {
                    throw new ArgumentException("Expressions must be either Unary or Member Expressions");
                }

                rootEntityQuery.TermKeys.Add(termKey);
            }

            return this;
        }

        public EntityQueryBuilder<T> SetEntityFilterConjunctionType(ConjunctionType conjunctionType)
        {
            rootEntityQuery.Filter.Conjunction = conjunctionType;

            return this;
        }

        public EntityQueryBuilder<T> Where(Expression<Func<T, bool>> predicate)
        {
            if (predicate.Body is BinaryExpression)
            {
                BinaryExpression binaryExpression = (BinaryExpression)predicate.Body;

                EntityFieldFilter entityFieldFilter = new EntityFieldFilter()
                {
                    TermKey = GetTermKey(binaryExpression),
                    Operator = operatorLookup[binaryExpression.NodeType],
                    Value = GetValue(binaryExpression)
                };

                rootEntityQuery.Filter.FieldFilters.Add(entityFieldFilter);

                return this;
            }
            else
            { 
                throw new ArgumentException($"The predicate must be a binary expression since filters are on a per field basis");
            }
        }

        public EntityQueryBuilder<T> Project(string name, EntityQuery childEntityQuery)
        {
            EntitySubQuery subQuery = new EntitySubQuery()
            {
                Name = name,
                EntityQuery = childEntityQuery
            };

            rootEntityQuery.Projections.Add(subQuery);

            return this;
        }

        public EntityQueryBuilder<T> Collect(string name, EntityQuery childEntityQuery)
        {
            EntitySubQuery subQuery = new EntitySubQuery()
            {
                Name = name,
                EntityQuery = childEntityQuery
            };

            rootEntityQuery.Collections.Add(subQuery);

            return this;
        }

        public EntityQuery ToQuery() => rootEntityQuery;

        public void Dispose() => rootEntityQuery = null;

        private List<string> GetTermKeys(Type modelType) =>
             modelType
            .GetProperties()
            .Select(p => p.GetCustomAttribute<TermKey>()?.Value)
            .Where(termKey => !string.IsNullOrEmpty(termKey))
            .ToList();

        private string GetTermKey(BinaryExpression binaryExpression)
        {
            MemberExpression left = (MemberExpression)binaryExpression.Left;

            return GetTermKey(left);
        }

        private string GetTermKey(MemberExpression memberExpression)
        {
            string memberName = memberExpression.Member.Name;

            return rootModelType.GetProperty(memberName).GetCustomAttribute<TermKey>()?.Value;
        }

        public EntityQueryBuilder<T> OrderBy(Expression<Func<T, object>> expression)
        {
            try
            {
                return InternalOrderBy(expression, EntitySortType.Ascending);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public EntityQueryBuilder<T> OrderByDescending(Expression<Func<T, object>> expression)
        {
            try
            {
                return InternalOrderBy(expression, EntitySortType.Descending);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private EntityQueryBuilder<T> InternalOrderBy(Expression<Func<T, object>> expression, EntitySortType entitySortType)
        {
            if (expression.Body is MemberExpression)
            {
                MemberExpression memberExpression = (MemberExpression)expression.Body;

                EntitySort entitySort = new EntitySort()
                {
                    TermKey = GetTermKey(memberExpression),
                    Type = entitySortType
                };

                rootEntityQuery.Sortings.Add(entitySort);

                return this;
            }
            else
            {
                throw new ArgumentException("The expression must be a MemberExpression.");
            }
        }

        private object GetValue(BinaryExpression binaryExpression)
        {
            if (binaryExpression.Right is ConstantExpression)
            {
                ConstantExpression right = (ConstantExpression)binaryExpression.Right;

                return right.Value;
            }
            else if (binaryExpression.Right is MemberExpression)
            {
                MemberExpression right = (MemberExpression)binaryExpression.Right;

                return Expression.Lambda(right).Compile().DynamicInvoke();
            }
            else
            {
                string error = $"The right hand side of the expression must either be a constant of a variable. '{binaryExpression.Right.Type}' is not supported";

                throw new ArgumentException(error);
            }
        }
    }
}
