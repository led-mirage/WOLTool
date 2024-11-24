# WOLTool

**WOLTool** is a lightweight command-line utility to send Wake-on-LAN (WOL) magic packets to remote devices. It also provides features to verify if the target device is powered on, making it a comprehensive tool for managing WOL operations.

## Features

- **Wake-on-LAN Magic Packet Sender:** Easily send WOL magic packets to specified devices using their MAC address.
- **Host Status Check:** Optionally verify if the target device is powered on using its hostname or IP address.
- **Configurable Parameters:**
    - Custom broadcast address and port.
    - Adjustable timeout for boot confirmation.
- **Silent Mode:** Suppress all output for script integration.
- **Cross-platform:** Runs on any platform supported by .NET.

## Installation

### Download Pre-built Executables

Pre-built executables for Windows, Linux, and macOS are available on the [Releases]( https://github.com/led-mirage/WOLTool/releases ) page.

#### VirusTotal Scan Result

All executables were scanned using VirusTotal to ensure safety. While the majority of antivirus programs detected no issues, the Windows version (`win-x64`) had **4 detections out of 72 scanners**. These detections are likely false positives caused by the single-file publishing process used for .NET applications.

#### Scan Results:

- [win-x64: 4/72 detected]( https://www.virustotal.com/gui/file/6a56e1f320cbe4ac400697f43807d2c045a8587e3141e2b5d02db9c7f4086c54?nocache=1 )
- [linux-x64: 0/65 detected]( https://www.virustotal.com/gui/file/fa924632d68004d8af7e270222aadf10ceb640c259c42101978d8858400e5ae5?nocache=1 )
- [osx-x64: 0/64 detected]( https://www.virustotal.com/gui/file/524bc5f4ee2d0f849dd0bbe14eee841943fb69703f82aa8136609d6f9d85b321?nocache=1 )

### Build from Source

If you prefer to build the application from source, you can do so using the .NET 8.0 SDK (8.0.404). Follow these steps:

1. **Install the .NET 8.0 SDK (8.0.404):**
    - Download it from the [.NET Download page]( https://dotnet.microsoft.com/download/dotnet/8.0 ).

2. **Clone the repository or download the source code:**
    ```sh
    git clone https://github.com/led-mirage/WOLTool.git
    ```

3. **Navigate to the project directory:**
    ```sh
    cd WOLTool
    ```

4. **Build the project:**
    ```sh
    dotnet build
    ```

5. **Run the application:**
    ```sh
    dotnet run -- [arguments]
    ```

### Publish for Multiple Platforms

You can publish the application as a single-file executable for Windows, Linux, and macOS using the included publish.bat script:

1. **Open a command prompt and navigate to the project directory.**

2. **Run the `publish.bat` script:**
    ```sh
    publish.bat
    ```

The script will create platform-specific executables in the following paths:  
- Windows: `bin/Release/net8.0/win-x64/publish`  
- Linux: `bin/Release/net8.0/linux-x64/publish`  
- macOS: `bin/Release/net8.0/osx-x64/publish`

## Usage

```text
Usage:
  WOLTool --mac_address <mac_address> [--hostname <hostname>] [--broadcast <broadcast_address>] [--port <port_number>] [--timeout <timeout_sec>] [--no-wait] [--silent] [--help]

Options:
  --mac_address, -m   The MAC address of the target host.
  --hostname, -H      The hostname or IP address of the target host.
  --broadcast, -b     The broadcast address. Default is determined automatically.
  --port, -p          The port number. Default is 9.
  --timeout, -t       Maximum time (in seconds) to wait for the host to boot up. Default is 300 seconds.
  --no-wait, -n       Do not wait for the host to boot up.
  --silent, -s        Run without output.
  --help, -h          Display this help message.
```

## Example

**Wake a Device Using its MAC Address**

```sh
WOLTool --mac_address AA:BB:CC:DD:EE:FF
```

**Wake a Device and Check if it Boots**

```sh
WOLTool --mac_address AA:BB:CC:DD:EE:FF --hostname 192.168.1.100
```

**Specify a Custom Broadcast Address and Port**

```sh
WOLTool --mac_address AA:BB:CC:DD:EE:FF --broadcast 192.168.1.255 --port 7
```

**Silent Mode for Scripting**

```sh
WOLTool --mac_address AA:BB:CC:DD:EE:FF --silent
```

## How It Works

WOLTool uses Wake-on-LAN (WOL) magic packets to remotely wake up devices. A magic packet is a specially formatted data packet containing the target device's MAC address repeated 16 times, prefixed by 6 bytes of `0xFF`. These packets are broadcast to the network, waking any compatible device that matches the MAC address.

## Exit Codes

| Exit Code | Description |
|-|-|
| 0 | Success. |
| 1 | General error. |
| 2 | The host is already powered on. |
| 3 | Timeout: the host did not respond. |

## License

WOLTool is licensed under the [MIT License](LICENSE). Feel free to use and modify it as you wish.

## Contributions

Contributions are welcome! Please open an issue or submit a pull request for any bug fixes, features, or improvements.

## Copyright

© 2024 led-mirage. All rights reserved.
