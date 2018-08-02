using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCXO_App
{
    public class Ageing
    {
        int nCounter = 0;
        double oldPhase = 0;
        public TuningResult ageing(double lastDAC, double lastPhase)
        {
            nCounter++;
            if(nCounter == 1000)
            {
                nCounter = 0;
                lastDAC--;
                /*
                if (lastPhase < 0)
                {
                    if (oldPhase < Math.Abs(lastPhase)) { lastDAC--; }
                    //return new TuningResult(lastDAC - 1, TuningResult.Result.NOT_FINISHED);  // aging (+1)
                }
                else
                {
                    if (oldPhase < Math.Abs(lastPhase)) { lastDAC++; }
                    //return new TuningResult(lastDAC + 1, TuningResult.Result.NOT_FINISHED);
                }
                oldPhase = Math.Abs(lastPhase);*/
            }
            return new TuningResult(lastDAC, TuningResult.Result.NOT_FINISHED);
        }
    }
}
