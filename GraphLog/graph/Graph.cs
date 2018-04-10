using System;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;

namespace GraphLog.graph
{
    public class Graph
    {
        protected readonly HorizontalGridLinePainter _horizontalGridLinePainter;
        protected readonly object _lockObject = new Object();
        protected List<GraphPoint> _samples;
        //protected CircularBuffer<GraphPoint> _samples;
        protected CircularBuffer<Point> _pixels;
        protected bool _isVisible = true;
        protected int _capacity;

        public String name;

       // public CircularBuffer<GraphPoint> Buffer { get { return _samples; } }

        // Y range
        protected float _yMin;
        protected float _yMax;

        // Line and grid attributes
        protected Pen _graphPen;
        protected Pen _yGridPen;
        protected SolidBrush _yBrushFont;

        public UInt32 usedLength = 0;

        public Projection projection;
        public UInt32 Length { get { return usedLength; } } // _samples.Count; } }
        // public GraphPoint[] Samples { get { return _samples.ToArray(); } }
        public List<GraphPoint> Samples { get { return _samples; } }

        protected int nVerticalOffset = 500;

        

        public Graph() : this("", 10000, 20)
        {
            usedLength = 0;
        }

        public Graph(int capacity, float minimumAutoscaleYDiff) : this("", capacity, minimumAutoscaleYDiff)
        {
            usedLength = 0;
        }

        public Graph(String graphName, int capacity, float minimumAutoscaleYDiff)
        {
            name = graphName;
            _capacity = capacity;
            _samples = new List<GraphPoint>(capacity); // CircularBuffer<GraphPoint>(capacity);

            projection = new Projection(minimumAutoscaleYDiff);
            _horizontalGridLinePainter = new HorizontalGridLinePainter(Projection.Width, projection);

            SetPalette(Color.Red, Color.Gray, Color.Gray);
            usedLength = 0;
        }

        public void SetPalette(Color colorGraphLine, Color colorLineYGrid, Color colorFontYGrid)
        {
            SetPalette(colorGraphLine, colorLineYGrid, colorFontYGrid, 1, false);
        }

        public void SetPalette(Color colorGraphLine, Color colorLineYGrid, Color colorFontYGrid, int penWidth, bool dash)
        {
            _graphPen = new Pen(colorGraphLine, penWidth);
            if (dash)
            {
                _graphPen.DashPattern = new float[] { 4, 6 };
                //_graphPen.DashCap = System.Drawing.Drawing2D.DashCap.Round;
            }
            _yGridPen = new Pen(colorLineYGrid, 1);
            //_yGridPen.DashPattern = new float[]{2, 6};

            _yBrushFont = new SolidBrush(colorFontYGrid); // Font for Y-grid
        }

        public void Clear()
        {
            _pixels = null;
            _samples = new List<GraphPoint>(_capacity); //new CircularBuffer<GraphPoint>(_capacity);
            projection.ClearRange();
            usedLength = 0;
        }

        public void AddPoint(GraphPoint point, bool bUpdateAutoscroll)
        {
            lock (_lockObject)
            {
                _samples.Add(point);
                usedLength++;
                // Updating Y range
                if (_samples.Count == 1)
                {
                    if (bUpdateAutoscroll) 
                        projection.UpdateAutoscrollFromMeasurement(point.Y);
                    _yMin = point.Y;
                    _yMax = point.Y;
                    return;
                }

                if (point.Y < _yMin) //Projection.YMin)
                {
                    _yMin = point.Y;

                    if (bUpdateAutoscroll) 
                        projection.UpdateAutoscrollFromMeasurement(_yMin);
                }
                else if (point.Y > _yMax) //Projection.YMax)
                {
                    _yMax = point.Y;

                    if (bUpdateAutoscroll) 
                        projection.UpdateAutoscrollFromMeasurement(_yMax);
                }
            }
        }

        public void ClearRange()
        {
            projection.ClearRange();
        }

        public void setShown_Y(float yMin, float yMax)
        {
            projection.setRangeY(yMin, yMax);
        }

        public void SetY_Limit(float fMin, float fMax)
        {
            projection.SetY_Limit(fMin, fMax);
        }

        public void SetRangeAuto()
        {
            projection.setRangeY(0 /*_yMin*/, _yMax * 1.05f);  // add 5 %
        }
        
        public void SetVisible(bool visible)
        {
            _isVisible = visible;
        }

        public bool IsVisible()
        {
            return _isVisible;
        }

        public virtual void PaintLines(Graphics graphics)
        {
            int nCount = 0;

            if ((_isVisible == false) || (usedLength < 2)) //(_samples.Count < 2))
                return;

            lock (_lockObject)
            {
                _pixels = new CircularBuffer<Point>((int)usedLength); //_samples.Count);

                GraphPoint previous = new GraphPoint();
                Point previousProjected = new Point();
                bool bFirst = true;

                foreach (var sample in _samples)
                {
                    nCount++;

                    if (sample.X >= Projection.XMin && sample.X <= Projection.XMax)
                    {
                        Point p = projection.ConvertToScreenPoint(sample);
                        if (bFirst)
                        {
                            bFirst = false;
                            previous = sample;
                            previousProjected = p;
                        }
                        else if (sample.Y < 10000000 && sample.Y > -10000000)  // ovo je samo zbog neke greske u fazi koja uzrokuje da program ide u exception pa bude spor
                        {
                            try
                            {
                                if (!float.IsNaN(sample.Y) && !float.IsNaN(previous.Y))
                                    graphics.DrawLine(_graphPen, p.X, p.Y, previousProjected.X, previousProjected.Y);
                            }
                            catch
                            {
                                Console.WriteLine("Err. 229911: " + sample.Y + "   " + previous.Y);
                            }
                            // _pixels.Add(projection.ConvertToScreenPoint(sample));

                            previous = sample;
                            previousProjected = p;
                        }
                        else
                        {
                            // Console.WriteLine("Err. 229912: " + sample.Y + "   " + previous.Y);
                        }
                    }
                }

              //  if (_pixels.Count > 1)
                //    graphics.DrawLines(_graphPen, _pixels.ToArray());
            }
        }

        public virtual void PaintPoints(Graphics graphics)
        {
            int nCount = 0;

            if ((_isVisible == false) || (usedLength < 2)) //(_samples.Count < 2))
                return;

            lock (_lockObject)
            {
                _pixels = new CircularBuffer<Point>((int)usedLength); //_samples.Count);
                float fPrevY = float.NaN;

                foreach (var sample in _samples)
                {
                    nCount++;
                    
                    if (sample.X >= Projection.XMin && sample.X <= Projection.XMax)
                    {
                        Point p = projection.ConvertToScreenPoint(sample);

                        if (!float.IsNaN(sample.Y)/* && sample.Y != fPrevY*/ && p.Y > -10000000 && p.Y < 10000000)  // ovo sa " p.Y < 10000000" je dodano da ne pada program. TODO: pogledaj zasto ima takve vrijednosti
                        {
                            graphics.FillEllipse(new SolidBrush(_graphPen.Color), p.X - 2, p.Y - 2, 4, 4);
                        }
                        fPrevY = sample.Y;
                    }
                }
            }
        }

        public void paintHorisontalGridLines(Graphics g)
        {
            paintHorisontalGridLines(g, true);
        }

        public void paintHorisontalGridLines(Graphics g, bool bPaintTextOnLeft)
        {
            if (bPaintTextOnLeft)
                _horizontalGridLinePainter.PaintGrid(g, _yGridPen, _yBrushFont, bPaintTextOnLeft);
            else
                _horizontalGridLinePainter.PaintGrid(g, new Pen(Color.FromArgb(220, 220, 220)), _yBrushFont, bPaintTextOnLeft);
        }

        public void Resize(int width, int height)
        {
            _horizontalGridLinePainter.Resize(width, height);
            //projection.RecalculateY(); // AutoScrollUpdate();
        }

        public void RecalculateY()
        {
            projection.RecalculateY();
        }

        public GraphPoint GetFirstPoint()
        {
            // TODO: Check if okay
            if (usedLength == 0) //_samples.Count == 0)
                return new GraphPoint(0, 0);


            return _samples.ToArray()[0];
        }

        public GraphPoint GetLastPoint()
        {
            if (usedLength == 0) //_samples.Count == 0)
                return new GraphPoint(0, 0);

            return _samples.ToArray()[usedLength -1]; //_samples.Count - 1];
        }

        public GraphPoint GetClosestPointTo_X(float fRealX)
        {
            if (usedLength == 0) //_samples.Count == 0)
                return null;

            int start = 0;
            int end = (int)usedLength - 1; // _samples.Count - 1;


            while (end - start > 5)
            {
                if (_samples.ElementAt((end + start) / 2).X < fRealX)
                {
                    start = (end + start) / 2 ;
                }
                else
                {
                    end = (end + start) / 2 ;
                }
            }

            for (int i = start; i < end -1; i++)
            {
                if (_samples.ElementAt(i).X < fRealX && _samples.ElementAt(i + 1).X > fRealX)
                {
                    if (Math.Abs(_samples.ElementAt(i).X - fRealX) > Math.Abs(_samples.ElementAt(i + 1).X - fRealX))
                    {
                        return _samples.ElementAt(i + 1);
                    }
                    else
                        return _samples.ElementAt(i);
                }
            }

            return null;
        }
        /*
        public GraphPoint GetClosestPointTo_X(float fRealX)
        {
            if (_samples.Count == 0)
                return new GraphPoint(0, 0);

            for (int i = 0; i < _samples.Count; i++)
            {
                if (_samples.ElementAt(i).X < fRealX && _samples.ElementAt(i + 1).X > fRealX)
                {
                    if (Math.Abs(_samples.ElementAt(i).X - fRealX) > Math.Abs(_samples.ElementAt(i + 1).X - fRealX))
                    {
                        return _samples.ElementAt(i);
                    }
                    else
                        return _samples.ElementAt(i + 1);
                }
            }

            return null;
        }*/

        // TODO only because of GraphVector (remove later)

        public void setVeritcalOffset(int nOffset)
        {
            nVerticalOffset = nOffset;
        }
    }
}
