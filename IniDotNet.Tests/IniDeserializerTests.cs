using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace IniDotNet.Tests
{
    public class IniDeserializerTests
    {
        [Fact]
        public void DeserializesIntoDictionary()
        {
            // Arrange.
            var deserializer = new IniDeserializer();

            // Act.
            var dict = deserializer.Deserialize("Abc = 321\n[Foo]\nBar = 123\n");

            // Assert.
            Assert.Equal(2, dict.Count);

            var first = dict.First();
            var second = dict.ElementAt(1);

            Assert.Equal("", first.Key);
            Assert.Equal(1, first.Value.Count);
            Assert.True(first.Value.ContainsKey("Abc"));
            Assert.Equal("321", first.Value["Abc"]);

            Assert.Equal("Foo", second.Key);
            Assert.Equal(1, second.Value.Count);
            Assert.True(second.Value.ContainsKey("Bar"));
            Assert.Equal("123", second.Value["Bar"]);
        }

        [Fact]
        public void DeserializesTopLevelIntoDictionary()
        {
            // Arrange.
            var deserializer = new IniDeserializer();

            // Act.
            var dict = deserializer.Deserialize("Bar = 123\n");

            // Assert.
            Assert.Equal(1, dict.Count);
            Assert.Equal("", dict.First().Key);
            Assert.Equal(1, dict.First().Value.Count);
            Assert.True(dict.First().Value.ContainsKey("Bar"));
            Assert.Equal("123", dict.First().Value["Bar"]);
        }

        [Fact]
        public void DeserializesIntoPoco()
        {
            // Arrange.
            var deserializer = new IniDeserializer();

            // Act.
            var cfg = deserializer.Deserialize<Config>("Number = 321\n[Foo]\nBar = 123\n");

            // Assert.
            Assert.NotNull(cfg);
            Assert.NotNull(cfg.Foo);
            Assert.Equal(321, cfg.Number);
            Assert.Equal(123, cfg.Foo.Bar);
        }

        [Fact]
        public void DeserializesIntoPocoWithAttributes()
        {
            // Arrange.
            var deserializer = new IniDeserializer();

            // Act.
            var cfg = deserializer.Deserialize<ConfigWithAttributes>("Num = 321\n[Fuzz]\nBuzz = 123\n");

            // Assert.
            Assert.NotNull(cfg);
            Assert.NotNull(cfg.Foo);
            Assert.Equal(321, cfg.Number);
            Assert.Equal(123, cfg.Foo.Bar);
        }

        [Fact]
        public void DeserializesSectionIntoDictionary()
        {
            // Arrange.
            var deserializer = new IniDeserializer();

            // Act.
            IDictionary<string, string> dict = deserializer.DeserializeSection("[Foo]\nBar = 123\n", "Foo");

            // Assert.
            Assert.Equal(1, dict.Count);
            Assert.True(dict.ContainsKey("Bar"));
            Assert.Equal("123", dict["Bar"]);
        }

        [Fact]
        public void DeserializesTopLevelSectionIntoDictionary()
        {
            // Arrange.
            var deserializer = new IniDeserializer();

            // Act.
            IDictionary<string, string> dict = deserializer.DeserializeSection("\nBar = 123\n", "");

            // Assert.
            Assert.Equal(1, dict.Count);
            Assert.True(dict.ContainsKey("Bar"));
            Assert.Equal("123", dict["Bar"]);
        }

        [Fact]
        public void DeserializesSectionIntoPoco()
        {
            // Arrange.
            var deserializer = new IniDeserializer();

            // Act.
            var foo = deserializer.DeserializeSection<FooSection>("[Foo]\nBar = 123\n", "Foo");

            // Assert.
            Assert.NotNull(foo);
            Assert.Equal(123, foo.Bar);
        }

        [Fact]
        public void DeserializesSectionIntoPocoWithAttributes()
        {
            // Arrange.
            var deserializer = new IniDeserializer();

            // Act.
            var foo = deserializer.DeserializeSection<FooSectionWithAttributes>("[Fuzz]\nBuzz = 123\n", "Fuzz");

            // Assert.
            Assert.NotNull(foo);
            Assert.Equal(123, foo.Bar);
        }

        private class Config
        {
            public int Number { get; set; }

            public FooSection Foo { get; set; }
        }

        private class FooSection
        {
            public int Bar { get; set; }
        }

        private class ConfigWithAttributes
        {
            [IniProperty("Num")]
            public int Number { get; set; }

            [IniSection("Fuzz")]
            public FooSectionWithAttributes Foo { get; set; }
        }

        private class FooSectionWithAttributes
        {
            [IniProperty("Buzz")]
            public int Bar { get; set; }
        }
    }
}
