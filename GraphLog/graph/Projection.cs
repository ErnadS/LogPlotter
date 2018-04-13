using System.Drawing;

namespace GraphLog.graph
{
    public class Projection
    {
        // X and Y ranges
        public static float XMin { get; set; }  // TODO: change name to shown_Xmin, shown...
        public static float XMax { get; set; }
        public float YMin { get; set; }
        public float YMax { get; set; }
        public static float XRange { get; set; }
        public /*static*/ float YRange { get; set; }

        public static float X_limit_Min = -1;
        public static float X_limit_Max = -1;

        public float Y_limit_Min = -1;
        public float Y_limit_Max = -1;

        // Factor used to scale value to pixel width
        private static float _xScalingFactor; // {get; private set;}
        private /*static*/ float _yScalingFactor;

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
            IsAutoscaleY = false;

            MinimumAutoscaleYdifference = minimumAutoscaleYdifference;
        }

        public static void SetSize(int width, int height)
        {
            Width = width;
            Height = height;

            RecalculateX();
        }

        public static void SetX_Limit(float fMin, float fMax)
        {
            X_limit_Min = fMin;
            X_limit_Max = fMax;

            if (XMin < X_limit_Min)
                XMin = X_limit_Min;

            if (fMax > X_limit_Max)
                fMax = X_limit_Max;
        }

        

        // Common for all graphs
        public static void setRangeX(float xMin, float xMax)
        {
            XRange = XMax - XMin; // remember old XRange

            XMin = xMin;
            XMax = xMax;

            if (XMin < X_limit_Min && (X_limit_Min != X_limit_Max))
            {
                XMin = X_limit_Min;
                XMax = XMin + XRange; // do not change range, just move to limit
                return;
            } 
            else if (XMax > X_limit_Max && (X_limit_Min != X_limit_Max))
            {
                XMax = X_limit_Max;
                XMin = XMax - XRange;
                return;
            }

            RecalculateX();
        }

        public static void moveRangeX(float delta)
        {
            XMin += delta;
            XMax += delta;

            if (XMin < X_limit_Min && (X_limit_Min != X_limit_Max))
            {
                XMin = X_limit_Min;
                return;
            }

            if (XMax > X_limit_Max && (X_limit_Min != X_limit_Max))
            {
                XMax = X_limit_Max;
                return;
            }

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

        public void SetY_Limit(float fMin, float fMax)
        {
            Y_limit_Min = fMin;
            Y_limit_Max = fMax;

            setRangeY(Y_limit_Min, Y_limit_Max);
        }

        public void setRangeY(float yMin, float yMax)
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

        public void RecalculateY()
        {
            YRange = YMax - YMin;
            if (YRange == 0)
               YRange = 1; //?? TODO

            _yScalingFactor = Height / YRange;            
        }

        public Point ConvertToScreenPoint(GraphPoint point)
        {
            Point pointResult = new Point();
            
            pointResult.X = (int)((point.X - XMin) * _xScalingFactor + 0.5);
            //pointResult.Y = Height - (int)((point.Y - Y_limit_Min) * _yScalingFactor + 0.5);
            
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

        public int ConvertYtoScreenPoint(float y)
        {
            //return Height - (int)((y - Y_limit_Min) * _yScalingFactor + 0.5);
            return Height - (int)((y - YMin) * _yScalingFactor + 0.5);
        }

        public void ClearRange()
        {
            _yMinFromMeasurement = float.MaxValue;
            _yMaxFromMeasurement = float.MinValue;
            pointCounter = 0;
            RecalculateY();
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

            bool update = false;
            if (_yMinFromMeasurement < YMin)
            {
                YMin = _yMinFromMeasurement;
                update = true;
            }

            if (_yMaxFromMeasurement > YMax)
            {
                YMax = _yMaxFromMeasurement;
                update = true;
            }

            if (update == true)
            {
                float diff = YMax - YMin;

                if (diff == 0.0f)
                {
                    YMax = 1.05f * YMax;
                    YMin = 0.95f * YMin;
                }
                else
                {
                    YMax += 0.05f * diff;
                    YMin -= 0.05f * diff;
                }
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
    }
}
