using EntityQueryLanguage.Components.Models;
using EntityQueryLanguage.Components.Services.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EntityQueryLanguage.Components.Services.Deserialization
{

    public interface IModelDeserializer
    {
        T Deserialize<T>(DataRow dataRow);
    }

    [EqlService(typeof(IModelDeserializer))]
    public class ModelDeserializer: IModelDeserializer
    {
        public T Deserialize<T>(DataRow dataRow)
        {
            Type modelType = typeof(T);
            
            T instance = Activator.CreateInstance<T>();
            
            var properties = GetTermKeyProperties(modelType);

            foreach (PropertyInfo property in properties)
            {
                TermKey termKey = property.GetCustomAttribute<TermKey>();

                object value = dataRow[termKey.Value];

                if (value != DBNull.Value)
                {
                    property.SetValue(instance, value);
                }
            }

            return instance;
        }

        private IEnumerable<PropertyInfo> GetTermKeyProperties(Type modelType) =>
             modelType
            .GetProperties()
            .Where(property => property.GetCustomAttribute<TermKey>() != null);
    }
}
