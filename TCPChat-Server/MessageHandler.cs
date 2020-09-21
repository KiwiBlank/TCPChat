using CommonDefines;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;

namespace TCPChat_Server
{

    class MessageHandler
    {
        // Keep a list of all current network streams to repeat incoming messages to.
        public static List<NetworkStream> NetStreams = new List<NetworkStream>();


        // TODO implement serialization for repeating to clients.
        public static void RepeatToAllClients(string serializedMessage, TcpClient client)
        {
            for (int i = 0; i < NetStreams.Count; i++)
            {
                ServerHandler.SendMessage(serializedMessage, NetStreams[i]);
            }
        }

        public static void ServerRecievedMessage(List<MessageFormat> list)
        {
            OutputMessage.OutputMessageWithColor(list[0].message, list[0].IP, list[0].Username, list[0].UserNameColor);
        }

        public static byte[] ExtractKeyFromMessage(byte[] data)
        {
            List<byte> byteList = new List<byte>();


            // TODO Simplify This
            for (int i = 0; i < data.Length; i++)
            {
                if (i < 256)
                {
                    byteList.Add(data[i]);
                }
            }
            byte[] bytes = byteList.ToArray();
            return bytes;
        }
        public static byte[] ExtractIVFromBytes(byte[] data)
        {
            List<byte> byteList = new List<byte>();

            // TODO Simplify This
            for (int i = 0; i < data.Length; i++)
            {
                if (i >= 0 && i < 16)
                {
                    byteList.Add(data[i]);
                }
            }
            byte[] bytes = byteList.ToArray();
            return bytes;
        }
        public static byte[] ExtractKeyFromBytes(byte[] data)
        {
            List<byte> byteList = new List<byte>();

            // TODO Simplify This
            for (int i = 0; i < data.Length; i++)
            {
                if (i > 15 && i < 48)
                {
                    byteList.Add(data[i]);
                }
            }
            byte[] bytes = byteList.ToArray();
            return bytes;
        }

        // The loop to recieve incoming packets.
        public static void RecieveMessage(NetworkStream stream, TcpClient client)
        {

            byte[] bytes = new byte[8192];

            int iStream;

            while ((iStream = stream.Read(bytes, 0, bytes.Length)) > 0)
            {
                new Thread(() =>
                {
                    // I had some issues with trailing zero bytes, and this solves that.
                    int i = bytes.Length - 1;
                    while (bytes[i] == 0)
                    {
                        --i;
                    }

                    byte[] bytesResized = new byte[i + 1];
                    Array.Copy(bytes, bytesResized, i + 1);



                    // Extract the 256 bytes that make up the AES Key and IV at the beginning of the message.
                    byte[] keyData = ExtractKeyFromMessage(bytesResized);




                    // Get the AES Key and IV bytes and decrypt them using the server's private RSA Key
                    // Returns 48 Bytes
                    // 32 Bytes Key
                    // 16 Bytes IV
                    byte[] decryptKeyRSA = Encryption.DecryptData(keyData, Encryption.privKey);


                    // Separate the Key and IV from decryptKeyRSA
                    byte[] AESKey = ExtractKeyFromBytes(decryptKeyRSA);
                    byte[] AESIV = ExtractIVFromBytes(decryptKeyRSA);



                    // Remove the keys from the other message data.
                    // The keys are appended at the beginning of the stream
                    // As such they are not part of the json formatting and have to be removed.
                    // TODO Simplify This
                    byte[] RemoveKeysFromDataBytes = new byte[bytesResized.Length - 256];
                    Array.Copy(bytesResized, 256, RemoveKeysFromDataBytes, 0, RemoveKeysFromDataBytes.Length);



                    // Decrypt the main message using the decryped key and IV
                    // TODO Make some sort of visual aid to explain this.
                    byte[] AESDecrypt = Encryption.AESDecrypt(RemoveKeysFromDataBytes, AESKey, AESIV);



                    string message = System.Text.Encoding.ASCII.GetString(AESDecrypt);


                    string messageFormatted = MessageSerialization.ReturnEndOfStreamString(message);
                    List<MessageFormat> messageList = Serialization.DeserializeMessageFormat(messageFormatted);


                    // Re-serialize to repeat for clients.
                    // TODO Implement Server Encryption for repeating messages.
                    // At the moment only the client encrypts its messages.

                    string repeatMessage = JsonSerializer.Serialize(messageList);

                    ServerRecievedMessage(messageList);

                    RepeatToAllClients(repeatMessage, client);

                }).Start();
            }
        }

    }
}
