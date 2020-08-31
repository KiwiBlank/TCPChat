using System;
using System.Net;
using System.Net.Sockets;

namespace MessageDefs
{
    [Serializable]
    // Inherits the userconfigformat as of now.
    public class MessageFormat : UserConfigFormat
    {
        public string message { get; set; }
    }
    // TODO Implement this to better identify users.
    public class UserInfo
    {
        public string name { get; set; }

        public int id { get; set; }

        public EndPoint ip { get; set; }

        public TcpClient userInfoclient { get; set; }

    }

    [Serializable]
    public class UserConfigFormat
    {
        public string Username { get; set; }
        public ConsoleColor UserNameColor { get; set; }
    }
}
