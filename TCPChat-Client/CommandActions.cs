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
            Commands.commandList.Add(new CommandFormat
            {
                Option = "msg",
                Action = PrivateMessageAction.Execute,
                Alias = { },
                Help = "Input a client's ID & a message to send to a specific client."
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
        // TODO Show more channel information other than ID.
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

            // Make a request to the server to switch to a different channel.
            message.Add(new DataRequestFormat
            {
                MessageType = MessageTypes.DATAREQUEST,
                DataType = CommandDataTypes.CHANNELSWITCH,
                Parameters = Commands.CommandArguments[0]
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
    class PrivateMessageAction
    {
        public static void Execute()
        {
            List<DataRequestFormat> message = new();

            if (Commands.CommandArguments[0] == null || Commands.CommandArguments[1] == null)
            {
                return;
            }

            message.Add(new DataRequestFormat
            {
                MessageType = MessageTypes.DATAREQUEST,
                DataType = CommandDataTypes.PRIVATEMESSSAGE,
                Parameters = String.Format("{0}, {1}", Commands.CommandArguments[0], Commands.CommandArguments[1])
            });
            MessageHandler.PrepareMessage(message, Program.staticClient, Program.staticStream, true, false);
        }
    }
}
