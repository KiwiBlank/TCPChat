﻿using CommonDefines;
using System;
using System.Collections.Generic;
using System.Text;

namespace TCPChat_Client
{
    public class ClientRecievedTypes
    {
        public static int ClientAssignedID;
        public static int CurrentChannelID;
        public static void ClientRecievedWelcomeMessage(byte[] data, Int32 bytes)
        {
            string responseData = Encoding.ASCII.GetString(data, 0, bytes);
            string text = Common.ReturnEndOfStream(responseData);

            List<WelcomeMessageFormat> connectList = Serialization.DeserializeWelcomeMessageFormat(text);

            ClientAssignedID = connectList[0].ClientID;
            CurrentChannelID = connectList[0].DefaultChannelID;

            ConsoleOutput.RecievedWelcomeMessageFormat(connectList);
        }
        public static void ClientRecievedEncryptedMessage(byte[] data)
        {
            string message = Encryption.DecryptMessageData(data);


            string messageFormatted = Common.ReturnEndOfStream(message);

            // I have to return it to bytes for some reason, otherwise i get an incorrect character on byte pos 16.
            byte[] messageBytes = Encoding.ASCII.GetBytes(messageFormatted);

            switch (Common.ReturnMessageType(messageBytes))
            {
                case MessageTypes.MESSAGEREPLY:
                    List<MessageReplyFormat> messageFormatList = Serialization.DeserializeMessageReplyFormat(messageFormatted);
                    ConsoleOutput.RecievedMessageReplyFormat(messageFormatList, CurrentChannelID);
                    break;
                case MessageTypes.SERVER:
                    List<ServerMessageFormat> serverFormatList = Serialization.DeserializeServerMessageFormat(messageFormatted);
                    ConsoleOutput.RecievedServerMessageFormat(serverFormatList);
                    break;
                case MessageTypes.DATAREPLY:
                    List<DataReplyFormat> dataFormatList = Serialization.DeserializeDataReplyFormat(messageFormatted);
                    CommandHandler.RecievedDataReply(dataFormatList);
                    break;
                case MessageTypes.PRIVATEMESSAGE:
                    List<PrivateMessageFormat> privateMessagFormatList = Serialization.DeserializePrivateMessageFormat(messageFormatted);
                    ConsoleOutput.RecievedPrivateMessageFormat(privateMessagFormatList);
                    break;
            }
        }
    }
}
