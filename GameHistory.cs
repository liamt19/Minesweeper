using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SweeperGame
{
    [Serializable]
    public class GameHistory
    {
        public List<ClickResult> clicks;
        public Cell[,] initialGrid;

        public GameHistory()
        {
            clicks = new List<ClickResult>();
        }

        public void SetGrid(Cell[,] grid)
        {
            initialGrid = grid.DeepClone();
        }
    }
}
