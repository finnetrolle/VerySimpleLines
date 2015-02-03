using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace AStar
{
    class AStar
    {
        public static Field GenerateField(int[,] cells, int[] walkingValues = null)
        {
            if ((walkingValues == null) || (walkingValues.Length == 0))
                return new Field(cells);
            else
            {
                Field f = new Field(cells, walkingValues[0]);
                for (int i = 1; i < walkingValues.Length; ++i)
                    f.AddWalkableValue(walkingValues[i]);
                return f;
            }
        }

        public static List<Point> Find(Point from, Point to, Field field)
        {
            PathNodeList closed = new PathNodeList();
            PathNodeList opened = new PathNodeList();

            PathNode start = new PathNode(from, 0, to);
            opened.AddPathNode(start);

            // search childs of start
            List<PathNode> children = start.CreateChildsHV(to, field);
            foreach(PathNode child in children)
                opened.AddPathNode(child);
            // move start
            closed.AddPathNode(opened.PullPathNode(start.Location));

            PathNode next = opened.GetNodeWithBestFValue();
            while (next.Location != to)
            {
                children = next.CreateChildsHV(to, field);
                closed.AddPathNode(opened.PullPathNode(next.Location));
                foreach (PathNode node in children)
                {
                    if (closed.GetNodeByLocation(node.Location) == null)
                    {
                        opened.AddPathNode(node);
                    }
                }                
                next = opened.GetNodeWithBestFValue();
                if (next == null)
                    break;
            }

            if (next == null)
                return null;

            List<Point> result = new List<Point>();
            while (next.Parent != null)
            {
                result.Insert(0, next.Location);
                next = next.Parent;
            }

            return result;

        }

        public static List<Point> FindPath(Point from, Point to, Field field)
        {
            Console.WriteLine(field);
            // generate lists
            PathNodeList closed = new PathNodeList();
            PathNodeList opened = new PathNodeList();
            // make start pathNode and add it into opened list
            PathNode start = new PathNode(from, 0, to);
            opened.AddPathNode(start);
            Console.WriteLine("Step 1 completed with " + start.ToString());
            Console.WriteLine("Starting rec");
            FindPath(start, to, opened, closed, field);
            // step last
            // output endpoint
            PathNode end = closed.GetNodeByLocation(to);
            if (end != null)
                Console.WriteLine("End " + end);
            else
                Console.WriteLine("NO WAY!");
            //end.PrintPath();
            return null;
        }

        private static void FindPath(PathNode current, Point goal, PathNodeList opened, PathNodeList closed, Field field)
        {
            Console.WriteLine("FindPath for " + current);
            // move current from opened to closed
            Console.WriteLine("O -> C : ");
            closed.AddPathNode(opened.PullPathNode(current.Location));
            // if we're in goal point - stop recursion
            if (current.Location == goal)
            {
                return;
            }
            // get all childs for current
            List<PathNode> childs = current.CreateChildsHV(goal, field);
            Console.WriteLine("Making childs...");
            foreach(PathNode child in childs)
            {
                if (closed.GetNodeByLocation(child.Location) == null)
                { 
                    // if we have no same point in closed list - add into open list
                    opened.AddPathNode(child);
                    FindPath(child, goal, opened, closed, field);
                }
            }
        }
    }
}
