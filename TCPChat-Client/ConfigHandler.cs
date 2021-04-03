using CommonDefines;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
namespace TCPChat_Client
{
    class ConfigHandler
    {
        public static string configFilePath;
        public static string configFileName;
        public static string configFileCombined;
        public static void WriteDefaultConfig()
        {
            configFilePath = String.Format(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "TCPChat"));
            configFileName = "clientconfig.json";
            configFileCombined = Path.Combine(configFilePath, configFileName);

            if (!Directory.Exists(configFilePath) || !File.Exists(configFileCombined))
            {
                List<UserConfigFormat> defaultConfig = new();

                defaultConfig.Add(new UserConfigFormat
                {
                    Username = Environment.MachineName,
                    UserNameColor = ConsoleColor.Gray
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
            List<UserConfigFormat> userConfig = DeserializeConfig(configRead);

            UserConfigFormat.userChosenName = userConfig[0].Username;
            UserConfigFormat.userChosenColor = userConfig[0].UserNameColor;

        }
        public static List<UserConfigFormat> DeserializeConfig(string config)
        {
            List<UserConfigFormat> json = JsonSerializer.Deserialize<List<UserConfigFormat>>(config);

            return json;
        }
    }
}
