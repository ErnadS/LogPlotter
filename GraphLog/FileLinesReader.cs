using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GraphLog
{
    public class FileLinesReader
    {
        private UInt32 nArrayOffset = 0;
        private byte[] fileByteArray = null;

        /***************************************************************************
         * Read all lines from file and return List. !!! \r\n are removed form lines
         ***************************************************************************/
        public List<String> GetNmeaLines(String fileName)
        {
            nArrayOffset = 0;
            fileByteArray = FileReader.ReadFile(fileName);


            List<String> list = new List<String>();

            byte[] lineBuff;
            String tempString;

            while ((lineBuff = GetNextLine()) != null)
            {
                if (lineBuff.Length > 2)
                {
                    System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
                    tempString = enc.GetString(lineBuff);
                    list.Add(tempString);
                }
            }

            return list;
        }

        /***************************************************************************
         * Read all lines from file and return List. !!! \r\n are removed form lines
         ***************************************************************************/
        public List<byte[]> GetLines(String fileName)
        {
            nArrayOffset = 0;
            fileByteArray = FileReader.ReadFile(fileName);


            List<byte[]> list = new List<byte[]>();

            byte[] lineBuff;
            byte[] tempBuffer;

            while ((lineBuff = GetNextLine()) != null)
            {
                if (lineBuff.Length > 2)
                {
                    tempBuffer = new byte[lineBuff.Length + 6];
                    tempBuffer[0] = (byte)'U';
                    tempBuffer[1] = (byte)'d';
                    tempBuffer[2] = (byte)'P';
                    tempBuffer[3] = (byte)'b';
                    tempBuffer[4] = (byte)'C';
                    tempBuffer[5] = (byte)' ';
                    Array.Copy(lineBuff, 0, tempBuffer, 6, lineBuff.Length);
                    list.Add(tempBuffer);
                }
                //nOffset += (lineBuff.Length + 2);
            }

            return list;
        }

        /***************************************************************************
         * Start from "nArrayOffset" and read until "\r\n". Return line as byte array
         ***************************************************************************/
        private byte[] GetNextLine()
        {
            UInt32 i;
            int newLineLength = 0;
            if (nArrayOffset >= fileByteArray.Length)
                return null;

            while (fileByteArray[nArrayOffset] == '\n' || fileByteArray[nArrayOffset] == '\r')
            {
                nArrayOffset++;
                if (nArrayOffset >= fileByteArray.Length - 1)
                    return null;
            }

            UInt32 nTempOffset = nArrayOffset;

            for (i = nArrayOffset; i < fileByteArray.Length; i++)
            {
                nArrayOffset++;

                if (fileByteArray[i] == '\n')
                {
                    if (i > 0 && fileByteArray[i - 1] == '\r')
                        newLineLength--;
                    break;
                }
                newLineLength++;
            }

            if (newLineLength > 0)
            {
                byte[] newLine = new byte[newLineLength]; // + 1];

                Array.Copy(fileByteArray, nTempOffset, newLine, 0, newLineLength);
                return newLine;
            }
            return null;
        }
    }
}
