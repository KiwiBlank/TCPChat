using CommonDefines;
using System;
using System.Net;

namespace TCPChat_Server
{
    class AddCommands
    {
        public static void Add()
        {
            // TODO Improve commands with help text, alias and more information.
            Commands.commandList.Add(new CommandFormat
            {
                Option = "help",
                Action = HelpAction.Execute,
                Alias = { "h" },
                Help = "This command lists all available commands and their description."
            });
            Commands.commandList.Add(new CommandFormat
            {
                Option = "exit",
                Action = ExitAction.Execute,
                Alias = { "q" },
                Help = "Exit the application."
            });
            Commands.commandList.Add(new CommandFormat
            {
                Option = "list",
                Action = ClientListAction.Execute,
                Alias = { },
                Help = "Queries the server and recieves a list of all connected users."
            });
            Commands.commandList.Add(new CommandFormat
            {
                Option = "clear",
                Action = ClearAction.Execute,
                Alias = { },
                Help = "Clear your console."
            });
            Commands.commandList.Add(new CommandFormat
            {
                Option = "banip",
                Action = BanIPAction.Execute,
                Alias = { },
                Help = "Input a client's ID to ban their IP from the server. See bans.dat to edit bans."
            });
            Commands.commandList.Add(new CommandFormat
            {
                Option = "kick",
                Action = KickAction.Execute,
                Alias = { },
                Help = "Input a client's ID to remove them from the server temporarily."
            });
        }
    }
    class ExitAction
    {
        public static void Execute()
        {
            Environment.Exit(0);
        }
    }
    class HelpAction
    {
        public static void Execute()
        {
            Console.WriteLine("The available commands are:");
            for (int i = 0; i < Commands.commandList.Count; i++)
            {
                Console.WriteLine("Command:");
                Console.WriteLine(String.Format("/{0}", Commands.commandList[i].Option));
                if (Commands.commandList[i].Alias.Count > 0)
                {
                    Console.WriteLine("Alias:");
                }
                for (int j = 0; j < Commands.commandList[i].Alias.Count; j++)
                {
                    Console.WriteLine(String.Format("/{0}", Commands.commandList[i].Alias[j]));
                }
                Console.WriteLine("Information:");
                Console.WriteLine(String.Format("{0}", Commands.commandList[i].Help));
                Console.WriteLine();
            }
        }
    }
    class ClientListAction
    {
        public static void Execute()
        {

            string clients = CommandHandler.ClientListString();
            clients = clients.Replace(",", Environment.NewLine);
            Console.WriteLine("These users are currently online:");
            Console.WriteLine(clients);
        }
    }
    class ClearAction
    {
        public static void Execute()
        {
            Console.Clear();
        }
    }
    class BanIPAction
    {
        public static void Execute()
        {
            for (int i = 0; i < ServerHandler.activeClients.Count; i++)
            {
                int outNum;
                bool parse = int.TryParse(Commands.CommandArgument, out outNum);
                if (parse && ServerHandler.activeClients[i].ID == outNum)
                {
                    string message = String.Format("({0}) {1} has been banned.", ServerHandler.activeClients[i].ID, ServerHandler.activeClients[i].Username);
                    ServerMessage.ServerGlobalMessage(ConsoleColor.Yellow, message);
                    Bans.AddNewBan(((IPEndPoint)ServerHandler.activeClients[i].TCPClient.Client.RemoteEndPoint).Address.ToString());
                    // Try / catch as an out of range solution. 
                    try
                    {
                        ServerHandler.activeClients[i].TCPClient.Close();
                    }
                    catch (ArgumentOutOfRangeException) { }
                    return;
                }
            }
        }
    }
    class KickAction
    {
        public static void Execute()
        {
            for (int i = 0; i < ServerHandler.activeClients.Count; i++)
            {
                int outNum;
                bool parse = int.TryParse(Commands.CommandArgument, out outNum);
                if (parse && ServerHandler.activeClients[i].ID == outNum)
                {
                    string message = String.Format("({0}) {1} has been kicked.", ServerHandler.activeClients[i].ID, ServerHandler.activeClients[i].Username);
                    ServerMessage.ServerGlobalMessage(ConsoleColor.Yellow, message);
                    // Try / catch as an out of range solution. 
                    try
                    {
                        ServerHandler.activeClients[i].TCPClient.Close();
                    }
                    catch (ArgumentOutOfRangeException) { }
                    return;
                }
            }
        }
    }
}
