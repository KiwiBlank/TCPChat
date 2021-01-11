using CommonDefines;
using System;
using System.Collections.Generic;

namespace TCPChat_Client
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
                Option = "client",
                Action = ClientDataAction.Execute,
                Alias = { },
                Help = "WIP Not in use."
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
                Option = "channel",
                Action = ChannelSwitchAction.Execute,
                Alias = { "c" },
                Help = "Switch your channel to ID."
            });
            Commands.commandList.Add(new CommandFormat
            {
                Option = "channelinfo",
                Action = CurrentChannelAction.Execute,
                Alias = { "cinfo" },
                Help = "Get information about your current channel."
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
                Option = "ping",
                Action = PingAction.Execute,
                Alias = { },
                Help = "Ping the server and see the latency between client and server."
            });
            Commands.commandList.Add(new CommandFormat
            {
                Option = "clear",
                Action = ClearAction.Execute,
                Alias = { },
                Help = "Clear your console."
            });
        }
    }
    class HelpAction
    {
        public static void Execute()
        {
            Commands.HelpCommandCommon();
        }
    }
    class ExitAction
    {
        public static void Execute()
        {
            Environment.Exit(0);
        }
    }
    class CurrentChannelAction
    {
        public static void Execute()
        {
            Console.WriteLine("Current channel is ID: {0}", ClientRecievedTypes.CurrentChannelID);
        }
    }
    class ChannelSwitchAction
    {
        public static void Execute()
        {
            List<DataRequestFormat> message = new();

            // TODO Documentation
            message.Add(new DataRequestFormat
            {
                MessageType = MessageTypes.DATAREQUEST,
                DataType = CommandDataTypes.CHANNELSWITCH,
                Parameters = Commands.CommandArgument
            });
            MessageHandler.PrepareMessage(message, Program.staticClient, Program.staticStream, true, false);
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
