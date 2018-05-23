using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace CLMS.Business
{
    public class PingHelper
    {
        public async Task<PingReply> DoPing(string ipAddress)
        {
            Ping pingSender = new Ping();
            return await pingSender.SendPingAsync(ipAddress, 2000, new byte[] { 1, 1, 1 }, new PingOptions() { DontFragment = true, Ttl = 20 });
        }
    }
}
