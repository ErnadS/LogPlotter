
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace utility
{
    public class NumberConvertor
    {
        private static bool bFormatDetected = false;
        public static bool bNumberWithComma = false;

        

        public static string validateInt(string strInt)
        {
            if (string.IsNullOrEmpty(strInt))
            {
                return "must not be empty";
            }
            try
            {
                Int32.Parse(strInt);
                return "";
            }
            catch
            {
                return "must be integer";
            }
        }

        public static string validateFloat(string strFloat)
        {
            float ff = convertToFloat(strFloat);
            if (float.IsNaN(ff))
                return "must be desimal number";

            return "";
        }

        public static float convertToFloat(string strNumber)
        {
            if (!bFormatDetected)
                detectFormat();

            if (strNumber.Length == 0)
                return float.NaN;

            try
            {
                if (bNumberWithComma)
                    strNumber = strNumber.Replace('.',',');
                else
                    strNumber = strNumber.Replace(',','.');

                if (strNumber.StartsWith("+"))
                    strNumber = strNumber.Substring(1);

                while (strNumber.Length > 1 && strNumber.StartsWith("0") && strNumber[1] != '.' && strNumber[1] != ',') // remove leading zerro
                    strNumber = strNumber.Substring(1);

                return float.Parse(strNumber);
            }
            catch
            {
                return float.NaN;
            }
        }

        public static int convertToInt(string strNumber)
        {
            try
            {
                if (strNumber.StartsWith("+"))
                    strNumber = strNumber.Substring(1);
                while (strNumber.StartsWith("0")) // remove leading zerros
                    strNumber = strNumber.Substring(1);
                return Int32.Parse(strNumber);
            }
            catch
            {
                return Int32.MinValue; //TODO  this is not good. 
            }
        }

        private static void detectFormat()
        {
            try {
                String testNumberText = "1,1";
                float testFloat = float.Parse(testNumberText);
                if (("" + testFloat) == testNumberText)
                {
                    bNumberWithComma = true;
                }
                else
                {
                    bNumberWithComma = false;
                }
            }
            catch{
                bNumberWithComma = false;
            }
            bFormatDetected = true;
        }
    }
}
