using CommonDefines;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace TCPChat_Client
{
    class MessageHandler
    {
        /// Serialize the messageFormat with json to transmit.
        public static void PrepareMessage<T>(List<T> message, TcpClient client, NetworkStream stream, bool Encrypt, bool loopReadInput)
        {
            string json = Serialization.Serialize(message, false);

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
                    // Check if first character is / which means a command is being input.
                    if (messageString.Substring(0, 1) == "/")
                    {

                        Commands.GetCommandType(messageString);
                        InputMessage(client, stream);

                    }
                    else
                    // Regular message.
                    {
                        List<MessageFormat> newMessage = new List<MessageFormat>();

                        // See the messageformat class in VariableDefines.
                        // The formatting for a client's message
                        newMessage.Add(new MessageFormat
                        {
                            MessageType = MessageTypes.MESSAGE,
                            Message = messageString,
                            Username = UserConfigFormat.userChosenName,
                            UserNameColor = UserConfigFormat.userChosenColor,
                            ID = ClientRecievedTypes.ClientAssignedID
                        });
                        PrepareMessage(newMessage, client, stream, true, true);
                    }
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
                switch (Common.ReturnMessageType(data))
                {
                    // Welcome can not be encrypted as that is when keys are sent.
                    case MessageTypes.WELCOME:
                        ClientRecievedTypes.ClientRecievedWelcomeMessage(data, bytes);
                        break;
                    case MessageTypes.ENCRYPTED:
                        ClientRecievedTypes.ClientRecievedEncryptedMessage(data);
                        break;
                }
            }
        }
    }
}