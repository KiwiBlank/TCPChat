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
            });
            Commands.commandList.Add(new CommandFormat
            {
                Option = "exit",
                Action = ExitAction.Execute,
            });
            Commands.commandList.Add(new CommandFormat
            {
                Option = "list",
                Action = ClientListAction.Execute,
            });
            Commands.commandList.Add(new CommandFormat
            {
                Option = "clear",
                Action = ClearAction.Execute,
            });
            Commands.commandList.Add(new CommandFormat
            {
                Option = "banip",
                Action = BanIPAction.Execute,
            });
            Commands.commandList.Add(new CommandFormat
            {
                Option = "kick",
                Action = KickAction.Execute,
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
                Console.WriteLine(String.Format("/{0}", Commands.commandList[i].Option));
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
                    string message = String.Format("{0} has been banned.", ServerHandler.activeClients[i].Username);
                    ServerMessage.ServerGlobalMessage(ConsoleColor.Yellow, message);
                    Bans.AddNewBan(((IPEndPoint)ServerHandler.activeClients[i].TCPClient.Client.RemoteEndPoint).Address.ToString());
                    // Try / catch as an out of range solution. 
                    try
                    {
                        ServerHandler.activeClients[i].TCPClient.Close();
                    }
                    catch (ArgumentOutOfRangeException) {}
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
                    string message = String.Format("{0} has been kicked.", ServerHandler.activeClients[i].Username);
                    ServerMessage.ServerGlobalMessage(ConsoleColor.Yellow, message);
                    // Try / catch as an out of range solution. 
                    try
                    {
                        ServerHandler.activeClients[i].TCPClient.Close();
                    }
                    catch (ArgumentOutOfRangeException) {}
                    return;
                }
            }
        }
    }
}
