using System.Drawing;

namespace GraphLog.graph
{
    public class Projection
    {
        // X and Y ranges
        public static float XMin { get; set; }
        public static float XMax { get; set; }
        public static float YMin { get; set; }
        public static float YMax { get; set; }
        public static float XRange { get; set; }
        public static float YRange { get; set; }

        // Factor used to scale value to pixel width
        private static float _xScalingFactor;
        private static float _yScalingFactor;

        // Width and Height of the axis
        public static int Width { get; set; }
        public static int Height { get; set; }
        
        public float MinimumAutoscaleYdifference { get; set; }

        private float _yMinFromMeasurement = float.MaxValue;
        private float _yMaxFromMeasurement = float.MinValue;

        private int pointCounter = 0;

        public bool IsAutoscaleY { get; set; }

        public Projection(float minimumAutoscaleYdifference)
        {
            YMin = float.MaxValue;
            YMax = float.MinValue;
            IsAutoscaleY = true;

            MinimumAutoscaleYdifference = minimumAutoscaleYdifference;
        }

        public static void SetSize(int width, int height)
        {
            Width = width;
            Height = height;

            RecalculateX();
        }

        // Common for all graphs
        public static void setRangeX(float xMin, float xMax)
        {
            XMin = xMin;
            XMax = xMax;

            RecalculateX();
        }

        public static void moveRangeX(float delta)
        {
            XMin += delta;
            XMax += delta;

            RecalculateX();
        }
        public static float GetMaxRangeX()
        {
            return XMax;
        }

        public static float GetMinRangeX()
        {
            return XMin;
        }
        public static void setRangeY(float yMin, float yMax)
        {
            YMin = yMin;
            YMax = yMax;

            RecalculateY();
        }

        private static void RecalculateX()
        {
            XRange = XMax - XMin;
            _xScalingFactor = Width / XRange;
        }

        private static void RecalculateY()
        {
            YRange = YMax - YMin;
            _yScalingFactor = Height / YRange;            
        }

        public Point ConvertToScreenPoint(GraphPoint point)
        {
            Point pointResult = new Point();
            
            pointResult.X = (int)((point.X - XMin) * _xScalingFactor + 0.5);
            pointResult.Y = Height - (int) ((point.Y - YMin) * _yScalingFactor + 0.5);
            
            return pointResult;
        }

        public float ConvertScreenXToRealValue(int x)
        {
            return x / _xScalingFactor + XMin;
        }

        public static int ConvertXtoScreenValue(float x)
        {
            return (int)((x - XMin) * _xScalingFactor + 0.5);
        }


        public void ClearRange()
        {
            _yMinFromMeasurement = float.MaxValue;
            _yMaxFromMeasurement = float.MinValue;
            pointCounter = 0;
            AutoScrollUpdate();
        }

        public void UpdateAutoscrollFromMeasurement(float y)
        {
            pointCounter++;

            if (IsAutoscaleY == true)
            {
                if (y < _yMinFromMeasurement)
                {
                    _yMinFromMeasurement = y;
                    AutoScrollUpdate();
                }

                if (y > _yMaxFromMeasurement)
                {
                    _yMaxFromMeasurement = y;
                    AutoScrollUpdate();
                }
            }
        }

        public void AutoScrollUpdate()
        {
            if ((pointCounter < 2) || (IsAutoscaleY == false))
                return;

            if (_yMinFromMeasurement < YMin)
            {
                YMin = _yMinFromMeasurement;
            }

            if (_yMaxFromMeasurement > YMax)
            {
                YMax = _yMaxFromMeasurement;
            }

            RecalculateY();
        }

        // Autocalculate grid step. Try to make step 10, 5 or 2  (avoid strange numbers on scale as e.g. 2.487)
        public static float GridStepX()
        {
            float fGridStep = 10000;

            for (int i = 0; i < 10; i++)
            {
                if (fGridStep > XRange)
                {
                    fGridStep = fGridStep / 10;
                }
                else
                {
                    int ratio = (int)(XRange / fGridStep);

                    switch (ratio)
                    {
                        case 1:
                            return fGridStep / 5;

                        case 2:
                            return fGridStep / 2;
                    
                        default:
                            return fGridStep;
                    }
                }
            }

            return XRange / 5;
        }

        // Autocalculate grid step. Try to make step 10, 5 or 2  (avoid strange numbers on scale as e.g. 2.487)
        public float GridStepY()
        {
            float nGridStep = 100000000f;

            for (int i = 0; i < 10; i++)
            {
                if (nGridStep > YRange)
                {
                    nGridStep = nGridStep / 10;
                }
                else
                {
                    int ratio = (int)(YRange / nGridStep);


                    switch (ratio)
                    {
                        case 1:
                            return nGridStep / 5;
                        
                        case 2:
                            return nGridStep / 2;
                        
                        default:
                            return nGridStep;
                    }
                }
            }

            return (YRange / 5);
        }

        public int ConvertYtoScreenPoint(float y)
        {
            return Height - (int)((y - YMin) * _yScalingFactor + 0.5);
        }
    }
}
