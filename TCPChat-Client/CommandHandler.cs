﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace TCPChat_Client
{
    public class CommandFormat
    {
        public string Option { get; set; }
        public Action Action { get; set; }
    }
    public class CommandHandler
    {
        // Keep a list of all commands.
        public static List<CommandFormat> commandList = new List<CommandFormat>();
        // A variable to update when an argument is written as a command.
        public static string CommandArgument;

        public static void GetCommandType(string command)
        {
            string commandOption = command.Split('/', ' ')[1];
            CommandArgument = command.Split(' ', command.Last())[1];

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
    }
}