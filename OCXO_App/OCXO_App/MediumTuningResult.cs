using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OCXO_App
{
    public class MediumTuningResult
    {
        public double phaseAvg_start { get; private set; }
        public double phaseAvg_stop { get; private set; }
        public double part_angle { get; private set; }

        public double part_DAC { get; private set; }

        public bool finished { get; private set; }

        public int nCounter { get; private set; }

        AverageExp phaseExpAvgStart = new AverageExp();
        AverageExp phaseExpAvgStop = new AverageExp();


        int nStartEndIndex;
        int nStopEndIndex;

        public MediumTuningResult()
        {
            finished = false;
            phaseExpAvgStart = new AverageExp();
            phaseExpAvgStop = new AverageExp();
        }

        public void init(double part_DAC, int nStartEndIndex, int nStopEndIndex)
        {
            this.part_DAC = part_DAC;
            this.nStartEndIndex = nStartEndIndex;
            this.nStopEndIndex = nStopEndIndex;
            finished = false;
            phaseExpAvgStart = new AverageExp();
            phaseExpAvgStop = new AverageExp();
        }

        public void AddPoint(double phase)
        {
            nCounter++;

            if (nCounter <= nStartEndIndex)
            {
                phaseAvg_start = phaseExpAvgStart.calculateExpAvg(phase);
            }
            else if (nCounter <= nStopEndIndex)
            {
                phaseAvg_stop = phaseExpAvgStop.calculateExpAvg(phase);

                if (nCounter == nStartEndIndex)
                {
                    CalculateAngle();
                    finished = true;
                }
            }
        }

        // Exaple: ako faza se promjeni za 0.1 u toku zadnjih 30 mjerenja, ugao je 0.1 * 1000 / 30 = 3.3
        private void CalculateAngle()
        {
            part_angle = (phaseAvg_stop - phaseAvg_start) * 1000/ nStopEndIndex;  // "1000" je cisto da broj ne bude premal
        }
    }
}
