#define DEVMODE

using System;
using System.Net;

namespace TCPChat_Server
{

    class Program
    {

        static void Main(string[] args)
        {
            bool quitNow = false;
            while (!quitNow)
            {
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

            ServerHandler.StartServer("127.0.0.1", "6060");

#else

            Console.WriteLine("Port to listen on:");

            string serverPort = Console.ReadLine();

            string publicIP = GetPublicIP();
            Console.WriteLine("IP to listen on:");
            Console.WriteLine("Recommended Public IP is: {0}", publicIP);
            Console.WriteLine("Use 127.0.0.1 to listen only on local network.");


            string serverIP = Console.ReadLine();

            ServerHandler.StartServer(serverIP, serverPort);

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
