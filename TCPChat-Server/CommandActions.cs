using CommonDefines;
using System;
using System.Net;

namespace TCPChat_Server
{
    class AddCommands
    {
        public static void Add()
        {
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
                Option = "clientinfo",
                Action = ClientDataAction.Execute,
                Alias = { "c" },
                Help = "Input a client's ID and get specific information such as username, IP etc."
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
            Commands.commandList.Add(new CommandFormat
            {
                Option = "global",
                Action = GlobalMessageAction.Execute,
                Alias = { },
                Help = "Input a message to send globally to all clients."
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
            Commands.HelpCommandCommon();
        }
    }
    class ClientDataAction
    {
        public static void Execute()
        {
            for (int i = 0; i < ServerHandler.activeClients.Count; i++)
            {
                int outNum;
                bool parse = int.TryParse(Commands.CommandArgument, out outNum);
                if (parse && ServerHandler.activeClients[i].ID == outNum)
                {
                    Console.WriteLine("ID: {0}", ServerHandler.activeClients[i].ID);
                    Console.WriteLine("Name: {0}", ServerHandler.activeClients[i].Username);
                    Console.WriteLine("IP: {0}", ((IPEndPoint)ServerHandler.activeClients[i].TCPClient.Client.RemoteEndPoint).Address.ToString());
                    Console.WriteLine("Username Color: {0}", ServerHandler.activeClients[i].UsernameColor);
                    Console.WriteLine("Current Channel ID: {0}", ServerHandler.activeClients[i].ChannelID);
                }
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
    class GlobalMessageAction
    {
        public static void Execute()
        {
            ServerMessage.ServerGlobalMessage(ConsoleColor.Yellow, Commands.CommandArgument);
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
