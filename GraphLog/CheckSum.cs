using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphLog
{
    public class CheckSum
    {
        public static String calculateCheckSum_8_NMEA(String strBuff)
        {
            byte[] byteBuffer = Encoding.ASCII.GetBytes(strBuff);
            return calculateCheckSum_8_NMEA(byteBuffer);
        }

        public static String calculateWrongCheckSum_8_NMEA(String strBuff)
        {
            byte[] byteBuffer = Encoding.ASCII.GetBytes(strBuff);
            byteBuffer[0] += 2;
            return calculateCheckSum_8_NMEA(byteBuffer);
        }
        //byte[] send_buffer = Encoding.ASCII.GetBytes(strMsg);

        /************************************************************
         * 
         ***********************************************************/
        public static String calculateCheckSum_8_NMEA(byte[] byteBuffer)
        {
            int nStart = 0;
            int nEnd = byteBuffer.Length - 1;

            int i;

            for (i = 0; i < byteBuffer.Length; i++)
            {
                if (byteBuffer[i] == (byte)'$')
                    nStart = i+1;
                else if (byteBuffer[i] == (byte)'*')
                    nEnd = i;
            }

            if (nEnd <= nStart)
            {
                //Console.WriteLine("Error, cannot calculate checksum for: " + byteBuffer.ToString());
                return null;
            }
            int newLength = nEnd - nStart;
            if (nStart == 0) newLength++;
            byte[] crcBuff = new byte[newLength];
            Array.Copy(byteBuffer, nStart, crcBuff, 0, newLength);

            byte crc = CheckSum.calculateCheckSum_8(crcBuff, newLength);

            System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
            String str = enc.GetString(crcBuff);

            return crc.ToString("X2");
        }

        public static byte calculateCheckSum_8(byte[] buffer, int length)
        {
            byte ret = 0;
            for (int i = 0; i < length; i++)
            {
                ret ^= buffer[i];
            }

            return ret;
        }
    }
}
