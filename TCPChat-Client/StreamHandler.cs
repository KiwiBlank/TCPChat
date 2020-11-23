using CommonDefines;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;

namespace TCPChat_Client
{
    public class StreamHandler
    {
        // Using locks to make sure only one write can be called at at a time.
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
        public static void SendConnectionMessage(TcpClient client, NetworkStream stream)
        {
            List<ConnectionMessageFormat> message = new List<ConnectionMessageFormat>();

            // See the messageformat class in VariableDefines.
            // The formatting for a client's message
            message.Add(new ConnectionMessageFormat
            {
                MessageType = MessageTypes.CONNECTION,
                Username = UserConfigFormat.userChosenName,
                RSAModulus = Encryption.RSAModulus,
                RSAExponent = Encryption.RSAExponent,
                ClientVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString()
            });
            // Do not encrypt and do not read console after.
            MessageHandler.PrepareMessage(message, client, stream, false, false);
        }
    }

}