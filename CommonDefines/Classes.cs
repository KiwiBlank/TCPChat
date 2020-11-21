using System;
using System.Net.Sockets;

namespace CommonDefines
{
    // Types of messages that can be sent, will be deprecated soon.
    public enum MessageTypes : int
    {
        MESSAGE = 0,
        CONNECTION = 1,
        WELCOME = 2,
        SERVER = 3,
        ENCRYPTED = 4
    }
    // Keeps a list of clients and their RSA public key.
    public class ClientList
    {
        public TcpClient TCPClient { get; set; }
        public byte[] RSAExponent { get; set; }
        public byte[] RSAModulus { get; set; }

    }
    public class ConnectionMessageFormat
    {
        public MessageTypes messageType { get; set; }
        // Client's public key data.
        public byte[] RSAExponent { get; set; }
        public byte[] RSAModulus { get; set; }
        public string ClientVersion { get; set; }

    }
    [Serializable]
    public class WelcomeMessageFormat
    {
        public MessageTypes messageType { get; set; }
        public string connectMessage { get; set; }
        public string serverName { get; set; }
        // The server's public key, that all users need to encrypt their message with.
        public byte[] keyExponent { get; set; }
        public byte[] keyModulus { get; set; }
        public string ServerVersion { get; set; }
    }
    [Serializable]
    // Inherits the userconfigformat as of now.
    public class MessageFormat : UserConfigFormat
    {
        public MessageTypes messageType { get; set; }
        public string message { get; set; }
    }
    [Serializable]
    // The format that the user config should follow.
    public class UserConfigFormat
    {
        // Variables to store the values that the config reader gets.
        public static ConsoleColor userChosenColor;
        public static string userChosenName;
        public string Username { get; set; }
        public ConsoleColor UserNameColor { get; set; }
    }
    public class ServerConfigFormat
    {
        // Variables to store the values that the config reader gets.
        public static string serverChosenName;
        public static string serverChosenWelcomeMessage;
        public string serverName { get; set; }
        public string serverWelcomeMessage { get; set; }
    }
}
