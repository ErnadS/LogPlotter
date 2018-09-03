using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OCXO_App
{
    public class SlidingFrame
    {
        public double phaseAvg_start { get; private set; }
        public double phaseAvg_stop { get; private set; }
        public double part_angle { get; private set; }

        // public double part_DAC { get; private set; }

        public bool finished { get; private set; }  // postane "true" kada se baoybu "phaseArray" i ostane i dalje "true" dok se slidingFrame pomice udesno

        public int nCounter { get; private set; } // povecavamo counter samo dok ne dodje do "averageSize" i onda ostaje na toj vrijednosti

        double[] phaseArray; // klizna Array
        int totalFrameSize;  // ukupna duzina frame. Sastoji se od "averageSize" za racunicu prve tacke, pauze izmedju 2 tacke i jos jedne "averageSize" za drugu tacku
        int averageSize;  // koliko elemenata se koristi da se izracuna tacka A1 ili A2 

        public SlidingFrame()
        {
            finished = false;
            nCounter = 0;
        }

        public void init(int totalFrameSize, int averageSize)
        {
            this.totalFrameSize = totalFrameSize;
            this.averageSize = averageSize;
            phaseArray = new double[totalFrameSize];

            finished = false;
        }

        public void clear()
        {
            nCounter = 0;
            finished = false;
        }

        public void AddPoint(double phase)
        {
            if (nCounter == totalFrameSize)
            {
                for (int i = 0; i < totalFrameSize - 1; i++) // izbaci prvi element i prekopiraj clanove uljevo
                {
                    phaseArray[i] = phaseArray[i + 1];
                }

                phaseArray[totalFrameSize - 1] = phase; // dodaj novi element
                finished = true;  // nije potrebno ovdje jer vec je bio "finished" dole kada je napunio array ali za svaki slucaj ...
            }
            else
            {
                phaseArray[nCounter] = phase;
                nCounter++;  // povecavamo counter samo ako nije dosao do averageSize

                if (nCounter == totalFrameSize) // upravo popunili array po prvi put
                    finished = true;
            }

            if (finished)
            {
                phaseAvg_start = /*AverageExp*/AverageNormal.calculateExpAvgFromArray(phaseArray, 0, averageSize);
                phaseAvg_stop = /*AverageExp*/AverageNormal.calculateExpAvgFromArray(phaseArray, totalFrameSize - averageSize - 1, averageSize);
                CalculateAngle();
            }
        }

        // Example: ako faza se promjeni za 3ns a total Frame = 100, onda je ovo 3 deg. Ako se faza promjeni za 1.5 a total frame = 50, onda je to opet = 3 deg
        private void CalculateAngle()
        {
            part_angle = (phaseAvg_stop - phaseAvg_start) * Math.Pow(10, 9) * 100 / totalFrameSize;
        }
    }
}
