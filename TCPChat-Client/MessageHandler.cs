using CommonDefines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace TCPChat_Client
{
    class MessageHandler
    {
        /// Serialize the messageFormat with json to transmit.
        public static void SerializePrepareMessage(List<MessageFormat> message, TcpClient client, NetworkStream stream)
        {
            string json = JsonSerializer.Serialize(message);

            Byte[] data = Encoding.ASCII.GetBytes(json);
            List<Byte> byteToList = data.ToList();

            byteToList.Add(0x01); // Add end char

            Byte[] dataToArray = byteToList.ToArray();

            SendMessage(dataToArray, client, stream);
        }

        // Write to the stream, then continue looping for new console input.
        public static void SendMessage(byte[] message, TcpClient client, NetworkStream stream) 
        { 
            // Encrypt Message Data
            byte[] encrypt = Encryption.AESEncrypt(message, Encryption.AESKey, Encryption.AESIV);

            // Encrypt Key Data
            byte[] finalBytes = Encryption.AppendKeyToMessage(encrypt, Encryption.AESKey, Encryption.AESIV, message);

            stream.Write(finalBytes, 0, finalBytes.Length);

            InputMessage(client, stream);
        }

        // Console Read Loop
        public static void InputMessage(TcpClient client, NetworkStream stream)
        {
            new Thread(() =>
            {
                string messageString = Console.ReadLine();

                // Disallow sending empty information to stream.
                if (!string.IsNullOrWhiteSpace(messageString))
                {
                    List<MessageFormat> newMessage = new List<MessageFormat>();

                    Console.WriteLine("Client IP: {0}", ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());

                    // See the messageformat class in VariableDefines.
                    // The formatting for a client's message
                    newMessage.Add(new MessageFormat
                    {
                        messageType = MessageTypes.MESSAGE,
                        message = messageString,
                        Username = UserConfigFormat.userChosenName,
                        UserNameColor = UserConfigFormat.userChosenColor,
                        IP = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString()
                    });
                    SerializePrepareMessage(newMessage, client, stream);
                }
                else
                {
                    InputMessage(client, stream);
                }


            }).Start();
        }

        // The incoming messages are read and output.
        public static void ClientRecieveMessage(NetworkStream stream)
        {
            while (true)
            {
                Byte[] data = new Byte[8192]; // Unsure what this should be atm.
                Int32 bytes = stream.Read(data, 0, data.Length);


                string responseData = Encoding.ASCII.GetString(data, 0, bytes);

                //Console.WriteLine("Response Data: {0}", responseData);

                string text = MessageSerialization.ReturnEndOfStreamString(responseData);

                int typeOfList = MessageSerialization.FindTypeOfList(text);

                // This is where I put the actions for each type of message that the client has recieved.
                switch (typeOfList)
                {
                    case 0: // Nothing Found
                        List<MessageFormat> defaultList = Serialization.DeserializeDefault(text);
                        break;
                    case 1: // List<MessageFormat>
                        List<MessageFormat> messageList = Serialization.DeserializeMessageFormat(text);
                        OutputMessage.ClientRecievedMessageFormat(messageList);
                        break;
                    case 2: // List<ConntectedMessageFormat>
                        List<ConntectedMessageFormat> connectList = Serialization.DeserializeConntectedMessageFormat(text);
                        OutputMessage.ClientRecievedConnectedMessageFormat(connectList);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
