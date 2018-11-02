using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OCXO_App
{
    public class Phase
    {
        public static double calculatePhaseFromInputString(string input)   //calculating phase Value from received string from OCXO Unit or OCXO Test Board
        {
            string[] subInputs = input.Split(',');
            List<Int32> inputValues = new List<Int32>();
            double phase = 0;
            foreach (string s in subInputs)
            {
                inputValues.Add(Int32.Parse(s));
            }
            if (inputValues.Count == 5)
            {
                // inputValues[2] - 32-bit counter, used for coarse measurement
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // Fine measurement
                // inputValues[3] * (20 * Math.Pow(10, (-11)))
                // inputValues[3] number of logical cells which has log. 1 at output
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                if (inputValues[0] == 0)   // inputValues[0] = 0, external PPS comes first
                {
                    if (inputValues[1] == 0)  // inputValues[1] == 0, phase is small use coarse and fine measurement, phase sampling frequency 400MHz (on OCXO unit is 409.6MHz)
                    {
                        phase = ((Convert.ToDouble(inputValues[2]) / (409600000)) + inputValues[4] * (15 * Math.Pow(10, (-11))) - inputValues[3] * (15 * Math.Pow(10, -11))) * (-1);             
                    }
                    else // phase is not small, use coarse measurement only, phase sampling frequency is 200MHz
                    {
                        phase = (Convert.ToDouble(inputValues[2])) / 200000000 * (-1); // 200MHz sampling
                    }
                }
                else
                {
                    if (inputValues[1] == 0)
                    {
                        phase = ((Convert.ToDouble(inputValues[2]) / (409600000)) + inputValues[4] * (15 * Math.Pow(10, (-11))) - inputValues[3] * (15 * Math.Pow(10, -11)));
                    }
                    else
                    {
                        phase = (Convert.ToDouble(inputValues[2])) / 200000000;
                    }
                }
            }
            return phase;
        }


        public static double calculatePhaseFromInputString_B(string input)
        {
            string[] subInputs = input.Split(',');
            List<Int32> inputValues = new List<Int32>();
            double phase = 0;
            /****************************************
             * calculating phase from input string, raw data (not used)
             ****************************************/
            foreach (string s in subInputs)
            {
                inputValues.Add(Int32.Parse(s));
            }
            phase = inputValues[2];

            if (inputValues[0] == 0)
            {
                phase *= (-1);
            }

            return phase;
        }
    }
}
