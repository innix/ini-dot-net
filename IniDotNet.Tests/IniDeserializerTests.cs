using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace IniDotNet.Tests
{
    public class IniDeserializerTests
    {
        [Fact]
        public void DeserializesIntoDictionary()
        {
            var deserializer = new IniDeserializer();
            var dict = deserializer.Deserialize("[Foo]\nBar = 123\n");

            Assert.Equal(1, dict.Count);
            Assert.Equal("Foo", dict.First().Key);
            Assert.Equal(1, dict.First().Value.Count);
            Assert.True(dict.First().Value.ContainsKey("Bar"));
            Assert.Equal("123", dict.First().Value["Bar"]);
        }

        [Fact]
        public void DeserializesIntoPoco()
        {
            var deserializer = new IniDeserializer();
            var cfg = deserializer.Deserialize<FooConfig>("[Foo]\nBar = 123\n");

            Assert.NotNull(cfg);
            Assert.NotNull(cfg.Foo);
            Assert.Equal(123, cfg.Foo.Bar);
        }

        [Fact]
        public void DeserializesSectionIntoDictionary()
        {
            var deserializer = new IniDeserializer();
            IDictionary<string, string> dict = deserializer.DeserializeSection("[Foo]\nBar = 123\n", "Foo");

            Assert.Equal(1, dict.Count);
            Assert.True(dict.ContainsKey("Bar"));
            Assert.Equal("123", dict["Bar"]);

        }

        [Fact]
        public void DeserializesSectionIntoPoco()
        {
            var deserializer = new IniDeserializer();
            var foo = deserializer.DeserializeSection<Foo>("[Foo]\nBar = 123\n", "Foo");

            Assert.NotNull(foo);
            Assert.Equal(123, foo.Bar);
        }

        private class FooConfig
        {
            public Foo Foo { get; set; }
        }

        private class Foo
        {
            public int Bar { get; set; }
        }
    }
}
