using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SweeperGame
{
    //  Represents the result of a mine being clicked.
    [Serializable]
    public class ClickResult
    {
        public Cell clickedCell;
        //  The list of any cells that were revealed due to the click
        public List<Cell> revealedCells;
        //  If the cell that was clicked was a mine
        public bool ClickedMine;

        public ClickResult(Cell clickedCell)
        {
            this.clickedCell = clickedCell;
            revealedCells = new List<Cell>();
            ClickedMine = clickedCell.Mined;
        }
    }
}
