#define DEVMODE

using CommonDefines;
using System;
namespace TCPChat_Client
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
                Console.Title = "TCPChat - Client";
                Console.WriteLine("Commands: \n");
                Console.WriteLine("/connect \n");
                Console.WriteLine("/config \n");
                Console.WriteLine("/quit \n");
#if (DEVMODE)
                InputConnectInfo();
#else
                string command = Console.ReadLine();
                switch (command)
                {
                    case "/connect":
                        InputConnectInfo();
                        break;
                    case "/config":
                        ConfigHelp();
                        break;
                    case "/quit":
                        quitNow = true;
                        break;
                }
#endif



            }
        }
        // Prepare connection information.
        public static void InputConnectInfo()
        {
#if (DEVMODE)

            Connections.Connect("127.0.0.1", "6060");

#else

            Console.WriteLine("Server IP:");
            string serverIp = Console.ReadLine();

            Console.WriteLine("Server Port:");
            string serverPort = Console.ReadLine();

            Connections.Connect(serverIp, serverPort);

#endif

        }
        public static void ConfigHelp()
        {
            Console.Clear();

            Console.WriteLine("\nTo change your username and username color, edit the userconfig.json in this directory.");
            Console.WriteLine("To view what colors are available, please visit:");
            Console.WriteLine("https://docs.microsoft.com/en-us/dotnet/api/system.consolecolor?view=netcore-3.1 \n");

        }
    }
}
