﻿using System.Drawing;


namespace GraphLog.graph
{
    public class HorizontalGridLinePainter
    {
        private readonly Font _yAxisFont = new Font("Times New Roman", 10); 
        private int _width;
        private readonly Projection _projection;

        public HorizontalGridLinePainter(int width, Projection projection)
        {
            _width = width;
            _projection = projection;            
        }

        public void Resize(int width, int height)
        {
            _width = width;
        }

        public void PaintGrid(Graphics graphic, Pen pen, Brush fontBrush, bool textOnLeftSide)
        {
            float yStepSize = _projection.GridStepY();
            int count = (int)(_projection.YRange / yStepSize) + 1;
            float gridStart = 0;// ((float)((int)(_projection.YMin / yStepSize)) + 1) * (yStepSize);

            for (int i = 0; i < count; i++)
            {
                float yValue = gridStart + i * yStepSize;
                int yPixelWidth = _projection.ConvertYtoScreenPoint(yValue);
                graphic.DrawLine(pen, 0, yPixelWidth, _width, yPixelWidth);
                if (textOnLeftSide)
                    graphic.DrawString("" + yValue, _yAxisFont, fontBrush, 0, yPixelWidth);
                else
                    graphic.DrawString("" + yValue, _yAxisFont, fontBrush, _width - 50, yPixelWidth);
            }
        }
    }
}


