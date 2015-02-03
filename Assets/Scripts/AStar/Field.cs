using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AStar
{
    class Field
    {
        private int[,] fields;
        public int XSize { get; private set; }
        public int YSize { get; private set; }

        private List<int> walkableValues = new List<int>();

        public void AddWalkableValue(int value)
        {
            walkableValues.Add(value);
        }

        public Field(int[,] fields, int defaultWalkableValue = 0)
        {
            XSize = fields.GetUpperBound(0) + 1;
            YSize = fields.GetUpperBound(1) + 1;
            this.fields = fields;
            AddWalkableValue(defaultWalkableValue);
            //Console.WriteLine("XSize = " + XSize + "; YSize = " + YSize);
        }

        public bool IsPointInside(Point point)
        {
            if ((point.X >= 0) && (point.X < XSize) && (point.Y >= 0) && (point.Y < YSize))
                return true;
            return false;
        }

        public int GetFieldValue(Point position)
        {
            if (IsPointInside(position))
            {
                return fields[position.X, position.Y];
            }
            else
                return int.MinValue;
        }

        public int GetFieldValue(int x, int y)
        {
            Point p = new Point(x, y);
            return GetFieldValue(p);
        }

        public bool IsWalkable(Point position)
        {
            int value = GetFieldValue(position);
            if (value != int.MinValue)
            {
                if (walkableValues.IndexOf(value) == -1)
                {
                    return false; // value is not in list
                }
                else
                {
                    return true;
                }
            }
            return false; // field outside of field is really not walkable
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("  ");
            for (int x = 0; x < XSize; ++x)
            {
                sb.Append(x);
            }
            sb.AppendLine().AppendLine();
            for (int y = 0; y < YSize; ++y)
            {
                sb.Append(y + " ");
                for (int x = 0; x < XSize; ++x)
                {
                    sb.Append((IsWalkable(new Point(x, y))) ? " " : "#");
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

    }
}
