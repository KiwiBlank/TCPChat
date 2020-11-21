using CommonDefines;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TCPChat_Server
{
    public class ClientInstance
    {
        public TcpClient client;
        public NetworkStream stream;
        // Client verified means that the client has sent over its encryption keys, and therefore can send encrypted messages.
        public bool clientVerified = false;
    }
    class ServerHandler
    {
        public static List<ClientList> activeClients = new List<ClientList>();

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
                ClientInstance instance = new ClientInstance();
                instance.client = client;
                instance.stream = client.GetStream();

                Console.WriteLine("{0} Has Connected", ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());


                // Loop to receive all the data sent by the client.

                MessageHandler.RecieveMessage(instance);

                // Will check if the client is actually sending and recieiving messages.
                ClientHeartbeat(instance);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e);
                client.Close();
            }
        }
        public static void ClientHeartbeat(ClientInstance instance)
        {
            bool active = false;
            for (int i = 0; i < activeClients.Count; i++)
            {
                if (activeClients[i].TCPClient == instance.client)
                {
                    active = true;
                }
            }
            if (!active)
            {
                Console.WriteLine("{0} Was kicked, user did not attempt communication.", ((IPEndPoint)instance.client.Client.RemoteEndPoint).Address.ToString());
                instance.client.Close();
            }
        }
    }
}
