using CommonDefines;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TCPChat_Client
{
    public class CommandHandler
    {
        public static void RecievedDataReply(List<DataReplyFormat> list)
        {
            switch (list[0].DataType)
            {
                case CommandDataTypes.CLIENTLIST:
                    RecievedClientList(list[0].Data);
                    break;
                case CommandDataTypes.PING:
                    RecievedPingReply(list[0].Data);
                    break;
                default:
                    break;
            }
        }
        public static void RecievedClientList(string data)
        {
            data = data.Replace(",", Environment.NewLine);
            Console.WriteLine("These users are currently online:");
            Console.WriteLine(data);
        }
        public static void RecievedPingReply(string data)
        {
            Console.WriteLine("Your latency to the server is: {0} ms", data);
        }
    }
}
