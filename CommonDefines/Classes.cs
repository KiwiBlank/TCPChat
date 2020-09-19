using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace CommonDefines
{
    [Serializable]

    public class ConntectedMessageFormat
    {
        public string connectMessage { get; set; }

        public string serverName { get; set; }

        public RSAParameters publicKey { get; set; }
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
        public string Username { get; set; }
        public ConsoleColor UserNameColor { get; set; }
    }
}
