using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamProjectMPI
{
    class Line
    {
        int x1;
        int x2;
        int y1;
        int y2;

        public double slope;
        public double yIntersect;

        public void ComputeSlopeAndIntersect()
        {
            var d = x1 - x2;
            if ((x1 - x2) != 0)
            {
                //slope = (double)(y1 - y2) / (double)(x1 - x2);
                slope = (double)Decimal.Divide((y1 - y2), (x1 - x2));
                yIntersect = y1 - slope * x1;
            }
            else
            {
                slope = double.NaN;
                yIntersect = x1;
            }
        }


        public bool CheckIfPointBelongsToLine(int x, int y)
        {
            if (slope != double.NaN)
                return y == (int)((slope * x) + yIntersect);

            return false;
        }


        public static bool CheckIfPointShouldBeColoured(List<Line> lines, int x, int y)
        {
            foreach (var line in lines)
            {
                if (line != null)
                {
                    if (line.CheckIfPointBelongsToLine(x, y))
                        return true;
                }
               
            }

            return false;
        }


        public Line(int x1, int y1, int x2, int y2)
        {
            this.x1 = x1;
            this.x2 = x2;
            this.y1 = y1;
            this.y2 = y2;
            ComputeSlopeAndIntersect();

        }


    }
}

