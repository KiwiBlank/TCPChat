using System;

namespace CommonDefines
{
    [Serializable]

    public class ConntectedMessageFormat
    {
        public string connectMessage { get; set; }

        public string serverName { get; set; }

        // The server's public key, that all users need to encrypt their message with.
        public byte[] keyExponent { get; set; }
        public byte[] keyModulus { get; set; }

    }
    [Serializable]
    // Inherits the userconfigformat as of now.
    public class MessageFormat : UserConfigFormat
    {
        public string message { get; set; }
        public string IP { get; set; }
    }
    [Serializable]
    // The format that the user config should follow.
    public class UserConfigFormat
    {
        public static ConsoleColor userChosenColor;
        public static string userChosenName;
        public string Username { get; set; }
        public ConsoleColor UserNameColor { get; set; }
    }
    public class ServerConfigFormat
    {
        public static string serverChosenName;
        public static string serverChosenWelcomeMessage;

        public string serverName { get; set; }
        public string serverWelcomeMessage { get; set; }
    }
}
