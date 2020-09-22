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
        public static void SerializeMessage(List<MessageFormat> message, TcpClient client, NetworkStream stream)
        {

            // stream.Write(message, 0, message.Length);
            string json = JsonSerializer.Serialize(message);
            SendMessage(json, client, stream);

        }

        public static byte[] AppendKeyToMessage(byte[] data, byte[] IV, byte[] key, byte[] data2)
        {
            List<byte> listKey = new List<byte>();
            List<byte> listMain = new List<byte>();

            // Add the keys to a separate list
            listKey.AddRange(key);
            listKey.AddRange(IV);
            byte[] byteKeyArray = listKey.ToArray();

            // 256 Bytes
            byte[] encryptKey = Encryption.EncryptData(byteKeyArray, Encryption.clientCopyOfServerPublicKey);

            listMain.AddRange(encryptKey);
            listMain.AddRange(data);
            byte[] finalBytes = listMain.ToArray();


            return finalBytes;


        }
        // Write to the stream, then continue looping for new console input.
        public static void SendMessage(string message, TcpClient client, NetworkStream stream)
        {
            Byte[] data = Encoding.ASCII.GetBytes(message);


            // See Server's MessageHandler (FindEndOfStream method)
            List<Byte> byteToList = data.ToList();
            byteToList.Add(0x01); // Used to indicate when data should end.

            //byte[] testdata = Convert.FromBase64String(message);
            Byte[] dataToArray = byteToList.ToArray();

            // Encrypt Message Data
            byte[] encrypt = Encryption.AESEncrypt(dataToArray, Encryption.AESKey, Encryption.AESIV);

            // Encrypt Key Data
            byte[] finalBytes = AppendKeyToMessage(encrypt, Encryption.AESKey, Encryption.AESIV, dataToArray);


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
                        message = messageString,
                        Username = UserConfigFormat.userChosenName,
                        UserNameColor = UserConfigFormat.userChosenColor,
                        IP = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString()
                    });
                    SerializeMessage(newMessage, client, stream);
                }
                else
                {
                    InputMessage(client, stream);
                }


            }).Start();
        }
        public static void ClientRecievedMessageFormat(List<MessageFormat> list)
        {
            OutputMessage.OutputMessageWithColor(list[0].message, list[0].IP, list[0].Username, list[0].UserNameColor);
        }
        public static void ClientRecievedConnectedMessageFormat(List<ConntectedMessageFormat> list)
        {
            // Output Info
            Console.WriteLine(list[0].serverName);
            Console.WriteLine(list[0].connectMessage);

            // Encryption
            RSAParameters key = Encryption.RSAParamaterCombiner(list[0].keyModulus, list[0].keyExponent);
            Encryption.clientCopyOfServerPublicKey = key;

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
                        ClientRecievedMessageFormat(messageList);
                        break;
                    case 2: // List<ConntectedMessageFormat>
                        List<ConntectedMessageFormat> connectList = Serialization.DeserializeConntectedMessageFormat(text);
                        ClientRecievedConnectedMessageFormat(connectList);
                        break;
                    default:
                        break;
                }


            }
        }
    }
}
