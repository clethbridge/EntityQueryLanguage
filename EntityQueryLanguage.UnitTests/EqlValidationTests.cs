using EntityQueryLanguage.Components.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace EntityQueryLanguage.UnitTests
{
    public class EqlValidationTests
    {
        [Fact(DisplayName = "An EqlValidation has an empty list of errors by default")]
        public void Init()
        {
            EqlValidation eqlValidation = new EqlValidation();

            Assert.NotNull(eqlValidation.Errors);
            Assert.Empty(eqlValidation.Errors);
        }

        [Fact(DisplayName = "An EqlValidation is valid when it has no errors")]
        public void Valid()
        {
            EqlValidation eqlValidation = new EqlValidation();

            Assert.True(eqlValidation.IsValid);
        }

        [Fact(DisplayName = "An EqlValidation is invalid when it has errors")]
        public void Invalid()
        {
            EqlValidation eqlValidation = new EqlValidation();

            eqlValidation.Errors.Add("Error");

            Assert.False(eqlValidation.IsValid);
        }
    }
}
