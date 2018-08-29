using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OCXO_App
{
    class AverageNormal
    {
        public static double calculateExpAvgFromArray(double[] dInputs, int nStartInd, int length)
        {
            double output = 0;

            for (int i = nStartInd; i < nStartInd + length; i++) 
            {
                output += dInputs[i];
            }
            return output / length;
        }
    }
}
