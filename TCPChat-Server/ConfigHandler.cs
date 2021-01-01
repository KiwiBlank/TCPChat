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
                List<ServerConfigFormat> defaultConfig = new List<ServerConfigFormat>();

                defaultConfig.Add(new ServerConfigFormat { ServerName = "Your Server Name", ServerWelcomeMessage = "Welcome to my server!", ClientTimeBetweenMessages = 500 });

                string serialize = Serialization.Serialize(defaultConfig, true);

                File.WriteAllText(fileDir, serialize);

            }

            string configRead = File.ReadAllText(fileDir);
            ReadConfig(configRead);

        }
        public static void ReadConfig(string configRead)
        {
            List<ServerConfigFormat> userConfig = DeserializeConfig(configRead);

            ServerConfigFormat.serverChosenName = userConfig[0].ServerName;
            ServerConfigFormat.serverChosenWelcomeMessage = userConfig[0].ServerWelcomeMessage;
            ServerConfigFormat.serverChosenClientTime = userConfig[0].ClientTimeBetweenMessages;

        }

        public static List<ServerConfigFormat> DeserializeConfig(string config)
        {
            List<ServerConfigFormat> json = JsonSerializer.Deserialize<List<ServerConfigFormat>>(config);

            return json;
        }
    }
}
