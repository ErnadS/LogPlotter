using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;


namespace GraphLog.graph
{
    public class GraphVector : Graph
    {
        bool bPaintOnMap = false;
        

        public GraphVector(String graphName, int capacity, float minimumAutoscaleYDiff)
            : base(graphName, capacity, minimumAutoscaleYDiff)
        {
        }

        enum EndpointStyle
        {
            ArrowHead,
            Fletching,
            None
        };


        public override void PaintLines(Graphics graphics)
        {
            int nCount = 0;

            if ((_isVisible == false) || (_samples.Count < 2))
                return;

            lock (_lockObject)
            {
                _pixels = new CircularBuffer<Point>(_samples.Count);

                GraphPoint previous = new GraphPoint();

                int nPrevPaintedX = -5;

                foreach (var sample in _samples)
                {
                    nCount++;

                    if (sample.X >= Projection.XMin && sample.X <= Projection.XMax)
                    {

                        Point p = projection.ConvertToScreenPoint(sample);

                        if (!float.IsNaN(sample.Y))
                        {
                            if (!float.IsNaN(sample.Angle))  // We have 3 coordinats
                            {
                                float fAngle = sample.Angle;
                                // TODO: Min/max of graf is from graph -constructor. 
                                int nArrowLength = (int)(sample.Y * 35 / 3); // max length 35 pix. WC speed is between 0 and 5. Sow, 5 kn is shown as 35 pix
                                int f_dX = (int)(nArrowLength * Math.Sin(fAngle * Math.PI / 180));
                                int f_dY = (int)(nArrowLength * Math.Cos(fAngle * Math.PI / 180));
                                p.Y = nVerticalOffset;
                                Point p2 = new Point(p.X + f_dX, p.Y - f_dY);

                                if (Math.Abs(nPrevPaintedX - p.X) > 12)
                                {
                                    DrawArrow(graphics, _graphPen, new PointF(p.X, p.Y), new PointF(p2.X, p2.Y), EndpointStyle.None, EndpointStyle.ArrowHead);
                                    nPrevPaintedX = p.X;
                                }
                            }
                            else
                            {
                                float fAngle = sample.Y;
                                int f_dX = (int)(12 * Math.Sin(fAngle * Math.PI / 180));
                                int f_dY = (int)(12 * Math.Cos(fAngle * Math.PI / 180));
                                p.Y = nVerticalOffset;
                                Point p2 = new Point(p.X + f_dX, p.Y - f_dY);

                                if (Math.Abs(nPrevPaintedX - p.X) > 15)
                                {
                                    DrawArrow(graphics, _graphPen, new PointF(p.X, p.Y), new PointF(p2.X, p2.Y), EndpointStyle.None, EndpointStyle.ArrowHead);
                                    nPrevPaintedX = p.X;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void DrawArrow(Graphics gr, Pen pen, PointF p1, PointF p2, EndpointStyle style1, EndpointStyle style2)
        {
            // Draw the shaft.
            gr.DrawLine(pen, p1, p2);

            // Find the arrow shaft unit vector.
            float vx = p2.X - p1.X;
            float vy = p2.Y - p1.Y;
            float dist = (float)Math.Sqrt(vx * vx + vy * vy);
            vx /= dist;
            vy /= dist;

            float length = dist / 4;

            if (dist < 4) // do not try to paint arrow with length < 1
            {
                if (dist >= 1) // paint nothing if line is < 1
                    gr.DrawLine(pen, p1, p2);
                return;
            }
            // Draw the start.
            if (style1 == EndpointStyle.ArrowHead)
            {
                DrawArrowhead(gr, pen, p1, -vx, -vy, length);
            }
            else if (style1 == EndpointStyle.Fletching)
            {
                DrawArrowhead(gr, pen, p1, vx, vy, length);
            }

            // Draw the end.
            if (style2 == EndpointStyle.ArrowHead)
            {
                DrawArrowhead(gr, pen, p2, vx, vy, length);
            }
            else if (style2 == EndpointStyle.Fletching)
            {
                DrawArrowhead(gr, pen, p2, -vx, -vy, length);
            }
        }

        private void DrawArrowhead(Graphics gr, Pen pen, PointF p, float nx, float ny, float length)
        {
            float ax = length * (-ny - nx);
            float ay = length * (nx - ny);
            PointF[] points =
            {
                new PointF(p.X + ax, p.Y + ay),  p, new PointF(p.X - ay, p.Y + ax)
            };
            gr.DrawLines(pen, points);
        }
    }
}
