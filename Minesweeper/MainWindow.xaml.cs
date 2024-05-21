using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Minesweeper
{
    public partial class MainWindow : Window
    {
        private const int Rows = 10;
        private const int Columns = 10;
        private const int MinesCount = 10;

        private Button[,] buttons;
        private bool[,] mines;
        private bool[,] revealed;

        private DispatcherTimer timer;
        private int elapsedTime;

        public MainWindow()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            buttons = new Button[Rows, Columns];
            mines = new bool[Rows, Columns];
            revealed = new bool[Rows, Columns];

            GameGrid.Children.Clear();
            GameGrid.RowDefinitions.Clear();
            GameGrid.ColumnDefinitions.Clear();

            for (int row = 0; row < Rows; row++)
            {
                GameGrid.RowDefinitions.Add(new RowDefinition());
            }
            for (int col = 0; col < Columns; col++)
            {
                GameGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    var button = new Button();
                    button.Click += Button_Click;
                    button.MouseRightButtonDown += Button_RightClick;
                    Grid.SetRow(button, row);
                    Grid.SetColumn(button, col);
                    GameGrid.Children.Add(button);
                    buttons[row, col] = button;
                }
            }

            PlaceMines();
            StartTimer();
        }

        private void StartTimer()
        {
            if (timer != null)
            {
                timer.Stop();
            }

            elapsedTime = 0;
            TimerTextBlock.Text = elapsedTime.ToString();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            elapsedTime++;
            TimerTextBlock.Text = elapsedTime.ToString();
        }

        private void PlaceMines()
        {
            Random random = new Random();
            int placedMines = 0;
            while (placedMines < MinesCount)
            {
                int row = random.Next(Rows);
                int col = random.Next(Columns);
                if (!mines[row, col])
                {
                    mines[row, col] = true;
                    placedMines++;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            int row = Grid.GetRow(button);
            int col = Grid.GetColumn(button);

            if (button.Content!=null && button.Content.ToString() == "🚩") return;
            if (mines[row, col])
            {
                button.Content = "💣";
                button.Background = Brushes.Red;
                timer.Stop();
                MessageBox.Show("Game Over!");
                InitializeGame();
            }
            else
            {
                RevealCell(row, col);
            }
        }

        private void Button_RightClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Button button = (Button)sender;
            if (button.Content == null || button.Content.ToString() == "")
            {
                button.Content = "🚩";
                
            }
            else if (button.Content.ToString() == "🚩")
            {
                
                button.Content = "";
                
            }
        }

        private void RevealCell(int row, int col)
        {
            if (row < 0 || row >= Rows || col < 0 || col >= Columns || revealed[row, col])
                return;

            revealed[row, col] = true;
            int mineCount = CountAdjacentMines(row, col);

            Button button = buttons[row, col];
            button.Content = mineCount > 0 ? mineCount.ToString() : "";
            button.Background = Brushes.Green;

            if (mineCount == 0)
            {
                RevealCell(row - 1, col);
                RevealCell(row + 1, col);
                RevealCell(row, col - 1);
                RevealCell(row, col + 1);
                RevealCell(row - 1, col - 1);
                RevealCell(row - 1, col + 1);
                RevealCell(row + 1, col - 1);
                RevealCell(row + 1, col + 1);
            }
        }

        private int CountAdjacentMines(int row, int col)
        {
            int count = 0;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int r = row + i;
                    int c = col + j;
                    if (r >= 0 && r < Rows && c >= 0 && c < Columns && mines[r, c])
                    {
                        count++;
                    }
                }
            }
            return count;
        }
    }
}
