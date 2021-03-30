using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using Xunit;
using EntityQueryLanguage.Components;
using System.Linq;

namespace EntityQueryLanguage.UnitTests
{
    public class ExtensionTests
    {
        [Fact(DisplayName = "Can add fields to an Expando Object dynamically")]
        public void ExpandoObjectTest()
        {
            ExpandoObject model = new ExpandoObject();

            model.AddField("Id", 1);

            var dictionary = (IDictionary<string, dynamic>)model; 

            Assert.Equal("Id", dictionary.Keys.First());
            Assert.Equal(1, dictionary.Values.First());
        }
    }
}
