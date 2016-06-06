using System;
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
            // Arrange.
            IFileParser parser = new DefaultFileParser();

            // Act.
            var sections = parser.Parse(new StringReader(string.Empty));

            // Assert.
            Assert.False(sections.Any());
        }

        [Fact]
        public void ParsesSingleSection()
        {
            // Arrange.
            IFileParser parser = new DefaultFileParser();
            string data = "[Foo]\nAbc = 123\nBar = hello";

            // Act.
            var sections = parser.Parse(new StringReader(data)).ToList();
            var section = sections.FirstOrDefault();

            // Assert.
            Assert.Equal(1, sections.Count);
            Assert.NotNull(section);

            Assert.Equal("Foo", section.Name);
            Assert.Equal(2, section.Contents.Count);

            Assert.True(section.Contents.ContainsKey("Abc"));
            Assert.Equal("123", section.Contents["Abc"]);

            Assert.True(section.Contents.ContainsKey("Bar"));
            Assert.Equal("hello", section.Contents["Bar"]);
        }

        [Fact]
        public void ParsesMultipleSections()
        {
            // Arrange.
            IFileParser parser = new DefaultFileParser();
            string data = "[Foo]\nAbc = 12\n[Bar]\nDef = 34";

            // Act.
            var sections = parser.Parse(new StringReader(data)).ToList();
            var sectionOne = sections.FirstOrDefault();
            var sectionTwo = sections.Skip(1).FirstOrDefault();

            // Assert.
            Assert.Equal(2, sections.Count);
            Assert.NotNull(sectionOne);
            Assert.NotNull(sectionTwo);

            Assert.Equal("Foo", sectionOne.Name);
            Assert.Equal(1, sectionOne.Contents.Count);

            Assert.True(sectionOne.Contents.ContainsKey("Abc"));
            Assert.Equal("12", sectionOne.Contents["Abc"]);

            Assert.Equal("Bar", sectionTwo.Name);
            Assert.Equal(1, sectionTwo.Contents.Count);

            Assert.True(sectionTwo.Contents.ContainsKey("Def"));
            Assert.Equal("34", sectionTwo.Contents["Def"]);
        }

        [Theory]
        [InlineData("Abc = =", "=")]
        [InlineData("Abc = ==", "==")]
        [InlineData("Abc==", "=")]
        [InlineData("Abc===", "==")]
        [InlineData("Abc = =foo", "=foo")]
        [InlineData("Abc = = foo", "= foo")]
        [InlineData("Abc = foo=bar", "foo=bar")]
        [InlineData("Abc = foo = bar", "foo = bar")]
        public void ParsesValuesWithEqualsTokenValue(string line, string expected)
        {
            // Arrange.
            IFileParser parser = new DefaultFileParser();
            string data = $"[Foo]\n{line}\n";

            // Act.
            var sections = parser.Parse(new StringReader(data)).ToList();
            var section = sections.Single();

            // Assert.
            Assert.Equal(expected, section.Contents.First().Value);
        }

        [Theory]
        [InlineData("=123")]
        [InlineData("= 123")]
        [InlineData(" =123")]
        [InlineData("  =123")]
        [InlineData(" = 123")]
        [InlineData("  = 123")]
        [InlineData("=")]
        [InlineData(" =")]
        [InlineData(" = ")]
        [InlineData("  = ")]
        [InlineData("   = ")]
        public void ThrowsExceptionWhenEmptyKeys(string line)
        {
            IFileParser parser = new DefaultFileParser();

            Assert.ThrowsAny<IniException>(
                () => parser.Parse(new StringReader($"[Foo]\n{line}\n")).ToList());
        }

        [Fact]
        public void ThrowsWhenDuplicateKey()
        {
            IFileParser parser = new DefaultFileParser(true);
            string data = "[Foo]\nAbc = 123\nAbc = 456\n";

            Assert.ThrowsAny<IniException>(
                () => parser.Parse(new StringReader(data)).ToList());
        }

        [Fact]
        public void DoesntThrowWhenDuplicateKeyOptionDisabled()
        {
            IFileParser parser = new DefaultFileParser(false);
            string data = "[Foo]\nAbc = 123\nAbc = 456\n";

            Exception ex = Record.Exception(
                () => parser.Parse(new StringReader(data)).ToList());

            Assert.Null(ex);
        }
    }
}
