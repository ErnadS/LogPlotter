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
    public class PhaseHandler
    {
        GraphPainter graphPainter;


        float[] fPhase;
        float[] fPhaseAvg;

        float[] fPhaseDerivAvg;

        Average phaseAvg;
        Average phaseDevAvg;

        public PhaseHandler(GraphPainter graphPainter)
        {
            this.graphPainter = graphPainter;

            phaseAvg = new Average();
            phaseDevAvg = new Average();
            // phaseDevAvg.setAlpha(0.1f);
            // phaseDevAvg.setSize(20);
        }

        public void parseFiles(String fileName)
        {
            FileParser parser = new FileParser();
            parser.setFileNames(fileName);

            DacMeasurement dacMeasur = parser.parseDacFile();

            if (dacMeasur != null)
            {
                for (int i = 0; i < dacMeasur.fDAC.Length; i++)
                {
                    AddDacToGraph(dacMeasur.fTime[i], dacMeasur.fDAC[i]);
                }
            }

            fPhase = parser.parsePhaseFile();
            if (fPhase != null)
            {
                fPhaseAvg = new float[fPhase.Length];
                fPhaseDerivAvg = new float[fPhase.Length];

                for (int i = 0; i < fPhase.Length; i++)
                {
                    AddPhaseToGraph(dacMeasur.fTime[0] + i, fPhase[i] * 1000000000);   //Phase is measured each second (?). Use DAC first time as start point. Phase is in [ns]
                
                    fPhaseAvg[i] = phaseAvg.calculateAvg(fPhase[i]);
                    if (fPhaseAvg[i] < -180)
                        fPhaseAvg[i] = fPhaseAvg[i];

                    AddPhaseAvgToGraph(dacMeasur.fTime[0] + i, fPhaseAvg[i] * 1000000000);//* 1000000000);
                }


                for (int i = 20; i < fPhase.Length; i++)
                {
                    float fPhaseDeriv = calculatePhaseFirstDerivation(i);
                    AddPhaseFirstDeviationToGraph(dacMeasur.fTime[0] + i, fPhaseDeriv);
                    // fPhaseDerivAvg[i] = phaseDevAvg.calculateAvg(fPhaseDeriv);
                    // AddPhaseFirstDeviationToGraph(dacMeasur.fTime[0] + i, calculatePhaseSecondDerivation(i));//);
                }
            }

            TemperMeasurment temper = parser.parseTemperartureFile();

            if (temper != null)
            {
                for (int i = 0; i < temper.fTemper.Length; i++)
                {
                    AddTemperatureToGraph(temper.fTimeTemper[i], temper.fTemper[i]);
                }
            }

            Console.WriteLine("finished reading");
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


        public void AddPhaseToGraph(float fX, float fPhase)
        {
            graphPainter.AddPoint(new GraphPoint(fX, fPhase), 0, false);   // "0" je za graf br. 0 (dodaj tacuku u graf br. 0)
        }

        public void AddDacToGraph(float fX, float fDAC)
        {
            graphPainter.AddPoint(new GraphPoint(fX, fDAC), 1, false);   // "1" je za graf br. 1 (dodaj tacuku u graf br. 1)
        }

        public void AddTemperatureToGraph(float fX, float fTemper)
        {
            graphPainter.AddPoint(new GraphPoint(fX, fTemper), 2, false);   // "1" je za graf br. 1 (dodaj tacuku u graf br. 1)
        }

        public void AddPhaseAvgToGraph(float fX, float fDACavg)
        {
            graphPainter.AddPoint(new GraphPoint(fX, fDACavg), 3, false);   //
        }

        public void AddPhaseFirstDeviationToGraph(float fX, float fDACdir)
        {
            graphPainter.AddPoint(new GraphPoint(fX, fDACdir), 4, false);   //
        }
    }
}
