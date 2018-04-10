using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;

namespace GraphLog
{
    public class RS232ModuleNmea
    {
        private SerialPort m_CommPort;
        public String strSerialName;

       // private int channell;
        //bool runThread = false;
        int comId;

        public RS232ModuleNmea(int comId)
        {
            this.comId = comId;
        }  

        public String[] DetectComPorts()
        {
            return SerialPort.GetPortNames();
        }

        public bool Open(String strCommNo, int nBaud, Parity parity, int bits, StopBits stopBits)
        {
            if (strCommNo == null || strCommNo.Length == 0)
                return false;


            m_CommPort = new SerialPort(strCommNo, nBaud, parity, bits, stopBits);
            m_CommPort.Handshake = Handshake.XOnXOff;
            //m_CommPort.ReadTimeout = 5000;
            try
            {
                m_CommPort.Open();
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
            //event koji se okida cim nesto dodje na serial port od mobitela
            //m_CommPort.DataReceived += new SerialDataReceivedEventHandler(m_CommPort_DataReceived);

            if (m_CommPort.IsOpen)
            {
                strSerialName = strCommNo;

                //MainForm.LogReceived("Listening on " + strSerialName + ", baud: " + nBaud);
                m_CommPort.DataReceived += new SerialDataReceivedEventHandler(commPort_DataReceived);
                return true;
            }

            return false;
        }



        byte[] byteAnswer = new byte[3];

        public bool SendCommand(String strMsg)
        {
            return SendCommand(Encoding.ASCII.GetBytes(strMsg), strMsg.Length);
        }


        public bool SendCommand(byte[] byBuff, int nLength)
        {
            try
            {
                m_CommPort.Write(byBuff, 0, nLength);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public bool IsOpen()
        {
            if (m_CommPort == null || !m_CommPort.IsOpen)
            {
                return false;
            }

            return true;
        }

        public void Close()
        {
            //runThread = false;

            if (m_CommPort == null)
                return;

            try
            {
                m_CommPort.DataReceived -= commPort_DataReceived;
                Thread.Sleep(500);
                //m_CommPort.ReadTimeout = 1;

                 if (m_CommPort.IsOpen)
                 {
                    m_CommPort.Close();
                    Thread.Sleep(1000);
                }
            }
            catch { }
        }

        String strRec = "";

        private void commPort_DataReceived(
                        object sender,
                        SerialDataReceivedEventArgs e)
        {
            String msg;
            strRec += m_CommPort.ReadExisting();
            while (strRec.IndexOf("\r\n") == 0)
            {
                strRec = strRec.Substring(2);
            }

            int ind = strRec.IndexOf("\r\n");
            if (ind > 0)
            {
                msg = strRec.Substring(0, ind);
                int aa = msg.LastIndexOf((char)0);
                //*****************************************Poziv funkcije za obradu poruke        
                // msgHandler.OnReceivedMsg(msg.Substring(aa + 1), comId); 
            }
            else
            {
                return;
            }

            strRec = strRec.Substring(ind);
        }
    }
}
