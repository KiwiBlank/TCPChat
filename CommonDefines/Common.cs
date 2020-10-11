using System;
using System.Collections.Generic;
using System.Text.Json;

namespace CommonDefines
{

    public class MessageSerialization
    {

        public static string ReturnEndOfStreamString(string text)
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

    }

}
