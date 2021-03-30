using System;
using System.Collections.Generic;
using System.Text;

namespace EntityQueryLanguage.Components.Models
{
    public class SqlStatement
    {
        public string Sql { get; set; }

        public Dictionary<string, dynamic> Parameters { get; set; } = new Dictionary<string, dynamic>();
    }
}
