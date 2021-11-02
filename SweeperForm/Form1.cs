using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.Runtime.InteropServices;

using SweeperForm.Properties;

using SweeperGame;

using static SweeperGame.Utilities;


namespace SweeperForm
{
    public partial class Form1 : Form
    {
        public Point TopLeft;
        public Size CellSize = new Size(16, 16);
        public Game currGame;
        public List<GameHistory> historyList;

        public PictureBox[,] pictureBoxes;

        public Form1()
        {
            InitializeComponent();
            historyList = new List<GameHistory>();
            TopLeft = new Point(12, 12 + menuStrip1.Size.Height);
            InitGame(Game.Easy());
        }

        public void InitGame(Game game)
        {
            this.currGame = game;
            this.currGame.OnMineHit += OnMineHit;
            if (pictureBoxes != null)
            {
                foreach(var pb in pictureBoxes)
                {
                    if (pb.Image != null)
                    {
                        pb.Image.Dispose();
                    }

                    pb.Dispose();
                }
            }

            pictureBoxes = new PictureBox[this.currGame.sizeX, this.currGame.sizeY];

            for (int x = 0; x < this.currGame.sizeX; x++)
            {
                for (int y = 0; y < this.currGame.sizeY; y++)
                {
                    PictureBox newPB = CreateNewCell(x, y);
                    newPB.Click += OnClickCell;
                    Controls.Add(newPB);
                    pictureBoxes[x,y] = newPB;
                }
            }

            Log("Made " + pictureBoxes.Length + " new cells");

            int newSizeX = (TopLeft.X * 2) + (CellSize.Width * (this.currGame.sizeX + 1));
            int newSizeY = (TopLeft.Y * 2) + (CellSize.Height * (this.currGame.sizeY + 1));
            Log("New width is " + (TopLeft.X * 2) + " + " + CellSize.Width + " * " + this.currGame.sizeX + " = " + newSizeX);
            Log("New height is " + (TopLeft.Y * 2) + " + " + CellSize.Height + " * " + this.currGame.sizeY + " = " + newSizeY);
            Size = new Size(newSizeX, newSizeY);
            Refresh();
        }

        private void OnClickCell(object sender, EventArgs eventArgs)
        {
            string name = ((PictureBox)sender).Name;
            int x = int.Parse(name.Substring(0, name.IndexOf(',')));
            int y = int.Parse(name.Substring(name.IndexOf(',') + 1));

            MouseEventArgs e = (MouseEventArgs)eventArgs;
            if (e.Button == MouseButtons.Left)
            {
                currGame.Click(x, y, out ClickResult cr);
            }
            else if (e.Button == MouseButtons.Right) {
                currGame.RightClick(x, y);
            }
            else if (e.Button == MouseButtons.Middle)
            {
                (List<Cell> mines, List<Cell> safe) = Solver.Solve(currGame.grid);
                foreach (Cell cell in mines)
                {
                    if (!cell.Flagged)
                    {
                        currGame.RightClick(cell.x, cell.y);
                    }
                    //pictureBoxes[cell.x, cell.y].Image = Resources.Flagged;
                }
                foreach(Cell cell in safe) 
                {
                    currGame.Click(cell.x, cell.y, out _);
                    //pictureBoxes[cell.x, cell.y].Image = Resources.Question;
                }
                //return;
            }
            
            if (!currGame.Over)
            {
                RedrawCells();
            }
        }

        public void RedrawCells()
        {
            for (int x = 0; x < this.currGame.sizeX; x++)
            {
                for (int y = 0; y < this.currGame.sizeY; y++)
                {
                    if (currGame.grid[x, y].Flagged)
                    {
                        pictureBoxes[x, y].Image = Resources.Flagged;
                    }
                    else if (currGame.grid[x, y].Covered)
                    {
                        pictureBoxes[x, y].Image = Resources.Covered;
                    }
                    else if (!currGame.grid[x, y].Covered && currGame.grid[x, y].MinedNeighbors == 0)
                    {
                        pictureBoxes[x, y].Image = Resources.Empty;
                    }
                    else if (!currGame.grid[x, y].Covered && currGame.grid[x, y].MinedNeighbors != 0)
                    {
                        pictureBoxes[x, y].Image = (Image)Resources.ResourceManager.GetObject("_" + currGame.grid[x, y].MinedNeighbors);
                    }
                }
            }
        }

        public void OnMineHit(int x, int y)
        {
            RevealMines();
            pictureBoxes[x, y].Image = Resources.MineHit;
            historyList.Add(currGame.history);
        }

        public void RevealMines()
        {
            for (int x = 0; x < this.currGame.sizeX; x++)
            {
                for (int y = 0; y < this.currGame.sizeY; y++)
                {
                    if (currGame.grid[x, y].Flagged)
                    {
                        if (currGame.grid[x, y].Mined)
                        {
                            pictureBoxes[x, y].Image = Resources.MineX;
                        }
                        else
                        {
                            pictureBoxes[x, y].Image = Resources.Flagged;
                        }
                    }
                    else if (currGame.grid[x, y].Covered && currGame.grid[x, y].Mined)
                    {
                        pictureBoxes[x, y].Image = Resources.Mine;
                    }
                    else if (!currGame.grid[x, y].Covered && currGame.grid[x, y].MinedNeighbors == 0)
                    {
                        pictureBoxes[x, y].Image = Resources.Empty;
                    }
                    else if (!currGame.grid[x, y].Covered && currGame.grid[x, y].MinedNeighbors != 0)
                    {
                        pictureBoxes[x, y].Image = (Image)Resources.ResourceManager.GetObject("_" + currGame.grid[x, y].MinedNeighbors);
                    }
                }
            }
        }

        public PictureBox CreateNewCell(int x, int y)
        {
            PictureBox newPB = new PictureBox();
            newPB.BackColor = SystemColors.Control;
            newPB.Image = Properties.Resources.Covered;
            newPB.InitialImage = null;

            int locX = TopLeft.X + (x * CellSize.Width);
            int locY = TopLeft.Y + (y * CellSize.Height);

            newPB.Location = new Point(locX, locY);
            newPB.Margin = new Padding(0);
            newPB.Name = x + ", " + y;
            newPB.Size = CellSize;
            newPB.TabIndex = 1;
            newPB.TabStop = false;

            return newPB;
        }

        private void easyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mediumToolStripMenuItem.Checked = false;
            hardToolStripMenuItem.Checked = false;
            InitGame(Game.Easy());
        }

        private void mediumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            easyToolStripMenuItem.Checked = false;
            hardToolStripMenuItem.Checked = false;
            InitGame(Game.Medium());
        }

        private void hardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            easyToolStripMenuItem.Checked = false;
            mediumToolStripMenuItem.Checked = false;
            InitGame(Game.Hard());
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (easyToolStripMenuItem.Checked)
            {
                InitGame(Game.Easy());
            }
            else if (mediumToolStripMenuItem.Checked)
            {
                InitGame(Game.Medium());
            }
            else if (hardToolStripMenuItem.Checked)
            {
                InitGame(Game.Hard());
            }
            else
            {
                LogW("None of the form difficulties were selected!");
            }
        }
    }
}
