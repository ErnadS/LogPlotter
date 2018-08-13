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

        private const int GOOD_PHASE  = 4; 
        private const int GOOD_ANGLE  = 2;

        const int TOTAL_FRAME_SIZE = 100;
        const int AVERAGING_SIZE = 30; // da li ovo treba smanjiti? ranije smo uzimali 10 za average

        const int IGNORING_CONST_DAC_COUNT = AVERAGING_SIZE;  // Kada jednom nadjemo "optimalanDAC", onda necemo sljedeceg traziti sve dok se ne skupi totalno novi average

        int[] lastConstDAC = {0, 0};  // ovdje cemo cuvati dva zadnja "idealna DAC-a". "optimalDAC" se racuna kao sredina izmedju njih dva
        int lastConstDAC_index = 0;
        int optimalDac = 0;  // ovo je srednja vrijednost od "lastConstDAC" elemenata

        int nTimeFromPreviousConstDAC;


        private void updateConstandDacArray(int lastDAC) // u ovu array ubacujemo DAC za koji imamo konstantnu fazu (bez obzira da li je trenutna faza daleko od nule).
        {
            if (nTimeFromPreviousConstDAC >= IGNORING_CONST_DAC_COUNT) // ako je od proslog "idealnog DAC-a" proslo dovoljno vremena (minimalno averaging time), onda cemo prihvatiti novi "idealni DAC" ako se desi
            {
                if (Math.Abs(phaseAverageExp.part_angle) < GOOD_ANGLE)
                {
                    nTimeFromPreviousConstDAC = 0; // prihvaticemo ovu vrijednost tako da ce morati sacekati prije novog prihvacanja

                    lastConstDAC[lastConstDAC_index] = lastDAC;
                    if (lastConstDAC_index == 0) // ako punimo prvi elemenat i ako drugi jos nije postavljen, stavimo i drugi na istu vrjednost (tako da prihvatimo ovu vrijednost kao idealnu)
                    {
                        if (lastConstDAC[1] == 0)
                            lastConstDAC[1] = lastDAC;
                    }

                    if (Math.Abs(lastConstDAC[0] - lastConstDAC[1]) <= 3)
                        optimalDac = (lastConstDAC[0] + lastConstDAC[1]) / 2; // zlatna srednia (mada ce se zaokruziti na manji broj)
                }
            }
            else
            {
                nTimeFromPreviousConstDAC = 0;
            }
        }


        public TuningResult tune(double lastDAC, double lastPhase)
        {
            if (firstTime)
            {
                phaseAverageExp.init(TOTAL_FRAME_SIZE, AVERAGING_SIZE);  
                firstTime = false;

                nTimeFromPreviousConstDAC = IGNORING_CONST_DAC_COUNT; // Ovo je prvi put. Zelimo da prihvatimo "optimalan DAC" ako naidje u samom startu
            }
            
            phaseAverageExp.AddPoint(lastPhase);
            nCounter ++;

            updateConstandDacArray((int)lastDAC);

            if (optimalDac != 0) // ako imamo izracunat optimalni
            {
                if (Math.Abs(phaseAverageExp.phaseAvg_stop) <= GOOD_PHASE) // usli smo unutar zadovoljavajuce faze
                {
                    nCounter = 0;
                    return new TuningResult(optimalDac, TuningResult.Result.NOT_FINISHED); // ovdje je malo glupo da koristimo "NOT_FINISHED" ... Vjerovatno bi trebali samo dac je ovjde nema zavrsenog stanja?
                }
            }

            if (nCounter == 100) {
                nCounter = 0;
                // TODO: izbaci ove komentare ako nisu vazni:
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
                        if (phaseAverageExp.part_angle > 4)   // TODO: ove konstane "4" itd treba vidjeti da li je to isto kao i "GOOD_ANGLE * 2" pa zamjeniti. Trebamo one 3-4 konstante u pocetku klase ubaciti svugdje u slucaju da budemo mijenjali
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
    }
}
