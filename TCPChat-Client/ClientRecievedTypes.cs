using CommonDefines;
using System;
using System.Collections.Generic;
using System.Text;

namespace TCPChat_Client
{
    public class ClientRecievedTypes
    {
        public static void ClientRecievedWelcomeMessage(byte[] data, Int32 bytes)
        {
            string responseData = Encoding.ASCII.GetString(data, 0, bytes);
            string text = Common.ReturnEndOfStream(responseData);

            List<WelcomeMessageFormat> connectList = Serialization.DeserializeWelcomeMessageFormat(text);

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
                case MessageTypes.MESSAGE:
                    List<MessageFormat> messageFormatList = Serialization.DeserializeMessageFormat(messageFormatted);
                    ConsoleOutput.RecievedMessageFormat(messageFormatList);
                    break;
                case MessageTypes.SERVER:
                    List<ServerMessageFormat> serverFormatList = Serialization.DeserializeServerMessageFormat(messageFormatted);
                    ConsoleOutput.RecievedServerMessageFormat(serverFormatList);
                    break;
                case MessageTypes.DATAREPLY:
                    List<DataReplyFormat> dataFormatList = Serialization.DeserializeDataReplyFormat(messageFormatted);
                    CommandHandler.RecievedDataReply(dataFormatList);
                    break;
            }
        }
    }
}
