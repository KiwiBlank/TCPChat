using CommonDefines;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace TCPChat_Client
{
    public class StreamHandler
    {
        public static void WriteToStream(NetworkStream stream, byte[] data)
        {
            object __lockObj = stream;
            bool __lockWasTaken = false;
            try
            {
                System.Threading.Monitor.Enter(__lockObj, ref __lockWasTaken);

                stream.Write(data, 0, data.Length);

            }
            finally
            {
                if (__lockWasTaken) System.Threading.Monitor.Exit(__lockObj);
            }
        }

        // Doesn't work, just hangs and client cant send data.
        // TODO Fix
        public static byte[] ReadFromStream(NetworkStream stream)
        {
            object __lockObj = stream;
            bool __lockWasTaken = false;
            try
            {
                System.Threading.Monitor.Enter(__lockObj, ref __lockWasTaken);

                Byte[] data = new Byte[8192]; // Unsure what this should be atm.
                stream.Read(data, 0, data.Length);
                return data;
            }
            finally
            {
                if (__lockWasTaken) System.Threading.Monitor.Exit(__lockObj);
            }

        }
        public static void SendConnectionMessage(TcpClient client, NetworkStream stream)
        {
            List<ConnectionMessageFormat> message = new List<ConnectionMessageFormat>();

            // See the messageformat class in VariableDefines.
            // The formatting for a client's message
            message.Add(new ConnectionMessageFormat
            {
                messageType = MessageTypes.CONNECTION,
                RSAModulus = Encryption.RSAModulus,
                RSAExponent = Encryption.RSAExponent
            });
            // Do not encrypt and do not read console after.
            MessageHandler.SerializePrepareMessage(message, client, stream, false, false);
        }
    }

}