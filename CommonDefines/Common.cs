using System;
using System.Collections.Generic;
using System.Text.Json;

namespace CommonDefines
{

    public class MessageSerialization
    {

        // This method is supposed to figure out what class the serialized message belongs to.
        // When Deserializing, you can output it to any list class regardless if it has the correct members.
        // The incorrect values will be null when deserializing.
        public static int FindTypeOfList(string message)
        {

            List<MessageFormat> messageFormatList = new List<MessageFormat>();
            messageFormatList = JsonSerializer.Deserialize<List<MessageFormat>>(message);

            // Has to be constant value that can never be null for this check to work.
            if (messageFormatList[0].message != null)
            {
                return 1;
            }

            List<ConntectedMessageFormat> conntectedMessageFormatList = new List<ConntectedMessageFormat>();
            conntectedMessageFormatList = JsonSerializer.Deserialize<List<ConntectedMessageFormat>>(message);

            // Has to be constant value that can never be null for this check to work.
            if (conntectedMessageFormatList[0].connectMessage != null)
            {
                return 2;
            }
            return 0;

        }
        public static string ReturnEndOfStreamString(string text)
        {
            int indexToRemove = FindEndOfStream(text.ToCharArray());
            if (indexToRemove != 0)
            {
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
                    if (arr[i] == '}' && arr[i + 1] == ']' && arr[i + 2] == '')
                    {
                        return i + 2;
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
