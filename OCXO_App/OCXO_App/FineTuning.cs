﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace OCXO_App
{
    public class FineTuning
    {
        int nCounter = 0;
        int smallPhasecounter = 0;
        double oldPhase = 0;
        //AverageExp phaseAverageExp = new AverageExp();
        SlidingFrame phaseAverageExp;
        bool firstTime = true;
        double phaseAverage = 0;

        private const int GOOD_PHASE  = 4; 
        private const int GOOD_ANGLE  = 2;
        /*
        const int TOTAL_FRAME_SIZE = 100;
        const int AVERAGING_SIZE = 30; // da li ovo treba smanjiti? ranije smo uzimali 10 za average

        const int IGNORING_CONST_DAC_COUNT = AVERAGING_SIZE;  // Kada jednom nadjemo "optimalanDAC", onda necemo sljedeceg traziti sve dok se ne skupi totalno novi average

        int[] lastConstDAC = {0, 0};  // ovdje cemo cuvati dva zadnja "idealna DAC-a". "optimalDAC" se racuna kao sredina izmedju njih dva
        int lastConstDAC_index = 0;
        int optimalDac = 0;  // ovo je srednja vrijednost od "lastConstDAC" elemenata

        int nTimeFromPreviousConstDAC;
        */

        const int TUNNING_SLEEP_TIME = 25; // 
        const int FRAME_SIZE = 40;   // u jednom frame mjerimo 2 AVG_TIME bez pauze izmedju
        const int AVG_TIME = 20;


        public FineTuning()
        {
            phaseAverageExp = new SlidingFrame();
            phaseAverageExp.init(FRAME_SIZE, AVG_TIME);
            nCounter = 0;
        }
        /*
        private void updateConstandDacArray(int lastDAC, int nTime) // u ovu array ubacujemo DAC za koji imamo konstantnu fazu (bez obzira da li je trenutna faza daleko od nule).
        {
            if (nTimeFromPreviousConstDAC >= IGNORING_CONST_DAC_COUNT) // ako je od proslog "idealnog DAC-a" proslo dovoljno vremena (minimalno averaging time), onda cemo prihvatiti novi "idealni DAC" ako se desi
            {
                if (Math.Abs(phaseAverageExp.part_angle) < GOOD_ANGLE)
                {
                    nTimeFromPreviousConstDAC = 0; // prihvaticemo ovu vrijednost tako da ce morati sacekati prije novog prihvacanja

                    lastConstDAC[lastConstDAC_index] = lastDAC;
                    if (lastConstDAC_index == 0 && lastConstDAC[1] == 0) // ako punimo prvi elemenat i ako drugi jos nije postavljen, stavimo i drugi na istu vrjednost (tako da prihvatimo ovu vrijednost kao idealnu)
                    {
                            lastConstDAC[1] = lastDAC;
                    }

                    if (Math.Abs(lastConstDAC[0] - lastConstDAC[1]) <= 3)
                    {
                        optimalDac = (lastConstDAC[0] + lastConstDAC[1]) / 2; // zlatna sredia (mada ce se zaokruziti na manji broj)
                        writeServiceFile("Izracunati optimal DAC: " + optimalDac.ToString() + ". Time: " + nTime);
                    }

                }
            }
            else
            {
                nTimeFromPreviousConstDAC++;
            }
        }
        */

        public TuningResult tune(double lastDAC, double lastPhase, int nTime)
        {
            phaseAverageExp.AddPoint(lastPhase);
            nCounter++;

            // writeServiceFile("C: " + nCounter);

            if (nCounter < TUNNING_SLEEP_TIME)
            {
                return new TuningResult(lastDAC, TuningResult.Result.NOT_FINISHED); // nemoj nista mijenjati
            }

            // updateConstandDacArray((int)lastDAC, nTime);

            if (nCounter == TUNNING_SLEEP_TIME + FRAME_SIZE)
            {
                nCounter = 0;

                if (Math.Abs(phaseAverageExp.phaseAvg_stop) < 2.8 * Math.Pow(10, -9))
                {
                    writeServiceFile("Time: " + nTime + ". FT: |phase| <2.8, do nothing. Phase: " + phaseAverageExp.phaseAvg_stop + ", angle: " +  phaseAverageExp.part_angle);
                    return new TuningResult(lastDAC, TuningResult.Result.NOT_FINISHED); // nemoj nista mijenjati
                }
                    // PHASE IS POSITIVE
                else if (phaseAverageExp.phaseAvg_stop > 2.8 * Math.Pow(10, -9))
                {
                    if (phaseAverageExp.part_angle < 0) // ide prema nuli
                    {
                        writeServiceFile("Time: " + nTime + ". FT: phase positive: " + phaseAverageExp.phaseAvg_stop + ", angle: " + phaseAverageExp.part_angle + ". going to zero, do nothing. New dac: " + lastDAC);
                        return new TuningResult(lastDAC, TuningResult.Result.NOT_FINISHED);
                    }
                    else
                    {
                        writeServiceFile("Time: " + nTime + ". FT: phase positive: " + phaseAverageExp.phaseAvg_stop + ", angle: " + phaseAverageExp.part_angle + ".  not going to zero, increase one New dac: " + (lastDAC + 1));
                        return new TuningResult(lastDAC + 1, TuningResult.Result.NOT_FINISHED);
                    }
                }
                else // phase < -2.8ns
                {
                    if (phaseAverageExp.part_angle > 0) // ide prema nuli
                    {
                        writeServiceFile("Time: " + nTime + ". FT: phase negative: " + phaseAverageExp.phaseAvg_stop + ", angle: " + phaseAverageExp.part_angle + ".  going to zero, do nothing. New dac: " + lastDAC);
                        return new TuningResult(lastDAC, TuningResult.Result.NOT_FINISHED);
                    }
                    else
                    {
                        writeServiceFile("Time: " + nTime + ". FT: phase negative: " + phaseAverageExp.phaseAvg_stop + ", angle: " + phaseAverageExp.part_angle + ".  not going to zero, decrease one. New dac: " + (lastDAC - 1));
                        return new TuningResult(lastDAC - 1, TuningResult.Result.NOT_FINISHED);
                    }
                }
            }
            else
            {
                return new TuningResult(lastDAC, TuningResult.Result.NOT_FINISHED); // nemoj nista mijenjati, cekamo da zavrsi sliding frame
            }
            
        }

        private void writeServiceFile(string str)
        {
            using (StreamWriter st = new StreamWriter("serviceFile.txt", true))
            {
                st.WriteLine(str);
                st.Close();
            }
        }
    }
}
