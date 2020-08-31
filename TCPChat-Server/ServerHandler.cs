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
        public static void SendMessage(string message, NetworkStream stream, ConsoleColor color)
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
        public static void StartServer(string portServer, string serverIPString)
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

                Console.WriteLine("Server has been started on: \n IP: {0} \n Port: {1}", serverIP, serverPort);

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
                    string connectedMessage = string.Format("Connected to {0}", Program.GetPublicIP());

                    SendMessage(connectedMessage, stream, ConsoleColor.DarkGray);

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
