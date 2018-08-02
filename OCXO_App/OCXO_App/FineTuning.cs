using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OCXO_App
{
    public class FineTuning
    {
        int nCounter = 0;
        int smallPhasecounter = 0;
        double oldPhase = 0;
        AverageExp phaseAverageExp = new AverageExp();
        double phaseAverage = 0;

        public TuningResult tune(double lastDAC, double lastPhase)
        {
            phaseAverage = phaseAverageExp.calculateExpAvg(lastPhase);
            if (Math.Abs(phaseAverage) < 5 * Math.Pow(10, -9))//ovaj if izbaciti izvan ovog velikof if-a i gledati average phase
            {
                smallPhasecounter++;
                if (smallPhasecounter == 500)
                {
                    //return new TuningResult(lastDAC, TuningResult.Result.FINISHED);
                }
            }
            else
            {
                smallPhasecounter = 0;
            }
            nCounter ++;
            if (nCounter == 100) {
                nCounter = 0;
                if (phaseAverage < 0)
                {
                    if(oldPhase < Math.Abs(phaseAverage)) { lastDAC--; }
                    //return new TuningResult(lastDAC - 1, TuningResult.Result.NOT_FINISHED);  // aging (+1)
                }
                else
                {
                    if (oldPhase < Math.Abs(phaseAverage)) { lastDAC++; }
                    //return new TuningResult(lastDAC + 1, TuningResult.Result.NOT_FINISHED);
                }
                oldPhase = Math.Abs(phaseAverage);
            }
            return new TuningResult(lastDAC, TuningResult.Result.NOT_FINISHED);
        }

        //public double 
    }
}
