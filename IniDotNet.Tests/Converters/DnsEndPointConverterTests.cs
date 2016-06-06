using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using IniDotNet.Converters;
using Xunit;

namespace IniDotNet.Tests.Converters
{
    public class DnsEndPointConverterTests
    {
        [Theory]
        [InlineData("127.0.0.1", 1234)]
        [InlineData("www.google.co.uk", 80)]
        [InlineData("localhost", 443)]
        public void ConversionSucceeds(string host, int port)
        {
            // Arrange.
            IConverter converter = new DnsEndPointConverter();

            // Act.
            object result;
            bool converted = converter.TryConvertTo(typeof(DnsEndPoint), $"{host}:{port}", out result);
            DnsEndPoint ep = result as DnsEndPoint;

            // Assert.
            Assert.True(converted);
            Assert.NotNull(ep);
            Assert.Equal(host, ep.Host);
            Assert.Equal(port, ep.Port);
        }
    }
}
