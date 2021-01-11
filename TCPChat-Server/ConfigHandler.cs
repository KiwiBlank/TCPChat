using CommonDefines;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace TCPChat_Server
{
    class ConfigHandler
    {

        public static void WriteDefaultConfig()
        {
            string fileDir = String.Format(Path.Combine(Directory.GetCurrentDirectory(), "serverconfig.json"));

            if (!File.Exists(fileDir))
            {
                List<ServerConfigFormat> defaultConfig = new();

                defaultConfig.Add(new ServerConfigFormat { 
                    ServerName = "Your Server Name", 
                    ServerWelcomeMessage = "Welcome to my server!", 
                    ClientTimeBetweenMessages = 500, 
                    DefaultChannelID = 0 
                });

                string serialize = Serialization.Serialize(defaultConfig, true);

                File.WriteAllText(fileDir, serialize);

            }

            string configRead = File.ReadAllText(fileDir);
            ReadConfig(configRead);

        }
        public static void ReadConfig(string configRead)
        {
            List<ServerConfigFormat> userConfig = JsonSerializer.Deserialize<List<ServerConfigFormat>>(configRead);

            ServerConfigFormat.serverChosenName = userConfig[0].ServerName;
            ServerConfigFormat.serverChosenWelcomeMessage = userConfig[0].ServerWelcomeMessage;
            ServerConfigFormat.serverChosenClientTime = userConfig[0].ClientTimeBetweenMessages;
            ServerConfigFormat.serverChosenDefaultChannelID = userConfig[0].DefaultChannelID;

        }
    }
}
