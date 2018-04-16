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
        private readonly GraphFormInterface _form;



        public GraphPainter(PictureBox pictureBox, GraphFormInterface form)
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
            SetXAxisGridColor(Color.FromArgb(180, 180, 180));
        }

        public void AddGraph(Graph graph)
        {
            _graphList.Add(graph);
        }

        public void AddPoint(GraphPoint point,  int nGraphIndex, bool bRecalculateVertical)
        {
            _graphList[nGraphIndex].AddPoint(point, bRecalculateVertical);
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
            //_xGridPen.DashPattern = new float[]{2, 6};
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
                source.RecalculateY();
            }

            paintAll(_graphicsPictureBox);
        }

        public void MouseMoved(object sender, MouseEventArgs e)
        {
            if (System.Windows.Forms.Control.MouseButtons != MouseButtons.Left)
                return;

            int nXscreen = e.X;

            float fReal;
            paintAll(_graphicsPictureBox);
            int x = 0;
            String strX = "";

            for (int i = 0; i < _graphList.Count; i++)
            {
                if (_graphList[i].IsVisible())
                {
                    fReal = _graphList[i].projection.ConvertScreenXToRealValue(nXscreen);
                    strX = "" + (int)fReal;
                    GraphPoint closest = _graphList[i].GetClosestPointTo_X(fReal);
                    if (closest != null && !float.IsNaN(closest.Y))
                    {
                        Point screenPoint = _graphList[i].projection.ConvertToScreenPoint(closest);
                        if (screenPoint.X > 0 && screenPoint.Y > 0)
                        {
                            x = screenPoint.X;
                           // strX = "" + closest.X;
                            _graphicsPictureBox.FillRectangle(new SolidBrush(Color.Yellow), screenPoint.X, screenPoint.Y, 25, 25);
                            _graphicsPictureBox.FillRectangle(new SolidBrush(Color.FromArgb(0xff, 0x00, 0x00)), screenPoint.X - 2, screenPoint.Y - 2, 4, 4);

                            _graphicsPictureBox.DrawString("" + closest.Y, new Font("Times New Roman", 10), new SolidBrush(Color.Black), screenPoint.X, screenPoint.Y);
                            this._pictureBox.Invalidate();
                        }
                    }
                }
            }

            _graphicsPictureBox.DrawString(strX, new Font("Times New Roman", 10), new SolidBrush(Color.Black), x, this.graphHeight - 20);
                           
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

        public void zoom_Y_in(bool bIn, int graphHeight)
        {
            //int graphHeight = ((PictureBox)sender).Height;

            Point screenPoint = Cursor.Position;
            Point graphPoint = _pictureBox.PointToClient(screenPoint);
            float graphicMinY;
            float graphicMaxY;
            float graphicRangeY;
            float fZoomingPoint_real;
            float newRange;
            float fNewY_end;
            float fNewY_start;

            for (int i = 0; i < _graphList.Count; i++)
            {
                graphicMinY = _graphList[i].projection.YMin;
                graphicMaxY = _graphList[i].projection.YMax;
                graphicRangeY = graphicMaxY - graphicMinY;

                fZoomingPoint_real = graphicMaxY - ((float)graphicRangeY * graphPoint.Y / graphHeight);

                if (bIn)
                {
                    newRange = graphicRangeY / 1.5f;
                }
                else
                {
                    newRange = graphicRangeY * 1.5f;
                }

                fNewY_end = fZoomingPoint_real + ((float)graphPoint.Y / graphHeight) * newRange;
                fNewY_start = fNewY_end - newRange;

                setShown_Y(i, fNewY_start, fNewY_end);
            }

            Refresh();
            _form.positionPanel_Refresh();
        }

        public void zoom_Y(object sender, MouseEventArgs e)
        {
            int graphHeight = ((PictureBox)sender).Height;

            Point screenPoint = Cursor.Position;
            Point graphPoint = _pictureBox.PointToClient(screenPoint);

            float graphicMinY;
            float graphicMaxY;
            float graphicRangeY;
            float fZoomingPoint_real;
            float newRange;
            float fNewY_end;
            float fNewY_start;

            for (int i = 0; i < _graphList.Count; i++)
            {
                graphicMinY = _graphList[i].projection.YMin;
                graphicMaxY = _graphList[i].projection.YMax;
                graphicRangeY = graphicMaxY - graphicMinY;


                fZoomingPoint_real = graphicMaxY - ((float)graphicRangeY * graphPoint.Y / graphHeight);


                if (e.Delta > 0)
                {
                    newRange = graphicRangeY / 1.5f;
                }
                else
                {
                    newRange = graphicRangeY * 1.5f;
                }

                fNewY_end = fZoomingPoint_real + ((float)graphPoint.Y / graphHeight) * newRange;
                fNewY_start = fNewY_end - newRange;

                setShown_Y(i, fNewY_start, fNewY_end);
            }


            Refresh();
            _form.positionPanel_Refresh();
        }

        private void setShown_Y(int nGraphIndex, float fNewY_start, float fNewY_end)
        {
            _graphList[nGraphIndex].setShown_Y(fNewY_start, fNewY_end);
        }

        public void autozoom(int nGraphIndex)
        {
            _graphList[nGraphIndex].autozoom();
        }

        public void Zoom_X(bool bIn, int graphWidth)
        {
            // int graphWidth = ((PictureBox)sender).Width;

            Point screenPoint = Cursor.Position;
            Point graphPoint = _pictureBox.PointToClient(screenPoint);

            // TODO: min/max X should be found when adding points to graphs. Change "GetMinimuxX()";
            float graphicMinX = _graphList[0].GetFirstPoint().X;// GetMinimumX();
            float graphicMaxX = _graphList[0].GetLastPoint().X;//GetMaximumX();
            float graphicRangeX = graphicMaxX - graphicMinX;

            float fZoomingPointProportion = ((float)graphPoint.X) / graphWidth;
            float fZoomingPoint_real = Projection.XMin + (Projection.XRange * fZoomingPointProportion);

            float newRange;

            if (bIn)
            {
                newRange = Projection.XRange / 2;
            }
            else
            {
                newRange = Projection.XRange * 2;
            }

            float fNewX_start = fZoomingPoint_real - fZoomingPointProportion * newRange;
            float fNewX_end = fNewX_start + newRange;

            if (fNewX_start < graphicMinX)
                fNewX_start = graphicMinX;

            if (fNewX_end > graphicMaxX)
                fNewX_end = graphicMaxX;

            Projection.setRangeX(fNewX_start, fNewX_end);
            Refresh();
            _form.positionPanel_Refresh();
        }


        public void MouseWheel(object sender, MouseEventArgs e)
        {
            Point screenPoint = Cursor.Position;
            Point graphPoint = _pictureBox.PointToClient(screenPoint);
            float fZoomingPoint_real;
            float newRange;

            if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                zoom_Y(sender, e);
                return;
            }
            else if ((Control.ModifierKeys & Keys.Alt) == Keys.Alt)
            {
                float fMax = _graphList[1].projection.YMax;
                float fMin = _graphList[1].projection.YMin;
                int graphHeight = ((PictureBox)sender).Height;

                Point relatMousePoint = _pictureBox.PointToClient(screenPoint);

                float fZoomingCoef = ((float)relatMousePoint.Y) / graphHeight; // Projection.Height;
                fZoomingPoint_real = fMax - (_graphList[1].projection.YRange * fZoomingCoef);

                if (e.Delta > 0)
                {
                    newRange = _graphList[1].projection.YRange / 1.2f;
                }
                else
                {
                    newRange = _graphList[1].projection.YRange * 1.2f;
                }

                float fNewY_end  = fZoomingPoint_real + fZoomingCoef * newRange;

                float fNewY_start = fNewY_end - newRange;
                /*
                if (fNewY_start < graphicMinX)
                    fNewY_start = graphicMinX;

                if (fNewY_end > graphicMaxX)
                    fNewY_end = graphicMaxX;
                */

                _graphList[1].projection.SetY_Limit(fNewY_start, fNewY_end);


                Refresh();
                _form.positionPanel_Refresh();

                return;
            }

            int graphWidth = ((PictureBox)sender).Width;

            

            // TODO: min/max X should be found when adding points to graphs. Change "GetMinimuxX()";
            float graphicMinX = _graphList[0].GetFirstPoint().X;// GetMinimumX();
            float graphicMaxX = _graphList[0].GetLastPoint().X;//GetMaximumX();
            float graphicRangeX = graphicMaxX - graphicMinX;

            float fZoomingPointProportion = ((float)graphPoint.X) / graphWidth;
            fZoomingPoint_real = Projection.XMin + (Projection.XRange * fZoomingPointProportion);

            if (e.Delta > 0)
            {
                newRange = Projection.XRange / 2;
            }
            else
            {
                newRange = Projection.XRange * 2;
            }

            float fNewX_start = fZoomingPoint_real - fZoomingPointProportion * newRange;
            float fNewX_end = fNewX_start + newRange;

            if (fNewX_start < graphicMinX)
                fNewX_start = graphicMinX;

            if (fNewX_end > graphicMaxX)
                fNewX_end = graphicMaxX;

            Projection.setRangeX(fNewX_start, fNewX_end);
            Refresh();
            _form.positionPanel_Refresh();
        }

        // Call this after fylling with all points (after parsing of the file)
        public void init_X_Limit()
        {
            Projection.SetX_Limit(GetMinimumX(), GetMaximumX());
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


        public void paintAll(Graphics graphics)
        {
            try
            {
                _pictureBox.Invoke((MethodInvoker)delegate
               {    
                   // Projection.SetX_Limit(_graphList[0].GetFirstPoint().X, _graphList[0].GetLastPoint().X);

                   PaintCommonGraph(graphics);

                   _graphList[0].PaintLines(graphics);
                   _graphList[1].PaintPoints(graphics);
                   _graphList[2].PaintLines(graphics);
                   _graphList[3].PaintLines(graphics);
                   _graphList[4].PaintPoints(graphics);
                   _graphList[5].PaintPoints(graphics);
                   /*
                   foreach (Graph graph in _graphList)
                   {
                       graph.PaintLines(graphics);
                   }
                   */
                   _pictureBox.Invalidate();
               });
            }
            catch { }
        }

        // Used only when printing (no background color to save color)
        public override void paintAllNotBackground(Graphics g)
        {
            _pictureBox.Invoke((MethodInvoker)delegate
            {
                SolidBrush scaleBrush = new SolidBrush(Color.FromArgb(0xEE, 0xEE, 0xEE));
                g.FillRectangle(scaleBrush, 0, graphHeight, graphWidth, ScalePixelWidth);

                paintVerticalGridLines(g);

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
                _graphList[0].paintHorisontalGridLines(graphics); // Paint horisontal grid-lines only for first graph

            foreach (Graph gr in _graphList)
            {
                if (gr.name == "DAC")
                    gr.paintHorisontalGridLines(graphics, false);
            }

            paintVerticalGridLines(graphics);
        }

        private void paintVerticalGridLines(Graphics g)
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

        public void Clear(int nGraphIndex)
        {
            _graphList[nGraphIndex].Clear();
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
