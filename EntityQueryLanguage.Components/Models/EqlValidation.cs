using System;
using System.Collections.Generic;
using System.Text;

namespace EntityQueryLanguage.Components.Models
{
    public class EqlValidation
    {
        public bool IsValid => Errors.Count == 0;

        public List<string> Errors { get; set; } = new List<string>();
    }
}
