using System;
using System.Text;

namespace CommonDefines
{

    public class Common
    {

        public static string ReturnEndOfStream(string text)
        {
            int indexToRemove = FindEndOfStream(text.ToCharArray());
            if (indexToRemove != 0)
            {
                // Find the location of 0x01 end char.
                text = text.Remove(indexToRemove);
            }
            return text;
        }

        // Find the end of stream, to then trim trailing characters.
        public static int FindEndOfStream(char[] arr)
        {

            for (int i = 0; i < arr.Length; i++)
            {
                try
                {
                    // A really bad way to find the end of stream.
                    // This is to return the point where all trailing bytes should be removed.
                    if (arr[i] == 0x01)
                    {
                        return i;
                    }
                }
                catch (IndexOutOfRangeException)
                {
                }

            }
            return 0;
        }
        public static MessageTypes ReturnMessageType(byte[] data)
        {
            // TODO Improve this system, may be causing irregular cryptography errors.
            string byteASCII = Encoding.ASCII.GetString(new byte[] { data[16] });

            // First step. Try parse to find out if character is an integer or not.
            bool parse = int.TryParse(byteASCII, out int outNum);
            if (!parse)
            {
                return MessageTypes.ENCRYPTED;
            }

            // Second step. Check if the parsed integer is actually part of enum.
            if (!Enum.IsDefined(typeof(MessageTypes), outNum))
            {
                return MessageTypes.ENCRYPTED;
            }
            // TODO Find better way to define byte locations.
            // Byte number 16 is the position of the byte that indicates messagetype in a json formatted message.
            return (MessageTypes)outNum;
        }
    }
}