//#define DEVMODE

using CommonDefines;
using System;
namespace TCPChat_Client
{
    class Program
    {
        static void Main(string[] args)
        {
            ConfigHandler.WriteDefaultConfig();
            Encryption.GenerateKeyPair();
            Encryption.GenerateAESKeys();

            bool quitNow = false;
            while (!quitNow)
            {
                Console.WriteLine("Commands: \n");
                Console.WriteLine("/connect \n");
                Console.WriteLine("/config \n");
                Console.WriteLine("/quit \n");
#if (DEVMODE)
                inputConnectInfo();
#else
                string command = Console.ReadLine();
                switch (command)
                {
                    case "/connect":
                        inputConnectInfo();
                        break;
                    case "/config":
                        configHelp();
                        break;
                    case "/quit":
                        quitNow = true;
                        break;
                }
#endif



            }
        }
        // Prepare connection information.
        public static void inputConnectInfo()
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
        public static void configHelp()
        {
            Console.Clear();

            Console.WriteLine("\nTo change your username and username color, edit the userconfig.json in this directory.");
            Console.WriteLine("To view what colors are available, please visit:");
            Console.WriteLine("https://docs.microsoft.com/en-us/dotnet/api/system.consolecolor?view=netcore-3.1 \n");

        }
    }
}
