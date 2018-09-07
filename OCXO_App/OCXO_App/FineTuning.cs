using System;
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
       

        const int TUNNING_SLEEP_TIME = 25; // 
        const int FRAME_SIZE = 40;   // u jednom frame mjerimo 2 AVG_TIME bez pauze izmedju
        const int AVG_TIME = 20;

        enum FineState
        {
            NORMAL,  // ovdje koristimo normalni algoritam (+-1 ili +-2)
            GOOING_ZERO_POSITIV,   // faza pozitivna i opada prema nuli (prati dalji razvoj i ako dodje blizu nuli, smanji DAC za 1)
            GOOING_ZERO_NEGATIVE,    // faza negativna i raste prema nuli (prati dalji razvoj i ako dodje blizu nuli, povecaj DAC za 1)
            CLOSE_TO_ZERO           // faza je oko nule (-1, 1). Prati razvoj i prebaci u normal cim izadje iz granica
        }

        FineState state = FineState.NORMAL;


        public FineTuning()
        {
            phaseAverageExp = new SlidingFrame();
            phaseAverageExp.init(FRAME_SIZE, AVG_TIME);
            nCounter = 0;
            state = FineState.NORMAL;
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

            if (state == FineState.NORMAL)  // u normalnom state, cekaj da napuni frame size i onda reaguj
            {
                if (nCounter >= TUNNING_SLEEP_TIME + FRAME_SIZE)
                {
                    nCounter = 0;

                    if (Math.Abs(phaseAverageExp.phaseAvg_stop) < 1 * Math.Pow(10, -9))  // TODO: ovo nam mozda ne treba? Is normal state je trebao preci u gooing to zero ako se priblizuje nuli. Ovdje ispada da je iznenada skocio blizu nule.
                    {
                        state = FineState.CLOSE_TO_ZERO;
                        writeServiceFile("Time: " + nTime + ". FT*1: Unexpected |phase| <1. Phase: " + phaseAverageExp.phaseAvg_stop + ", angle: " + phaseAverageExp.part_angle + ", new state = CLOSE_TO_ZERO. DAC not changed");
                        return new TuningResult(lastDAC, TuningResult.Result.NOT_FINISHED); // nemoj nista mijenjati
                    }
                    // PHASE IS POSITIVE
                    else if (phaseAverageExp.phaseAvg_stop > 1 * Math.Pow(10, -9))
                    {
                        if (phaseAverageExp.part_angle < 0) // ide prema nuli
                        {
                            writeServiceFile("Time: " + nTime + ". FT*2: phase positive: " + phaseAverageExp.phaseAvg_stop + ", angle: " + phaseAverageExp.part_angle + ". going to zero, new state = GOOING_ZERO_POSITIV. DAC not changed: " + lastDAC);
                            state = FineState.GOOING_ZERO_POSITIV;
                            return new TuningResult(lastDAC, TuningResult.Result.NOT_FINISHED);
                        }
                        else
                        {
                            if (phaseAverageExp.phaseAvg_stop < 3.8 * Math.Pow(10, -9))   // faza manja od 2.5, povecaj DAC za 1. // TODO: ne bi trebali gledati fazu nego ugao? Ako je ugao < xx, +1 ili +2?
                            {
                                writeServiceFile("Time: " + nTime + ". FT*3: phase positive: " + phaseAverageExp.phaseAvg_stop + ", angle: " + phaseAverageExp.part_angle + ".  not going to zero, increase one New dac: " + (lastDAC + 1));
                                return new TuningResult(lastDAC + 1, TuningResult.Result.NOT_FINISHED);
                            }
                            else
                            {   // faza je veca od 2.5, povecaj za 2
                                writeServiceFile("Time: " + nTime + ". FT*4: phase positive: " + phaseAverageExp.phaseAvg_stop + ", angle: " + phaseAverageExp.part_angle + ".  not going to zero, increase two New dac: " + (lastDAC + 1));
                                return new TuningResult(lastDAC + 2, TuningResult.Result.NOT_FINISHED);
                            }

                        }
                    }
                    else // phase < -1 ns
                    {
                        if (phaseAverageExp.part_angle > 0) // ide prema nuli
                        {
                            writeServiceFile("Time: " + nTime + ". FT*5: phase negative: " + phaseAverageExp.phaseAvg_stop + ", angle: " + phaseAverageExp.part_angle + ".  going to zero, new state = GOOING_ZERO_NEGATIVE. DAC not changed: " + lastDAC);
                            state = FineState.GOOING_ZERO_NEGATIVE;
                            return new TuningResult(lastDAC, TuningResult.Result.NOT_FINISHED);
                        }
                        else
                        {
                            if (phaseAverageExp.phaseAvg_stop > -3.8 * Math.Pow(10, -9))
                            {
                                writeServiceFile("Time: " + nTime + ". FT*6: phase negative: " + phaseAverageExp.phaseAvg_stop + ", angle: " + phaseAverageExp.part_angle + ".  not going to zero, decrease one. New dac: " + (lastDAC - 1));
                                return new TuningResult(lastDAC - 1, TuningResult.Result.NOT_FINISHED);
                            }
                            else
                            {
                                writeServiceFile("Time: " + nTime + ". FT*7: phase negative: " + phaseAverageExp.phaseAvg_stop + ", angle: " + phaseAverageExp.part_angle + ".  not going to zero, decrease two. New dac: " + (lastDAC - 1));
                                return new TuningResult(lastDAC - 2, TuningResult.Result.NOT_FINISHED);
                            }
                        }
                    }
                }
                else
                {
                    return new TuningResult(lastDAC, TuningResult.Result.NOT_FINISHED); // nemoj nista mijenjati, cekamo da zavrsi sliding frame
                }
            }
            else if (state == FineState.GOOING_ZERO_POSITIV)
            {
                if (phaseAverageExp.phaseAvg_stop < 1 * Math.Pow(10, -9))  // usli blizu nule, zaustavi opadanje
                {
                    state = FineState.CLOSE_TO_ZERO;
                    writeServiceFile("Time: " + nTime + ". FT1: phase positive, gooing zero. Phase: " + phaseAverageExp.phaseAvg_stop + ", angle: " + phaseAverageExp.part_angle + ". New state: CLOSE_TO_ZERO, New dac -1: " + lastDAC);
                    return new TuningResult(lastDAC-1, TuningResult.Result.NOT_FINISHED);
                }
                else if (phaseAverageExp.part_angle > 0) // ne ide vise prema nuli
                {
                    state = FineState.NORMAL; // vrati na normalnu regulaciju
                    nCounter = TUNNING_SLEEP_TIME + FRAME_SIZE - 1;  // Stavi counter tako da sljedeci put izracuna kao da je normalna regulacija
                    writeServiceFile("Time: " + nTime + ". FT2: phase positive, gooing zero. !!! Now is gooing up " + phaseAverageExp.phaseAvg_stop + ", angle: " + phaseAverageExp.part_angle + ". New state: NORMAL. DAC not changed: " + lastDAC);
                    return new TuningResult(lastDAC, TuningResult.Result.NOT_FINISHED); // ??? nemoj mijenjati DAC? jer je mozda privremeni skok?
                }
                else
                {
                    return new TuningResult(lastDAC, TuningResult.Result.NOT_FINISHED); // do nothing
                }
            }
            else if (state == FineState.GOOING_ZERO_NEGATIVE)
            {
                if (phaseAverageExp.phaseAvg_stop > -1 * Math.Pow(10, -9))  // usli blizu nule, zaustavi rast
                {
                    state = FineState.CLOSE_TO_ZERO;
                    writeServiceFile("Time: " + nTime + ". FT3: phase negative, gooing zero. Phase: " + phaseAverageExp.phaseAvg_stop + ", angle: " + phaseAverageExp.part_angle + ". New state: CLOSE_TO_ZERO, New dac +1: " + lastDAC);
                    return new TuningResult(lastDAC + 1, TuningResult.Result.NOT_FINISHED);
                }
                else if (phaseAverageExp.part_angle < 0) // ne ide vise prema nuli
                {
                    state = FineState.NORMAL; // vrati na normalnu regulaciju
                    nCounter = TUNNING_SLEEP_TIME + FRAME_SIZE - 1;  // Stavi counter tako da sljedeci put izracuna kao da je normalna regulacija
                    writeServiceFile("Time: " + nTime + ". FT4: phase negative, gooing zero. !!! Now is gooing down " + phaseAverageExp.phaseAvg_stop + ", angle: " + phaseAverageExp.part_angle + ". New state: NORMAL. DAC not changed: " + lastDAC);
                    return new TuningResult(lastDAC, TuningResult.Result.NOT_FINISHED); // ??? nemoj mijenjati DAC? jer je mozda privremeni skok?
                }
                else
                {
                    return new TuningResult(lastDAC, TuningResult.Result.NOT_FINISHED); // do nothing
                }
            }
            else // if (state == FineState.CLOSE_TO_ZERO)
            {
                if (Math.Abs(phaseAverageExp.phaseAvg_stop) > 1) // iskocio iz zone blizu nule
                {
                    state = FineState.NORMAL; // vrati na normalnu regulaciju
                    nCounter = TUNNING_SLEEP_TIME + FRAME_SIZE - 1;  // Stavi counter tako da sljedeci put izracuna kao da je normalna regulacija
                    writeServiceFile("Time: " + nTime + ". FT5: phase was close to zero, but now out of limit: " + phaseAverageExp.phaseAvg_stop + ", angle: " + phaseAverageExp.part_angle + ". New state: NORMAL. DAC not changed: " + lastDAC);
                    return new TuningResult(lastDAC, TuningResult.Result.NOT_FINISHED); // ??? nemoj mijenjati DAC? jer je mozda privremeni skok?
                }
                else
                    return new TuningResult(lastDAC, TuningResult.Result.NOT_FINISHED); // do nothing
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
