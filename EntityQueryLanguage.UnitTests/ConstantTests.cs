using EntityQueryLanguage.Components;
using System;
using Xunit;

namespace EntityQueryLanguage.UnitTests
{
    public class ConstantTests
    {
        [Theory(DisplayName = "Key words have correct hard-coded values")]
        [InlineData("SELECT", Constants.SELECT)]
        [InlineData("FROM", Constants.FROM)]
        public void KeyWords(string expected, string actual)
        {
            Assert.Equal(expected, actual);
        }
    }
}
