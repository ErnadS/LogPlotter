using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using Print;

namespace GraphLog.graph
{
    public class GraphPainter : PrintPane
    {
        private Graphics _graphicsPictureBox;
        private readonly PictureBox _pictureBox;

        private readonly List<Graph> _graphList = new List<Graph>();
        public List<Graph> GraphList
        {
            get
            {
                return _graphList;
            }
        }

        // Get from user-code more info about what to write on x/y axis:
        public delegate String OnDrawXAxisLabelEvent(float value);
        public delegate String OnDrawYAxisLabelEvent(float value);

        public OnDrawXAxisLabelEvent OnRenderXAxisLabel = null;
        public OnDrawYAxisLabelEvent OnRenderYAxisLabel = null;

        // place for text (x-axis values)
        private const int ScalePixelWidth  = 20; 
        private Color _backgroundColor = Color.White;

        private Pen _xGridPen;
        
        private readonly Font _xAxisFont = new Font("Times New Roman", 10);
        private SolidBrush _brushFontXGrid;

        private Bitmap _bitmapGraph;
        private readonly MainForm _form;


        public GraphPainter(PictureBox pictureBox, MainForm form)
        {
            _form = form;
            _pictureBox = pictureBox;

            graphWidth = pictureBox.Width;
            graphHeight = pictureBox.Height - ScalePixelWidth;
            _bitmapGraph = new Bitmap(pictureBox.Width, pictureBox.Height);
            _graphicsPictureBox = Graphics.FromImage(_bitmapGraph);
            _pictureBox.Image = _bitmapGraph;

            Projection.SetSize(graphWidth, graphHeight);

            clearRange();

            Projection.setRangeX(10, 60);

            SetXAxisGridFontColor(Color.Gray);
            SetXAxisGridColor(Color.Blue);
        }

        public void AddGraph(Graph graph)
        {
            _graphList.Add(graph);
        }

        public void SetBackgroundColor(Color backgroundColor)
        {
            _backgroundColor = backgroundColor;
        }

        public void SetXAxisGridFontColor(Color fontColor)
        {
            _brushFontXGrid = new SolidBrush(fontColor);
        }

        public void SetXAxisGridColor(Color gridColor)
        {
            _xGridPen = new Pen(gridColor, 1);
            _xGridPen.DashPattern = new float[]{2, 6};
        }

        public void Refresh()
        {
            paintAll(_graphicsPictureBox);
        }

        public void Resize(int width, int height)
        {
            if (width == 0 || height == 0)
                return;

            graphWidth = width; // _pictureBox.Width;
            graphHeight = height - ScalePixelWidth;  // _pictureBox.Height - ScalePixelWidth;

            _bitmapGraph = new Bitmap(width, height); // !!! bitmap is "graphHeight" + "ScalePixelWidth"
        
            _graphicsPictureBox = Graphics.FromImage(_bitmapGraph);
            _pictureBox.Image = _bitmapGraph;

           // graph_A.updateGraphics(graphicsEcho);
           // graph_B.updateGraphics(graphicsEcho);

            Projection.SetSize(graphWidth, graphHeight);

            foreach (Graph source in _graphList)
            {
                source.Resize(width, height);
            }

            paintAll(_graphicsPictureBox);
        }


        public void SetGraphVisible(int nGraphIndex, bool bShown)
        {
            // TODO: add "id" to Graph.cs
            /*
            foreach (Graph source in GraphList)
            {
                if (source.id == nGraphIndex)
                {
                    source.SetVisible(bShown);
                    return;
                }
            }*/

            paintAll(_graphicsPictureBox);
        }

        public void MouseWheel(object sender, MouseEventArgs e)
        {
            float fNewStart;
            float fNewEnd;
            float fDiff;

            //_form.bUserMovingGraph = true; // Stop graph sliding from timer

            fNewStart = Projection.XMin;
            fNewEnd = Projection.XMax;
            fDiff = (fNewEnd - fNewStart) / 10;  // zopm with 10%

            if (e.Delta < 0) // Zoom in
            {
                fNewStart   -= fDiff;
                fNewEnd     += fDiff;
            }
            else  // Zoom out
            {
                fNewStart += fDiff;
                fNewEnd   -= fDiff;    
            }

            float nPermitedMin = float.MaxValue;
            float nPermitedMax = float.MinValue;

            foreach (Graph source in _graphList) // find start and end "X" for all graphs
            {
                if (source.Length > 0)
                {
                    if (nPermitedMin > source.Samples[0].X)
                        nPermitedMin = source.Samples[0].X;

                    if (nPermitedMax < source.Samples[source.Length - 1].X)
                        nPermitedMax = source.Samples[source.Length - 1].X;
                }
            }

            if (fNewStart < nPermitedMin)  // Prevent zooming out of min/max
                fNewStart = nPermitedMin;

            if (fNewEnd > nPermitedMax)
                fNewEnd = nPermitedMax;

            if (fNewStart < fNewEnd)
                Projection.setRangeX(fNewStart, fNewEnd);
        }

        public void SetRangeX(float xMin, float xMax)
        {
            Projection.setRangeX(xMin, xMax); // Common x-axis for all graphs
            paintAll(_graphicsPictureBox);
        }
        public void MoveRangeX(float delta)
        {
            Projection.moveRangeX(delta); // Common x-axis for all graphs
            paintAll(_graphicsPictureBox);
        }
        public float GetMaxRangeX()
        {
            return Projection.GetMaxRangeX();
        }

        public float GetMinRangeX()
        {
            return Projection.GetMinRangeX();
        }

        public void SetRangeY(float yMin, float yMax)
        {
            foreach (Graph source in _graphList)
            {
                source.SetRangeY(yMin, yMax);
            }
            
            paintAll(_graphicsPictureBox);
        }

        public void paintAll(Graphics graphics)
        {
            try
            {
               _form.Invoke((MethodInvoker)delegate
               {
                   PaintCommonGraph(graphics);

                   foreach (Graph graph in _graphList)
                   {
                       graph.PaintLines(graphics);
                   }

                   _pictureBox.Invalidate();
               });
            }
            catch { }
        }

        // Used only when printing (no background color to save color)
        public override void paintAllNotBackground(Graphics g)
        {
            _form.Invoke((MethodInvoker)delegate
            {
                SolidBrush scaleBrush = new SolidBrush(Color.FromArgb(0xEE, 0xEE, 0xEE));
                g.FillRectangle(scaleBrush, 0, graphHeight, graphWidth, ScalePixelWidth);

                paintGrid_X_axis(g);

                foreach (Graph source in _graphList)
                {
                    source.PaintLines(g);
                }
            });
        }

        private void PaintCommonGraph(Graphics graphics)
        {
            graphics.Clear(_backgroundColor);
            if (_graphList.Count > 0)
                _graphList[0].paintGrid_Y_axis(graphics); // Paint vertical grid only for graph "A" (common with graph "B")
            paintGrid_X_axis(graphics);
        }

        private void paintGrid_X_axis(Graphics g)
        {
            int nXpix;

            float fStep = Projection.GridStepX();
            int nCount = (int)(Projection.XRange / fStep) + 1;

            float gridStart = ((int)(Projection.XMin / fStep) + 1) * (fStep);

            String text;

            for (int i = 0; i < nCount; i++)
            {
                nXpix = Projection.ConvertXtoScreenValue(gridStart + i * fStep);
                g.DrawLine(_xGridPen, nXpix, 0,
                    Projection.ConvertXtoScreenValue(gridStart + i * fStep), graphHeight);

                if (OnRenderXAxisLabel != null)
                    text = OnRenderXAxisLabel(gridStart + i * fStep);
                else
                    text = "" + (gridStart + i * fStep);
                g.DrawString(text, _xAxisFont, _brushFontXGrid, nXpix - 10, graphHeight);
            }

        }


        public void Clear()
        {
            foreach (Graph graph in _graphList)
            {
                graph.Clear();
            }

        }

        public void clearRange()
        {
            foreach (Graph graph in _graphList)
            {
                graph.ClearRange();
            }
        }

        public float GetMinimumX()
        {
            if (GraphList.Count == 0)
                throw new InvalidOperationException("No graphics are present in painter.");

            float min = float.MaxValue;
            foreach (var graph in GraphList)
            {
                float value = graph.GetFirstPoint().X;

                if (value < min)
                    min = value;
            }

            return min;
        }

        public float GetMaximumX()
        {
            if (GraphList.Count == 0)
                throw new InvalidOperationException("No graphics are present in painter.");

            float max = float.MinValue;
            foreach (var graph in GraphList)
            {
                float value = graph.GetLastPoint().X;

                if (value > max)
                    max = value;
            }

            return max;
        }
    }
}
