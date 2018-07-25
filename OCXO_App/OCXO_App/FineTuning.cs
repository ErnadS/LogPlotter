using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OCXO_App
{
    public class FineTuning
    {
        int nCounter = 0;

        public TuningResult tune(double lastDAC, double lastPhase)
        {
            nCounter ++;
            if (nCounter == 50) {
                nCounter = 0;

                if (lastPhase < 0)
                {
                    lastDAC--;
                    //return new TuningResult(lastDAC - 1, TuningResult.Result.NOT_FINISHED);  // aging (+1)
                }
                else
                {
                    lastDAC++;
                    //return new TuningResult(lastDAC + 1, TuningResult.Result.NOT_FINISHED);
                }
            }
            return new TuningResult(lastDAC, TuningResult.Result.NOT_FINISHED);
        }

        //public double 
    }
}
