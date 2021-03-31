using CommonDefines;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace TCPChat_Server
{
    public class ChannelFileFormat
    {
        public int ID { get; set; }
        public string Name { get; set; }
        // TODO Add more channel features. Such as permissions, descriptions etc.
    }

    public class ChannelHandler
    {
        public static string channelDataFileLocation;
        public static List<ChannelFileFormat> serverChannels;
        public static void CreateChannelDataFile()
        {
            channelDataFileLocation = String.Format(Path.Combine(Directory.GetCurrentDirectory(), "channels.json"));
            if (!ChannelDataFileExists())
            {
                WriteDefaultChannels();
            }
            ReadChannelsFile();

            if (!VerifyChannelStructure())
            {
                File.WriteAllText(channelDataFileLocation, "FILE FORMAT VERIFICATION FAILED.");
            }
        }
        public static bool ChannelDataFileExists()
        {
            if (File.Exists(channelDataFileLocation))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static void WriteDefaultChannels()
        {
            List<ChannelFileFormat> defaultChannels = new();
            // Default lobby id can be configured in serverconfig.json
            defaultChannels.Add(new ChannelFileFormat
            {
                ID = 0,
                Name = "Lobby",
            });
            defaultChannels.Add(new ChannelFileFormat
            {
                ID = 1,
                Name = "Chat 1",
            });
            defaultChannels.Add(new ChannelFileFormat
            {
                ID = 2,
                Name = "Chat 2",
            });

            string json = JsonSerializer.Serialize(defaultChannels, new JsonSerializerOptions { WriteIndented = true });

            File.Create(channelDataFileLocation).Dispose();
            File.WriteAllText(channelDataFileLocation, json);
        }
        public static void ReadChannelsFile()
        {
            if (ChannelDataFileExists())
            {
                string fileRead = File.ReadAllText(channelDataFileLocation);
                try
                {
                    serverChannels = JsonSerializer.Deserialize<List<ChannelFileFormat>>(fileRead);
                }
                catch (JsonException)
                {
                    // The error messages will be created in VerifyChannelStructure.
                }
            }
        }
        public static bool VerifyChannelStructure()
        {
            List<int> idList = new();
            if (!serverChannels.Any())
            {
                return false;
            }
            for (int i = 0; i < serverChannels.Count; i++)
            {
                // Check if there are any IDs that already exists.
                // Could do a loop, but linq seems easier in this case.
                if (idList.Contains(serverChannels[i].ID))
                {
                    return false;
                }
                idList.Add(serverChannels[i].ID);
                // Anything negative is reserved. May switch to unsigned int in the future.
                if (serverChannels[i].ID < 0)
                {
                    return false;
                }
                // Just a theoretical limit.
                if (serverChannels[i].ID > 128)
                {
                    return false;
                }
            }
            // Make sure at least the lobby channel exists.
            if (!idList.Contains(ServerConfigFormat.serverChosenDefaultChannelID))
            {
                return false;
            }
            return true;
        }
        public static void ClientRequestsChannelSwitch(ClientInstance instance, string parameters)
        {
            bool parse = int.TryParse(parameters, out int outNum);
            int index = ServerMessage.FindClientKeysIndex(instance.client);

            // If client has sent an invalid id.
            if (!parse)
            {
                // Simply send back the users current channel id, which has not changed.
                CommandHandler.ReplyToDataRequest(instance, ServerHandler.activeClients[index].ChannelID.ToString(), CommandDataTypes.CHANNELSWITCH);
                return;
            }

            // Check if the client's requested new channel id actually exists.
            bool foundID = false;
            for (int i = 0; i < serverChannels.Count; i++)
            {
                if (serverChannels[i].ID == outNum)
                {
                    foundID = true;
                }
            }
            if (!foundID)
            {
                // Simply send back the users current channel id, which has not changed.
                CommandHandler.ReplyToDataRequest(instance, ServerHandler.activeClients[index].ChannelID.ToString(), CommandDataTypes.CHANNELSWITCH);
                return;
            }
            // Check if client is trying to switch to the channel they are currently in.
            if (ServerHandler.activeClients[index].ChannelID == outNum)
            {
                // Simply send back the users current channel id, which has not changed.
                CommandHandler.ReplyToDataRequest(instance, ServerHandler.activeClients[index].ChannelID.ToString(), CommandDataTypes.CHANNELSWITCH);
                return;
            }

            ServerHandler.activeClients[index].ChannelID = outNum;

            CommandHandler.ReplyToDataRequest(instance, outNum.ToString(), CommandDataTypes.CHANNELSWITCH);

            string message = String.Format("({0}) {1} switched channel to: {2}", ServerHandler.activeClients[index].ID, ServerHandler.activeClients[index].Username, outNum.ToString());
            ServerMessage.ServerGlobalMessage(ConsoleColor.Yellow, message);
        }
    }
}
