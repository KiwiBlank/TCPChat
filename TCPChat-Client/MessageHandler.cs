﻿using CommonDefines;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
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
            if (!Encrypt)
            {
                StreamHandler.WriteToStream(stream, data);
            }
            // When message has been sent, return to reading console input.
            if (loopReadInput)
            {
                InputMessage(client, stream);
            }
        }

        // Finalize message strucutre then write.
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

                // Check message type
                // Even if message type is encrypted
                // The decrypted message can be an different type.
                switch (ReturnMessageType(data))
                {
                    // Welcome can not be encrypted as that is when keys are sent.
                    case MessageTypes.WELCOME:
                        ClientRecievedWelcomeMessage(data, bytes);
                        break;
                    case MessageTypes.ENCRYPTED:
                        ClientRecievedEncryptedMessage(data);
                        break;
                }
            }
        }
        // Method to compare the server's version and client.
        // The check is also done on the server, but this does another check and outputs a helpful message.
        public static bool VerifyVersion(string serverVersion)
        {
            if (serverVersion == Assembly.GetExecutingAssembly().GetName().Version.ToString())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static void ClientRecievedWelcomeMessage(byte[] data, Int32 bytes)
        {
            string responseData = Encoding.ASCII.GetString(data, 0, bytes);
            string text = MessageSerialization.ReturnEndOfStream(responseData);

            List<WelcomeMessageFormat> connectList = Serialization.DeserializeWelcomeMessageFormat(text);

            //REMOVE ME WHEN VERSION CHECK WROKS
            OutputMessage.ClientRecievedConnectedMessageFormat(connectList);
        }
        public static void ClientRecievedEncryptedMessage(byte[] data)
        {
            string message = Encryption.DecryptMessageData(data);

            string messageFormatted = MessageSerialization.ReturnEndOfStream(message);

            // I have to return it to bytes for some reason, otherwise i get an incorrect character on byte pos 16.
            byte[] messageBytes = Encoding.ASCII.GetBytes(messageFormatted);


            switch (ReturnMessageType(messageBytes))
            {
                case MessageTypes.MESSAGE:
                    List<MessageFormat> messageList = Serialization.DeserializeMessageFormat(messageFormatted);
                    OutputMessage.ClientRecievedMessageFormat(messageList);
                    break;
            }
        }
        public static MessageTypes ReturnMessageType(byte[] data)
        {
            string byteASCII = Encoding.ASCII.GetString(new byte[] { data[16] });

            int outNum;
            // First step. Try parse to find out if character is an integer or not.
            bool parse = int.TryParse(byteASCII, out outNum);
            if (!parse)
            {
                return MessageTypes.ENCRYPTED;
            }

            // Second step. Check if the parsed integer is actually part of enum.
            if (!Enum.IsDefined(typeof(MessageTypes), outNum))
            {
                return MessageTypes.ENCRYPTED;
            }
            // TODO Find better way to define byte locations.
            // Byte number 16 is the position of the byte that indicates messagetype in a json formatted message.

            return (MessageTypes)outNum;
        }
    }
}