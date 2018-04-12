using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphLog.DL2_analyse
{
    public class TemperMeasurment
    {
        public float[] fTemper; // nije najbolje da su public ... ovako mi brze
        public float[] fTimeTemper;

        public TemperMeasurment(int count)
        {
            fTemper = new float[count];
            fTimeTemper = new float[count];
        }
    }
}
