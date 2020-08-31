using System;
using MessageDefs;
namespace TCPChat_Client
{

    class Program
    {
        static void Main(string[] args)
        {
            ConfigHandler.WriteDefaultConfig();

            bool quitNow = false;
            while (!quitNow)
            {
                Console.WriteLine("Commands: \n");
                Console.WriteLine("/connect \n");
                Console.WriteLine("/config \n");
                Console.WriteLine("/quit \n");

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
            }
        }
        // Prepare connection information.
        public static void inputConnectInfo()
        {
            Console.WriteLine("Server IP:");
            string serverIp = Console.ReadLine();

            Console.WriteLine("Server Port:");
            string serverPort = Console.ReadLine();

            Connections.Connect(serverIp, Int32.Parse(serverPort));
        }
        public static void configHelp()
        {
            Console.Clear();

            Console.WriteLine("\nTo change your username and username color, change the userconfig.json in this directory.");
            Console.WriteLine("To view what colors are available, please visit:");
            Console.WriteLine("https://docs.microsoft.com/en-us/dotnet/api/system.consolecolor?view=netcore-3.1 \n");

        }
    }
}
