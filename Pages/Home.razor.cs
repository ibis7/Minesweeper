using Minesweeper.Models;

namespace Minesweeper.Pages
{
    public partial class Home
    {
        public Game? Game { get; set; } = null;
        public int SelectedRows { get; set; } = 16;
        public int SelectedColumns { get; set; } = 16;
        public int SelectedMines { get; set; } = 40;

        public void StartGame()
        {
            Game = new Game(SelectedRows, SelectedColumns, SelectedMines);
        }
    }
}
