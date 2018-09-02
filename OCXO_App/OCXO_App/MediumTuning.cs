using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace OCXO_App
{
    public class MediumTuning
    {/*
        double phaseAvg_part_1_A; // startna faza od prvog dijela
        double phaseAvg_part_1_B; // krajnja faza od prvog dijela
        double phaseAvg_part_2_A;
        double phaseAvg_part_2_B;*/
        /// <summary>
        /// //////////////////////////////////////////////////////
        /// </summary>
        /// 
        /*
        double DAC_part_1;
        double DAC_part_2;
        */
     

        int nCounterPhase = 0;

        /*****************************************
        * Funkcija za racunanje finog podesenja. Return je nova vrijednost DAC-a
        ******************************************/
        // public const int VRIJEME_UMIRIVANJA = 15;
        // public const int VRIJEME_MJERENJA_BLOKA = VRIJEME_UMIRIVANJA *2 + AverageExp.AVG_SIZE * 2;//TODO:


        // AverageExp phaseExpAvg;

        const int TUNNING_SLEEP_TIME = 20; // kada jednom izracunamo "optimalni" DAC, onda ce funkcija "spavati" u "TUNNING_SLEEP_TIME" sec. i onda opet iz pocetka.
        const int FRAME_SIZE = 30;   // u jednom frame mjerimo 2 AVG_TIME. Znaci avg od prvih 10 i zadnjih 10 (sa 10 izmedju koji se ne koriste za sada)
        const int AVG_TIME = 10;

        int tunningSleepCounter = 0;

        double calculatedDAC = 0;  // ovu bi trebali resetovati kada ispadne iz medium tuninga (ili samo napraviti novi objekat ove klase?)
        double lastOptimalDac = 0;

        enum MediumState
        {
            TUNING_SLEEP_1,  // kada ne diramo DAC npr. 100 sec da vidimo kako se ponasa. Ovdje cemo kasnije mozda mjenjati DAC za starenje ili cemo ici na fino mjerenje
            MEASURING_BLOCK_1,
            TUNING_SLEEP_2,
            MEASURING_BLOCK_2,          
            FINISHED
        }

        MediumState state = MediumState.TUNING_SLEEP_1;

        SlidingFrame slidingFrame_1;
        SlidingFrame slidingFrame_2;

        double DAC_frame_1;
        double DAC_frame_2;


        public MediumTuning() {
            slidingFrame_1 = new SlidingFrame();
            slidingFrame_1.init(FRAME_SIZE, AVG_TIME);

            slidingFrame_2 = new SlidingFrame();
            slidingFrame_2.init(FRAME_SIZE, AVG_TIME);
        }


        public TuningResult calculateMediumTuning(double lastDAC, double lastPhase, int nTime)
        {
            if (state == MediumState.TUNING_SLEEP_1)
            {
                tunningSleepCounter++;
                if (tunningSleepCounter == TUNNING_SLEEP_TIME)
                {
                    resetMediumTuning(); // idemo opet da mjerimo 2 blocka
                    DAC_frame_1 = lastDAC;
                    state = MediumState.MEASURING_BLOCK_1;
                }
                return new TuningResult(lastDAC, TuningResult.Result.NOT_FINISHED);
            }
            else if (state == MediumState.MEASURING_BLOCK_1)
            {
                slidingFrame_1.AddPoint(lastPhase);

                if (slidingFrame_1.finished) //   measure_blocks_1(lastDAC, lastPhase))
                {
                    double crossingZeroTime = getZerroCrosingTime();
                    if (crossingZeroTime < TUNNING_SLEEP_TIME + FRAME_SIZE)
                    {
                        if (lastOptimalDac != 0)
                        {
                            calculatedDAC = lastOptimalDac; // Crossing zero soon. Set optimal dac and do not make a measure block 2
                            state = MediumState.TUNING_SLEEP_1; // do not go to tuning state 2 (back to start). 
                            tunningSleepCounter = 0;
                            writeServiceFile("Time: " + nTime + ". crossingZeroTime = " + crossingZeroTime + ". New DAC = " + lastOptimalDac);
                            return new TuningResult(lastOptimalDac, TuningResult.Result.NOT_FINISHED);
                        }
                    }
                    else
                    {
                        writeServiceFile("Time: " + nTime + ". crossingZeroTime not soon = " + crossingZeroTime + ". Measure block 2");
                    }

                    state = MediumState.TUNING_SLEEP_2; //  MEASURING_BLOCK_2;
                    calculatedDAC = lastDAC + calculateDacStep(slidingFrame_1.phaseAvg_stop); //   block_1.phaseAvg_stop);
                    DAC_frame_2 = calculatedDAC;
                }
                else
                {
                    calculatedDAC = lastDAC;
                }
                return new TuningResult(calculatedDAC, TuningResult.Result.NOT_FINISHED);
            }
            else if (state == MediumState.TUNING_SLEEP_2)
            {
                tunningSleepCounter++;
                if (tunningSleepCounter == TUNNING_SLEEP_TIME)
                {
                    tunningSleepCounter = 0;
                    state = MediumState.MEASURING_BLOCK_2;
                }
                return new TuningResult(lastDAC, TuningResult.Result.NOT_FINISHED);
            }
            else if (state == MediumState.MEASURING_BLOCK_2)
            {
                slidingFrame_2.AddPoint(lastPhase);

                if (slidingFrame_2.finished) //measure_blocks_2(lastDAC, lastPhase))  // zavrsio oba bloka
                {
                    calculateNewDac(nTime);

                    // TESTIRAMO MEDIUM. NECEMO ICI U FINE TUNINIG
                    /*
                    if (Math.Abs(block_2.part_angle) < 3 && block_2.phaseAvg_stop < 5 * Math.Pow(10, -9)) // blizu nule i lagano se mijenja
                    {
                        state = MediumState.FINISHED;
                        return new TuningResult(calculatedDAC, TuningResult.Result.FINISHED);
                    }
                    else
                    {*/
                        state = MediumState.TUNING_SLEEP_1;
                   // }
                }

                return new TuningResult(calculatedDAC, TuningResult.Result.NOT_FINISHED);
            }
            else
            {
                writeServiceFile("!!!! NE BI SMJELO DOCI OVDJE, State: " + state.ToString());
                return new TuningResult(lastDAC, TuningResult.Result.NOT_FINISHED);
            }
        }
        /*
        private bool measure_blocks_1(double lastDAC, double lastPhase)
        {
            if (block_1.nCounter == 0)
            {
                block_1.init(lastDAC, VRIJEME_UMIRIVANJA + AverageExp.AVG_SIZE, VRIJEME_MJERENJA_BLOKA);
            }
            
            block_1.AddPoint(lastPhase);

            if (block_1.finished)
                return true;

            return false;
        }

        private bool measure_blocks_2(double lastDAC, double lastPhase)
        {
            if (block_2.nCounter == 0)
            {
                block_2.init(lastDAC, VRIJEME_UMIRIVANJA + AverageExp.AVG_SIZE, VRIJEME_MJERENJA_BLOKA);
            }

            block_2.AddPoint(lastPhase);

            if (block_2.finished)
                return true;

            return false;
        }*/
        
        private int calculateDacStep(double phaseAvg_part_1_B)
        {
            double absPart_1_B_ns = Math.Abs(phaseAvg_part_1_B)*Math.Pow(10,9);
            int nDacChangeStep;

            if (absPart_1_B_ns < 3)
                nDacChangeStep = 0;
            if (absPart_1_B_ns < 6)  
                nDacChangeStep = 1;   
            if (absPart_1_B_ns < 8)  
                nDacChangeStep = 2;
            else if (absPart_1_B_ns < 12)
                nDacChangeStep = 4;
            else if (absPart_1_B_ns < 20)
                nDacChangeStep = 9;
            else if (absPart_1_B_ns < 30)
                nDacChangeStep = 20;
            else if (absPart_1_B_ns < 100)
                nDacChangeStep = 50;
            else
                nDacChangeStep = 150;

            if (phaseAvg_part_1_B < 0)
            {
                nDacChangeStep *= -1;
            }
            return nDacChangeStep;
        }
        /*
        private double calculateNewDac()
        {
            double newDAC = block_1.part_DAC - (block_1.phaseAvg_stop - block_1.phaseAvg_start) * (block_2.part_DAC - block_1.part_DAC) /
                ((block_2.phaseAvg_stop - block_2.phaseAvg_start) - (block_1.phaseAvg_stop - block_1.phaseAvg_start));
            writeServiceFile("DAC_part_1 = " + block_1.part_DAC + ", DAC_part_2 = " + block_2.part_DAC);
            writeServiceFile("Old DAC: " + calculatedDAC + ", New DAC: " + newDAC);
            if (calculatedDAC != 0) // ako nije prvo mjerenje medium tuningcalculatedDAC-a:
                writeServiceFile("Starenje u zadnjih 100 sec: " + (newDAC - calculatedDAC));

            calculatedDAC = newDAC;
            return calculatedDAC;
        }
        */
        private double calculateNewDac(int nTime)
        {
            if (slidingFrame_1.part_angle == slidingFrame_2.part_angle) // ako se ugao nije promjenio, dijelicemo sa nulom i dobiti beskonacan broj
                return calculatedDAC; // vrati proslu vrijednost
            
            double newDAC = DAC_frame_1 - (slidingFrame_1.phaseAvg_stop - slidingFrame_1.phaseAvg_start) * (DAC_frame_2 - DAC_frame_1) /
                ((slidingFrame_2.phaseAvg_stop - slidingFrame_2.phaseAvg_start) - (slidingFrame_1.phaseAvg_stop - slidingFrame_1.phaseAvg_start));
            writeServiceFile("Time: " + nTime + ". DAC_frame_1 = " + DAC_frame_1 + "start: " + slidingFrame_1.phaseAvg_start + ", stop: " + slidingFrame_1.phaseAvg_stop +
                           ", DAC_frame_2 = " + DAC_frame_2 + "start: " + slidingFrame_2.phaseAvg_start + ", stop: " + slidingFrame_2.phaseAvg_stop);
            writeServiceFile("Old DAC: " + calculatedDAC + ", New DAC: " + newDAC);
            if (calculatedDAC != 0) // ako nije prvo mjerenje medium tuningcalculatedDAC-a:
                writeServiceFile("Starenje u zadnjih " + FRAME_SIZE + " sec: " + (slidingFrame_2.phaseAvg_stop - slidingFrame_1.phaseAvg_stop));

            calculatedDAC = newDAC;
            lastOptimalDac = newDAC; // TODO: imamo previse varijabli. "calculatedDAC bi trebao biti moznda "lastDac" a izracunati: "optimalDAC" ...
            return calculatedDAC;
        }

        
        /*
        private double calculateDAC()
        {
            double newDAC = DAC_part_1 - (phaseAvg_part_1_B - phaseAvg_part_1_A) * (DAC_part_2 - DAC_part_1) / ((phaseAvg_part_2_B - phaseAvg_part_2_A) - (phaseAvg_part_1_B - phaseAvg_part_1_A));
            writeServiceFile("DAC_part_1 = " + DAC_part_1 + ", DAC_part_2 = " + DAC_part_2);
            writeServiceFile("Old DAC: " + calculatedDAC + ", New DAC: " + newDAC);
            if (calculatedDAC != 0) // ako nije prvo mjerenje medium tuningcalculatedDAC-a:
                writeServiceFile("Starenje u zadnjih 100 sec: " + (newDAC - calculatedDAC));

            calculatedDAC = newDAC;
            return calculatedDAC;
        }*/

        public void resetMediumTuning()
        {
            nCounterPhase = 0;         
            tunningSleepCounter = 0;

            slidingFrame_1 = new SlidingFrame();
            slidingFrame_1.init(30, 10);

            slidingFrame_2 = new SlidingFrame();
            slidingFrame_2.init(30, 10);
        }

        private void writeServiceFile(string str)
        {
            using (StreamWriter st = new StreamWriter("serviceFile.txt", true))
            {
                st.WriteLine(str);
                st.Close();
            }
        }

        double getZerroCrosingTime()
        {
            double x1 = 0;
            double x2 = FRAME_SIZE - AVG_TIME;
            double y1 = slidingFrame_2.phaseAvg_start;
            double y2 = slidingFrame_2.phaseAvg_stop;
            double x = x1 - y1 * (x2 - x1) / (y2 - y1);
            return x;
        }
    }
}
