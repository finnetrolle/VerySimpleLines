using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AStar
{
    class Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point()
        {
            X = 0;
            Y = 0;
        }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static bool operator== (Point a, Point b)
        {
            if ((a.X == b.X) && (a.Y == b.Y))
                return true;
            return false;
        }

        public static bool operator!= (Point a, Point b)
        {
            if (a == b)
                return false;
            return true;
        }

        public static int DistanceManhattan(Point a, Point b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }

        public override string ToString()
        {
            return "[" + X + "," + Y + "]";
        }
    }
}
