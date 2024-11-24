// © 2024 led-mirage. All rights reserved.

using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace WOLTool;

public class WOLHelper
{
    public static void SendMagicPacket(string macAddress, IPAddress? broadcast = null, int port = 9)
    {
        try
        {
            broadcast ??= GetBroadcastAddress();

            byte[] macBytes = ParseMacAddress(macAddress);

            using (UdpClient client = new UdpClient())
            {
                client.Connect(broadcast, port);

                byte[] packet = new byte[102];
                for (int i = 0; i < 6; i++)
                {
                    packet[i] = 0xFF;
                }
                for (int i = 6; i < packet.Length; i += macBytes.Length)
                {
                    Buffer.BlockCopy(macBytes, 0, packet, i, macBytes.Length);
                }

                client.Send(packet, packet.Length);
            }
        }
        catch (Exception e)
        {
            throw new MagicPacketException("Failed to send magic packet.", e);
        }
    }

    public static byte[] ParseMacAddress(string macAddress)
    {
        string[] macParts = macAddress.Split(':');
        if (macParts.Length != 6)
        {
            throw new ArgumentException("Invalid MAC address format. Expected format 'AA:BB:CC:DD:EE:FF'.");
        }

        byte[] macBytes = new byte[6];
        for (int i = 0; i < 6; i++)
        {
            try
            {
                macBytes[i] = Convert.ToByte(macParts[i], 16);
            }
            catch
            {
                throw new ArgumentException($"Invalid MAC address part: '{macParts[i]}'. Each part must be a hexadecimal byte.");
            }
        }

        return macBytes;
    }

    public static List<IPAddress> GetLocalIpAddresses()
    {
        var addresses = new List<IPAddress>();
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                addresses.Add(ip);
            }
        }
        return addresses;
    }

    public static IPAddress GetBroadcastAddress(IPAddress? ip = null)
    {
        if (ip == null)
        {
            List<IPAddress> ipAddresses = GetLocalIpAddresses();
            if (ipAddresses.Count == 0)
            {
                throw new InvalidOperationException("No local IP addresses found.");
            }
            ip = ipAddresses[0];
        }

        IPAddress? mask = GetSubnetMask(ip);
        if (mask == null)
        {
            throw new ArgumentException($"Can't find subnet mask for IP address '{ip}'");
        }
    
        byte[] ipAddressBytes = ip.GetAddressBytes();
        byte[] subnetMaskBytes = mask.GetAddressBytes();

        byte[] broadcastAddress = new byte[ipAddressBytes.Length];
        for (int i = 0; i < broadcastAddress.Length; i++)
        {
            broadcastAddress[i] = (byte)(ipAddressBytes[i] | (subnetMaskBytes[i] ^ 255));
        }
        return new IPAddress(broadcastAddress);
    }

    public static IPAddress? GetSubnetMask(IPAddress address)
    {
        foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
        {
            foreach (UnicastIPAddressInformation unicastIPAddressInfo in adapter.GetIPProperties().UnicastAddresses)
            {
                if (unicastIPAddressInfo.Address.Equals(address))
                {
                    return unicastIPAddressInfo.IPv4Mask;
                }
            }
        }
        return null;
    }
}
