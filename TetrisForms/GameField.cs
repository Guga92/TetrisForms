using System;
using System.Drawing;
using System.Windows.Forms;
using TetrisForms.TetrisForms;

namespace TetrisForms
{
    public class GameField : Panel
    {
        private const int _blockSize = 30;
        private const int _rows = 20;
        private const int _cols = 10;

        private int[,] _field;

        private Tetromino _currentTetromino;
        private Timer _gameTimer;

        public GameField()
        {
            DoubleBuffered = true;
            Size = new Size(_cols * _blockSize, _rows * _blockSize);
            _field = new int[_rows, _cols];
            _currentTetromino = GenerateRandomTetromino();

            _gameTimer = new Timer();
            _gameTimer.Interval = 200;
            _gameTimer.Tick += GameTimer_Tick;
            _gameTimer.Start();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            MoveTetrominoDown();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Draw(e.Graphics);
        }

        private void Draw(Graphics g)
        {
            g.Clear(Color.Black);

            for (int y = 0; y < _rows; y++)
            {
                for (int x = 0; x < _cols; x++)
                {
                    if (_field[y, x] != 0)
                    {
                        Brush brush = _field[y, x] == 1 ? Brushes.Blue : Brushes.Red;
                        g.FillRectangle(brush, x * _blockSize, y * _blockSize, _blockSize, _blockSize);
                        g.DrawRectangle(Pens.Black, x * _blockSize, y * _blockSize, _blockSize, _blockSize);
                    }
                }
            }

            for (int y = 0; y < _currentTetromino.shape.GetLength(0); y++)
            {
                for (int x = 0; x < _currentTetromino.shape.GetLength(1); x++)
                {
                    if (_currentTetromino.shape[y, x] != 0)
                    {
                        Brush brush = Brushes.Yellow;
                        g.FillRectangle(brush, (_currentTetromino.X + x) * _blockSize, (_currentTetromino.Y + y) * _blockSize, _blockSize, _blockSize);
                        g.DrawRectangle(Pens.Black, (_currentTetromino.X + x) * _blockSize, (_currentTetromino.Y + y) * _blockSize, _blockSize, _blockSize);
                    }
                }
            }
        }

        public void MoveTetrominoDown()
        {
            if (CanPlaceTetromino(_currentTetromino, _currentTetromino.X, _currentTetromino.Y + 1))
            {
                _currentTetromino.Y++;
                Invalidate();
            }
            else
            {
                PlaceTetromino(_currentTetromino);
                ClearLines();

                _currentTetromino = GenerateRandomTetromino();

                if (!CanPlaceTetromino(_currentTetromino, _currentTetromino.X, _currentTetromino.Y))
                {
                    _gameTimer.Stop();
                    MessageBox.Show("Game Over! Press OK to restart.");
                    StartNewGame();
                }
            }
        }

        public bool CanPlaceTetromino(Tetromino tetromino, int xOffset, int yOffset)
        {
            for (int y = 0; y < tetromino.shape.GetLength(0); y++)
            {
                for (int x = 0; x < tetromino.shape.GetLength(1); x++)
                {
                    if (tetromino.shape[y, x] != 0)
                    {
                        int newX = x + xOffset;
                        int newY = y + yOffset;

                        if (newX < 0 || newX >= _cols || newY < 0 || newY >= _rows || _field[newY, newX] != 0)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public void PlaceTetromino(Tetromino tetromino)
        {
            for (int y = 0; y < tetromino.shape.GetLength(0); y++)
            {
                for (int x = 0; x < tetromino.shape.GetLength(1); x++)
                {
                    if (tetromino.shape[y, x] != 0)
                    {
                        _field[tetromino.Y + y, tetromino.X + x] = tetromino.shape[y, x];
                    }
                }
            }
        }

        public void ClearLines()
        {
            for (int y = _rows - 1; y >= 0; y--)
            {
                bool isLineFull = true;

                for (int x = 0; x < _cols; x++)
                {
                    if (_field[y, x] == 0)
                    {
                        isLineFull = false;
                        break;
                    }
                }

                if (isLineFull)
                {
                    for (int yy = y; yy > 0; yy--)
                    {
                        for (int xx = 0; xx < _cols; xx++)
                        {
                            _field[yy, xx] = _field[yy - 1, xx];
                        }
                    }

                    for (int xx = 0; xx < _cols; xx++)
                    {
                        _field[0, xx] = 0;
                    }

                    y++;
                }
            }
        }

        private Tetromino GenerateRandomTetromino()
        {
            int[][,] shapes = new int[][,]
            {
            new int[,] {{1, 1, 1, 1}}, // I
            new int[,] {{1, 1}, {1, 1}}, // O
            new int[,] {{0, 1, 0}, {1, 1, 1}}, // T
            new int[,] {{1, 0, 0}, {1, 1, 1}}, // L
            new int[,] {{0, 0, 1}, {1, 1, 1}}, // J
            new int[,] {{1, 1, 0}, {0, 1, 1}}, // S
            new int[,] {{0, 1, 1}, {1, 1, 0}} // Z
            };

            Random random = new Random();
            int index = random.Next(shapes.Length);
            return new Tetromino(shapes[index]);
        }

        public void StartNewGame()
        {
            _field = new int[_rows, _cols];
            _currentTetromino = GenerateRandomTetromino();
            _gameTimer.Start();
        }

        public void HandleInput(Keys key)
        {
            switch (key)
            {
                case Keys.Left:
                    if (CanPlaceTetromino(_currentTetromino, _currentTetromino.X - 1, _currentTetromino.Y))
                    {
                        _currentTetromino.X--;
                        Invalidate();
                    }
                    break;

                case Keys.Right:
                    if (CanPlaceTetromino(_currentTetromino, _currentTetromino.X + 1, _currentTetromino.Y))
                    {
                        _currentTetromino.X++;
                        Invalidate();
                    }
                    break;

                case Keys.Up:
                    Tetromino rotatedTetromino = new Tetromino(_currentTetromino);
                    rotatedTetromino.Rotate();

                    if (CanPlaceTetromino(rotatedTetromino, _currentTetromino.X, _currentTetromino.Y))
                    {
                        _currentTetromino = rotatedTetromino;
                        Invalidate();
                    }

                    break;

                case Keys.Down:
                    MoveTetrominoDown();
                    break;
            }
        }

    }
}
