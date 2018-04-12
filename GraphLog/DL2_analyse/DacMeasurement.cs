using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphLog.DL2_analyse
{
    public class DacMeasurement
    {
        public float[] fTime; // nije najbolje da su public ... ovako mi brze
        public float[] fDAC;

        public DacMeasurement(int count)
        {
            fTime = new float[count];
            fDAC = new float[count];
        }
    }
}
