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
        const double ALPHA_NORMAL = 0.1;  // mora biti izmedju 0 i 1. Sto je manji, nova vrijednost utice manje na rezultat

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

        // !!! OVDJE IMA GRESKA sve dok nemas dovoljno elementata (AVG_SIZE), on vraca "fNextInput". Tek poslije toga, on pocinje da pravi 
        // average od sljedecih elemenata. To znaci da ignorise prvih "AVG_SIZE" elemenata i ne koristi ih u average
        public double calculateExpAvg(double fNextInput)
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

        public static double calculateExpAvgFromArray(double[] dInputs, int nStartInd, int length)
        {
            double previousOutputValue;
            double output = 0;

            previousOutputValue = dInputs[nStartInd]; // uzmi prvi element kao da je output

            for (int i = nStartInd + 1; i < nStartInd + length; i++) // i nastavi od sljedeceg elementa
            {
                output = ALPHA_NORMAL * dInputs[i] + (1.0 - ALPHA_NORMAL) * previousOutputValue;
                previousOutputValue = output;
            }
            return output;
        }
    }
}
