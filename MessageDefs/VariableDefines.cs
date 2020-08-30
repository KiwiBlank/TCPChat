using System;
using System.Net;
using System.Net.Sockets;

namespace MessageDefs
{
    [Serializable]
    public class MessageFormat
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
}
