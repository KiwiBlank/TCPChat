//#define DEVMODE

using CommonDefines;
using System;
using System.Net;

namespace TCPChat_Server
{

    class Program
    {

        static void Main(string[] args)
        {

            ConfigHandler.WriteDefaultConfig();
            Encryption.GenerateRSAKeys();
            Encryption.GenerateAESKeys();

            bool quitNow = false;
            while (!quitNow)
            {
                Console.Title = "TCPChat - Server";
                Console.WriteLine("Commands: \n");
                Console.WriteLine("/server \n");
                Console.WriteLine("/quit \n");
#if (DEVMODE)
                InputServerConfig();
#else
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
#endif

            }
        }
        public static void InputServerConfig()
        {
#if (DEVMODE)

            ServerHandler.StartServer("0.0.0.0", "6060");

#else

            Console.WriteLine("Port to listen on:");

            string serverPort = Console.ReadLine();

            // 0.0.0.0 To listen on all network interfaces
            ServerHandler.StartServer("0.0.0.0", serverPort);

#endif


        }
        public static string GetPublicIP()
        {
            WebClient webClient = new WebClient();

            // Easiest solution i can come up with.
            string serverIP = webClient.DownloadString("https://icanhazip.com/");

            return serverIP;
        }
    }
}
