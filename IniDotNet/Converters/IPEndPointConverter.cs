using System;
using System.Net;

namespace IniDotNet.Converters
{
    public sealed class IPEndPointConverter : IConverter
    {
        public bool TryConvertTo(Type type, string stringValue, out object converted)
        {
            if (type != typeof(IPEndPoint))
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

            IPAddress ipAddress;
            if (!IPAddress.TryParse(stringValue.Substring(0, idx), out ipAddress))
            {
                converted = null;
                return false;
            }

            int port;
            if (!int.TryParse(stringValue.Substring(idx + 1), out port))
            {
                converted = null;
                return false;
            }

            converted = new IPEndPoint(ipAddress, port);
            return true;
        }
    }
}
