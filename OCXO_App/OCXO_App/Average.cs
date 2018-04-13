using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCXO_App
{
    public class Average
    {
        int m_sampleCount = 0;
        double previousOutputValue = 0;
        double outputValue;

        const int AVG_SIZE = 10;
        const double ALPHA_NORMAL = 0.1;


        public double calculateAvg(double fNextInput)
        {
            m_sampleCount++;

            if (m_sampleCount < AVG_SIZE)
            {
                previousOutputValue = fNextInput;
                return fNextInput;
            }

            outputValue = ALPHA_NORMAL * fNextInput + (1.0 - ALPHA_NORMAL) * previousOutputValue;
            previousOutputValue = outputValue;
            return outputValue;
        }
    }
}
