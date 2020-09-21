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

                defaultConfig.Add(new ServerConfigFormat { serverName = "Your Server Name", serverWelcomeMessage = "Welcome to my server!" });

                string serialize = SerializeConfig(defaultConfig);

                File.WriteAllText(fileDir, serialize);

            }

            string configRead = File.ReadAllText(fileDir);
            ReadConfig(configRead);

        }
        public static void ReadConfig(string configRead)
        {
            List<ServerConfigFormat> userConfig = DeserializeConfig(configRead);

            ServerConfigFormat.serverChosenName = userConfig[0].serverName;
            ServerConfigFormat.serverChosenWelcomeMessage = userConfig[0].serverWelcomeMessage;

        }

        public static string SerializeConfig(List<ServerConfigFormat> config)
        {
            string json = JsonSerializer.Serialize(config);

            return json;
        }
        public static List<ServerConfigFormat> DeserializeConfig(string config)
        {
            List<ServerConfigFormat> json = JsonSerializer.Deserialize<List<ServerConfigFormat>>(config);

            return json;
        }
    }
}
