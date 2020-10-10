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
        public static void SerializePrepareMessage<T>(List<T> message, TcpClient client, NetworkStream stream, bool Encrypt, bool loopReadInput)
        {
            string json = Serialization.Serialize(message);

            byte[] data = Serialization.AddEndCharToMessage(json);

            // Encrypts message and sends.
            if (Encrypt)
            {
                EncryptSendMessage(data, client, stream);

            }
            // Does not encrypt, just sends.
            else
            {
                StreamHandler.WriteToStream(stream, data);
            }
            // When message has been sent, return to reading console input.
            if (loopReadInput)
            {
                InputMessage(client, stream);
            }
        }

        // Write to the stream, then continue looping for new console input.
        public static void EncryptSendMessage(byte[] message, TcpClient client, NetworkStream stream)
        {
            // Encrypt Message Data
            byte[] encrypt = Encryption.AESEncrypt(message, Encryption.AESKey, Encryption.AESIV);

            // Encrypt Key Data
            byte[] finalBytes = Encryption.AppendKeyToMessage(encrypt, Encryption.AESKey, Encryption.AESIV, Encryption.clientCopyOfServerPublicKey);

            StreamHandler.WriteToStream(stream, finalBytes);
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

                    // See the messageformat class in VariableDefines.
                    // The formatting for a client's message
                    newMessage.Add(new MessageFormat
                    {
                        messageType = MessageTypes.MESSAGE,
                        message = messageString,
                        Username = UserConfigFormat.userChosenName,
                        UserNameColor = UserConfigFormat.userChosenColor
                    });
                    SerializePrepareMessage(newMessage, client, stream, true, true);
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

                // Try catch as to check whether data is valid json.
                // Not a very future proof solution, but gets the job done.
                try
                {
                    string responseData = Encoding.ASCII.GetString(data, 0, bytes);
                    string text = MessageSerialization.ReturnEndOfStreamString(responseData);

                    List<WelcomeMessageFormat> connectList = Serialization.DeserializeWelcomeMessageFormat(text);
                    OutputMessage.ClientRecievedConnectedMessageFormat(connectList);
                }
                catch (JsonException)
                {

                    string message = Encryption.DecryptMessageData(data);

                    string messageFormatted = MessageSerialization.ReturnEndOfStreamString(message);
                    List<MessageFormat> messageList = Serialization.DeserializeMessageFormat(messageFormatted);
                    OutputMessage.ClientRecievedMessageFormat(messageList);
                }
            }
        }
    }
}