using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OCXO_App
{
    public class TuningResult
    {
        public enum Result
        {
            NOT_FINISHED,
            FINISHED,
            OUT_OF_RANGE  // previse velika fazna razlika (treba izaci iz ovog state)
        }

        public double newDAC { get; set; }
        public Result stateResult { get; set; }

        public TuningResult(double newDAC, Result stateResult)
        {
            this.newDAC = newDAC;
            this.stateResult = stateResult;
        }
    }
}
