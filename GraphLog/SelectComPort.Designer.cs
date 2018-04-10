namespace GraphLog
{
    partial class SelectComPort
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectComPort));
            this.button1 = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.comboBoxComPort_A = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.labelMsgToUser = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButton115200 = new System.Windows.Forms.RadioButton();
            this.radioButton38400 = new System.Windows.Forms.RadioButton();
            this.radioButton19200 = new System.Windows.Forms.RadioButton();
            this.radioButton9600 = new System.Windows.Forms.RadioButton();
            this.radioButton2400 = new System.Windows.Forms.RadioButton();
            this.radioButton4800 = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButtonTwoComPorts = new System.Windows.Forms.RadioButton();
            this.radioButtonOneCom = new System.Windows.Forms.RadioButton();
            this.comboBoxComPort_B = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(24, 270);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(81, 28);
            this.button1.TabIndex = 11;
            this.button1.Text = "Exit";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(437, 270);
            this.buttonOK.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(81, 28);
            this.buttonOK.TabIndex = 10;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // comboBoxComPort_A
            // 
            this.comboBoxComPort_A.FormattingEnabled = true;
            this.comboBoxComPort_A.Location = new System.Drawing.Point(85, 127);
            this.comboBoxComPort_A.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.comboBoxComPort_A.Name = "comboBoxComPort_A";
            this.comboBoxComPort_A.Size = new System.Drawing.Size(125, 24);
            this.comboBoxComPort_A.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(91, 106);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 17);
            this.label1.TabIndex = 8;
            this.label1.Text = "Select COM Port";
            // 
            // labelMsgToUser
            // 
            this.labelMsgToUser.Location = new System.Drawing.Point(13, 9);
            this.labelMsgToUser.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelMsgToUser.Name = "labelMsgToUser";
            this.labelMsgToUser.Size = new System.Drawing.Size(512, 44);
            this.labelMsgToUser.TabIndex = 12;
            this.labelMsgToUser.Text = "-";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButton115200);
            this.groupBox1.Controls.Add(this.radioButton38400);
            this.groupBox1.Controls.Add(this.radioButton19200);
            this.groupBox1.Controls.Add(this.radioButton9600);
            this.groupBox1.Controls.Add(this.radioButton2400);
            this.groupBox1.Controls.Add(this.radioButton4800);
            this.groupBox1.Location = new System.Drawing.Point(3, 161);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Size = new System.Drawing.Size(531, 53);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Baud Rate";
            // 
            // radioButton115200
            // 
            this.radioButton115200.AutoSize = true;
            this.radioButton115200.Location = new System.Drawing.Point(423, 21);
            this.radioButton115200.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.radioButton115200.Name = "radioButton115200";
            this.radioButton115200.Size = new System.Drawing.Size(77, 21);
            this.radioButton115200.TabIndex = 11;
            this.radioButton115200.Text = "115200";
            this.radioButton115200.UseVisualStyleBackColor = true;
            // 
            // radioButton38400
            // 
            this.radioButton38400.AutoSize = true;
            this.radioButton38400.Location = new System.Drawing.Point(183, 21);
            this.radioButton38400.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.radioButton38400.Name = "radioButton38400";
            this.radioButton38400.Size = new System.Drawing.Size(69, 21);
            this.radioButton38400.TabIndex = 10;
            this.radioButton38400.Text = "38400";
            this.radioButton38400.UseVisualStyleBackColor = true;
            // 
            // radioButton19200
            // 
            this.radioButton19200.AutoSize = true;
            this.radioButton19200.Location = new System.Drawing.Point(344, 21);
            this.radioButton19200.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.radioButton19200.Name = "radioButton19200";
            this.radioButton19200.Size = new System.Drawing.Size(69, 21);
            this.radioButton19200.TabIndex = 9;
            this.radioButton19200.Text = "19200";
            this.radioButton19200.UseVisualStyleBackColor = true;
            // 
            // radioButton9600
            // 
            this.radioButton9600.AutoSize = true;
            this.radioButton9600.Location = new System.Drawing.Point(97, 21);
            this.radioButton9600.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.radioButton9600.Name = "radioButton9600";
            this.radioButton9600.Size = new System.Drawing.Size(61, 21);
            this.radioButton9600.TabIndex = 8;
            this.radioButton9600.Text = "9600";
            this.radioButton9600.UseVisualStyleBackColor = true;
            // 
            // radioButton2400
            // 
            this.radioButton2400.AutoSize = true;
            this.radioButton2400.Location = new System.Drawing.Point(5, 21);
            this.radioButton2400.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.radioButton2400.Name = "radioButton2400";
            this.radioButton2400.Size = new System.Drawing.Size(61, 21);
            this.radioButton2400.TabIndex = 6;
            this.radioButton2400.Text = "2400";
            this.radioButton2400.UseVisualStyleBackColor = true;
            // 
            // radioButton4800
            // 
            this.radioButton4800.AutoSize = true;
            this.radioButton4800.Checked = true;
            this.radioButton4800.Location = new System.Drawing.Point(269, 21);
            this.radioButton4800.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.radioButton4800.Name = "radioButton4800";
            this.radioButton4800.Size = new System.Drawing.Size(61, 21);
            this.radioButton4800.TabIndex = 7;
            this.radioButton4800.TabStop = true;
            this.radioButton4800.Text = "4800";
            this.radioButton4800.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButtonTwoComPorts);
            this.groupBox2.Controls.Add(this.radioButtonOneCom);
            this.groupBox2.Location = new System.Drawing.Point(16, 33);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox2.Size = new System.Drawing.Size(517, 57);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            // 
            // radioButtonTwoComPorts
            // 
            this.radioButtonTwoComPorts.AutoSize = true;
            this.radioButtonTwoComPorts.Location = new System.Drawing.Point(256, 23);
            this.radioButtonTwoComPorts.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.radioButtonTwoComPorts.Name = "radioButtonTwoComPorts";
            this.radioButtonTwoComPorts.Size = new System.Drawing.Size(150, 21);
            this.radioButtonTwoComPorts.TabIndex = 1;
            this.radioButtonTwoComPorts.Text = "Use two COM ports";
            this.radioButtonTwoComPorts.UseVisualStyleBackColor = true;
            this.radioButtonTwoComPorts.CheckedChanged += new System.EventHandler(this.radioButtonTwoComPorts_CheckedChanged);
            // 
            // radioButtonOneCom
            // 
            this.radioButtonOneCom.AutoSize = true;
            this.radioButtonOneCom.Checked = true;
            this.radioButtonOneCom.Location = new System.Drawing.Point(8, 23);
            this.radioButtonOneCom.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.radioButtonOneCom.Name = "radioButtonOneCom";
            this.radioButtonOneCom.Size = new System.Drawing.Size(149, 21);
            this.radioButtonOneCom.TabIndex = 0;
            this.radioButtonOneCom.TabStop = true;
            this.radioButtonOneCom.Text = "Use One COM port";
            this.radioButtonOneCom.UseVisualStyleBackColor = true;
            this.radioButtonOneCom.CheckedChanged += new System.EventHandler(this.radioButtonOneCom_CheckedChanged);
            // 
            // comboBoxComPort_B
            // 
            this.comboBoxComPort_B.Enabled = false;
            this.comboBoxComPort_B.FormattingEnabled = true;
            this.comboBoxComPort_B.Location = new System.Drawing.Point(259, 127);
            this.comboBoxComPort_B.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.comboBoxComPort_B.Name = "comboBoxComPort_B";
            this.comboBoxComPort_B.Size = new System.Drawing.Size(125, 24);
            this.comboBoxComPort_B.TabIndex = 16;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(268, 106);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 17);
            this.label2.TabIndex = 15;
            this.label2.Text = "Select COM Port";
            // 
            // SelectComPort
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(536, 313);
            this.ControlBox = false;
            this.Controls.Add(this.comboBoxComPort_B);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.labelMsgToUser);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.comboBoxComPort_A);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "SelectComPort";
            this.Text = "FormSelectComPort";
            this.Load += new System.EventHandler(this.SelectComPort_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.ComboBox comboBoxComPort_A;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelMsgToUser;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButton115200;
        private System.Windows.Forms.RadioButton radioButton38400;
        private System.Windows.Forms.RadioButton radioButton19200;
        private System.Windows.Forms.RadioButton radioButton9600;
        private System.Windows.Forms.RadioButton radioButton2400;
        private System.Windows.Forms.RadioButton radioButton4800;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioButtonTwoComPorts;
        private System.Windows.Forms.RadioButton radioButtonOneCom;
        private System.Windows.Forms.ComboBox comboBoxComPort_B;
        private System.Windows.Forms.Label label2;
    }
}