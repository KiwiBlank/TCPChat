using CommonDefines;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace TCPChat_Server
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
    }
}
