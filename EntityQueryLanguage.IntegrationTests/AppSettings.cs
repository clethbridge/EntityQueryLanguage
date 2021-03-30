using EntityQueryLanguage.Components.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityQueryLanguage.IntegrationTests
{
    public class AppSettings
    {
        public string ConnectionString { get; set; }

        public EQLSettings EQL { get; set; }
    }
}
