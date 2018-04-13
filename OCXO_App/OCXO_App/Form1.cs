using System;
using System.Collections.Generic;
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
        double dac_value = 131072;
        double oldValue = 131072;
        List<double> oldValues = new List<double>(131072);
        List<Int32> phaseArr = new List<Int32>();
        double movingAverage = 0;
        Int32 counter = 0;
        Int32 cnt = 0;
        bool fastFlag = true;
        bool mediumFlag = false;
        string inputData;
        public Form1()
        {
            InitializeComponent();
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
                t = new Thread(updatePhaseValue);
                t.Start();
                stopwatch.Start();
            }
            catch(Exception) { }
        }

        private void updatePhaseValue()
        {
            while (serialPhasePort.IsOpen)
            {
                try
                {
                    inputData = serialPhasePort.ReadLine();
                    double phase = calculatePhase(inputData);
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        currentPhaseValue.Text = phase.ToString();
                        cartesianChart1.Series[0].Values.Add(new ObservablePoint {
                            X = stopwatch.Elapsed.TotalSeconds,
                            Y = phase
                        });
                        if(cartesianChart1.Series[0].Values.Count >= 500)
                        {
                            cartesianChart1.Series[0].Values.RemoveAt(0);
                        }
                        using (StreamWriter ph = new StreamWriter("phase.txt", true))
                        {
                            ph.WriteLine(phase.ToString());
                            ph.Close();
                        }
                        cnt++;
                        if (cnt == 60)
                        {
                            cnt = 0;
                            serialOCXOPort.Write("!T\r\n");
                            string s = serialOCXOPort.ReadLine();
                            s.Replace("\r", "");
                            label10.Text = s;

                            serialOCXOPort.DiscardInBuffer();
                            serialOCXOPort.DiscardOutBuffer();
                            using (StreamWriter temp = new StreamWriter("temperature.txt", true))
                            {
                                temp.WriteLine(stopwatch.Elapsed.TotalSeconds.ToString() + " " + s);
                                temp.Close();
                            }
                        }
                        if (closedLoopFlag) //closed loop operation
                        {/*
                            cnt++;
                            if (cnt == 60)
                            {
                                cnt = 0;
                                serialOCXOPort.Write("!T\r\n");
                                string s = serialOCXOPort.ReadLine();
                                s.Replace("\r", "");
                                label10.Text = s;

                                serialOCXOPort.DiscardInBuffer();
                                serialOCXOPort.DiscardOutBuffer();
                                using (StreamWriter temp = new StreamWriter("temperature.txt", true))
                                {
                                    temp.WriteLine(stopwatch.Elapsed.TotalSeconds.ToString() + " " + s);
                                    temp.Close();
                                }
                            }
                            else
                            {*/
                                dac_value = calculateDACValue(inputData);
                                if (dac_value != oldValue)
                                {
                                    serialOCXOPort.Write("!D" + dac_value.ToString() + "\r\n");
                                }
                            //}
                            this.Invoke((MethodInvoker)delegate ()
                            {
                                currentDacValue.Text = dac_value.ToString();
                            });
                            cartesianChart2.Series[0].Values.Add(new ObservablePoint
                            {
                                X = stopwatch.Elapsed.TotalSeconds,
                                Y = Int32.Parse(dac_value.ToString())
                            });
                            if (cartesianChart2.Series[0].Values.Count >= 500)
                            {
                                cartesianChart2.Series[0].Values.RemoveAt(0);
                            }
                            writeDACToFile(dac_value.ToString(), stopwatch.Elapsed.TotalSeconds);
                            oldValue = dac_value;
                        }
                    });
                }
                catch(Exception) { }
            }
        }

        private Int32 calculateDACValue(string input)
        {
            string[] subInputs = input.Split(',');
            List<Int32> inputValues = new List<Int32>();
            double phase = 0;
            /****************************************
             * Racunanje faze iz primljene poruke
             ****************************************/
            foreach (string s in subInputs)
            {
                inputValues.Add(Int32.Parse(s));
            }
            phase = inputValues[2];
            if (inputValues[0] == 0)
            {
                phase = (phase + inputValues[3]/4 - inputValues[4]/4) * (-1);
            }
            else
            {
                phase = (phase + inputValues[4] / 4 - inputValues[3] / 4);
            }
            /****************************************
             * Racunanje faze iz primljene poruke
             ****************************************/

            /****************************************
             * fastFlag == grubo podesavanje
             * kada se starta prvo ide grubo podesavanje dok fazna razlika 
             * ne bude mala a nakon toga se prelazi na fino
             ****************************************/
            if (Math.Abs(calculatePhase(inputData)) < 0.8)
            {
                if (fastFlag)
                {
                    if (oldValues.Count == 499)
                    {
                        movingAverage -= oldValues[0];
                        oldValues.RemoveAt(0);
                    }
                    double tmp = movingAverage + phase * 20;
                    if (tmp > 261144)
                    {
                        tmp = 262143;
                    }
                    else if (tmp < 0)
                    {
                        tmp = 0;
                    }
                    movingAverage += tmp / 500;
                    oldValues.Add(tmp / 500);

                    //Uslov za fino podesenje da je faza 100 sekundi manja od 5*7.3ns
                    if (phase < 5)
                    {
                        counter++;
                        if (counter == 100)
                        {
                            fastFlag = false;
                            mediumFlag = true;
                            label9.Text = "Medium Tuning ON";
                        }
                    }
                    return Convert.ToInt32(tmp);
                }
                /****************************************
                 * Kraj grubog podesenja
                 ****************************************/

                /****************************************
                 * Fino podesenje
                 ****************************************/
                else if (mediumFlag)
                {
                    phaseArr.Add(Convert.ToInt32(calculateAvg(phase)));
                    if (phaseArr.Count == 10)
                    {
                        // Faza se dodaje u listu i svako deset sekundo se poziva funkcija calculateMediumtuning
                        Int32 retval = Convert.ToInt32(calculateMediumTuning());
                        phaseArr.Clear();
                        return retval;
                    }
                }
            }
            return Convert.ToInt32(dac_value);
        }

        /*****************************************
        * Funkcija za racunanje finog podesenja
        ******************************************/
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
                sumFirst5 += phaseArr[i];
                sumSecond5 += phaseArr[i + 5];
            }
            double derv = (sumSecond5 - sumFirst5) * 1000;
            if ((derv > 0 && sumFirst5 > 0 && sumSecond5 > 0) || (derv < 0 && sumFirst5 < 0 && sumSecond5 < 0)) // ako je zadnjih pet vece od prvih pet znaci pocelo je da luta
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
                double CVTotal = CVOffset / 500 + CVPhaseError / 5000; //1000 i 10000

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

        int m_sampleCount = 0;
        double previousOutputValue = 0;
        double outputValue;

        const int AVG_SIZE = 10;
        const double ALPHA_NORMAL = 0.1;


        private double calculateAvg(double fNextInput)
        {
            m_sampleCount++;

            if (m_sampleCount < AVG_SIZE)
            {
                previousOutputValue = fNextInput;
                return fNextInput;
            }

            outputValue = ALPHA_NORMAL * fNextInput + (1.0f - ALPHA_NORMAL) * previousOutputValue;
            previousOutputValue = outputValue;
            return outputValue;
        }

        private double calculatePhase(string input)
        {
            string[] subInputs = input.Split(',');
            List<Int32> inputValues = new List<Int32>();
            double phase = 0;
            foreach (string s in subInputs)
            {
                inputValues.Add(Int32.Parse(s));
            }
            if (inputValues.Count == 5)
            {
                if (inputValues[0] == 0)
                {
                    if (inputValues[1] == 0)
                    {
                        phase = ((Convert.ToDouble(inputValues[2]) / 400000000) + inputValues[3] * (20 * Math.Pow(10, (-11))) + 2.5 * Math.Pow(10, -9) - inputValues[4] * (20 * Math.Pow(10, -11))) * (-1);
                    }
                    else
                    {
                        phase = (Convert.ToDouble(inputValues[2])) / 200000000 * (-1);
                    }
                }
                else
                {
                    if (inputValues[1] == 0)
                    {
                        phase = ((Convert.ToDouble(inputValues[2]) / 400000000) + inputValues[3] * (20 * Math.Pow(10, (-11))) + 2.5 * Math.Pow(10, -9) - inputValues[4] * (20 * Math.Pow(10, -11)));
                    }
                    else
                    {
                        phase = (Convert.ToDouble(inputValues[2])) / 200000000;
                    }
                }
            }
            return phase;
        }

        private void disconnect_Click(object sender, EventArgs e)
        {
            serialPhasePort.Close();
            serialOCXOPort.Close();
            stopwatch.Stop();
        }

        private void sync_Click(object sender, EventArgs e)
        {
            serialOCXOPort.Write("!S\r\n");
            serialOCXOPort.ReadLine();
            serialOCXOPort.DiscardInBuffer();
            serialOCXOPort.DiscardOutBuffer();
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
            using(StreamWriter dv = new StreamWriter("dac_value.txt", true))
            {
                dv.WriteLine(seconds.ToString() + " " + dac_value);
                dv.Close();
            }
        }
    }
}
