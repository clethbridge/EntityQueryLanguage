using EntityQueryLanguage.Components.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityQueryLanguage.Components.Services.Builders
{
    public interface IMutationStatementBuilder
    {
        SqlStatement Build(EntityMutation entityMutation, Dictionary<string, dynamic> model);
    }
}
