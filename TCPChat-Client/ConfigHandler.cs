using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using MessageDefs;
namespace TCPChat_Client
{
    class ConfigHandler
    {
        public static ConsoleColor userChosenColor;
        public static string userChosenName;

        public static void WriteDefaultConfig()
        {
            string fileDir = String.Format(Path.Combine(Directory.GetCurrentDirectory(), "userconfig.json"));

            if (!File.Exists(fileDir))
            {
                List<UserConfigFormat> defaultConfig = new List<UserConfigFormat>();

                defaultConfig.Add(new UserConfigFormat { Username = Environment.MachineName, UserNameColor = ConsoleColor.Gray });

                string serialize = SerializeConfig(defaultConfig);
                
                File.WriteAllText(fileDir, serialize);

            }

            string configRead = File.ReadAllText(fileDir);
            ReadConfig(configRead);

        }
        public static void ReadConfig(string configRead)
        {
            List<UserConfigFormat> userConfig = DeSerializeConfig(configRead);

            userChosenName = userConfig[0].Username;
            userChosenColor = userConfig[0].UserNameColor;

        }

        public static string SerializeConfig(List<UserConfigFormat> config)
        {
            string json = JsonSerializer.Serialize(config);

            return json;
        }
        public static List<UserConfigFormat> DeSerializeConfig(string config)
        {
            List<UserConfigFormat> json = JsonSerializer.Deserialize<List<UserConfigFormat>>(config);

            return json;
        }
    }
}
