using System;
using System.Drawing;
using System.Linq;

namespace GraphLog.graph
{
    public class Graph
    {
        private readonly HorizontalGridLinePainter _horizontalGridLinePainter;
        private readonly object _lockObject = new Object();
        private readonly CircularBuffer<GraphPoint> _samples;
        private CircularBuffer<Point> _pixels;
        private bool _isVisible = true;

        // Y range
        float _yMin; 
        float _yMax;

        // Line and grid attributes
        Pen _graphPen;
        Pen _yGridPen;
        SolidBrush _yBrushFont;

        public Projection projection;
        public int Length { get { return _samples.Count; } }
        public GraphPoint[] Samples { get { return _samples.ToArray(); } } 


        public Graph(int capacity, float minimumAutoscaleYDiff)
        {
            _samples = new CircularBuffer<GraphPoint>(capacity);

            projection = new Projection(minimumAutoscaleYDiff);
            _horizontalGridLinePainter = new HorizontalGridLinePainter(Projection.Width, projection);

            SetPalette(Color.Red, Color.Gray, Color.Gray);
        }

        public void SetPalette(Color colorGraphLine, Color colorLineYGrid, Color colorFontYGrid)
        {
            _graphPen = new Pen(colorGraphLine, 1);

            _yGridPen = new Pen(colorLineYGrid, 1);
            _yGridPen.DashPattern = new float[]{2, 6};

            _yBrushFont = new SolidBrush(colorFontYGrid); // Font for Y-grid
        }

        public void Clear()
        {
            _pixels = null;
            projection.ClearRange();
        }

        public void AddPoint(GraphPoint point)
        {
            lock (_lockObject)
            {
                _samples.Add(point);

                // Updating Y range
                if (_samples.Count == 0)
                {
                    projection.UpdateAutoscrollFromMeasurement(point.Y);
                    _yMin = point.Y;
                    _yMax = point.Y;
                    return;
                }   

                if (point.Y < Projection.YMin)
                {
                    _yMin = point.Y;
                    projection.UpdateAutoscrollFromMeasurement(_yMin);
                }
                else if (point.Y > Projection.YMax)
                {
                    _yMax = point.Y;
                    projection.UpdateAutoscrollFromMeasurement(_yMax);
                }
            }
        }

        public void ClearRange()
        {
            projection.ClearRange();
        }

        public void SetRangeY(float yMin, float yMax)
        {
            Projection.setRangeY(yMin, yMax);
        }
        
        public void SetVisible(bool visible)
        {
            _isVisible = visible;
        }

        public void PaintLines(Graphics graphics)
        {
            if ((_isVisible == false) || (_samples.Count < 2))
                return;

            lock (_lockObject)
            {
                _pixels = new CircularBuffer<Point>(_samples.Count);

                foreach (var sample in _samples)
                {
                    if (sample.X >= Projection.XMin && sample.X <= Projection.XMax)
                    {
                        _pixels.Add(projection.ConvertToScreenPoint(sample));
                    }
                }

                if (_pixels.Count > 1)
                    graphics.DrawLines(_graphPen, _pixels.ToArray());
            }
        }

        public void paintGrid_Y_axis(Graphics g)
        {
            _horizontalGridLinePainter.PaintGrid(g, _yGridPen, _yBrushFont);
        }

        public void Resize(int width, int height)
        {
            _horizontalGridLinePainter.Resize(width, height);
            projection.AutoScrollUpdate();
        }

        public GraphPoint GetFirstPoint()
        {
            // TODO: Check if okay
            if (_samples.Count == 0)
                return new GraphPoint(0, 0);


            return _samples.ToArray()[0];
        }

        public GraphPoint GetLastPoint()
        {
            if (_samples.Count == 0)
                return new GraphPoint(0, 0);

            return _samples.ToArray()[_samples.Count - 1];
        }
    }
}
