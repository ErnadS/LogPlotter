namespace GraphLog.graph
{
    public class GraphPoint
    {
        public float X { get; set; }
        public float Y { get; set; }

        public GraphPoint()
        {
            X = 0;
            Y = 0;
        }

        public GraphPoint(float x, float y)
        {
            X = x;
            Y = y;
        }
    }
}
