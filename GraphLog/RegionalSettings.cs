using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphLog
{
    public class RegionalSettings
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

            try
            {
                if (bNumberWithComma)
                    strNumber = strNumber.Replace('.', ',');
                else
                    strNumber = strNumber.Replace(',', '.');
                return float.Parse(strNumber);
            }
            catch
            {
                return float.NaN;
            }
        }

        private static void detectFormat()
        {
            try
            {
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
            catch
            {
                bNumberWithComma = false;
            }
            bFormatDetected = true;
        }
    }
}
