using System;
using System.Collections.Generic;
using System.Text;

namespace CommonDefines
{
    public class ExceptionData
    {
        public static int ExceptionIdentification(Exception exception)
        {
            string baseEsception = exception.GetBaseException().ToString();
            string strID = baseEsception.Split('(', ')') [1];

            int id;
            bool parse = int.TryParse(strID, out id);

            // If parsing to integer was not possible, return -1
            if (!parse)
            {
                return -1;
            }
            return id;
        }
    }
}
