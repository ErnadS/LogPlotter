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

        public bool finished { get; private set; }  // this is true when phaseArray is full, and remains true when slidingFrame moves to the right

        public int nCounter { get; private set; } // increasing counter until it becomes equal to averageSize

        double[] phaseArray; // slidingArray
        int totalFrameSize;  // total frame size
        int averageSize;  // number of elements for average

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
                for (int i = 0; i < totalFrameSize - 1; i++) // reject the first element
                {
                    phaseArray[i] = phaseArray[i + 1];
                }

                phaseArray[totalFrameSize - 1] = phase; // adding new element
                finished = true;  
            }
            else
            {
                phaseArray[nCounter] = phase;
                nCounter++;

                if (nCounter == totalFrameSize) // array is full for first time
                    finished = true;
            }

            if (finished)
            {
                phaseAvg_start = /*AverageExp*/AverageNormal.calculateExpAvgFromArray(phaseArray, 0, averageSize);
                phaseAvg_stop = /*AverageExp*/AverageNormal.calculateExpAvgFromArray(phaseArray, totalFrameSize - averageSize - 1, averageSize);
                CalculateAngle();
            }
        }

        private void CalculateAngle()
        {
            part_angle = (phaseAvg_stop - phaseAvg_start) * Math.Pow(10, 9) * 100 / totalFrameSize;
        }
    }
}
