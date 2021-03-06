﻿using CommonDefines;
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
        public static List<ClientList> activeClients = new();

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

        // Console Read Loop
        public static void InputMessage()
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
                        InputMessage();

                    }
                    else
                    {
                        InputMessage();
                    }
                }
                else
                {
                    InputMessage();
                }
            }).Start();
        }

        // The object that is used for each client.
        public static void ServerObject(object obj)
        {
            var client = (TcpClient)obj;

            // Get a stream object for reading and writing
            try
            {
                ClientInstance instance = new ClientInstance
                {
                    client = client,
                    stream = client.GetStream()
                };

                // Check if client's IP is banned.
                if (Bans.IsBanned(((IPEndPoint)instance.client.Client.RemoteEndPoint).Address.ToString()))
                {
                    Console.WriteLine("{0} Was refused connection, client is banned.", ((IPEndPoint)instance.client.Client.RemoteEndPoint).Address.ToString());
                    instance.client.Close();
                }
                // Loop to receive all the data sent by the client.
                MessageHandler.RecieveMessage(instance);

                // Will check if the client is actually sending and recieiving messages.
                ClientHeartbeat(instance);
            }
            // ObjectDisposedException does not contain an error code, 
            // therefore it can not return a custom message for the moment.
            catch (ObjectDisposedException)
            {
                return;
            }
            // Any other exception should have an error code.
            // https://docs.microsoft.com/en-us/windows/win32/debug/system-error-codes
            catch (Exception e)
            {

                int excID = ExceptionData.ExceptionIdentification(e);
                int index = ServerMessage.FindClientKeysIndex(client);
                string message;

                switch (excID)
                {
                    // Seems that 10054 now appears as of .NET 5 when a disconnect occurs.
                    // Need to investigate if 10053 is still active.
                    case 10054:
                        message = String.Format("({0}) {1} disconnected.", activeClients[index].ID, activeClients[index].Username);
                        ServerMessage.ServerGlobalMessage(ConsoleColor.Yellow, message);
                        break;
                    case 10053:
                        message = String.Format("({0}) {1} disconnected.", activeClients[index].ID, activeClients[index].Username);
                        ServerMessage.ServerGlobalMessage(ConsoleColor.Yellow, message);
                        break;
                    // 10004 does not need certain handling messages.
                    // It appears when an user is kicked or banned.
                    // May need to be investigated further.
                    case 10004:
                        break;
                    default:
                        Console.WriteLine("Exception: {0}", e);
                        break;
                }

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
