using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SweeperGame
{
    [Serializable]
    public class Cell
    {
        public int x;
        public int y;
        public bool Covered = true;
        public bool Mined = false;
        public bool Flagged = false;
        public int MinedNeighbors;
        public bool HasNumber => (!Covered && MinedNeighbors != 0);

        public Cell(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return "[" + x + ", " + y + "]";
        }
    }
}
