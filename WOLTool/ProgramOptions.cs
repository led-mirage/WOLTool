// © 2024 led-mirage. All rights reserved.

using System.Net;

namespace WOLTool;

public class ProgramOptions
{
    public string MacAddress { get; set; } = "";

    public string? Hostname { get; set; } = null;

    public IPAddress? Broadcast { get; set; } = null;

    public int Port { get; set; } = 9;

    public int Timeout { get; set; } = 300;

    public bool Wait { get; set; } = true;

    public bool Silent { get; set; } = false;
}
