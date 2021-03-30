using EntityQueryLanguage.Components;
using EntityQueryLanguage.Components.Models;
using EntityQueryLanguage.Components.Services.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityQueryLanguage.IntegrationTests
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            AppSettings appSettings = Get<AppSettings>("appsettings");

            EntitySchema entitySchema = Get<EntitySchema>("eqlSchema");

            services
                .UseEQL(appSettings.EQL, entitySchema)
                .UseSqlServer(appSettings.ConnectionString);
        }

        private T Get<T>(string fileName)
        { 
            string json = System.IO.File.ReadAllText($"{AppContext.BaseDirectory}/{fileName}.json");

            T model = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);

            return model;
        }
    }
}
