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
    {
        double phaseAvg_part_1_A; // startna faza od prvog dijela
        double phaseAvg_part_1_B; // krajnja faza od prvog dijela
        double phaseAvg_part_2_A;
        double phaseAvg_part_2_B;
        /// <summary>
        /// //////////////////////////////////////////////////////
        /// </summary>
        double DAC_part_1;
        double DAC_part_2;


        int nCounterPhase = 0;

        /*****************************************
        * Funkcija za racunanje finog podesenja. Return je nova vrijednost DAC-a
        ******************************************/
        private const int VRIJEME_UMIRIVANJA = 20;//bilo 5
        private const int VRIJEME_MJERENJA_BLOKA = 30;//bilo 10


        AverageExp phaseExpAvg;

        const int TUNNING_SLEEP_TIME = 100; // kada jednom izracunamo "optimalni" DAC, onda ce funkcija "spavati" u 100 sec. i onda opet iz pocetka.
        int tunningSleepCounter = 0;

        double calculatedDAC = 0;  // ovu bi trebali resetovati kada ispadne iz medium tuninga (ili samo napraviti novi objekat ove klase?)

        enum MediumState
        {
            MEASURING_2_BLOCK,
            GOOING_TO_ZERO_PHASE, // ovo jos nemamo. Tu bi isli kada izracunamo "pravi" DAC ali nam je faza daleko od nule. Onda bi promjenili DAC na neki drugi koji nas vodi prema nuli
            // i onda u pravom trenutku stavili na izracunati DAC.
            TUNING_SLEEP  // kada ne diramo DAC npr. 100 sec da vidimo kako se ponasa. Ovdje cemo kasnije mozda mjenjati DAC za starenje ili cemo ici na fino mjerenje
        }

        MediumState state = MediumState.MEASURING_2_BLOCK;


        public double calculateMediumTuning(double lastDAC, double lastPhase)
        {
            if (state == MediumState.TUNING_SLEEP)
            {
                tunningSleepCounter++;
                if (tunningSleepCounter == TUNNING_SLEEP_TIME)
                {
                    resetMediumTuning(); // idemo opet da mjerimo 2 blocka
                }
                return lastDAC;
            }
            else if (state == MediumState.MEASURING_2_BLOCK)
            {
                return measure_2_blocks(lastDAC, lastPhase);
            }
            else
            {
                writeServiceFile("!!!! NE BI SMJELO DOCI OVDJE");
                return lastDAC;
            }
        }

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
                // Promjenimo DAC
                /*
                if (phaseAvg_part_1_B < 0)
                {
                    DAC_part_2 = lastDAC - 10; // zelimo da "zaokrenemo" DAC u suprotnom smijeru i da onda izmjerimo novu fazu-average   
                }
                else
                {
                    DAC_part_2 = lastDAC + 10;
                }*/
                if((phaseAvg_part_1_B - phaseAvg_part_1_A) < 0)
                {
                    if(phaseAvg_part_1_B > 0)
                    {
                        DAC_part_2 = lastDAC + 10;
                    } else
                    {
                        DAC_part_2 = lastDAC - 10;
                    }
                } else
                {
                    if(phaseAvg_part_1_B > 0)
                    {
                        DAC_part_2 = lastDAC + 10;
                    } else
                    {
                        DAC_part_2 = lastDAC - 10;
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
                state = MediumState.TUNING_SLEEP; // necemo mjeanjati DAC u narednih 100 sec.
                return calculateDAC();
            }
            else
            {
                nCounterPhase++;
                //Console.WriteLine("!!!! NE BI SMJELO DOCI OVDJE 2222222222");
                phaseExpAvg.calculateExpAvg(lastPhase);
                calculatedDAC = lastDAC;
                return lastDAC;
            }
        }

        private double calculateDAC()
        {
            double newDAC = DAC_part_1 - (phaseAvg_part_1_B - phaseAvg_part_1_A) * (DAC_part_2 - DAC_part_1) / ((phaseAvg_part_2_B - phaseAvg_part_2_A) - (phaseAvg_part_1_B - phaseAvg_part_1_A));
            writeServiceFile("DAC_part_1 = " + DAC_part_1 + ", DAC_part_2 = " + DAC_part_2);
            writeServiceFile("Old DAC: " + calculatedDAC + ", New DAC: " + newDAC);
            if (calculatedDAC != 0) // ako nije prvo mjerenje medium tuningcalculatedDAC-a:
                writeServiceFile("Starenje u zadnjih 100 sec: " + (newDAC - calculatedDAC));

            calculatedDAC = newDAC;
            return calculatedDAC;
        }

        public void resetMediumTuning()
        {
            nCounterPhase = 0;
            state = MediumState.MEASURING_2_BLOCK;
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
