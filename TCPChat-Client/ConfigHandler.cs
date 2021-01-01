using CommonDefines;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
namespace TCPChat_Client
{
    class ConfigHandler
    {

        public static void WriteDefaultConfig()
        {
            string fileDir = String.Format(Path.Combine(Directory.GetCurrentDirectory(), "userconfig.json"));

            if (!File.Exists(fileDir))
            {
                List<UserConfigFormat> defaultConfig = new List<UserConfigFormat>();

                defaultConfig.Add(new UserConfigFormat { Username = Environment.MachineName, UserNameColor = ConsoleColor.Gray });

                string serialize = Serialization.Serialize(defaultConfig, true);

                File.WriteAllText(fileDir, serialize);

            }

            string configRead = File.ReadAllText(fileDir);
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
