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

            Console.WriteLine("IP to listen on:");
            Console.WriteLine("Recommended Public IP is: {0}", GetPublicIP());
            Console.WriteLine("Use 127.0.0.1 to listen only on local network.");


            string serverIP = Console.ReadLine();

            ServerHandler.StartServer(serverPort, serverIP);
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
