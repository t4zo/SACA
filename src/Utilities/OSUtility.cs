using System.Net.NetworkInformation;

namespace SACA.Utilities;

public static class OSUtility
{
    private static bool IsPortFree(int port)
    {
        var properties = IPGlobalProperties.GetIPGlobalProperties();
        var listeners = properties.GetActiveTcpListeners();
        var openPorts = listeners.Select(item => item.Port).ToArray<int>();
        
        return openPorts.All(openPort => openPort != port);
    }

    public static int NextFreePort(int port = 0) {
        port = (port > 0) ? port : new Random().Next(1, 65535);
        while (!IsPortFree(port)) {
            port += 1;
        }
        
        return port;
    }
}