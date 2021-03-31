using EntityQueryLanguage.Components.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityQueryLanguage.Tests.Shared
{
    public class SchemaProvider
    {
        public static EntitySchema GetSchema()
        {
            string json = System.IO.File.ReadAllText($"{AppContext.BaseDirectory}/eqlSchema.json");

            return JsonConvert.DeserializeObject<EntitySchema>(json);
        }
    }
}
