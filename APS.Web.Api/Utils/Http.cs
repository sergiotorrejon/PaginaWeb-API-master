using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace APS.Web.Api.Utils
{

    public static class Http
    {
        public static async Task<string> GetIPAddressAsync()
        {
            var hostname = Environment.MachineName;
            var host = (await Dns.GetHostEntryAsync(hostname));

            foreach (IPAddress IP in host.AddressList)
            {
                if (IP.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return Convert.ToString(IP);
                }
            }

            return null;
        }
    }
}
