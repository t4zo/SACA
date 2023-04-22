using System.Net;
using System.Net.NetworkInformation;

namespace SACA.Tests.Integration;

public static class OSUtility
{
    public static bool IsFree(int port)
    {
        IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
        IPEndPoint[] listeners = properties.GetActiveTcpListeners();
        int[] openPorts = listeners.Select(item => item.Port).ToArray<int>();
        return openPorts.All(openPort => openPort != port);
    }

    public static int NextFreePort(int port = 0) {
        port = (port > 0) ? port : new Random().Next(1, 65535);
        while (!IsFree(port)) {
            port += 1;
        }
        return port;
    }
}