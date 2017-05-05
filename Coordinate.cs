using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ComparisonOfOrderPickingAlgorithms
{
    public class Coordinate
    {
        private int x;

        public int X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
            }
        }

        private int y;

        public int Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
            }
        }

        public Coordinate()
        {
            x = 0;
            y = 0;
        }

        public Coordinate(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
