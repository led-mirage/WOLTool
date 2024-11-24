// © 2024 led-mirage. All rights reserved.

namespace WOLTool;

public class MagicPacketException : Exception
{
    public MagicPacketException() { }

    public MagicPacketException(string message) : base(message) { }

    public MagicPacketException(string message, Exception inner) : base(message, inner) { }
}
