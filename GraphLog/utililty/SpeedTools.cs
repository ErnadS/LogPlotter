using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GraphLog.utililty
{
    class SpeedTools
    {
        public static PointF calculateSpeedProjections(float fSpeed, float fHeadingDegTHS, float fTrackDegVTG)
        {
            if (float.IsNaN(fSpeed) || float.IsNaN(fHeadingDegTHS) || float.IsNaN(fTrackDegVTG))
                return new PointF(float.NaN, float.NaN);

            PointF project = new PointF();

            float speed_to_boat_angle = degToRad(fTrackDegVTG - fHeadingDegTHS);
            float long_speed  = (float)(fSpeed*Math.Cos(speed_to_boat_angle));
            float trans_speed = (float)(fSpeed*Math.Sin(speed_to_boat_angle));
            project.X = long_speed;
            project.Y = trans_speed;
            return project;
        }

        public static float degToRad(float fDeg)
        {
            return (float)(fDeg * Math.PI / 180);
        }
    }
}
