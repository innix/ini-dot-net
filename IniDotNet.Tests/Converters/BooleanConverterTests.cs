using System;
using IniDotNet.Converters;
using Xunit;

namespace IniDotNet.Tests.Converters
{
    public class BooleanConverterTests
    {
        [Theory]
        [InlineData(typeof(bool), "true", true)]
        [InlineData(typeof(bool), "True", true)]
        [InlineData(typeof(bool), "1", true)]
        [InlineData(typeof(bool), "yes", true)]
        [InlineData(typeof(bool), "Yes", true)]
        [InlineData(typeof(bool), "false", false)]
        [InlineData(typeof(bool), "False", false)]
        [InlineData(typeof(bool), "0", false)]
        [InlineData(typeof(bool), "no", false)]
        [InlineData(typeof(bool), "No", false)]
        public void ConvertsToCorrectValue(Type type, string val, object expectedConvertedVal)
        {
            // Arrange.
            IConverter converter = new BooleanConverter();

            // Act.
            object result;
            bool converted = converter.TryConvertTo(type, val, out result);

            // Assert.
            Assert.True(converted);
            Assert.IsType(type, result);
            Assert.Equal(result, expectedConvertedVal);
        }
    }
}
