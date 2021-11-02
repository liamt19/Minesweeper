using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SweeperGame.Utilities;

namespace SweeperGame
{
    [Serializable]
    public class Game
    {
        public Cell[,] grid;
        private bool[,] updatedCells;
        public int sizeX;
        public int sizeY;
        public int numMines;

        private bool isFirstClick;
        private Random random;
        public List<Cell> mines;
        public bool Over;

        public GameHistory history;

        public Action<int, int> OnMineHit;

        public Game(int sizeX, int sizeY, int numMines)
        {
            Log("Creating new game of size " + sizeX + "x" + sizeY + " with " + numMines + " mines");
            this.sizeX = sizeX;
            this.sizeY = sizeY;
            this.grid = new Cell[sizeX, sizeY];
            updatedCells = new bool[sizeX, sizeY];
            mines = new List<Cell>();
            this.numMines = numMines;
            random = new Random();
            isFirstClick = true;
            Over = false;

            history = new GameHistory();

            InitializeBlank();
        }

        public static Game Easy() => new Game(9, 9, 10);
        public static Game Medium() => new Game(16, 16, 40);
        public static Game Hard() => new Game(30, 16, 99);

        public void SetSeed(int seed)
        {
            random = new Random(seed);
        }

        //  Sets up the grid of cells to be all blank
        public void InitializeBlank()
        {
            for(int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    grid[x, y] = new Cell(x, y);
                }
            }
        }

        public void CheckWin()
        {
            int remCells = sizeX * sizeY;
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    if ((grid[x, y].Mined && grid[x, y].Flagged) || (!grid[x, y].Covered))
                    {
                        remCells--;
                    }
                }
            }

            if (remCells == 0)
            {
                Log("Player won!");
            }
        }

        //  Add or remove a flag from a cell
        public void RightClick(int x, int y)
        {
            if (grid[x,y].Covered)
            {
                grid[x, y].Flagged = !grid[x, y].Flagged;
            }
            CheckWin();
        }

        //  Occurs when the player left clicks a cell
        public void Click(int x, int y, out ClickResult cr)
        {
            cr = new ClickResult(grid[x, y]);

            if (isFirstClick)
            {
                isFirstClick = false;
                cr = FirstClick(x, y);
                history.clicks.Add(cr);
                return;
            }

            //  If you left click a flagged cell, it shouldn't do anything.
            //  Also do nothing if the cell being clicked has already been revealed.
            if (grid[x,y].Flagged)
            {
                Log("Ignoring clicked cell " + grid[x, y].ToString() + " because it is flagged");
                return;
            }
            if (!grid[x, y].Covered)
            {
                Log("Ignoring clicked cell " + grid[x, y].ToString() + " because it is already uncovered");
                return;
            }

            if (grid[x,y].Mined)
            {
                LogW("Cell at " + grid[x, y].ToString() + " had a mine!");
                Over = true;
                OnMineHit.Invoke(x, y);
            }
            else
            {
                //  Safe, reveal cells around this one
                //Log("Cell at " + grid[x, y].ToString() + " was safe");

                updatedCells.Initialize();
                RevealSurroundingCells(x,y);

                history.clicks.Add(cr);
                CheckWin();
            }
        }

        //  Uncovers the cell at x, y if it isn't a mine, and uncovers nearby cells if it doesn't have any mined neighbors.
        public void RevealSurroundingCells(int x, int y, bool IsFirst = false)
        {
            if (grid[x,y].Mined)
            {
                return;
            }

            (int all, int mined) = GetNumber(grid, x, y);
            //  Set this cell's number to display
            grid[x, y].MinedNeighbors = mined;
            grid[x, y].Covered = false;
            updatedCells[x, y] = true;

            //  If this cell is only surrounded by non-mined cells, uncover it and reveal it's neighbors.
            if (mined == 0 || IsFirst)
            {
                List<Cell> neighbors = GetNeighbors(grid, x, y);
                foreach (Cell n in neighbors)
                {
                    if (!updatedCells[n.x, n.y])
                    {
                        RevealSurroundingCells(n.x, n.y);
                    }
                }
            }
        }

        //  Occurs the first time a cell is clicked
        private ClickResult FirstClick(int x, int y)
        {

            int curMines = PlaceMines(x, y);

            history.SetGrid(grid);

            updatedCells.Initialize();
            RevealSurroundingCells(x, y, true);
            Print();
            ClickResult cr = new ClickResult(grid[x,y]);
            for (int i = 0; i < sizeX; i++)
            {
                for (int j = 0; j < sizeY; j++)
                {
                    if (updatedCells[i, j])
                    {
                        cr.revealedCells.Add(grid[i, j]);
                    }
                }
            }

            Log("Done with first click at " + x + ", " + y + ". Made " + curMines + " mines");

            return cr;
        }

        public int PlaceMines(int x, int y)
        {
            int curMines = 0;
            int tries = 0;
            while (curMines < numMines && tries < numMines * 10)
            {
                int mx = random.Next(0, sizeX);
                int my = random.Next(0, sizeY);

                //  First cell can't be a mine
                if (mx == x && my == y)
                {
                    continue;
                }

                if (!grid[mx, my].Mined)
                {
                    //  All cells neighboring the mine that might be created
                    List<Cell> newNeighbors = GetNeighbors(grid, x, y);
                    foreach (Cell cell in newNeighbors)
                    {
                        //  Get the current number of mines around each
                        var (all, mined) = GetNumber(grid, cell.x, cell.y);
                        //  If placing this mine would leave a cell with only mines neighboring it, continue
                        if (mined == all - 1)
                        {
                            LogW("Not mining cell at " + mx + ", " + my + " that would leave cell at " +
                                cell.x + ", " + cell.y + " with " + (mined + 1) + "/" + all + " mined neighbors");
                            continue;
                        }
                    }
                    //  Otherwise make this cell a mine
                    grid[mx, my].Mined = true;
                    mines.Add(grid[mx, my]);
                    curMines++;
                }

                tries++;
                if (tries == numMines * 10)
                {
                    LogE("Only placed " + curMines + " of " + numMines + " after " + tries + " steps!");
                }
            }
            return curMines;
        }

        public void Print()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("  ");
            for (int x = 0; x < sizeX; x++)
            {
                sb.Append(x + " ");
            }
            sb.AppendLine();

            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    if (x == 0)
                    {
                        sb.Append(y + "|");
                    }

                    if (grid[x, y].Mined)
                    {
                        sb.Append("M ");
                        continue;
                    }
                    (_, int mined) = GetNumber(grid, x, y);
                    if (mined > 0)
                    {
                        sb.Append(mined);
                    }
                    else
                    {
                        sb.Append(".");
                    }
                    sb.Append(" ");
                }
                sb.AppendLine();
            }

            Console.WriteLine(sb.ToString());
        }
    }
}
