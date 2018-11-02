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
            OUT_OF_RANGE  // to big phase difference
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
