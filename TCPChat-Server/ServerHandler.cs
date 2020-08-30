using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TCPChat_Server
{
    class ServerHandler
    {
        // Method to send messages from the server to a client.
        public static void SendMessage(string message, NetworkStream stream)
        {

            byte[] messageBytes = Encoding.ASCII.GetBytes(message);
            try
            {
                stream.Write(messageBytes, 0, messageBytes.Length);

            }
            // When a user disconnects, it has to be removed to not attempt to access a disposed object.
            catch (ObjectDisposedException)
            {
                MessageHandler.NetStreams.Remove(stream);
            }
        }
        public static void StartServer(Int32 portServer)
        {
            Console.Clear();

            TcpListener server = null;
            try
            {

                server = new TcpListener(IPAddress.Parse("127.0.0.1"), portServer);

                server.Start();

                Console.WriteLine("Server has been started");

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

            bool clientIsConnected = true;

            while (clientIsConnected)
            {
                // Get a stream object for reading and writing
                try
                {
                    NetworkStream stream = client.GetStream();

                    //Message client when connected
                    string connectedMessage = string.Format("Connected to SERVER:");

                    SendMessage(connectedMessage, stream);

                    Console.WriteLine("{0} Has Connected", ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());


                    // Loop to receive all the data sent by the client.

                    MessageHandler.RecieveMessage(stream, client);

                }
                catch (InvalidOperationException e)
                {
                    Console.WriteLine("InvalidOperationException: {0}", e);
                    clientIsConnected = false;
                    client.Close();
                }
                catch (IOException e)
                {
                    Console.WriteLine("IOException: {0}", e);
                    clientIsConnected = false;
                    client.Close();

                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: {0}", e);
                    clientIsConnected = false;
                    client.Close();
                }
            }
        }
    }
}
