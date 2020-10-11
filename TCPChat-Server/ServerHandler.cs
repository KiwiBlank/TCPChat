using CommonDefines;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TCPChat_Server
{
    class ServerHandler
    {
        public static List<ClientList> activeClients = new List<ClientList>();

        // Method to send messages from the server to a client.
        public static void SendMessage(byte[] data, NetworkStream stream)
        {
            StreamHandler.WriteToStream(stream, data);
        }
        public static void StartServer(string serverIPString, string portServer)
        {
            Console.Clear();

            TcpListener server = null;
            try
            {
                // Parse
                Int32 serverPort = Int32.Parse(portServer);
                IPAddress serverIP = IPAddress.Parse(serverIPString);

                server = new TcpListener(serverIP, serverPort);

                server.Start();

                Console.WriteLine("Server has been started");
                //Console.WriteLine("Your public IP is: {0}", Program.GetPublicIP());
                Console.WriteLine("Server Port: {0}", serverPort);

                Console.WriteLine("Waiting for a connection... ");


                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();
                    ThreadPool.QueueUserWorkItem(ServerObject, client);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e);
                server.Stop();
            }

            Console.WriteLine("\nHit enter to continue...");
            Console.ReadLine();
        }

        // The object that is used for each client.
        public static void ServerObject(object obj)
        {
            var client = (TcpClient)obj;

            // Get a stream object for reading and writing
            try
            {
                NetworkStream stream = client.GetStream();

                List<WelcomeMessageFormat> newMessage = new List<WelcomeMessageFormat>();

                newMessage.Add(new WelcomeMessageFormat
                {
                    messageType = MessageTypes.WELCOME,
                    connectMessage = ServerConfigFormat.serverChosenWelcomeMessage,
                    serverName = ServerConfigFormat.serverChosenName,
                    keyExponent = Encryption.RSAExponent,
                    keyModulus = Encryption.RSAModulus
                });


                SerializePrepareWelcome(newMessage, stream);

                Console.WriteLine("{0} Has Connected", ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());


                // Loop to receive all the data sent by the client.

                MessageHandler.RecieveMessage(stream, client);

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e);
                client.Close();
            }
        }
        public static void SerializePrepareWelcome(List<WelcomeMessageFormat> message, NetworkStream stream)
        {
            string json = Serialization.Serialize(message);

            byte[] data = Serialization.AddEndCharToMessage(json);

            SendMessage(data, stream);
        }

    }
}
