using CommonDefines;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace TCPChat_Server
{
    class ConfigHandler
    {
        public static string configFilePath;
        public static string configFileName;
        public static string configFileCombined;

        public static void WriteDefaultConfig()
        {
            configFilePath = String.Format(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TCPChat"));
            configFileName = "serverconfig.json";
            configFileCombined = Path.Combine(configFilePath, configFileName);
            if (!Directory.Exists(configFilePath) || !File.Exists(configFileCombined))
            {
                List<ServerConfigFormat> defaultConfig = new();

                defaultConfig.Add(new ServerConfigFormat
                {
                    ServerName = "Your Server Name",
                    ServerWelcomeMessage = "Welcome to my server!",
                    ClientTimeBetweenMessages = 500,
                    DefaultChannelID = 0,
                    EnableVersionCheck = true
                });

                string serialize = Serialization.Serialize(defaultConfig, true);

                Directory.CreateDirectory(configFilePath);
                File.WriteAllText(configFileCombined, serialize);
            }

            string configRead = File.ReadAllText(configFileCombined);
            ReadConfig(configRead);

        }
        public static void ReadConfig(string configRead)
        {
            List<ServerConfigFormat> userConfig = JsonSerializer.Deserialize<List<ServerConfigFormat>>(configRead);

            ServerConfigFormat.serverChosenName = userConfig[0].ServerName;
            ServerConfigFormat.serverChosenWelcomeMessage = userConfig[0].ServerWelcomeMessage;
            ServerConfigFormat.serverChosenClientTime = userConfig[0].ClientTimeBetweenMessages;
            ServerConfigFormat.serverChosenDefaultChannelID = userConfig[0].DefaultChannelID;
            ServerConfigFormat.serverChosenVersionCheck = userConfig[0].EnableVersionCheck;
        }
    }
}
