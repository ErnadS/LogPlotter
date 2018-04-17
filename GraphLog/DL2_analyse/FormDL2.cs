using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GraphLog; // wrong ...
using GraphLog.graph;


using System.Windows.Media.Imaging;

namespace GraphLog.DL2_analyse
{
    public partial class FormDL2 : Form, GraphFormInterface
    {
        String filePath;
        const int MAX_GRAPH_COUNT = 1000000;

        PositionSliderButton positionSliderButton;

        private System.Timers.Timer timerRefresh;

        public GraphPainter graphPainter;

        bool bPositionPanel_MouseIsDown = false;


        int nPositionPanel_lastMouse_X = -1;


        Graph Phase_graph;
        Graph DAC_graph;
        Graph Temper_graph;

        Graph Phase_avg_graph;
        Graph Phase_firstDeriv_graph;
        Graph Phase_secDeriv_graph;

        public FormDL2()
        {
            InitializeComponent();
            graphPrepare();

            BitmapImage bitmapImage = new BitmapImage();
            positionSliderButton = new PositionSliderButton();
            // this.positionPanel.Controls.Add(this.positionSliderButton);

            positionSliderButton.BackgroundImage = global::GraphLog.Properties.Resources.BlueRectanglePosition;
            positionSliderButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            positionSliderButton.Location = new System.Drawing.Point(274, 1);
            positionSliderButton.Name = "positionSliderButton";
            positionSliderButton.Size = new System.Drawing.Size(23, 20);
            positionSliderButton.BackColor = Color.Transparent;
        }

        PhaseHandler phaseHandler;
        
        private void buttonOpenFile_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "DAC|dac_value*.txt";
            openFileDialog1.Title = "Select a DAC File";  
            DialogResult result = openFileDialog1.ShowDialog();
            // DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {
                showGraphSlider(true);
                filePath = openFileDialog1.FileName;
                this.Text = filePath;
                String folderName;
                int nind = filePath.LastIndexOf("\\");
                folderName = filePath.Substring(0, nind);

               

                phaseHandler = new PhaseHandler(graphPainter);
              
     
                // graphPainter.init_X_Limit();
                // graphPainter.SetRangeY(-5, 20);

                Phase_graph.SetY_Limit(-200, 200);
                DAC_graph.SetY_Limit(100000, 150000);
                Temper_graph.SetY_Limit(0, 40);
                Phase_avg_graph.SetY_Limit(-200, 200);
                Phase_firstDeriv_graph.SetY_Limit(-200, 200);
                Phase_secDeriv_graph.SetY_Limit(-200, 200);

                phaseHandler.parseFiles(filePath);

                graphPainter.init_X_Limit();
                //graphPainter.SetRangeX(Projection.X_limit_Min, Projection.X_limit_Max);
                RefreshGraph();
            }
        }

        public void showGraphSlider(bool bShow)
        {
            this.Invoke((MethodInvoker)delegate
            {
             /*   if (bShow)
                    positionPanel.Show();
                else
                    positionPanel.Hide();*/
            });
        }
        


        public void graphPrepare()
        {
            graphPainter = new GraphPainter(pictureBox, this);

            Phase_graph = new Graph("Phase", MAX_GRAPH_COUNT, 20);
            Phase_graph.SetPalette(Color.Olive, Color.Gray, Color.Gray, 1, false);
            graphPainter.AddGraph(Phase_graph);

            DAC_graph = new Graph("DAC", MAX_GRAPH_COUNT, 20);
            DAC_graph.SetPalette(Color.Blue, Color.Gray, Color.Gray, 1, true);
            graphPainter.AddGraph(DAC_graph);

            Temper_graph = new Graph("TEMP", MAX_GRAPH_COUNT, 20);
            Temper_graph.SetPalette(Color.Red, Color.Gray, Color.Gray, 1, false);
            graphPainter.AddGraph(Temper_graph);


            Phase_avg_graph = new Graph("Phase_avg", MAX_GRAPH_COUNT, 20);
            Phase_avg_graph.SetPalette(Color.Green, Color.Gray, Color.Gray, 1, false);
            graphPainter.AddGraph(Phase_avg_graph);

            Phase_firstDeriv_graph = new Graph("First_dev", MAX_GRAPH_COUNT, 20);
            Phase_firstDeriv_graph.SetPalette(Color.Brown, Color.Gray, Color.Gray, 1, false);
            graphPainter.AddGraph(Phase_firstDeriv_graph);

            Phase_secDeriv_graph = new Graph("Sec_dev", MAX_GRAPH_COUNT, 20);
            Phase_secDeriv_graph.SetPalette(Color.FromArgb(192, 192, 0), Color.Gray, Color.Gray, 1, false);
            graphPainter.AddGraph(Phase_secDeriv_graph);


            Phase_graph.SetVisible(STW_R_L.Checked);
            DAC_graph.SetVisible(STW_R_T.Checked);
            Temper_graph.SetVisible(checkBox3.Checked);
            Phase_avg_graph.SetVisible(checkBox1.Checked);
            Phase_firstDeriv_graph.SetVisible(checkBox2.Checked);
            Phase_secDeriv_graph.SetVisible(checkBox4.Checked);
            //Phase_graph.SetY_Limit(-200, 200);
            //DAC_graph.SetY_Limit((int)numericUpDown2.Value, (int)numericUpDown1.Value);
                


            graphPainter.OnRenderXAxisLabel += RenderXLabel;
            // graphPainter.SetRangeY(0, 10);
            pictureBox.MouseWheel += new MouseEventHandler(graphPainter.MouseWheel);
            pictureBox.MouseMove += new MouseEventHandler(graphPainter.MouseMoved);
            // this.MouseWheel += new MouseEventHandler(graphPainter.MouseWheel);
        }

        private void showProgress(bool bShow)
        {
            this.Invoke((MethodInvoker)delegate
            {
                //progressBar1.Visible = bShow;
            });
        }

        private void updateProgress(int newValue)
        {
            try
            {
                this.Invoke((MethodInvoker)delegate
                {
                    // progressBar1.Value = newValue;
                });
            }
            catch { }
        }

        bool bHasTimeStample = false;

        public String RenderXLabel(float fValue)
        {
            if (bHasTimeStample)
            {
                DateTime dateTime = new DateTime((long)fValue);
                //string format = "yyyy_M_d_HH_mm_ss";  
                string format = "HH:mm:ss";
                return dateTime.ToString(format);
            }
            else
                return "" + fValue;
        }

        private Object graphUpdateLock = new Object();

        // Used to stop autosliding (timer) if user is moving graph (zoom or sliding)
        public bool bUserMovingGraph = false;  // changed e.g. when zooming

        public void RefreshGraph()
        {
            try
            {
                if (!InvokeRequired)
                {
                    lock (graphUpdateLock)
                    {
                        graphPainter.Refresh();
                    }
                }
                else
                    Invoke(new MethodInvoker(RefreshGraph));
            }
            catch
            {// do not show excepiont to user. Here is comming when closing window 
            }
        }


        // TODO: used only for informing MainForm that zoom is changed. Better to move "positionPanel" to GraphPainter? Argument of GraphPainter
        // can be one panel for postion slider? If "null", it is not used
        public void positionPanel_Refresh()
        {
            //positionPanel.Refresh();
        }

        private void positionPanel_Paint(object sender, PaintEventArgs e)
        {
            paintPanelPostitionPaintRectangle((Panel)sender);
        }

        private void paintPanelPostitionPaintRectangle(Panel parentPanel)
        {
            float graphicMinX = graphPainter.GetMinimumX();
            float graphicMaxX = graphPainter.GetMaximumX();
            float graphicRangeX = graphicMaxX - graphicMinX;

            int nRectangleStart = (int)((Projection.XMin - graphicMinX) * parentPanel.Size.Width / graphicRangeX);
            int nRectangleWidth = (int)(parentPanel.Size.Width * (Projection.XMax - Projection.XMin) / graphicRangeX);

            if (nRectangleWidth <= 5)
            {
                nRectangleWidth = 5;
            }

            positionSliderButton.Width = nRectangleWidth;
            positionSliderButton.Location = new System.Drawing.Point(nRectangleStart, 0);
        }

        private Boolean _isClicked = false;
        private Point _startPoint;

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            Point screenPoint = Cursor.Position;
            Point graphPoint = pictureBox.PointToClient(screenPoint);

            if ((graphPoint.X < 0) || (graphPoint.X > pictureBox.Width))
            {
                _isClicked = false;
                Cursor.Current = Cursors.Arrow;
                return;
            }

            if ((graphPoint.Y < 0) || (graphPoint.Y > pictureBox.Height))
            {
                _isClicked = false;
                Cursor.Current = Cursors.Arrow;
                return;
            }

            Projection projection = graphPainter.GraphList[0].projection;

            if ((e.Button == MouseButtons.Left) && (_isClicked == false))
            {
                _isClicked = true;
                _startPoint = graphPoint;
            }

            if ((e.Button == MouseButtons.Left) && (_isClicked == true))
            {
                Point stopPoint = graphPoint;
                // No change in position happened, therefore no translation is required
                int pixelDifference = stopPoint.X - _startPoint.X;
                if (pixelDifference == 0)
                    return;

                // Setting up hand cursor
                Cursor.Current = Cursors.Hand;

                float sensivity = 3.0f;
                float difference = sensivity * (projection.ConvertScreenXToRealValue(stopPoint.X) - projection.ConvertScreenXToRealValue(_startPoint.X));


                // Updating new X range based on difference
                graphPainter.SetRangeX(Projection.XMin - difference, Projection.XMax - difference);
                _isClicked = false;
                _startPoint = stopPoint;

                // positionPanel.Refresh();
                return;
            }

            _isClicked = false;
        }

        private void pictureBox_MouseLeave(object sender, EventArgs e)
        {
            ActiveControl = null;
        }

        private void pictureBox_MouseEnter(object sender, EventArgs e)
        {
            pictureBox.Focus();
        }

        private void positionPanel_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void positionPanel_MouseDown(object sender, MouseEventArgs e)
        {
            bPositionPanel_MouseIsDown = true;
            nPositionPanel_lastMouse_X = e.X;
        }

        private void positionPanel_MouseUp(object sender, MouseEventArgs e)
        {
            bPositionPanel_MouseIsDown = false;
        }

        private void FormDL2_Resize(object sender, EventArgs e)
        {
            if (graphPainter != null)
                graphPainter.Resize(pictureBox.Width, pictureBox.Height);
        }

        private void pictureBox_MouseWheel(object sender, MouseEventArgs e)
        {

        }

        private void pictureBox_MouseEnter_1(object sender, EventArgs e)
        {
            pictureBox.Focus(); // or mouse scroll not works on picture box
        }

        private void pictureBox_MouseLeave_1(object sender, EventArgs e)
        {
            ActiveControl = null;
        }

       

        private void STW_R_L_CheckedChanged(object sender, EventArgs e)
        {
            Phase_graph.SetVisible(STW_R_L.Checked);
            graphPainter.Refresh();
        }

        private void STW_R_T_CheckedChanged(object sender, EventArgs e)
        {
            DAC_graph.SetVisible(STW_R_T.Checked);
            graphPainter.Refresh();
        }


        




        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Down) // Keys.Down // Keys.Left
            {
                graphPainter.zoom_Y_in(false, pictureBox.Height);
            }
            else if (keyData == Keys.Up) // Keys.Up // Keys.Right
            {
                graphPainter.zoom_Y_in(true, pictureBox.Height);
            }
            else if (keyData == Keys.Right)  // Keys.Right  // Keys.Up
            {
                graphPainter.Zoom_X(true, pictureBox.Width);
            }
            else if (keyData == Keys.Left) // Keys.Left // Keys.Down
            {
                graphPainter.Zoom_X(false, pictureBox.Width);
            }

            

            return base.ProcessCmdKey(ref msg, keyData);
        }

       

       


        private void pictureBox_LoadCompleted(object sender, AsyncCompletedEventArgs e)
        {

        }

        private void FormDL2_Load(object sender, EventArgs e)
        {
           
        }   

        private void buttonRepaintGraph_Click(object sender, EventArgs e)
        {
            this.graphPainter.Clear();
            phaseHandler = new PhaseHandler(graphPainter);
    
            graphPainter.init_X_Limit();
            // graphPainter.SetRangeX(Projection.X_limit_Min, Projection.X_limit_Max);
            RefreshGraph();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void positionPanel_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Phase_avg_graph.SetVisible(checkBox1.Checked);
            graphPainter.Refresh();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            Phase_firstDeriv_graph.SetVisible(checkBox2.Checked);
            graphPainter.Refresh();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            Temper_graph.SetVisible(checkBox3.Checked);
            graphPainter.Refresh();
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            Phase_secDeriv_graph.SetVisible(checkBox4.Checked);
            graphPainter.Refresh();
        }
    }
}
