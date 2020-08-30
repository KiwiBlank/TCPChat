using System;

namespace TCPChat_Client
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
                    case "/connect":
                        inputConnectInfo();
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
    }
}
