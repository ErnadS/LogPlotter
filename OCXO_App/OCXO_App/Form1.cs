using System;
using System.Collections.Generic;
using System.Globalization;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using System.Threading;
using System.Diagnostics;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Defaults;

namespace OCXO_App
{
    public partial class Form1 : Form
    {
        SerialPort serialPhasePort;
        SerialPort serialOCXOPort;
        Thread t;
        Stopwatch stopwatch = new Stopwatch();
        bool closedLoopFlag = false;
        const double DEFAULT_DAC_VALUE = 131072;//131072;
        double dac_value = DEFAULT_DAC_VALUE;
        double oldValue = DEFAULT_DAC_VALUE;
        List<double> oldValues = new List<double>(0);
        List<Int32> phaseArr = new List<Int32>();
        double DAC_movingAverageSum = 0;// DEFAULT_DAC_VALUE; // NEW!!! Startamo sa 131072
        Int32 counter = 0;

        string inputData;

        AverageExp phaseExpAvg = new AverageExp();

        enum TuningState
        {
            CROASE,  // ne znam je li ovo pogresna rijec za "grubo"
            MEDIUM,
            FINE,
            AGEING
        }

        // Nemoj imati vise varijabli za jedno stanje (povecava sansu da imas gresku). Bolje je jedna varijabla sa vise stanja.
        //TuningState tuningState = TuningState.CROASE;
        TuningState tuningState = TuningState.MEDIUM;

        MediumTuning mediumTuning = new MediumTuning();
        FineTuning fineTuning = new FineTuning();
        Ageing ageing = new Ageing();
        DateTime dateTime = DateTime.Now;
        string phaseFile = "";
        string dacFile = "";


        public Form1()
        {
            InitializeComponent();
            initializeGraph();
            phaseFile = "phase_" + dateTime.ToString("yyyyMMdd-HHmm") + ".txt";
            dacFile = "dac_value_" + dateTime.ToString("yyyyMMdd-HHmm") + ".txt";
        }

        private void initializeGraph()
        {
            cartesianChart1.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Phase Measurement",
                    Values = new ChartValues<ObservablePoint>(){new ObservablePoint
                    {
                        X = 0,
                        Y = 0
                    } }
                }
            };

            cartesianChart1.AxisY.Add(new Axis
            {
                Title = "Phase difference [s]"
            });
            cartesianChart2.Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "DAC Values",
                    Values = new ChartValues<ObservablePoint>(){new ObservablePoint
                    {
                        X = 0,
                        Y = 0
                    } }
                }
            };
            Func<double, string> formatFunc2 = (x) => string.Format("{0:0}", x);
            cartesianChart2.AxisY.Add(new Axis
            {
                Title = "DAC Values"
            });
            cartesianChart1.DisableAnimations = true;
            cartesianChart2.DisableAnimations = true;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void connect_Click(object sender, EventArgs e)
        {
            try
            {
                serialPhasePort = new SerialPort(phaseComPort.Text, 57600);
                serialOCXOPort = new SerialPort(dacComPort.Text, 57600);
                serialPhasePort.Open();
                serialOCXOPort.Open();
                t = new Thread(handleInputFromFPGA);
                t.Start();
                stopwatch.Start();
            }
            catch(Exception) { }
        }

        private void addPhaseToGraph(double phase)
        {
            this.Invoke((MethodInvoker)delegate()
            {
                // Add to phase graph
                if (Math.Abs(phase) < 0.8)
                {
                    currentPhaseValue.Text = phase.ToString();
                    cartesianChart1.Series[0].Values.Add(new ObservablePoint
                    {
                        X = stopwatch.Elapsed.TotalSeconds,
                        Y = phase
                    });
                    if (cartesianChart1.Series[0].Values.Count >= 500)
                    {
                        cartesianChart1.Series[0].Values.RemoveAt(0);
                    }
                }
            });
        }

        private void writePhaseToFile(double phase)
        {
            using (StreamWriter ph = new StreamWriter(phaseFile, true))
            {
                ph.WriteLine(phase.ToString());
                ph.Close();
            }
        }

        private void addDacToGraph(double newDacValue)
        {
            this.Invoke((MethodInvoker)delegate()
            {
                currentDacValue.Text = newDacValue.ToString();

                cartesianChart2.Series[0].Values.Add(new ObservablePoint
                {
                    X = stopwatch.Elapsed.TotalSeconds,
                    Y = Int32.Parse(newDacValue.ToString())
                });
                if (cartesianChart2.Series[0].Values.Count >= 500)
                {
                    cartesianChart2.Series[0].Values.RemoveAt(0);
                }
            });
        }

        private void writeDacToFile(double newDacValue)
        {
            // write DAC to file
            writeDACToFile(newDacValue.ToString(), stopwatch.Elapsed.TotalSeconds);
            oldValue = newDacValue;
        }

        int nTime = 0;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Sljedece 3 funkcije bi trebalo prebaciti u neki "DacHandler.cs" ali to cemo drugi put. On bi primio kao argument objekt ove forme
        // i onda bi mogao osvjeziti graf kada obradi poruke
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void handleInputFromFPGA()  // promjenio sam ime "updatePhaseValue" jer mi zvuci kao da ova funkcija samo osvjezava vrjednost faze (a ovo je glavna funkcija). Ti ces sigurno naci bolje ime ("mainHandler"???)
        {
            while (serialPhasePort.IsOpen)
            {
                try
                {
                    inputData = serialPhasePort.ReadLine();
                    // TODO: !!! ovdje bi trebalo provjeriti da li je primljena linja ispravne duzine i ispisati gresku ako nastane
                    double lastPhase = Phase.calculatePhaseFromInputString(inputData);

                    addPhaseToGraph(lastPhase);

                    if (closedLoopFlag) //closed loop operation
                    {
                        dac_value = calculateNewDACValue(inputData, lastPhase, nTime);

                        // Send DAC (if changed) to FPGA
                        if (dac_value != oldValue)
                        {
                            serialOCXOPort.Write("!D" + dac_value.ToString() + "\r\n");
                        }

                        addDacToGraph(dac_value);
                        writeDacToFile(dac_value);
                        writePhaseToFile(lastPhase);
                    }

                    nTime++;
                }
                catch(Exception e) 
                {
                    Console.WriteLine("Exception 88: " + e.ToString()); // !!! Uvjek ispisuj Exceptions jer ti mogu prikazati neku gresku koja ce ti otkriti gdje imas problem
                }
            }
        }

        private Int32 calculateNewDACValue(string input, double lastPhase, int nTime)
        {
            // "phase" nije u ns nego broj imupulsa counter-a (2.5 ili 5ns)
            double phase = Phase.calculatePhaseFromInputString_B(input);

            if (Math.Abs(lastPhase) < 0.8)   // fail safe (ako ima suma koji ima vrlo visoku fazu)
            {
                if (tuningState == TuningState.CROASE) // Grubo podesavanje
                {
                    double tmp = coarseTuneDAC(phase, nTime);

                    checkTuningState(lastPhase);  // change state to MEDIUM if phase low in last 100 measurements

                    return Convert.ToInt32(tmp);
                }
                else if (tuningState == TuningState.MEDIUM) // Srednje podesavanje
                {
                    TuningResult result = mediumTuning.calculateMediumTuning(dac_value, lastPhase, nTime);
                    if (result.stateResult == TuningResult.Result.FINISHED)
                    {
                        writeServiceFile("Medium tuning finished. Time: " + nTime + ". Setting new DAC value before starting fine tuning: " + result.newDAC);
                        tuningState = TuningState.FINE;
                    }
                    return Convert.ToInt32(result.newDAC);
                    // OVO VRATI AKO HOCES DA PROBAS STARU FUNKCIJU
                    // Faza se dodaje u listu i svako deset sekundo se poziva funkcija calculateMediumtuning
                    /*phaseArr.Add(Convert.ToInt32(phaseExpAvg.calculateExpAvg(phase))); //calculateAvg(phase)));
                    if (phaseArr.Count == 10)
                    {                   
                        Int32 retval = Convert.ToInt32(calculateMediumTuning());
                        phaseArr.Clear();
                        return retval;
                    }*/
                }
                else if (tuningState == TuningState.FINE) // fino podesavanje
                {
                    TuningResult result = fineTuning.tune(dac_value, lastPhase, nTime);
                    if (result.stateResult == TuningResult.Result.FINISHED)// GO TO "AGING" state
                    {
                        tuningState = TuningState.AGEING;
                    }
                    return Convert.ToInt32(result.newDAC);
                }
                else if (tuningState == TuningState.AGEING)
                {
                    TuningResult result = ageing.ageing(dac_value, lastPhase);
                    return Convert.ToInt32(result.newDAC);
                }
            }

            return Convert.ToInt32(dac_value); // return previous value if lastPhase > 0.8
        }

        private void checkTuningState(double phase)
        {
            if (Math.Abs(phase) < 50  *Math.Pow(10, -9)) //Uslov za fino podesenje da je faza 100 sekundi manja od 5*2.5ns
            {
                /*
                counter++;
                if (counter == 200)
                {*/
                    tuningState = TuningState.MEDIUM;

                    label9.Text = "Medium Tuning ON";
                //}
            }
            /* else
            {
                counter = 0;
            }*/
        }

        private double coarseTuneDAC(double phase, int nTime)
        {
            // remove first value from average before adding the new (if array is full)
            if (oldValues.Count == 499) //  == 499   Tek ako imamo 500, onda moramo jedan izbaciti
            {
                DAC_movingAverageSum -= oldValues[0];
                oldValues.RemoveAt(0);
            }

            double tmp;
            if (oldValues.Count > 0)
                tmp = (DAC_movingAverageSum / oldValues.Count + phase * 50);   // PI regulator
            else
                tmp = DEFAULT_DAC_VALUE;

            if (tmp > 261144)  // Limit max. DAC
            {
                Console.WriteLine("!!! To big DAC: " + tmp);  // vazno je ispisivati ako ti npr. moze pokazati da zbog neke greske precesto dolazi ovdje 
                tmp = 262143;
            }
            else if (tmp < 0) // Limit min. DAC
            {
                Console.WriteLine("!!! Negative DAC: " + tmp);  // ako je normalno da ovdje dolazi puno puta, izbaci ove WriteLine jer onda usporavaju program
                tmp = 0;
            }

            oldValues.Add(tmp); // 500); // jos uvjek nismo dodali zadnji element pa imamo "+1" zbog njega
            DAC_movingAverageSum += tmp; // 500;  // sada smo dodali taj element pa ne trebao "+1" (imamo 500 ako je napunjena)


            return Convert.ToInt32(tmp);  // !!!HELP: Mozda mi ovo bude jasno kada objasnis kako racunas "phase" (u funkciji calculatePhaseFromInputString_B()/Phase.cs)
            // Ja bi ocekivao da ova funkcija prvi put vrati 131072 i da onda na tu vrjednost dodaje/oduzima "phase*20" ili tako nesto ali ne mislim da to radi. 
            // Prva vrijednost u dac_value.txt je obicno nula pa onda npr. 13380, 26347 ...
            // Ovdje mi je najcudnije da se ne returnira "tmp/500". Mozemo ovo na voice
        }


        private void writeServiceFile(string str)
        {
            using (StreamWriter st = new StreamWriter("serviceFile.txt", true))
            {
                st.WriteLine(str);
                st.Close();
            }
        }
        

        /*****************************************
        * Funkcija za racunanje finog podesenja (Terjina funkcija)
        ******************************************/
        /*
        private double calculateMediumTuning()
        {
            double sumFirst5 = 0;
            double sumSecond5 = 0;
            double sumXY = 0;
            double sumX = 0;
            double sumY = 0;
            double sumX2 = 0;
            for(int i = 0; i < 5; i++) //racuna se zbig prvih pet i zadnjih pet elemenata
            {
                sumFirst5 += Math.Abs(phaseArr[i]);
                sumSecond5 += Math.Abs(phaseArr[i + 5]);
            }
            if (sumFirst5 < sumSecond5) // ako je zadnjih pet vece od prvih pet znaci pocelo je da luta
            {
                for (int i = 0; i < phaseArr.Count; i++) //Racunanje koeficijenata za linearnu regresiju (Terjeove formule)
                {
                    sumXY += (i + phaseArr[i]);
                    sumX += i;
                    sumY += (phaseArr[i]);
                    sumX2 += Math.Pow(i + 1, 2);
                }

                //Racunanje slope-a i CVTotal (korekcioni faktor) (Terjeove formule)
                double slope = (phaseArr.Count * sumXY - sumX * sumY) / Convert.ToDouble(phaseArr.Count * sumX2 - Math.Pow(sumX, 2));
                double phaseError = sumY / phaseArr.Count + (slope * phaseArr.Count) / 2;
                double CVOffset = slope / 0.0002516;
                double CVPhaseError = phaseError / 0.0002516 * phaseArr.Count;
                double CVTotal = CVOffset / 1000 + CVPhaseError / 10000;

                if (CVTotal > 1000)//Cisto da ogranicimo ako se nesto "ludo" desi da se moze vratiti
                {
                    CVTotal = 1000;
                }
                else if (CVTotal < -1000)
                {
                    CVTotal = -1000;
                }
                phaseArr.Clear();
                double newDac = dac_value - CVTotal / 10;
                if (newDac > 262143)
                {
                    newDac = 262143;
                }
                else if (newDac < 0)
                {
                    newDac = 0;
                }
                return newDac;
            }
            else
            {
                return dac_value;
            }
        }
        */
        private void disconnect_Click(object sender, EventArgs e)
        {
            serialPhasePort.Close();
            serialOCXOPort.Close();
            stopwatch.Stop();
        }

        private void sync_Click(object sender, EventArgs e)
        {
            serialOCXOPort.Write("!S\r\n");
        }

        private void dac0_Click(object sender, EventArgs e)
        {
            serialOCXOPort.Write("!D0\r\n");
            int zero = 0;
            currentDacValue.Text = zero.ToString();
            cartesianChart2.Series[0].Values.Add(new ObservablePoint {
                X = stopwatch.Elapsed.TotalSeconds,
                Y = 0
            } );
            writeDACToFile(zero.ToString(), stopwatch.Elapsed.TotalSeconds);
        }

        private void dac131072_Click(object sender, EventArgs e)
        {
            int value = 131072;
            currentDacValue.Text = value.ToString();
            serialOCXOPort.Write("!D131072\r\n");
            cartesianChart2.Series[0].Values.Add(new ObservablePoint {
                X = stopwatch.Elapsed.TotalSeconds,
                Y = 131072
            });
            writeDACToFile(value.ToString(), stopwatch.Elapsed.TotalSeconds);
        }

        private void dac150000_Click(object sender, EventArgs e)
        {
            int value = 150000;
            currentDacValue.Text = value.ToString();
            serialOCXOPort.Write("!D150000\r\n");
            cartesianChart2.Series[0].Values.Add(new ObservablePoint
            {
                X = stopwatch.Elapsed.TotalSeconds,
                Y = 150000
            });
            writeDACToFile(value.ToString(), stopwatch.Elapsed.TotalSeconds);
        }

        private void dac262143_Click(object sender, EventArgs e)
        {
            int value = 262143;
            currentDacValue.Text = value.ToString();
            serialOCXOPort.Write("!D262143\r\n");
            cartesianChart2.Series[0].Values.Add(new ObservablePoint
            {
                X = stopwatch.Elapsed.TotalSeconds,
                Y = 262143
            });
            writeDACToFile(value.ToString(), stopwatch.Elapsed.TotalSeconds);
        }

        private void send_Click(object sender, EventArgs e)
        {
            string sendValue = sendedDacValue.Text;
            serialOCXOPort.Write("!D" + sendValue + "\r\n");
            currentDacValue.Text = sendValue;
            cartesianChart2.Series[0].Values.Add(new ObservablePoint
            {
                X = stopwatch.Elapsed.TotalSeconds,
                Y = Int32.Parse(sendValue)
            });
            writeDACToFile(sendValue, stopwatch.Elapsed.TotalSeconds);
        }

        private void startClosedLoop_Click(object sender, EventArgs e)
        {
            closedLoopFlag = true;
        }

        private void stopClosedLoop_Click(object sender, EventArgs e)
        {
            closedLoopFlag = false;
        }

        private void writeDACToFile(string dac_value, double seconds)
        {
            using(StreamWriter dv = new StreamWriter(dacFile, true))
            {
                dv.WriteLine(seconds.ToString() + " " + dac_value);
                dv.Close();
            }
        }

        private void tempStartLog_Click(object sender, EventArgs e)
        {
            try
            {
                serialOCXOPort = new SerialPort(dacComPort.Text, 57600);
                serialOCXOPort.Open();
                t = new Thread(LogTemperature);
                t.Start();
                stopwatch.Start();
            }
            catch (Exception) { }
        }

        private void LogTemperature()
        {
            while (serialOCXOPort.IsOpen)
            {
                try
                {
                    serialOCXOPort.Write("!T\r\n");
                    string temperature = serialOCXOPort.ReadLine();
                    writeTempToFile(temperature);
                    while(stopwatch.ElapsedMilliseconds < 60000)
                    {
                        //wait 60 sec
                    }
                    stopwatch.Restart();
                }
                catch(Exception) { }
            }
        }

        private void writeTempToFile(string temp)
        {
            using (StreamWriter tmp = new StreamWriter("temperature.txt", true))
            {
                tmp.WriteLine(temp);
                tmp.Close();
            }
        }

        private void tempStopLog_Click(object sender, EventArgs e)
        {
            serialOCXOPort.Close();
            stopwatch.Stop();
        }
    }
}
