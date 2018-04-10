namespace GraphLog.DL2_analyse
{
    partial class FormDL2
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
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.button2 = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.positionPanel = new System.Windows.Forms.Panel();
            this.STW_R_L = new System.Windows.Forms.CheckBox();
            this.STW_R_T = new System.Windows.Forms.CheckBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox
            // 
            this.pictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox.Location = new System.Drawing.Point(-49, 92);
            this.pictureBox.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(1312, 485);
            this.pictureBox.TabIndex = 35;
            this.pictureBox.TabStop = false;
            this.pictureBox.LoadCompleted += new System.ComponentModel.AsyncCompletedEventHandler(this.pictureBox_LoadCompleted);
            this.pictureBox.MouseEnter += new System.EventHandler(this.pictureBox_MouseEnter_1);
            this.pictureBox.MouseLeave += new System.EventHandler(this.pictureBox_MouseLeave_1);
            // 
            // button2
            // 
            this.button2.BackgroundImage = global::GraphLog.Properties.Resources.folder_icon1;
            this.button2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button2.Location = new System.Drawing.Point(561, 6);
            this.button2.Margin = new System.Windows.Forms.Padding(4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(70, 51);
            this.button2.TabIndex = 37;
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.buttonOpenFile_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // positionPanel
            // 
            this.positionPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.positionPanel.BackgroundImage = global::GraphLog.Properties.Resources.SliderLine;
            this.positionPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.positionPanel.Location = new System.Drawing.Point(3, 585);
            this.positionPanel.Margin = new System.Windows.Forms.Padding(4);
            this.positionPanel.Name = "positionPanel";
            this.positionPanel.Size = new System.Drawing.Size(1260, 20);
            this.positionPanel.TabIndex = 38;
            this.positionPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.positionPanel_Paint_1);
            // 
            // STW_R_L
            // 
            this.STW_R_L.AutoSize = true;
            this.STW_R_L.Checked = true;
            this.STW_R_L.CheckState = System.Windows.Forms.CheckState.Checked;
            this.STW_R_L.ForeColor = System.Drawing.Color.Olive;
            this.STW_R_L.Location = new System.Drawing.Point(21, 46);
            this.STW_R_L.Name = "STW_R_L";
            this.STW_R_L.Size = new System.Drawing.Size(70, 21);
            this.STW_R_L.TabIndex = 39;
            this.STW_R_L.Text = "Phase";
            this.STW_R_L.UseVisualStyleBackColor = true;
            this.STW_R_L.CheckedChanged += new System.EventHandler(this.STW_R_L_CheckedChanged);
            // 
            // STW_R_T
            // 
            this.STW_R_T.AutoSize = true;
            this.STW_R_T.Checked = true;
            this.STW_R_T.CheckState = System.Windows.Forms.CheckState.Checked;
            this.STW_R_T.ForeColor = System.Drawing.Color.Blue;
            this.STW_R_T.Location = new System.Drawing.Point(115, 46);
            this.STW_R_T.Name = "STW_R_T";
            this.STW_R_T.Size = new System.Drawing.Size(58, 21);
            this.STW_R_T.TabIndex = 40;
            this.STW_R_T.Text = "DAC";
            this.STW_R_T.UseVisualStyleBackColor = true;
            this.STW_R_T.CheckedChanged += new System.EventHandler(this.STW_R_T_CheckedChanged);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.checkBox3);
            this.panel2.Controls.Add(this.checkBox2);
            this.panel2.Controls.Add(this.checkBox1);
            this.panel2.Controls.Add(this.STW_R_T);
            this.panel2.Controls.Add(this.STW_R_L);
            this.panel2.Controls.Add(this.label5);
            this.panel2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.panel2.Location = new System.Drawing.Point(7, 5);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(470, 80);
            this.panel2.TabIndex = 56;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.ForeColor = System.Drawing.Color.SaddleBrown;
            this.checkBox2.Location = new System.Drawing.Point(328, 34);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(130, 21);
            this.checkBox2.TabIndex = 149;
            this.checkBox2.Text = "Phase Direction";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.ForeColor = System.Drawing.Color.Green;
            this.checkBox1.Location = new System.Drawing.Point(328, 7);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(135, 21);
            this.checkBox1.TabIndex = 148;
            this.checkBox1.Text = "Phase Avgerage";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(18, 4);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(102, 18);
            this.label5.TabIndex = 147;
            this.label5.Text = "Sensor Data";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(483, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(71, 49);
            this.button1.TabIndex = 63;
            this.button1.Text = "Deselect all";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button3.Location = new System.Drawing.Point(884, 612);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(379, 70);
            this.button3.TabIndex = 119;
            this.button3.Text = "Repaint";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.buttonRepaintGraph_Click);
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Checked = true;
            this.checkBox3.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox3.ForeColor = System.Drawing.Color.Red;
            this.checkBox3.Location = new System.Drawing.Point(197, 46);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(112, 21);
            this.checkBox3.TabIndex = 150;
            this.checkBox3.Text = "Temperature";
            this.checkBox3.UseVisualStyleBackColor = true;
            this.checkBox3.CheckedChanged += new System.EventHandler(this.checkBox3_CheckedChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Gray;
            this.label1.Location = new System.Drawing.Point(1005, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(246, 17);
            this.label1.TabIndex = 120;
            this.label1.Text = "- Zoom X-axis:               mouse scrolle";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.Gray;
            this.label2.Location = new System.Drawing.Point(963, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(288, 17);
            this.label2.TabIndex = 121;
            this.label2.Text = "- Zoom DAC Y-axis:       ALT + mouse scrolle";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.Gray;
            this.label3.Location = new System.Drawing.Point(951, 55);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(300, 17);
            this.label3.TabIndex = 122;
            this.label3.Text = "- Zoom all Y-axis:           CTRL + mouse scrolle";
            // 
            // FormDL2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1289, 694);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.positionPanel);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.pictureBox);
            this.Name = "FormDL2";
            this.Text = "ver. 1.03";
            this.Load += new System.EventHandler(this.FormDL2_Load);
            this.Resize += new System.EventHandler(this.FormDL2_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Panel positionPanel;
        private System.Windows.Forms.Panel panel2;
        public System.Windows.Forms.CheckBox STW_R_L;
        public System.Windows.Forms.CheckBox STW_R_T;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.CheckBox checkBox1;
        public System.Windows.Forms.CheckBox checkBox2;
        public System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}