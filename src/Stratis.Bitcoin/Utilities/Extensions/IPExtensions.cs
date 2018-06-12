﻿using System;
using System.Net;

namespace Stratis.Bitcoin.Utilities.Extensions
{
    public static class IPExtensions
    {
        /// <summary>Maps an end point to IPv6 if is not already mapped.</summary>
        public static IPEndPoint MapToIpv6(this IPEndPoint endPointv4)
        {
            if (endPointv4.Address.IsIPv4MappedToIPv6)
                return endPointv4;

            var mapped = new IPAddress(endPointv4.Address.GetAddressBytes()).MapToIPv6();
            var mappedIPEndPoint = new IPEndPoint(mapped, endPointv4.Port);
            return mappedIPEndPoint;
        }

        /// <summary>Match the end point with another by IP and port.</summary>
        public static bool Match(this IPEndPoint endPoint, IPEndPoint matchWith)
        {
            return endPoint.Address.ToString() == matchWith.MapToIpv6().Address.ToString() && endPoint.Port == matchWith.MapToIpv6().Port;
        }

        /// <summary>
        /// Converts a string to an IP endpoint.
        /// </summary>
        /// <param name="ipAddress">String to convert.</param>
        /// <param name="port">Port to use if <paramref name="ipAddress"/> does not specify it.</param>
        /// <returns>IP end point representation of the string.</returns>
        /// <remarks>
        /// IP addresses can have a port specified such that the format of <paramref name="ipAddress"/> is as such: address:port.
        /// IPv4 and IPv6 addresses are supported.
        /// In the case where the default port is passed and the IP address has a port specified in it, the IP address's port will take precedence.
        /// Examples of addresses that are supported are: 
        /// - 15.61.23.23
        /// - 15.61.23.23:1500
        /// - [1233:3432:2434:2343:3234:2345:6546:4534]
        /// - [1233:3432:2434:2343:3234:2345:6546:4534]:8333
        /// - ::ffff:192.168.4.1
        /// - ::ffff:192.168.4.1:80
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Thrown in case of the port number is out of range.</exception>    
        /// <exception cref="FormatException">Thrown in case of ipAddress is invalid.</exception>    
        public static IPEndPoint ToIPEndPoint(this string ipAddress, int port)
        {
            // Get the position of the last ':'.
            int colon = ipAddress.LastIndexOf(':');

            // If the last ':' is not followed by ']' or '.' then is must be an ip address / port number separator.
            if (colon >= 0 && ipAddress.IndexOf(']', colon) < 0 && ipAddress.IndexOf('.', colon) < 0)
            {
                port = int.Parse(ipAddress.Substring(colon + 1));
                ipAddress = ipAddress.Substring(0, colon);
            }

            return new IPEndPoint(IPAddress.Parse(ipAddress), port);
        }
    }
}