using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IniDotNet.Converters;
using Xunit;

namespace IniDotNet.Tests.Converters
{
    public class StandardConverterTests
    {
        [Theory]
        [InlineData(typeof(string), "123", "123")]
        [InlineData(typeof(int), "123", 123)]
        [InlineData(typeof(uint), "123", 123u)]
        [InlineData(typeof(bool), "true", true)]
        [InlineData(typeof(bool), "false", false)]
        [InlineData(typeof(bool), "0", false)]
        [InlineData(typeof(bool), "1", true)]
        public void ConvertsToCorrectType(Type type, string val, object expectedConvertedVal)
        {
            // Arrange.
            IConverter converter = new StandardConverter();

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
