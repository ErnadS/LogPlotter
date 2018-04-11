using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using utility;
using GraphLog.graph;
using System.IO;

namespace GraphLog.DL2_analyse
{
    public class DL2_LogParser
    {
        List<NmeaSearchParams> listNemaSearchParams = new List<NmeaSearchParams>(new NmeaSearchParams[2]);
  
        GraphPainter graphPainter;

        FormDL2 form;

        float[] fTime;
        float[] fDAC;
        float[] fPhase;
        float[] fPhaseAvg;
        float[] fTemper;
        float[] fTimeTemper;

        float[] fPhaseDerivAvg;

        Average phaseAvg;
        Average phaseDevAvg;

        public DL2_LogParser(FormDL2 form, GraphPainter graphPainter)
        {
            this.form = form;
            this.graphPainter = graphPainter;

            phaseAvg = new Average();
            phaseDevAvg = new Average();
            // phaseDevAvg.setAlpha(0.1f);
            // phaseDevAvg.setSize(20);
        }

        public void AddPoint(float fX, float fPhase, float fDAC)
        {
            graphPainter.AddPoint(new GraphPoint(fX, fPhase), 0, false);   // "0" je za graf br. 0 (dodaj tacuku u graf br. 0)
            graphPainter.AddPoint(new GraphPoint(fX, fDAC),   1, false);   // "1" je za graf br. 1 (dodaj tacuku u graf br. 1)
        }

        public void AddPhase(float fX, float fPhase)
        {
            graphPainter.AddPoint(new GraphPoint(fX, fPhase), 0, false);   // "0" je za graf br. 0 (dodaj tacuku u graf br. 0)
        }

        public void AddDAC(float fX, float fDAC)
        {
            graphPainter.AddPoint(new GraphPoint(fX, fDAC), 1, false);   // "1" je za graf br. 1 (dodaj tacuku u graf br. 1)
        }

        public void AddTemperature(float fX, float fTemper)
        {
            graphPainter.AddPoint(new GraphPoint(fX, fTemper), 2, false);   // "1" je za graf br. 1 (dodaj tacuku u graf br. 1)
        }

        public void AddPhaseAvg(float fX, float fDACavg)
        {
            graphPainter.AddPoint(new GraphPoint(fX, fDACavg), 3, false);   //
        }

        public void AddPhaseDirect(float fX, float fDACdir)
        {
            graphPainter.AddPoint(new GraphPoint(fX, fDACdir), 4, false);   //
        }

        

        public void readFile(String strDacFilePath)
        {
            String folderName;
            int nind = strDacFilePath.LastIndexOf("\\");
            folderName = strDacFilePath.Substring(0, nind);

            String fileName = strDacFilePath.Substring(nind + 1);


            String strPhaseFilePath = folderName + "\\phase" + fileName.Substring(9); // remove "dac_value" from file name
            String strTemperFilePath = folderName + "\\temperature" + fileName.Substring(9);

            if (!File.Exists(strDacFilePath))
            {
                MessageBox.Show("DAC file not exist: " + strDacFilePath);
                return;
            } else if (!File.Exists(strPhaseFilePath))
            {
                MessageBox.Show("Phase file not exist: " + strPhaseFilePath);
                return;
            }

            int nLength = parseDacFile(strDacFilePath);

            for (int i = 0; i < fDAC.Length; i++)
            {
                AddDAC(fTime[i], fDAC[i]);
            }


            parsePhaseFile(strPhaseFilePath);

            if (fPhase != null)
            {
                for (int i = 0; i < fPhase.Length; i++)
                {
                    AddPhase(fTime[0] + i, fPhase[i] * 1000000000);   //Phase is measured each second (?). Use DAC first time as start point. Phase is in [ns]
                }
            }

            if (fPhase != null)
            {
                for (int i = 0; i < fPhase.Length; i++)
                {
                    fPhaseAvg[i] = phaseAvg.calculateAvg(fPhase[i]);
                    if (fPhaseAvg[i] < -180)
                        fPhaseAvg[i] = fPhaseAvg[i];

                    AddPhaseAvg(fTime[0] + i, fPhaseAvg[i]* 1000000000);//* 1000000000);
                }


                for (int i = 20; i < fPhase.Length; i++)
                {
                    float fPhaseDeriv = calculatePhaseFirstDerivation(i);
                    // AddPhaseDirect(fTime[0] + i, fPhaseDeriv);
                     fPhaseDerivAvg[i] = phaseDevAvg.calculateAvg(fPhaseDeriv);
                     AddPhaseDirect(fTime[0] + i, calculatePhaseSecondDerivation(i));//);
                }
            }

            parseTemperartureFile(strTemperFilePath);

            if (fTimeTemper != null)
            {
                for (int i = 0; i < fTemper.Length; i++)
                {
                    AddTemperature(fTimeTemper[i], fTemper[i]);
                }
            }
        }

        const int DEV_1_IGNORE_COUNT = 30;
        const int DEV_1_COUNT =  30;

        private float calculatePhaseFirstDerivation(int sampleNo)
        {
            if (sampleNo < DEV_1_COUNT * 2 + DEV_1_IGNORE_COUNT)
                return 0;

            float firstPart = 0;
            float secondPart = 0;

            // pogledaj DIR_COUNT prethodnih mjerenja (podjeli ih u 2 grupe i nadji sumu)
            for (int i = 0; i < DEV_1_COUNT; i++)
            {
                firstPart += fPhaseAvg[sampleNo - DEV_1_IGNORE_COUNT - i - DEV_1_COUNT];  // prvih 10
                secondPart += fPhaseAvg[sampleNo      - i];         // drugih 10
            }

            float fAngle = (secondPart - firstPart) * 1000000000 / 3;  // ne bi trebalo mnoziti sa 10000 nego dijeliti sa dX ali je ovdje to malo pogresno. X-osa mora biti u skladu sa Y da bi bilo realno govoriti o uglu
            // Za sada je najbinije da imamo polaritet ugla (opada/raste) a sta nam znaci kada je ugao 45 stepeni, to mozemo riktati sa ovih mnozenjem 
            return fAngle;
        }
        
        const int ACCELER_IGNORE_COUNT = 50;
        const int ACCELER__COUNT = 10;

        private float calculatePhaseSecondDerivation(int sampleNo)
        {
            if (sampleNo < ACCELER_IGNORE_COUNT + 2 * ACCELER__COUNT)
                return fPhaseDerivAvg[sampleNo];

            float firstPart = 0;
            float secondPart = 0;

            // pogledaj DIR_COUNT prethodnih mjerenja (podjeli ih u 2 grupe i nadji sumu)
            for (int i = 0; i < ACCELER__COUNT; i++)
            {
                firstPart += fPhaseDerivAvg[sampleNo - ACCELER_IGNORE_COUNT - i - ACCELER__COUNT];  // prvih 10
                secondPart += fPhaseDerivAvg[sampleNo - i];         // drugih 10
            }

            float fAngle = (secondPart - firstPart) / (ACCELER__COUNT * 3);  // ne bi trebalo mnoziti sa 10000 nego dijeliti sa dX ali je ovdje to malo pogresno. X-osa mora biti u skladu sa Y da bi bilo realno govoriti o uglu
            // Za sada je najbinije da imamo polaritet ugla (opada/raste) a sta nam znaci kada je ugao 45 stepeni, to mozemo riktati sa ovih mnozenjem 
            return fAngle;
        }


        private int parseDacFile(String strDacFilePath)
        {
            String[] lines = File.ReadAllLines(strDacFilePath);
            fTime = new float[lines.Length];
            fDAC = new float[lines.Length];

            for (int i = 0; i < lines.Length; i++)
            {
                String[] parameters = lines[i].Split(' ');
                fTime[i] = float.Parse(parameters[0], System.Globalization.CultureInfo.InvariantCulture);
                fDAC[i] = float.Parse(parameters[1], System.Globalization.CultureInfo.InvariantCulture);
            }

            Console.WriteLine("Found: " + fTime.Length + " DAC measurements");
            return fTime.Length;
        }

        private void parsePhaseFile(String strPhaseFilePath) 
        {
            if (!File.Exists(strPhaseFilePath))
                return;

            String[] lines = File.ReadAllLines(strPhaseFilePath);
            fPhase = new float[lines.Length];
            fPhaseAvg = new float[lines.Length];
            fPhaseDerivAvg = new float[lines.Length];

            for (int i = 0; i < lines.Length; i++)
            {
                 // fPhase[i] = (float)(50 * Math.Sin(i * Math.PI / 180 / 20)); //
                fPhase[i] = float.Parse(lines[i], System.Globalization.CultureInfo.InvariantCulture);            
            }

            Console.WriteLine("Found: " + fPhase.Length + " phase measurements");
        }

        private int parseTemperartureFile(String strTempFilePath)
        {
            if (!File.Exists(strTempFilePath)) 
                return 0;

            String[] lines = File.ReadAllLines(strTempFilePath);
            fTimeTemper = new float[lines.Length];
            fTemper = new float[lines.Length];
            int j = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                String[] parameters = lines[i].Split(' ');
                if (parameters.Length != 2)
                    continue;

                fTimeTemper[j] = float.Parse(parameters[0], System.Globalization.CultureInfo.InvariantCulture);
                fTemper[j] = float.Parse(parameters[1], System.Globalization.CultureInfo.InvariantCulture);
                j++;
            }

            Console.WriteLine("Found: " + fTimeTemper.Length + " Temperature measurements");
            return fTimeTemper.Length;
        }
    }
}
