using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace CommonDefines
{
    public class OutputMessage
    {
        public static void ServerRecievedMessage(List<MessageFormat> list)
        {
            OutputMessage.OutputMessageWithColor(list[0].message, list[0].IP, list[0].Username, list[0].UserNameColor);
        }

        public static string ServerRecievedEncrypedMessage(byte[] dataMessage)
        {
            // Extract the 256 bytes that make up the AES Key and IV at the beginning of the message.
            byte[] keyData = Encryption.ExtractKeyFromMessage(dataMessage);

            // Get the AES Key and IV bytes and decrypt them using the server's private RSA Key
            // Returns 48 Bytes
            // 32 Bytes Key
            // 16 Bytes IV
            byte[] decryptKeyRSA = Encryption.RSADecrypt(keyData, Encryption.RSAPrivateKey);

            // Separate the Key and IV from decryptKeyRSA
            byte[] AESKey = Encryption.ExtractKeyFromBytes(decryptKeyRSA);
            byte[] AESIV = Encryption.ExtractIVFromBytes(decryptKeyRSA);

            // Remove the keys from the other message data.
            // The keys are appended at the beginning of the stream
            // As such they are not part of the json formatting and have to be removed.
            // TODO Simplify This
            byte[] RemoveKeysFromDataBytes = new byte[dataMessage.Length - 256];
            Array.Copy(dataMessage, 256, RemoveKeysFromDataBytes, 0, RemoveKeysFromDataBytes.Length);

            // Decrypt the main message using the decryped key and IV
            // TODO Make some sort of visual aid to explain this.
            byte[] AESDecrypt = Encryption.AESDecrypt(RemoveKeysFromDataBytes, AESKey, AESIV);

            string message = System.Text.Encoding.ASCII.GetString(AESDecrypt);

            return message;
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
        public static void OutputOnlyMessage(string message)
        {

            Console.WriteLine(message);
            Console.ResetColor();

        }
        // This is the common output method for both server and client.
        public static void OutputMessageWithColor(string message, string IP, string username, ConsoleColor color)
        {

            Console.ForegroundColor = color;
            string output = String.Format("{0} : {1} - {2}", IP, username, message);

            Console.WriteLine(output);
            Console.ResetColor();

        }
    }
}
