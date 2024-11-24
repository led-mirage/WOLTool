// © 2024 led-mirage. All rights reserved.

using System.Net;
using System.Net.NetworkInformation;

namespace WOLTool;

class Program
{
    private const string AppName = "WOLTool";
    private const string AppVersion = "1.0.0";
    private const string Copyright = "© 2024 led-mirage. All rights reserved.";

    private const int ExitCodeSuccess = 0;
    private const int ExitCodeError = 1;
    private const int ExitCodeAlreadyUp = 2;
    private const int ExitCodeTimeout = 3;

    private static ProgramOptions? _options = null;

    private static void ConsoleWriteLine(string message = "")
    {
        if (_options == null || !_options.Silent)
        {
            Console.WriteLine(message);
        }
    }

    private static void ConsoleWrite(string message = "")
    {
        if (_options == null || !_options.Silent)
        {
            Console.Write(message);
        }
    }

    private static void PrintAppInfo()
    {
        ConsoleWriteLine("---------------------------------------------------------------------------------------------------");
        ConsoleWriteLine($" {AppName} v{AppVersion}");
        ConsoleWriteLine();
        ConsoleWriteLine(" This tool sends a Wake-on-LAN (WOL) magic packet to the specified target host to wake it up.");
        ConsoleWriteLine();
        ConsoleWriteLine($" {Copyright}");
        ConsoleWriteLine("---------------------------------------------------------------------------------------------------");
        ConsoleWriteLine();
    }

    private static void PrintUsage()
    {
        ConsoleWriteLine("Usage:");
        ConsoleWriteLine("  WOLTool --mac_address <mac_address> [--hostname <hostname>] [--broadcast <broadcast_address>] [--port <port_number>] [--timeout <timeout_sec>] [--no-wait] [--silent] [--help]");
        ConsoleWriteLine();
        ConsoleWriteLine("Options:");
        ConsoleWriteLine("  --mac_address, -m   The MAC address of the target host.");
        ConsoleWriteLine("  --hostname, -H      The hostname or IP address of the target host.");
        ConsoleWriteLine("  --broadcast, -b     The broadcast address. Default is determined automatically.");
        ConsoleWriteLine("  --port, -p          The port number. Default is 9.");
        ConsoleWriteLine("  --timeout, -t       Maximum time (in seconds) to wait for the host to boot up. Default is 300 seconds.");
        ConsoleWriteLine("  --no-wait, -n       Do not wait for the host to boot up.");
        ConsoleWriteLine("  --silent, -s        Run without output.");
        ConsoleWriteLine("  --help, -h          Display this help message.");
        ConsoleWriteLine();
    }

    private static ProgramOptions GetOptions(string[] args)
    {
        string? hostname = null;
        string? macAddress = null;
        IPAddress? broadcast = null;
        int port = 9;
        int timeout = 300;
        bool wait = true;
        bool silent = false;

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--mac_address":
                case "-m":
                    if (args.Length > i + 1)
                    {
                        macAddress = args[i + 1];
                        try
                        {
                            WOLHelper.ParseMacAddress(macAddress);
                        }
                        catch
                        {
                            throw new ArgumentException("Invalid MAC address format.");
                        }
                    }
                    break;
                case "--hostname":
                case "-H":
                    if (args.Length > i + 1)
                    {
                        hostname = args[i + 1];
                    }
                    break;
                case "--broadcast":
                case "-b":
                    if (args.Length > i + 1)
                    {
                        if (!IPAddress.TryParse(args[i + 1], out broadcast))
                        {
                            throw new ArgumentException("Invalid broadcast address format.");
                        }
                    }
                    break;
                case "--port":
                case "-p":
                    if (args.Length > i + 1)
                    {
                        if (!int.TryParse(args[i + 1], out port) || port <= 0 || port > 65535)
                        {
                            throw new ArgumentException("Invalid port number.");
                        }
                    }
                    break;
                case "--timeout":
                case "-t":
                    if (args.Length > i + 1)
                    {
                        if (!int.TryParse(args[i + 1], out timeout) || timeout <= 0)
                        {
                            throw new ArgumentException("Invalid timeout value.");
                        }
                    }
                    break;
                case "--no-wait":
                case "-n":
                    wait = false;
                    break;
                case "--silent":
                case "-s":
                    silent = true;
                    break;
                case "--help":
                case "-h":
                    PrintAppInfo();
                    PrintUsage();
                    Environment.Exit(0);
                    break;
            }
        }

        if (macAddress == null)
        {
            throw new ArgumentException("MAC address are required.");
        }

        return new ProgramOptions {
            MacAddress = macAddress,
            Hostname = hostname,
            Broadcast = broadcast,
            Port = port,
            Timeout = timeout,
            Wait = wait,
            Silent = silent
        };
    }

    private static async Task<bool> IsHostAliveAsync(string hostname)
    {
        using (Ping ping = new Ping())
        {
            try
            {
                PingReply reply = await ping.SendPingAsync(hostname, 1000);
                return reply.Status == IPStatus.Success;
            }
            catch
            {
                return false;
            }
        }
    }

    private static async Task<int> Main(string[] args)
    {
        bool printAppInfo = false;

        try
        {
            _options = GetOptions(args);

            PrintAppInfo();
            printAppInfo = true;

            if (_options.Hostname != null && await IsHostAliveAsync(_options.Hostname))
            {
                ConsoleWriteLine($"{_options.Hostname} is already up.");
                return ExitCodeAlreadyUp;
            }

            if (_options.Broadcast == null)
            {
                _options.Broadcast = WOLHelper.GetBroadcastAddress();
            }

            ConsoleWriteLine($"Sending magic packet to broadcast address {_options.Broadcast}:{_options.Port}, targeting MAC {_options.MacAddress}.");
            WOLHelper.SendMagicPacket(_options.MacAddress, _options.Broadcast, _options.Port);
            ConsoleWriteLine("Magic packet sent.");
            ConsoleWriteLine();

            if (_options.Hostname != null && _options.Wait)
            {
                DateTime startTime = DateTime.Now;
                ConsoleWrite($"Checking host({_options.Hostname}) status");
                while (true)
                {
                    if (await IsHostAliveAsync(_options.Hostname))
                    {
                        TimeSpan elapsedTime = DateTime.Now - startTime;
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        ConsoleWriteLine($"\n{_options.Hostname} is up.");
                        ConsoleWriteLine($"It took {Math.Floor(elapsedTime.TotalSeconds)} seconds to boot.");
                        Console.ResetColor();
                        break;
                    }
                    else
                    {
                        TimeSpan elapsedTime = DateTime.Now - startTime;
                        if (elapsedTime.TotalSeconds > _options.Timeout)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            ConsoleWriteLine($"\nReached timeout ({_options.Timeout} seconds). Host did not respond.");
                            Console.ResetColor();
                            return ExitCodeTimeout;
                        }
                        ConsoleWrite(".");
                    }
                }
            }
            return ExitCodeSuccess;
        }
        catch (Exception e)
        {
            if (!printAppInfo) PrintAppInfo();
            ConsoleWriteLine(e.Message);
            ConsoleWriteLine();
            PrintUsage();
            return ExitCodeError;
        }
    }
}
