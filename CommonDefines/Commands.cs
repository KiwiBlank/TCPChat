using System;
using System.Collections.Generic;

namespace CommonDefines
{
    public class CommandFormat
    {
        public string Option { get; set; }
        public Action Action { get; set; }
        public string Help { get; set; }
        public List<string> Alias = new();
    }
    public class Commands
    {
        // Keep a list of all commands.
        public static List<CommandFormat> commandList = new();
        // A variable to update when an argument is written as a command.
        public static string CommandArgument;

        public static void GetCommandType(string command)
        {
            string commandOption = command.Split('/', ' ')[1];

            string argument = command.Substring(command.IndexOf(commandOption) + commandOption.Length);
            CommandArgument = argument.Replace(" ", "");

            bool foundCommand = false;

            for (int i = 0; i < commandList.Count; i++)
            {
                if (commandOption == commandList[i].Option)
                {
                    commandList[i].Action();
                    foundCommand = true;
                }
            }
            if (!foundCommand)
            {
                Console.WriteLine("ERROR: UNKNOWN COMMAND");
            }
        }
        public static void HelpCommandCommon()
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
}
