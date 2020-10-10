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

            // Doesn't matter what list type it is, as all of them have the messageType property.
            List<MessageFormat> typeList = new List<MessageFormat>();
            typeList = JsonSerializer.Deserialize<List<MessageFormat>>(message);

            switch (typeList[0].messageType)
            {
                case MessageTypes.MESSAGE:
                    return 1;
                case MessageTypes.WELCOME:
                    return 2;
                default:
                    return 0;
            }
        }
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
                    if (arr[i] == 0x01 && arr[i + 1] == 0x01)
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
