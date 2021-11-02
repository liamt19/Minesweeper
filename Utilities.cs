using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SweeperGame
{
    public static class Utilities
    {
        public static void Log(string s)
        {
            Console.WriteLine(s);
        }

        public static void LogW(string s)
        {
            Console.WriteLine("WARN: \t" + s);
        }

        public static void LogE(string s)
        {
            Console.WriteLine("ERROR:\t" + s);
        }

        public static void Print(List<Cell> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                Cell cell = list[i];
                Console.Write(cell.ToString() + (i != list.Count - 1 ? ", " : String.Empty));
            }
            Console.WriteLine();
        }

        //  Returns a list of the cell's neighbors
        public static List<Cell> GetNeighbors(Cell[,] grid, int x, int y)
        {
            List<Cell> list = new List<Cell>();

            int sizeX = grid.GetLength(0);
            int sizeY = grid.GetLength(1);

            if (x > 0)
            {
                if (y < sizeY - 1)
                {
                    list.Add(grid[x - 1, y + 1]);
                }

                list.Add(grid[x - 1, y]);

                if (y > 0)
                {
                    list.Add(grid[x - 1, y - 1]);
                }
            }

            if (y < sizeY - 1)
            {
                list.Add(grid[x, y + 1]);
            }

            if (y > 0)
            {
                list.Add(grid[x, y - 1]);
            }

            if (x < sizeX - 1)
            {
                if (y < sizeY - 1)
                {
                    list.Add(grid[x + 1, y + 1]);
                }

                list.Add(grid[x + 1, y]);

                if (y > 0)
                {
                    list.Add(grid[x + 1, y - 1]);
                }
            }

            return list;
        }

        public static List<(int x, int y)> GetNeighbors(bool[,] grid, int x, int y)
        {
            List<(int, int)> list = new List<(int, int)>();

            int sizeX = grid.GetLength(0);
            int sizeY = grid.GetLength(1);

            if (x > 0)
            {
                if (y < sizeY - 1)
                {
                    list.Add((x - 1, y + 1));
                }

                list.Add((x - 1, y));

                if (y > 0)
                {
                    list.Add((x - 1, y - 1));
                }
            }

            if (y < sizeY - 1)
            {
                list.Add((x, y + 1));
            }

            if (y > 0)
            {
                list.Add((x, y - 1));
            }

            if (x < sizeX - 1)
            {
                if (y < sizeY - 1)
                {
                    list.Add((x + 1, y + 1));
                }

                list.Add((x + 1, y));

                if (y > 0)
                {
                    list.Add((x + 1, y - 1));
                }
            }

            return list;
        }

        // Returns the total number of neighbors of a cell, and the number of mines surrounding it
        public static (int All, int Mined) GetNumber(Cell[,] grid, int x, int y) => GetNumber(GetNeighbors(grid, x, y));

        public static (int All, int Mined) GetNumber(List<Cell> list)
        {
            int n = 0;
            foreach (Cell cell in list)
            {
                if (cell.Mined)
                {
                    n++;
                }
            }

            return (list.Count, n);
        }

        public static T DeepClone<T>(this T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }
    }
}
