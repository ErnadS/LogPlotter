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
        MediumTuningResult block_1 = new MediumTuningResult();
        MediumTuningResult block_2 = new MediumTuningResult();

        int nCounterPhase = 0;

        /*****************************************
        * Funkcija za racunanje finog podesenja. Return je nova vrijednost DAC-a
        ******************************************/
        public const int VRIJEME_UMIRIVANJA = 20;//bilo 5
        public const int VRIJEME_MJERENJA_BLOKA = 30;//bilo 10


        // AverageExp phaseExpAvg;

        const int TUNNING_SLEEP_TIME = 100; // kada jednom izracunamo "optimalni" DAC, onda ce funkcija "spavati" u 100 sec. i onda opet iz pocetka.
        int tunningSleepCounter = 0;

        double calculatedDAC = 0;  // ovu bi trebali resetovati kada ispadne iz medium tuninga (ili samo napraviti novi objekat ove klase?)

        enum MediumState
        {
            MEASURING_BLOCK_1,
            MEASURING_BLOCK_2,
            TUNING_SLEEP,  // kada ne diramo DAC npr. 100 sec da vidimo kako se ponasa. Ovdje cemo kasnije mozda mjenjati DAC za starenje ili cemo ici na fino mjerenje
            FINISHED
        }

        MediumState state = MediumState.MEASURING_BLOCK_1;


        public TuningResult calculateMediumTuning(double lastDAC, double lastPhase)
        {
            if (state == MediumState.TUNING_SLEEP)
            {
                tunningSleepCounter++;
                if (tunningSleepCounter == TUNNING_SLEEP_TIME)
                {
                    resetMediumTuning(); // idemo opet da mjerimo 2 blocka
                }
                return new TuningResult(lastDAC, TuningResult.Result.NOT_FINISHED);
            }
            else if (state == MediumState.MEASURING_BLOCK_1)
            {
                if (measure_blocks_1(lastDAC, lastPhase))
                    state = MediumState.MEASURING_BLOCK_2;

                return new TuningResult(lastDAC, TuningResult.Result.NOT_FINISHED);
            }
            else if (state == MediumState.MEASURING_BLOCK_2)
            {
                if (measure_blocks_2(lastDAC, lastPhase))
                {
                    calculateNewDac();
                    if (block_2.part_angle < 5 && block_2.phaseAvg_stop < 5) // blizu nule i lagano se mijenja
                    {
                        state = MediumState.FINISHED;
                        return new TuningResult(calculatedDAC, TuningResult.Result.FINISHED);
                    }
                }

                return new TuningResult(calculatedDAC, TuningResult.Result.NOT_FINISHED);
            }
            else
            {
                writeServiceFile("!!!! NE BI SMJELO DOCI OVDJE, State: " + state.ToString());
                return new TuningResult(lastDAC, TuningResult.Result.NOT_FINISHED);
            }
        }

        private bool measure_blocks_1(double lastDAC, double lastPhase)
        {
            if (block_1.nCounter == 0)
            {
                block_1.init(lastDAC, VRIJEME_UMIRIVANJA, VRIJEME_MJERENJA_BLOKA);
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
                block_2.init(lastDAC, VRIJEME_UMIRIVANJA, VRIJEME_MJERENJA_BLOKA);
            }

            block_2.AddPoint(lastPhase);

            if (block_2.finished)
                return true;

            return false;
        }
        /*
        private double measure_2_blocks(double lastDAC, double lastPhase)
        {
            if (nCounterPhase == 0)  // Startamo mjerenje prvog bloka
            {
                phaseExpAvg = new AverageExp(); // startamo average
                nCounterPhase++;
                phaseExpAvg.calculateExpAvg(lastPhase); // ne treba nam average vrijednost nego samo ubacujemo da on gradi novu average
                DAC_part_1 = lastDAC;
                return DAC_part_1;
            }
            else if (nCounterPhase == (VRIJEME_UMIRIVANJA))
            {
                nCounterPhase++;
                phaseAvg_part_1_A = phaseExpAvg.calculateExpAvg(lastPhase);  // Uzmimo prvu tacku
                writeServiceFile("Prva tacka: " + phaseAvg_part_1_A.ToString());
                return DAC_part_1;
            }
            else if (nCounterPhase == (VRIJEME_UMIRIVANJA + VRIJEME_MJERENJA_BLOKA))
            {
                nCounterPhase++;
                phaseAvg_part_1_B = phaseExpAvg.calculateExpAvg(lastPhase); // Uzmimo drugu tacku
                writeServiceFile("Druga tacka: " + phaseAvg_part_1_B.ToString());

                int nDacChangeStep = calculateDacStep(phaseAvg_part_1_B);

                // Promjenimo DAC
                if((phaseAvg_part_1_B - phaseAvg_part_1_A) < 0) // opada
                {
                    if(phaseAvg_part_1_B > 0)   // i pozitivan (priblizava se nuli)
                    {
                        DAC_part_2 = lastDAC + nDacChangeStep;
                    } else
                    {
                        DAC_part_2 = lastDAC - nDacChangeStep;
                    }
                } else
                {
                    if(phaseAvg_part_1_B > 0)
                    {
                        DAC_part_2 = lastDAC + nDacChangeStep;
                    } else
                    {
                        DAC_part_2 = lastDAC - nDacChangeStep;
                    }
                }
                return DAC_part_2;
            }
            else if (nCounterPhase == 2 * VRIJEME_UMIRIVANJA + VRIJEME_MJERENJA_BLOKA) // proslo jos jedno vrijeme umirivanja poslije promjene DAC-a
            {
                nCounterPhase++;
                phaseAvg_part_2_A = phaseExpAvg.calculateExpAvg(lastPhase); // Uzmimo tacku 3
                writeServiceFile("Treca tacka: " + phaseAvg_part_2_A.ToString());
                return DAC_part_2;
            }
            else if (nCounterPhase == 2 * (VRIJEME_UMIRIVANJA + VRIJEME_MJERENJA_BLOKA))  // End
            {
                phaseAvg_part_2_B = phaseExpAvg.calculateExpAvg(lastPhase); // Uzmimo tacku 4
                writeServiceFile("Cetvrta tacka tacka: " + phaseAvg_part_2_B.ToString());
                
                double newDac = calculateDAC();
                
                state = MediumState.TUNING_SLEEP; // necemo mjeanjati DAC u narednih 100 sec.
                return newDac;
            }
            else
            {
                nCounterPhase++;
                phaseExpAvg.calculateExpAvg(lastPhase); // dodaj tacku u average
                calculatedDAC = lastDAC;
                return lastDAC;
            }
        }
        */
        private int calculateDacStep(double phaseAvg_part_1_B)
        {
            double absPart_1_B = Math.Abs(phaseAvg_part_1_B);
            int nDacChangeStep;

            if (absPart_1_B < 10)
                nDacChangeStep = 1;
            else if (absPart_1_B < 30)
                nDacChangeStep = 2;
            else if (absPart_1_B < 60)
                nDacChangeStep = 4;
            else if (absPart_1_B < 100)
                nDacChangeStep = 10;
            else
                nDacChangeStep = 20;

            return nDacChangeStep;
        }


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
            state = MediumState.MEASURING_BLOCK_1;
            tunningSleepCounter = 0;
        }

        private void writeServiceFile(string str)
        {
            using (StreamWriter st = new StreamWriter("service.txt", true))
            {
                st.WriteLine(str);
                st.Close();
            }
        }
    }
}
