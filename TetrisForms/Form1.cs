using System.Drawing;
using System.Windows.Forms;

namespace TetrisForms
{
    public partial class Form1 : Form
    {
        private GameField gameField;

        public Form1()
        {
            InitializeComponent();

            gameField = new GameField();
            gameField.Location = new Point(10, 10);

            Controls.Add(gameField);

            KeyDown += Form1_KeyDown;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            gameField.HandleInput(e.KeyCode);
        }
    }
}
