using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AStar
{
    class PathNode
    {
        public Point Location { get; private set; }
        public int GValue { get; private set; }
        public int HValue { get; private set; }
        public int FValue { get; private set; }

        public PathNode Parent { get; set; }

        public PathNode(Point location, int gValue, Point goal, PathNode parent = null) 
        {
            Location = location;
            GValue = gValue;
            HValue = Point.DistanceManhattan(Location, goal) * 10;
            FValue = GValue + HValue;
            Parent = parent;
        }

        /// <summary>
        /// Creates childs for Horizontal, Vertical and Diagonal neighbors
        /// </summary>
        /// <param name="goal">goal point</param>
        /// <param name="field">field with walkable marks</param>
        /// <returns>list of childs</returns>
        public List<PathNode> CreateChildsHV(Point goal, Field field)
        {
            List<PathNode> result = new List<PathNode>();
            Point p = null;
            // creating childs from myself
            //p = new Point(Location.X, Location.Y); 
            if (field.IsWalkable(p = new Point(Location.X + 1, Location.Y))) result.Add(new PathNode(p, 10 + this.GValue, goal, this));
            if (field.IsWalkable(p = new Point(Location.X - 1, Location.Y))) result.Add(new PathNode(p, 10 + this.GValue, goal, this));
            if (field.IsWalkable(p = new Point(Location.X, Location.Y + 1))) result.Add(new PathNode(p, 10 + this.GValue, goal, this));
            if (field.IsWalkable(p = new Point(Location.X, Location.Y - 1))) result.Add(new PathNode(p, 10 + this.GValue, goal, this)); 
            return result;
        }
        /// <summary>
        /// Creates childs for Horizontal, Vertical and Diagonal neighbors
        /// </summary>
        /// <param name="goal">goal point</param>
        /// <param name="field">field with walkable marks</param>
        /// <returns>list of childs</returns>
        public List<PathNode> CreateChildsHVD(Point goal, Field field)
        {
            List<PathNode> result = CreateChildsHV(goal, field);
            Point p = null;
            if (field.IsWalkable(p = new Point(Location.X + 1, Location.Y + 1))) result.Add(new PathNode(p, 14 + this.GValue, goal, this));
            if (field.IsWalkable(p = new Point(Location.X - 1, Location.Y - 1))) result.Add(new PathNode(p, 14 + this.GValue, goal, this));
            if (field.IsWalkable(p = new Point(Location.X - 1, Location.Y + 1))) result.Add(new PathNode(p, 14 + this.GValue, goal, this));
            if (field.IsWalkable(p = new Point(Location.X + 1, Location.Y - 1))) result.Add(new PathNode(p, 14 + this.GValue, goal, this));
            return result;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("PathNode ")
                .Append(Location.ToString())
                .Append("; G = ").Append(GValue)
                .Append("; H = ").Append(HValue)
                .Append("; F = ").Append(FValue);
            return sb.ToString();
        }

        public void PrintPath()
        {
            PrintPath(this);
        }

        private static void PrintPath(PathNode current)
        {
            Console.WriteLine(">>> " + current);
            if (current.Parent != null)
                PrintPath(current.Parent);
        }

    }
}
