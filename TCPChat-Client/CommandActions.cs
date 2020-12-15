using System;

namespace TCPChat_Client
{
    class AddCommands
    {
        public static void Add()
        {

            CommandHandler.commandList.Add(new CommandFormat
            {
                Option = "help",
                Action = HelpAction.Execute,
            });
            CommandHandler.commandList.Add(new CommandFormat
            {
                Option = "client",
                Action = ClientDataAction.Execute,
            });
            CommandHandler.commandList.Add(new CommandFormat
            {
                Option = "exit",
                Action = ExitAction.Execute,
            });
        }
    }
    class HelpAction
    {
        public static void Execute()
        {
            Console.WriteLine("The available commands are:");
            for (int i = 0; i < CommandHandler.commandList.Count; i++)
            {
                Console.WriteLine(String.Format("/{0}", CommandHandler.commandList[i].Option));
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

    // TODO Implement action to get certain data like color and time online from a specific user.
    class ClientDataAction
    {
        public static void Execute()
        {
            if (String.IsNullOrWhiteSpace(CommandHandler.CommandArgument))
            {
                Console.WriteLine("Command requires parameters.");
                return;
            }
            Console.WriteLine("{0}", CommandHandler.CommandArgument);
        }
    }
}
