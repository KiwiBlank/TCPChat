using System;
using System.Net.Sockets;

namespace TCPChat_Client
{
    class Connections
    {
        public static void Connect(String serverIP, string port)
        {
            Console.Clear();
            try
            {
                Int32 ServerPort = Int32.Parse(port);

                TcpClient client = new TcpClient(serverIP, ServerPort);
                NetworkStream stream = client.GetStream();

                // Loop to read console input as messages.
                MessageHandler.InputMessage(client, stream);

                // Read the incoming stream and output it.
                MessageHandler.ClientRecieveMessage(client, stream);
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
        }
    }
}
