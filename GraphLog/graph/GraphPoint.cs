namespace GraphLog.graph
{
    public class GraphPoint
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Angle { get; set; }

        public float Pos_X { get; set; }
        public float Pos_Y { get; set; }

        public GraphPoint()  
        {
            X = 0;
            Y = 0;
            Angle = float.NaN; // angle not used
            Pos_X = X;  // position not used
            Pos_Y = Y;  // position not used
        }

        public GraphPoint(float x, float y)  // Normal graph, we have only X and Y
        {
            X = x;
            Y = y;
            Angle = float.NaN; // angle not used
            Pos_X = X;  // position not used
            Pos_Y = Y;  // position not used
        }

        public GraphPoint(float x, float y, float angle)  // Arrow. We have intensity "y" and angle of vector
        {
            X = x;
            Y = y;
            Angle = angle;

            Pos_X = X;  // position not used
            Pos_Y = Y;  // position not used
        }

        public GraphPoint(float x, float y, float angle, float pos_x, float pos_y)  // Arrow painted in pos_X, pos_Y (map)
        {
            X = x;  // not used if painted on pos_X, pos_Y (on map)
            Y = y;
            Angle = angle;

            Pos_X = pos_x;  // position not used
            Pos_Y = pos_y;  // position not used
        }
    }
}
