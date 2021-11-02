using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SweeperGame.Utilities;

namespace SweeperGame
{
    public class Solver
    {
        public static (List<Cell> mines, List<Cell> safe) Solve(Cell[,] grid)
        {
            List <Cell> mines = new List<Cell>();

            int sizeX = grid.GetLength(0);
            int sizeY = grid.GetLength(1);
            Log("Solving for grid size " + sizeX + ", " + sizeY);

            //  First pass determines where mines are
            for(int x = 0; x < sizeX; x++)
            {
                for(int y = 0; y < sizeY; y++)
                {
                    Cell c = grid[x, y];

                    //  If this cell has a number
                    if (!c.Covered && c.MinedNeighbors != 0)
                    {
                        if (c.Mined)
                        {
                            Log("Skipping first pass for mined cell " + c.ToString());
                            continue;
                        }
                            

                        List<Cell> neighbors = GetNeighbors(grid, x, y);
                        (int all, int mined) = GetNumber(neighbors);

                        //  Get each of its covered neighbors
                        foreach(Cell neighbor in neighbors.ToArray())
                        {
                            if (!neighbor.Covered)
                            {
                                neighbors.Remove(neighbor);
                            }
                        }

                        //  If this cell has 2 mined neighbors, and only has 2 covered neighbors, both of those covered cells must be mines
                        if (neighbors.Count == mined)
                        {
                            //Log("Cell at " + c.ToString() + " has " + neighbors.Count + " covered and " + mined + " mined neighbors!");
                            foreach (Cell neighbor in neighbors.ToArray())
                            {
                                if (!mines.Contains(neighbor))
                                {
                                    mines.Add(neighbor);
                                }
                            }
                        }

                    }
                }
            }

            List<Cell> safe = new List<Cell>();
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    Cell c = grid[x, y];

                    if (!c.Covered && c.MinedNeighbors != 0)
                    {
                        List<Cell> neighbors = GetNeighbors(grid, x, y);
                        int removals = 0;
                        foreach(Cell neighbor in neighbors.ToArray())
                        {
                            if (mines.Contains(neighbor))
                            {
                                neighbors.Remove(neighbor);
                                removals++;
                            }
                        }

                        neighbors.RemoveAll(a => !a.Covered);

                        //  If this cell has more neighbors than it does mined neighbors,
                        //  and all of its mined neighbors are known to be mines,
                        //  then all other cells surrounding it can be clicked.
                        if (removals == c.MinedNeighbors)
                        {
                            foreach (Cell neighbor in neighbors)
                            {
                                //  This cell must be safe
                                if (!safe.Contains(neighbor))
                                {
                                    safe.Add(neighbor);
                                }
                            }
                        }
                    }
                }
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("Solver determined that these cells are mines: ");
            foreach(Cell c in mines)
            {
                sb.Append(c.ToString() + ", ");
            }

            sb.Append("\nThese cells are safe: ");

            foreach(Cell c in safe)
            {
                sb.Append(c.ToString() + ", ");
            }

            Log(sb.ToString().TrimEnd(','));

            return (mines, safe);
        }
    }
}
