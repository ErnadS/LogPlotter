namespace OCXO_App
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cartesianChart1 = new LiveCharts.WinForms.CartesianChart();
            this.cartesianChart2 = new LiveCharts.WinForms.CartesianChart();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.phaseComPort = new System.Windows.Forms.TextBox();
            this.dacComPort = new System.Windows.Forms.TextBox();
            this.connect = new System.Windows.Forms.Button();
            this.disconnect = new System.Windows.Forms.Button();
            this.currentDacValue = new System.Windows.Forms.TextBox();
            this.sendedDacValue = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.send = new System.Windows.Forms.Button();
            this.startClosedLoop = new System.Windows.Forms.Button();
            this.stopClosedLoop = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.dac0 = new System.Windows.Forms.Button();
            this.dac131072 = new System.Windows.Forms.Button();
            this.dac150000 = new System.Windows.Forms.Button();
            this.dac262143 = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.sync = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.currentPhaseValue = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cartesianChart1
            // 
            this.cartesianChart1.Location = new System.Drawing.Point(381, 12);
            this.cartesianChart1.Name = "cartesianChart1";
            this.cartesianChart1.Size = new System.Drawing.Size(1459, 490);
            this.cartesianChart1.TabIndex = 0;
            this.cartesianChart1.Text = "cartesianChart1";
            // 
            // cartesianChart2
            // 
            this.cartesianChart2.Location = new System.Drawing.Point(381, 511);
            this.cartesianChart2.Name = "cartesianChart2";
            this.cartesianChart2.Size = new System.Drawing.Size(1459, 392);
            this.cartesianChart2.TabIndex = 1;
            this.cartesianChart2.Text = "cartesianChart2";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(155, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Phase measurement COM Port:";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(56, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(111, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "DAC Value COM Port:";
            // 
            // phaseComPort
            // 
            this.phaseComPort.Location = new System.Drawing.Point(173, 34);
            this.phaseComPort.Name = "phaseComPort";
            this.phaseComPort.Size = new System.Drawing.Size(100, 20);
            this.phaseComPort.TabIndex = 4;
            // 
            // dacComPort
            // 
            this.dacComPort.Location = new System.Drawing.Point(173, 60);
            this.dacComPort.Name = "dacComPort";
            this.dacComPort.Size = new System.Drawing.Size(100, 20);
            this.dacComPort.TabIndex = 5;
            // 
            // connect
            // 
            this.connect.Location = new System.Drawing.Point(92, 98);
            this.connect.Name = "connect";
            this.connect.Size = new System.Drawing.Size(75, 23);
            this.connect.TabIndex = 6;
            this.connect.Text = "Connect";
            this.connect.UseVisualStyleBackColor = true;
            this.connect.Click += new System.EventHandler(this.connect_Click);
            // 
            // disconnect
            // 
            this.disconnect.Location = new System.Drawing.Point(173, 98);
            this.disconnect.Name = "disconnect";
            this.disconnect.Size = new System.Drawing.Size(75, 23);
            this.disconnect.TabIndex = 7;
            this.disconnect.Text = "Disconnect";
            this.disconnect.UseVisualStyleBackColor = true;
            this.disconnect.Click += new System.EventHandler(this.disconnect_Click);
            // 
            // currentDacValue
            // 
            this.currentDacValue.Location = new System.Drawing.Point(148, 455);
            this.currentDacValue.Name = "currentDacValue";
            this.currentDacValue.ReadOnly = true;
            this.currentDacValue.Size = new System.Drawing.Size(100, 20);
            this.currentDacValue.TabIndex = 8;
            // 
            // sendedDacValue
            // 
            this.sendedDacValue.Location = new System.Drawing.Point(148, 481);
            this.sendedDacValue.Name = "sendedDacValue";
            this.sendedDacValue.Size = new System.Drawing.Size(100, 20);
            this.sendedDacValue.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(43, 458);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(99, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Current DAC Value:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 484);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(128, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Send Custom DAC Value:";
            // 
            // send
            // 
            this.send.Location = new System.Drawing.Point(254, 479);
            this.send.Name = "send";
            this.send.Size = new System.Drawing.Size(75, 23);
            this.send.TabIndex = 12;
            this.send.Text = "Send";
            this.send.UseVisualStyleBackColor = true;
            this.send.Click += new System.EventHandler(this.send_Click);
            // 
            // startClosedLoop
            // 
            this.startClosedLoop.Location = new System.Drawing.Point(90, 304);
            this.startClosedLoop.Name = "startClosedLoop";
            this.startClosedLoop.Size = new System.Drawing.Size(75, 23);
            this.startClosedLoop.TabIndex = 13;
            this.startClosedLoop.Text = "Start";
            this.startClosedLoop.UseVisualStyleBackColor = true;
            this.startClosedLoop.Click += new System.EventHandler(this.startClosedLoop_Click);
            // 
            // stopClosedLoop
            // 
            this.stopClosedLoop.Location = new System.Drawing.Point(171, 304);
            this.stopClosedLoop.Name = "stopClosedLoop";
            this.stopClosedLoop.Size = new System.Drawing.Size(75, 23);
            this.stopClosedLoop.TabIndex = 14;
            this.stopClosedLoop.Text = "Stop";
            this.stopClosedLoop.UseVisualStyleBackColor = true;
            this.stopClosedLoop.Click += new System.EventHandler(this.stopClosedLoop_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(111, 288);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(118, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "Closed Loop Operation:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(111, 340);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(121, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "Predefined DAC Values:";
            // 
            // dac0
            // 
            this.dac0.Location = new System.Drawing.Point(90, 356);
            this.dac0.Name = "dac0";
            this.dac0.Size = new System.Drawing.Size(75, 23);
            this.dac0.TabIndex = 17;
            this.dac0.Text = "0";
            this.dac0.UseVisualStyleBackColor = true;
            this.dac0.Click += new System.EventHandler(this.dac0_Click);
            // 
            // dac131072
            // 
            this.dac131072.Location = new System.Drawing.Point(171, 356);
            this.dac131072.Name = "dac131072";
            this.dac131072.Size = new System.Drawing.Size(75, 23);
            this.dac131072.TabIndex = 18;
            this.dac131072.Text = "131072";
            this.dac131072.UseVisualStyleBackColor = true;
            this.dac131072.Click += new System.EventHandler(this.dac131072_Click);
            // 
            // dac150000
            // 
            this.dac150000.Location = new System.Drawing.Point(90, 385);
            this.dac150000.Name = "dac150000";
            this.dac150000.Size = new System.Drawing.Size(75, 23);
            this.dac150000.TabIndex = 19;
            this.dac150000.Text = "150000";
            this.dac150000.UseVisualStyleBackColor = true;
            this.dac150000.Click += new System.EventHandler(this.dac150000_Click);
            // 
            // dac262143
            // 
            this.dac262143.Location = new System.Drawing.Point(171, 385);
            this.dac262143.Name = "dac262143";
            this.dac262143.Size = new System.Drawing.Size(75, 23);
            this.dac262143.TabIndex = 20;
            this.dac262143.Text = "262143";
            this.dac262143.UseVisualStyleBackColor = true;
            this.dac262143.Click += new System.EventHandler(this.dac262143_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(136, 182);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(68, 13);
            this.label7.TabIndex = 21;
            this.label7.Text = "Synchronize:";
            // 
            // sync
            // 
            this.sync.Location = new System.Drawing.Point(132, 198);
            this.sync.Name = "sync";
            this.sync.Size = new System.Drawing.Size(75, 23);
            this.sync.TabIndex = 22;
            this.sync.Text = "SYNC";
            this.sync.UseVisualStyleBackColor = true;
            this.sync.Click += new System.EventHandler(this.sync_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(35, 511);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(107, 13);
            this.label8.TabIndex = 23;
            this.label8.Text = "Current Phase Value:";
            // 
            // currentPhaseValue
            // 
            this.currentPhaseValue.Location = new System.Drawing.Point(148, 507);
            this.currentPhaseValue.Name = "currentPhaseValue";
            this.currentPhaseValue.ReadOnly = true;
            this.currentPhaseValue.Size = new System.Drawing.Size(100, 20);
            this.currentPhaseValue.TabIndex = 24;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(145, 554);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(99, 13);
            this.label9.TabIndex = 25;
            this.label9.Text = "Medium tuning OFF";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(219, 579);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(13, 13);
            this.label10.TabIndex = 26;
            this.label10.Text = "0";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(145, 579);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(70, 13);
            this.label11.TabIndex = 27;
            this.label11.Text = "Temperature:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1852, 915);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.currentPhaseValue);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.sync);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.dac262143);
            this.Controls.Add(this.dac150000);
            this.Controls.Add(this.dac131072);
            this.Controls.Add(this.dac0);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.stopClosedLoop);
            this.Controls.Add(this.startClosedLoop);
            this.Controls.Add(this.send);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.sendedDacValue);
            this.Controls.Add(this.currentDacValue);
            this.Controls.Add(this.disconnect);
            this.Controls.Add(this.connect);
            this.Controls.Add(this.dacComPort);
            this.Controls.Add(this.phaseComPort);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cartesianChart2);
            this.Controls.Add(this.cartesianChart1);
            this.Name = "Form1";
            this.Text = "OCXO App";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private LiveCharts.WinForms.CartesianChart cartesianChart1;
        private LiveCharts.WinForms.CartesianChart cartesianChart2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox phaseComPort;
        private System.Windows.Forms.TextBox dacComPort;
        private System.Windows.Forms.Button connect;
        private System.Windows.Forms.Button disconnect;
        private System.Windows.Forms.TextBox currentDacValue;
        private System.Windows.Forms.TextBox sendedDacValue;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button send;
        private System.Windows.Forms.Button startClosedLoop;
        private System.Windows.Forms.Button stopClosedLoop;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button dac0;
        private System.Windows.Forms.Button dac131072;
        private System.Windows.Forms.Button dac150000;
        private System.Windows.Forms.Button dac262143;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button sync;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox currentPhaseValue;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
    }
}

