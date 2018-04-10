using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GraphLog
{
    class FileLoger
    {
        public string path;

        public FileLoger(string p)
        {
            //***********Kontrola unosa, postoji li file? Kreiraj, ako ne postoji?!
            path = p;
        }

        public void WriteToFile(string data)
        {
            StreamWriter sw = new StreamWriter(path, true);
            sw.Write(data);
            sw.Close();
            sw.Dispose();
        }
    }
}
