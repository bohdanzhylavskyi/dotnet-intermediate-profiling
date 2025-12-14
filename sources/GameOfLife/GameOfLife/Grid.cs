using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GameOfLife
{
    class Grid
    {

        private int SizeX;
        private int SizeY;
        private Cell[,] cells;
        private static Random rnd;
        private Canvas drawCanvas;

        
        public Grid(Canvas c)
        {
            drawCanvas = c;
            rnd = new Random();
            SizeX = (int) (c.Width / 5);
            SizeY = (int)(c.Height / 5);
            cells = new Cell[SizeX, SizeY];
 
            for (int i = 0; i < SizeX; i++)
                for (int j = 0; j < SizeY; j++)
                {
                    cells[i, j] = new Cell(i, j, 0, false);
                }

            SetRandomPattern();
            InitCellsVisuals();
            UpdateGraphics();
            SetupEventHandlers();  
        }


        public void Clear()
        {
            for (int i = 0; i < SizeX; i++)
                for (int j = 0; j < SizeY; j++)
                {
                    var cell = cells[i, j];

                    cell.Age = 0;
                    cell.IsAlive = false;
                    cell.Visual.Fill = Brushes.Gray;
                }
        }


        void MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            var pos = e.GetPosition(drawCanvas);

            int i = (int)(pos.X / 5);
            int j = (int)(pos.Y / 5);

            if (i < 0 || i >= SizeX || j < 0 || j >= SizeY)
                return;

            var cell = cells[i, j];
            
            if (!cell.IsAlive)
            {
                cell.IsAlive = true;
                cell.Age = 0;
                cell.Visual.Fill = Brushes.White;
            }
        }


        public void UpdateGraphics()
        {
            for (int i = 0; i < SizeX; i++) {
                for (int j = 0; j < SizeY; j++) {
                    var cell = cells[i, j];

                    cell.Visual.Fill = cell.IsAlive
                                            ? (cell.Age < 2 ? Brushes.White : Brushes.DarkGray)
                                            : Brushes.Gray;
                }
            }
                    
        }

        public void InitCellsVisuals()
        {
            for (int i = 0; i < SizeX; i++)
                for (int j = 0; j < SizeY; j++)
                {
                    var cell = cells[i, j];
                    var cellLeft = cells[i, j].PositionX;
                    var cellTop = cells[i, j].PositionY;

                    var ellipse = new Ellipse();

                    ellipse.Width = ellipse.Height = 5;
                    ellipse.Margin = new Thickness(cellLeft, cellTop, 0, 0);
                    ellipse.Fill = Brushes.Gray;

                    drawCanvas.Children.Add(ellipse);

                    cells[i, j].Visual = ellipse;
                }

            UpdateGraphics();
        }
        

        public static bool GetRandomBoolean()
        {
            return rnd.NextDouble() > 0.8;
        }

        public void SetRandomPattern()
        {
            for (int i = 0; i < SizeX; i++)
                for (int j = 0; j < SizeY; j++)
                    cells[i, j].IsAlive = GetRandomBoolean();
        }
        
        public void UpdateToNextGeneration()
        {
            for (int i = 0; i < SizeX; i++)
                for (int j = 0; j < SizeY; j++)
                {
                    bool alive = false;
                    int age = 0;

                    CalculateNextGeneration(i, j, ref alive, ref age);


                    cells[i, j].IsAlive = alive;
                    cells[i, j].Age = age;
                }

            UpdateGraphics();
        }
        

        public void Update()
        {
            
            UpdateToNextGeneration();
        }
        public void CalculateNextGeneration(int row, int column, ref bool isAlive, ref int age)     // OPTIMIZED
        {
            isAlive = cells[row, column].IsAlive;
            age = cells[row, column].Age;

            int count = CountNeighbors(row, column);

            if (isAlive && count < 2)
            {
                isAlive = false;
                age = 0;
            }

            if (isAlive && (count == 2 || count == 3))
            {
                cells[row, column].Age++;
                isAlive = true;
                age = cells[row, column].Age;
            }

            if (isAlive && count > 3)
            {
                isAlive = false;
                age = 0;
            }

            if (!isAlive && count == 3)
            {
                isAlive = true;
                age = 0;
            }
        }

        public int CountNeighbors(int i, int j)
        {
            int count = 0;

            if (i != SizeX - 1 && cells[i + 1, j].IsAlive) count++;
            if (i != SizeX - 1 && j != SizeY - 1 && cells[i + 1, j + 1].IsAlive) count++;
            if (j != SizeY - 1 && cells[i, j + 1].IsAlive) count++;
            if (i != 0 && j != SizeY - 1 && cells[i - 1, j + 1].IsAlive) count++;
            if (i != 0 && cells[i - 1, j].IsAlive) count++;
            if (i != 0 && j != 0 && cells[i - 1, j - 1].IsAlive) count++;
            if (j != 0 && cells[i, j - 1].IsAlive) count++;
            if (i != SizeX - 1 && j != 0 && cells[i + 1, j - 1].IsAlive) count++;

            return count;
        }

        void SetupEventHandlers()
        {
            drawCanvas.MouseMove += MouseMove;
            drawCanvas.MouseLeftButtonDown += MouseMove;
        }
    }
}