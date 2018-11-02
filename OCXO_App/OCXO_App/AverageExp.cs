using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCXO_App
{
    public class AverageExp
    {
        //////////////////////////////////////////////////////////////////////////
        // Exponential average
        //////////////////////////////////////////////////////////////////////////
        int m_sampleCount = 0;
        double previousOutputValue = 0;
        double outputValue;

        public const int AVG_SIZE = 10;
        const double ALPHA_NORMAL = 0.1;  // alpha si between 0 and 1

        int avg_size;
        double alpha_normal;

        public AverageExp()
        {
            avg_size = AVG_SIZE;  // use default average size and alpha
            alpha_normal = ALPHA_NORMAL;
        }

        // Change averaging size or alpha
        public void setExpAvgParams(int avg_size, double alpha_normal)
        {
            this.avg_size = avg_size;
            this.alpha_normal = alpha_normal;
        }

        public double calculateExpAvg(double fNextInput)
        {
            m_sampleCount++;
            /*
            if (m_sampleCount < AVG_SIZE)
            {
                previousOutputValue = fNextInput;
                return fNextInput;
            }*/
            
            if (m_sampleCount == 1)
            {
                previousOutputValue = fNextInput;
                return fNextInput;
            }

            outputValue = ALPHA_NORMAL * fNextInput + (1.0 - ALPHA_NORMAL) * previousOutputValue;
            previousOutputValue = outputValue;
            return outputValue;
        }

        public static double calculateExpAvgFromArray(double[] dInputs, int nStartInd, int length)
        {
            double previousOutputValue;
            double output = 0;

            previousOutputValue = dInputs[nStartInd];

            for (int i = nStartInd + 1; i < nStartInd + length; i++)
            {
                output = ALPHA_NORMAL * dInputs[i] + (1.0 - ALPHA_NORMAL) * previousOutputValue;
                previousOutputValue = output;
            }
            return output;
        }
    }
}
