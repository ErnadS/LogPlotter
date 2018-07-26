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

        public TuningResult tune(double lastDAC, double lastPhase)
        {
            nCounter ++;
            if (nCounter == 100) {
                nCounter = 0;
                if (Math.Abs(lastPhase) < 5 * Math.Pow(10, -9))
                {
                    smallPhasecounter++;
                    if(smallPhasecounter == 50)
                    {
                        return new TuningResult(lastDAC, TuningResult.Result.FINISHED);
                    }
                }
                else
                {
                    smallPhasecounter = 0;
                }
                if (lastPhase < 0)
                {
                    if(oldPhase < Math.Abs(lastPhase)) { lastDAC--; }
                    //return new TuningResult(lastDAC - 1, TuningResult.Result.NOT_FINISHED);  // aging (+1)
                }
                else
                {
                    if (oldPhase < Math.Abs(lastPhase)) { lastDAC++; }
                    //return new TuningResult(lastDAC + 1, TuningResult.Result.NOT_FINISHED);
                }
                oldPhase = Math.Abs(lastPhase);
            }
            return new TuningResult(lastDAC, TuningResult.Result.NOT_FINISHED);
        }

        //public double 
    }
}
