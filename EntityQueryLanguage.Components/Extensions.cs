using EntityQueryLanguage.Components.Models;
using EntityQueryLanguage.Components.Services.Attributes;
using EntityQueryLanguage.Components.Services.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EntityQueryLanguage.Components
{
    public static class Extensions
    {
        public static void AddField(this ExpandoObject model, string propertyName, dynamic value) =>
            ((IDictionary<string, dynamic>)model).Add(propertyName, value);

        public static string UnquoteName(this string source) =>
            source.Replace("[", "").Replace("]", "");

        public static IServiceCollection UseSqlServer(this IServiceCollection services, string connectionString) =>
            services.AddScoped(typeof(IDbContext), _ => new SqlServerDbContext(connectionString));

        public static IServiceCollection UseEQL(
            this IServiceCollection services, 
            EQLSettings eqlSettings,
            EntitySchema entitySchema
        ) 
        {
            services
           .AddSingleton(eqlSettings)
           .AddSingleton(entitySchema);
            
             GetEqlServices()
            .ForEach(eqlService => 
                RegisterService(services, eqlService)
            );

            return services;
        }

        private static void RegisterService(IServiceCollection services, Type eqlService)
        {
            EqlServiceAttribute eqlServiceAttribute = eqlService.GetCustomAttribute<EqlServiceAttribute>();

            switch (eqlServiceAttribute.EqlServiceType)
            {
                case EqlServiceType.Scoped:
                    services.AddScoped(eqlServiceAttribute.Abstraction, eqlService);
                    break;
                case EqlServiceType.Singleton:
                    services.AddSingleton(eqlServiceAttribute.Abstraction, eqlService);
                    break;
                case EqlServiceType.Transient:
                    services.AddTransient(eqlServiceAttribute.Abstraction, eqlService);
                    break;
            }
        }

        private static List<Type> GetEqlServices() =>
             Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(type => type.GetCustomAttribute<EqlServiceAttribute>() != null)
            .ToList();
    }
}
