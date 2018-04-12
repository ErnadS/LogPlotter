using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using utility;
using System.IO;
using System.Windows.Forms;

namespace GraphLog.DL2_analyse
{
    public class FileParser
    {
        float[] fTimeTemper;
        float[] fTemper;

        String fileName;
        String strPhaseFilePath;
        String strTemperFilePath;
        String strDacFilePath;

        public void setFileNames(String strDacFilePath)
        {
            String folderName;
            this.strDacFilePath = strDacFilePath;

            int nind = strDacFilePath.LastIndexOf("\\");
            folderName = strDacFilePath.Substring(0, nind);

            fileName = strDacFilePath.Substring(nind + 1);

            strPhaseFilePath = folderName + "\\phase" + fileName.Substring(9); // remove "dac_value" from file name
            strTemperFilePath = folderName + "\\temperature" + fileName.Substring(9);

            if (!File.Exists(strDacFilePath))
            {
                MessageBox.Show("DAC file not exist: " + strDacFilePath);
                return;
            }
            else if (!File.Exists(strPhaseFilePath))
            {
                MessageBox.Show("Phase file not exist: " + strPhaseFilePath);
                return;
            }
            /*
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

                    AddPhaseAvg(fTime[0] + i, fPhaseAvg[i] * 1000000000);//* 1000000000);
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
            }*/
        }

        public DacMeasurement parseDacFile()
        {
            String[] lines = File.ReadAllLines(strDacFilePath);

            DacMeasurement dac = new DacMeasurement(lines.Length);

            // fTime = new float[lines.Length];
            // fDAC = new float[lines.Length];

            for (int i = 0; i < lines.Length; i++)
            {
                String[] parameters = lines[i].Split(' ');
                if (parameters.Length != 2)
                    continue;
                dac.fTime[i] = float.Parse(parameters[0], System.Globalization.CultureInfo.InvariantCulture);
                dac.fDAC[i] = float.Parse(parameters[1], System.Globalization.CultureInfo.InvariantCulture);
            }

            Console.WriteLine("Found: " + dac.fTime.Length + " DAC measurements");
            return dac; // fTime.Length;
        }

        public TemperMeasurment parseTemperartureFile()
        {
            if (!File.Exists(strTemperFilePath))
                return null;

            String[] lines = File.ReadAllLines(strTemperFilePath);

            if (lines.Length == 0)
                return null;

            TemperMeasurment temperatur = new TemperMeasurment(lines.Length);

           // fTimeTemper = new float[lines.Length];
            // fTemper = new float[lines.Length];
            int j = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                String[] parameters = lines[i].Split(' ');
                if (parameters.Length != 2)
                    continue;
                try
                {
                    temperatur.fTimeTemper[j] = float.Parse(parameters[0], System.Globalization.CultureInfo.InvariantCulture);
                    temperatur.fTemper[j] = float.Parse(parameters[1], System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception e)
                {// TODO: fix problem with wrong tepmerature
                }
                j++;
            }

            Console.WriteLine("Found: " + temperatur.fTimeTemper.Length + " Temperature measurements");
            return temperatur;
        }

        public float[] parsePhaseFile()  // ovdje mozemo vratiti float[] jer nemamo posebno vrijeme za phase (svake sekunde)
        {
            if (!File.Exists(strPhaseFilePath))
                return null;

            String[] lines = File.ReadAllLines(strPhaseFilePath);
            float[] fPhase = new float[lines.Length];
            
            for (int i = 0; i < lines.Length; i++)
            {
                // fPhase[i] = (float)(50 * Math.Sin(i * Math.PI / 180 / 20)); //
                fPhase[i] = float.Parse(lines[i], System.Globalization.CultureInfo.InvariantCulture);
            }

            Console.WriteLine("Found: " + fPhase.Length + " phase measurements");
            return fPhase;
        }
    }
}
