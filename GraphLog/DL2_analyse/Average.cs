using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphLog.DL2_analyse
{
    public class Average
    {
        int m_sampleCount = 0;
        float previousOutputValue = 0;
        float outputValue;

        int AVG_SIZE          = 10;
        float ALPHA_NORMAL    = 0.1f;


        public float calculateAvg(float fNextInput)
        {
            m_sampleCount++;

            if (m_sampleCount < AVG_SIZE)
            {
                previousOutputValue = fNextInput;
                return fNextInput;
            }

            outputValue = ALPHA_NORMAL * fNextInput + (1.0f - ALPHA_NORMAL) * previousOutputValue;
            previousOutputValue = outputValue;
            return outputValue;
        }

        public void setAlpha(float fAlpha)
        {
            ALPHA_NORMAL = fAlpha;
        }

        public void setSize(int  nLength)
        {
            AVG_SIZE = nLength;
        }
    }
}
