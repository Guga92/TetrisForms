using System;

namespace TetrisForms
{
    namespace TetrisForms
    {
        public class Tetromino
        {
            public int[,] shape { get; private set; }
            public int X { get; set; }
            public int Y { get; set; }

            public Tetromino(int[,] shape)
            {
                this.shape = (int[,])shape.Clone();
                X = 0;
                Y = 0;
            }

            public Tetromino(Tetromino other)
            {
                shape = (int[,])other.shape.Clone();
                X = other.X;
                Y = other.Y;
            }

            public void Rotate()
            {
                int rows = shape.GetLength(0);
                int cols = shape.GetLength(1);
                int[,] rotated = new int[cols, rows];

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        rotated[j, rows - i - 1] = shape[i, j];
                    }
                }

                shape = rotated;
            }
        }
    }

}
