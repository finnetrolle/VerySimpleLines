using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AStar
{
    class PathNodeList
    {
        private List<PathNode> list = new List<PathNode>();
        /// <summary>
        /// Method inserts node into list
        /// If there are node with same Location in List, method select best f-value node
        /// </summary>
        /// <param name="pathNode">node</param>
        /// <returns>true if node is inserted</returns>
        public bool AddPathNode(PathNode pathNode)
        {
            PathNode contender = GetNodeByLocation(pathNode.Location);
            if (contender == null)
            {
                // just add
                list.Add(pathNode);
                return true;
            }
            else
            {
                if (contender.GValue >= pathNode.GValue)
                {
                    list.Remove(contender);
                    list.Add(pathNode);
                    return true;
                }
                else
                {
                    pathNode.Parent = contender.Parent;
                    return false;
                }
            }
        }

        public PathNode PullPathNode(Point location)
        {
            PathNode pulled = GetNodeByLocation(location);
            if (pulled != null)
            {
                list.Remove(pulled);
                return pulled;
            }
            return null;
        }

        public PathNode GetNodeByLocation(Point location)
        {
            foreach (PathNode n in list)
            {
                if (location == n.Location)
                    return n;
            }
            return null;
        }

        public PathNode GetNodeWithBestFValue()
        {
            if (list.Count == 0)
                return null;

            PathNode result = list[0];
            foreach (PathNode node in list)
            {
                if (result.FValue >= node.FValue)
                {
                    result = node;
                }
            }
            return result;
        }
    }
}
