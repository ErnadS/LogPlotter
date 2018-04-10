using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

namespace GraphLog
{
    public partial class SelectComPort : Form
    {
        ISelectComPort parentForm;

        public SelectComPort(ISelectComPort parentForm, String strMsg, bool only_2_baud)
        {
            InitializeComponent();
            this.parentForm = parentForm;
            labelMsgToUser.Text = strMsg;

            if (only_2_baud)
            {
                radioButton2400.Visible = false;
                radioButton9600.Visible = false;
                radioButton19200.Visible = false;
                radioButton38400.Visible = false;
            }

            string[] strPorts = SerialPort.GetPortNames();

            if (strPorts == null || strPorts.Length == 0)
            {
                labelMsgToUser.Text = "Have not found any COM ports";
                labelMsgToUser.ForeColor = Color.Red;
            }
            else
            {
                for (int i = 0; i < strPorts.Length; i++)
                {
                    comboBoxComPort_A.Items.Add(strPorts[i]);
                    comboBoxComPort_B.Items.Add(strPorts[i]);
                }
                if (strPorts.Length > 0)
                {
                    comboBoxComPort_A.SelectedItem = strPorts[0];
                    comboBoxComPort_B.SelectedItem = strPorts[0];
                }
            }
        }



        delegate void unsafeDelegate();

        private void button1_Click(object sender, EventArgs e)
        {
            if (!InvokeRequired)
            {
                Hide();// Dispose();
            }
            else
            {
                Invoke(new unsafeDelegate(Hide/*Dispose*/), new object[] { });
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (radioButtonOneCom.Checked)  // Only one
            {
                parentForm.setSelectedComPort((String)comboBoxComPort_A.SelectedItem, null, getBaudRate());
            }
            else // two
            {
                if ((String)comboBoxComPort_A.SelectedItem == (String)comboBoxComPort_B.SelectedItem) // only one
                    parentForm.setSelectedComPort((String)comboBoxComPort_A.SelectedItem, null, getBaudRate());
                else
                    parentForm.setSelectedComPort((String)comboBoxComPort_A.SelectedItem, (String)comboBoxComPort_B.SelectedItem, getBaudRate());
            }

            if (!InvokeRequired)
            {
                Hide();// Dispose();
            }
            else
            {
                Invoke(new unsafeDelegate(Hide/*Dispose*/), new object[] { });
            }
        }

        private void radioButtonTwoComPorts_CheckedChanged(object sender, EventArgs e)
        {
            updateSelected();
        }

        private void radioButtonOneCom_CheckedChanged(object sender, EventArgs e)
        {
            updateSelected();
        }

        // wait on form loading and then change text and enbaling.
        private void SelectComPort_Load(object sender, EventArgs e)
        {
            updateSelected();
        }

        private void updateSelected()
        {
            if (radioButtonOneCom.Checked)
            {
                comboBoxComPort_B.Enabled = false;
                label1.Text = "Source for Value 1 and 2";
                label2.Visible = false;
            }
            else
            {
                comboBoxComPort_B.Enabled = true;
                label1.Text = "Source Value 1";
                label2.Text = "Source Value 2";
                label2.Visible = true;
            }
        }

        private int getBaudRate()
        {
            int nBaudRate = 4800;

            if (radioButton2400.Checked)
            {
                nBaudRate = 2400;
            }
            else if (radioButton4800.Checked)
            {
                nBaudRate = 4800;
            }
            else if (radioButton9600.Checked)
            {
                nBaudRate = 9600;
            }
            else if (radioButton19200.Checked)
            {
                nBaudRate = 19200;
            }
            else if (radioButton38400.Checked)
            {
                nBaudRate = 38400;
            }
            else if (radioButton115200.Checked)
            {
                nBaudRate = 115200;
            }

            return nBaudRate;
        }

    }
}
