using System;
using System.Collections.Generic;
using System.Text;

namespace EntityQueryLanguage.Components.Models
{
    public class EQLSettings
    {
        /// <summary>
        /// The maximum amount of records that can be returned by a query and deserialized. The default is 5,000 (inclusive).
        /// </summary>
        public int RecordLimit { get; set; } = 5000;
    }
}
