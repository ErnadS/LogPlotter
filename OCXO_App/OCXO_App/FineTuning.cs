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
        //AverageExp phaseAverageExp = new AverageExp();
        SlidingFrame phaseAverageExp = new SlidingFrame();
        bool firstTime = true;
        double phaseAverage = 0;

        public TuningResult tune(double lastDAC, double lastPhase)
        {
            if (firstTime)
            {
                phaseAverageExp.init(100, 30);
                firstTime = false;
            }
            //phaseAverage = phaseAverageExp.calculateExpAvg(lastPhase);
            phaseAverageExp.AddPoint(lastPhase);
            nCounter ++;
            if (nCounter == 100) {
                nCounter = 0;
                /*
                if (phaseAverageExp.phaseAvg_stop < 0)
                {
                    if(Math.Abs(phaseAverageExp.phaseAvg_start) < Math.Abs(phaseAverageExp.phaseAvg_stop)) { lastDAC--; }
                    //return new TuningResult(lastDAC - 1, TuningResult.Result.NOT_FINISHED);  // aging (+1)
                }
                else
                {
                    if (Math.Abs(phaseAverageExp.phaseAvg_start) < Math.Abs(phaseAverageExp.phaseAvg_stop)) { lastDAC++; }
                    //return new TuningResult(lastDAC + 1, TuningResult.Result.NOT_FINISHED);
                }
                //oldPhase = Math.Abs(phaseAverage);
                */
                if(Math.Abs(phaseAverageExp.phaseAvg_stop) < 8*Math.Pow(10,-9) && Math.Abs(phaseAverageExp.part_angle) < 4)
                {
                    //do nothing
                }
                else if(Math.Abs(phaseAverageExp.phaseAvg_stop) > 8 * Math.Pow(10, -9))
                {
                    if(phaseAverageExp.phaseAvg_stop > 7 * Math.Pow(10, -9))
                    {
                        if(phaseAverageExp.part_angle > 4)
                        {
                            lastDAC += 2;
                        }
                        else if(phaseAverageExp.part_angle > 0)
                        {
                            lastDAC += 1;
                        }
                    }
                    else if(phaseAverageExp.phaseAvg_stop < -7 * Math.Pow(10, -9))
                    {
                        if (phaseAverageExp.part_angle < -4)
                        {
                            lastDAC -= 2;
                        }
                        else if (phaseAverageExp.part_angle < 0)
                        {
                            lastDAC -= 1;
                        }
                    }
                }
            }
            return new TuningResult(lastDAC, TuningResult.Result.NOT_FINISHED);
        }

        //public double 
    }
}
