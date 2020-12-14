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
        public string Username { get; set; }
        public byte[] RSAExponent { get; set; }
        public byte[] RSAModulus { get; set; }

    }
    public class ServerMessageFormat
    {
        public MessageTypes MessageType { get; set; }
        public byte[] RSAExponent { get; set; }
        public byte[] RSAModulus { get; set; }
        public ConsoleColor Color { get; set; }
        public string Message { get; set; }
    }
    public class ConnectionMessageFormat
    {
        public MessageTypes MessageType { get; set; }
        public string Username { get; set; }
        // Client's public key data.
        public byte[] RSAExponent { get; set; }
        public byte[] RSAModulus { get; set; }
        public string ClientVersion { get; set; }

    }
    public class WelcomeMessageFormat
    {
        public MessageTypes MessageType { get; set; }
        public string ConnectMessage { get; set; }
        public string ServerName { get; set; }
        // The server's public key, that all users need to encrypt their message with.
        public byte[] RSAExponent { get; set; }
        public byte[] RSAModulus { get; set; }
        public string ServerVersion { get; set; }
    }
    // Inherits the userconfigformat as of now.
    public class MessageFormat : UserConfigFormat
    {
        public MessageTypes MessageType { get; set; }
        public string Message { get; set; }
    }
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
        public string ServerName { get; set; }
        public string ServerWelcomeMessage { get; set; }
    }
}
