using Microsoft.AspNetCore.Components;
using Minesweeper.Models;

namespace Minesweeper.Pages
{
    public partial class MinesweeperGame
    {
        private System.Timers.Timer? _gameTimer;

        public int Timer { get; set; } = 0;

        [Parameter]
        public required Game Game { get; set; }

        private void StartTimer()
        {
            Timer = 0;
            _gameTimer = new System.Timers.Timer(10);
            _gameTimer.Elapsed += (sender, e) =>
            {
                Timer++;
                StateHasChanged();
            };
            _gameTimer.Start();
        }

        private void StopTimer()
        {
            if (_gameTimer != null)
            {
                _gameTimer.Stop();
                _gameTimer.Dispose();
                _gameTimer = null;
            }
        }

        public void RevealCell(Cell cell)
        {
            //if (IsGameOver || IsGameWon) return;

            if (cell == null || cell.IsRevealed || cell.IsFlagged) return;

            if (!Game.IsBoardStarted)
            {
                Game.CreateGame(cell.Row, cell.Column);
                StartTimer();
            }

            var success = cell.Reveal();

            Game.RevealedCells++;

            if (!success)
            {
                Game.IsGameOver = true;
                StopTimer();
            }

            if (cell.AdjacentMines == 0)
            {
                var surroundingCells = Game.GetSurroundingCells(cell.Row, cell.Column);
                foreach (var c in surroundingCells)
                {
                    RevealCell(c);
                }
            }

            CheckGameWon();
            StateHasChanged();
        }

        public void FlagCell(Cell cell)
        {
            //if (IsGameOver || IsGameWon) return;

            cell.ToggleFlag();
            StateHasChanged();
        }

        public void CheckGameWon()
        {
            if (Game.Board == null)
            {
                throw new InvalidOperationException("Board is not initialized");
            }

            if (Game.IsGameOver) return;

            var unrevealedCells = Game.TotalCells - Game.RevealedCells;

            if (unrevealedCells == Game.Mines)
            {
                Game.IsGameWon = true;
                StopTimer();
            }
        }
    }
}
