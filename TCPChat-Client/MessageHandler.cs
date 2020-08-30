using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using MessageDefs;

namespace TCPChat_Client
{
    class MessageHandler
    {
        /// Serialize the messageFormat with json to transmit.
        public static void SerializeMessage(List<MessageFormat> message, TcpClient client, NetworkStream stream)
        {

            // stream.Write(message, 0, message.Length);
            string json = JsonSerializer.Serialize(message);
            SendMessage(json, client, stream);

        }

        // Write to the stream, then continue looping for new console input.
        public static void SendMessage(string message, TcpClient client, NetworkStream stream)
        {
            Byte[] data = Encoding.ASCII.GetBytes(message);

            stream.Write(data, 0, message.Length);

            InputMessage(client, stream);
        }

        // Console Read Loop
        public static void InputMessage(TcpClient client, NetworkStream stream)
        {
            new Thread(() =>
            {
                string messageString = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(messageString))
                {
                    List<MessageFormat> newMessage = new List<MessageFormat>();
                    newMessage.Add(new MessageFormat { message = messageString });
                    SerializeMessage(newMessage, client, stream);
                }


            }).Start();
        }

        // The incoming messages are read and output.
        public static void ClientRecieveMessage(TcpClient client, NetworkStream stream)
        {
            while (true)
            {
                Byte[] data = new Byte[8192]; // Unsure what this should be atm.
                Int32 bytes = stream.Read(data, 0, data.Length);

                string responseData = Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine("{0}", responseData);
            }
        }
    }
}
