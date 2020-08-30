using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using MessageDefs;

namespace TCPChat_Server
{

    class Program
    {

        static void Main(string[] args)
        {
            bool quitNow = false;
            while (!quitNow)
            {
                string command = Console.ReadLine();
                switch (command)
                {
                    case "/server":
                        InputServerConfig();
                        break;
                    case "/quit":
                        quitNow = true;
                        break;
                }
            }
        }
        public static void InputServerConfig()
        {
            Console.WriteLine("Port to listen on:");
            string serverPort = Console.ReadLine();
            ServerHandler.StartServer(Int32.Parse(serverPort));
        }
    }
}
