using System;
using System.Net;

namespace IniDotNet.Converters
{
    public sealed class DnsEndPointConverter : IConverter
    {
        public bool TryConvertTo(Type type, string stringValue, out object converted)
        {
            if (type != typeof(DnsEndPoint))
            {
                converted = null;
                return false;
            }

            int idx = stringValue.IndexOf(':');
            if (idx == -1)
            {
                converted = null;
                return false;
            }

            string host = stringValue.Substring(0, idx);

            int port;
            if (!int.TryParse(stringValue.Substring(idx + 1), out port))
            {
                converted = null;
                return false;
            }

            try
            {
                converted = new DnsEndPoint(host, port);
                return true;
            }
            catch (ArgumentException)
            {
                converted = null;
                return false;
            }
        }
    }
}
