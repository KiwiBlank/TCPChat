﻿using System;
using System.Reflection;

namespace TCPChat_Server
{
    public class VersionHandler
    {
        public static bool VersionCheck(ClientInstance instance, string clientVersion)
        {
            // Check if versions do not match, if not close connection.
            if (clientVersion != Assembly.GetExecutingAssembly().GetName().Version.ToString())
            {
                string message = String.Format("Your version of: {0} does not match the server version of: {1}",
                    clientVersion,
                    Assembly.GetExecutingAssembly().GetName().Version.ToString());

                ServerMessage.ServerClientMessage(instance.client, ConsoleColor.Yellow, message);

                instance.client.Close();
                return false;
            }
            return true;
        }
    }
}
