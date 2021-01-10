using CommonDefines;
using System;
using System.Collections.Generic;

namespace TCPChat_Client
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
                Option = "client",
                Action = ClientDataAction.Execute,
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
                Option = "ping",
                Action = PingAction.Execute,
            });
            Commands.commandList.Add(new CommandFormat
            {
                Option = "clear",
                Action = ClearAction.Execute,
            });
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
    class ExitAction
    {
        public static void Execute()
        {
            Environment.Exit(0);
        }
    }
    class ClientListAction
    {
        public static void Execute()
        {
            List<DataRequestFormat> message = new();

            // See the messageformat class in VariableDefines.
            // The formatting for a client's message
            message.Add(new DataRequestFormat
            {
                MessageType = MessageTypes.DATAREQUEST,
                DataType = CommandDataTypes.CLIENTLIST,
                Parameters = null
            });
            MessageHandler.PrepareMessage(message, Program.staticClient, Program.staticStream, true, false);
        }
    }

    // TODO Implement action to get certain data like color and time online from a specific user.
    class ClientDataAction
    {
        public static void Execute()
        {
            if (String.IsNullOrWhiteSpace(Commands.CommandArgument))
            {
                Console.WriteLine("Command requires parameters.");
                return;
            }
            Console.WriteLine("{0}", Commands.CommandArgument);
        }
    }
    class PingAction
    {
        public static void Execute()
        {
            List<DataRequestFormat> message = new();

            message.Add(new DataRequestFormat
            {
                MessageType = MessageTypes.DATAREQUEST,
                DataType = CommandDataTypes.PING,
                Parameters = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()
            });
            MessageHandler.PrepareMessage(message, Program.staticClient, Program.staticStream, true, false);
        }
    }
    class ClearAction
    {
        public static void Execute()
        {
            Console.Clear();
        }
    }
}
