using System.IO;
using System.Linq;
using IniDotNet.Parsers;
using Xunit;

namespace IniDotNet.Tests.Parsers
{
    public class DefaultFileParserTests
    {
        [Fact]
        public void ReturnsNothingWhenProvidedEmptyString()
        {
            IFileParser parser = new DefaultFileParser();
            var sections = parser.Parse(new StringReader(string.Empty));

            Assert.False(sections.Any());
        }
    }
}
