using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCXO_App
{
    class Regulator // Regulator Class, implemented PI regulator with anti-windup scheme (anti-windup used because saturation at the output of the PI regulator)
    {
        /* Constants for PI Regulator */
        int saturationMax = 300, saturationMin = -300;
        double refPhase = 0;
        double p = -10123911730;
        double i = -20247823460/10;
        double phase = 0;
        public double dACOffset = 126690;
        static Int16 counter = 0;
        static double dacacc = 0;
        double iSignal = 0, satSignal = 0;
        double dac_corr = 0, e_t_acc = 0;
        double Ti = 0.0001;
        Int16 N = 60;
        /* Constants for PI Regulator */

        public Regulator() { }
        AverageExp average = new AverageExp();

        public Regulator(long P, long I, int saturationMax, int saturationMin, double DACOffset, double refPhase, double phase)
        {
            this.p = P;
            this.i = I;
            this.saturationMax = saturationMax;
            this.saturationMin = saturationMin;
            this.dACOffset = DACOffset;
            this.refPhase = refPhase;
            this.phase = phase;
        }

        int SaturationMax { get; set; }
        int SaturationMin { get; set; }
        double RefPhase { get; set; }
        double P { get; set; }
        double I { get; set; }
        double Phase { get; set; }
        public double DACOffset { get; set; }

        public double nextValue(double lastDAC, double lastPhase, int nTime)
        {
            double nextDAC = 0;
            double e_t, p_t, iSignal_temp;
            double deltaPhase, pSignal = 0, piSignal, piSaturated = 0;
            double newPhase = average.calculateExpAvg(lastPhase);
            /*
            counter++;
            if(counter == 4*N)
            {
                //dac_corr = dACOffset - dacacc / (N - 1);
                //iSignal += dac_corr;
                dACOffset = dacacc/(4*N-1);
                dacacc = 0;
                counter = 0;
            }
            else
            {
                dacacc += lastDAC;
                dac_corr = 0;
            }*/

            //Calculate error from first adder
            e_t = refPhase - newPhase;
            //e_t_acc = e_t_acc + Ti * e_t;

            //Calculate error from second adder
            deltaPhase = newPhase - phase;
            p_t = e_t - deltaPhase;

            //PI regulator output

            //P line
            pSignal = p_t * p;

            //I line
            if (Math.Abs(satSignal) < 0.000000001)
                iSignal_temp = p_t * i;
            else
            {
                //iSignal = 0;               //  In case we reset the integrator in anti windup, we use this clause
                iSignal_temp = 0;            //  In case we keep the integrator value constant, we use this one
            }

            iSignal += iSignal_temp;


            //I line
            //iSignal += p_t * i + satSignal;    // Back-calculation wind-up scheme

            //Saturation line
            piSignal = (pSignal + iSignal);
            if (piSignal > saturationMax)
                piSaturated = saturationMax;
            else if (piSignal < saturationMin)
                piSaturated = saturationMin;
            else
                piSaturated = piSignal;

            satSignal = piSaturated - piSignal;

            //Calculate nextDAC by adding DAC offset
            nextDAC = piSaturated + dACOffset ;

            //Update values
            phase = newPhase;

            return nextDAC;
        }
    }
}
