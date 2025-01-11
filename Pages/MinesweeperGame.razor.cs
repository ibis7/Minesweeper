using Microsoft.AspNetCore.Components;
using Minesweeper.Models;
using System.Timers;

namespace Minesweeper.Pages
{
    public partial class MinesweeperGame
    {
        private System.Timers.Timer? _gameTimer;
        private double _timer = 0;
        private readonly object _timerLock = new();

        public double Timer
        {
            get
            {
                lock (_timerLock)
                {
                    return _timer;
                }
            }
            private set
            {
                lock (_timerLock)
                {
                    _timer = value;
                }
            }
        }

        [Parameter]
        public required Game Game { get; set; }

        protected override void OnParametersSet()
        {
            ResetTimer();
            base.OnParametersSet();
        }

        #region Timer

        public void StartTimer()
        {
            Timer = 0;
            _gameTimer = new System.Timers.Timer(100);
            _gameTimer.Elapsed += OnTimerElapsed;
            _gameTimer.Start();
        }

        private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            Timer += 0.1;
            InvokeAsync(StateHasChanged);
        }

        public void StopTimer()
        {
            if (_gameTimer != null)
            {
                _gameTimer.Stop();
                _gameTimer.Dispose();
                _gameTimer = null;
            }
        }

        public void ResetTimer()
        {
            StopTimer();
            Timer = 0;
        }

        #endregion

        #region Dig

        public void DigCell(Cell cell)
        {
            //if (IsGameOver || IsGameWon) return;

            if (cell.IsRevealed)
            {
                EasyDigging(cell);
            }
            else
            {
                NormalDigging(cell);
            }
        }

        public void NormalDigging(Cell cell)
        {
            if (cell == null || cell.IsRevealed || cell.IsFlagged) return;

            if (!Game.IsBoardStarted)
            {
                Game.StartGame(cell.Row, cell.Column);
                StartTimer();
            }

            var success = cell.Reveal();
            Game.RevealedCells++;

            if (!success)
            {
                StopTimer();
                Game.IsGameOver = true;
                InvokeAsync(StateHasChanged);
                return;
            }

            if (cell.AdjacentMines == 0)
            {
                var surroundingCells = Game.GetSurroundingCells(cell.Row, cell.Column);
                foreach (var surroundingCell in surroundingCells)
                {
                    if (!surroundingCell.IsFlagged && !surroundingCell.IsRevealed)
                    {
                        NormalDigging(surroundingCell);
                    }
                }
            }

            CheckGameWonAfterDig();
            InvokeAsync(StateHasChanged);
        }

        private void EasyDigging(Cell cell)
        {
            if (cell.AdjacentMines > 0)
            {
                var surroundingCells = Game.GetSurroundingCells(cell.Row, cell.Column);
                if (surroundingCells.Count(c => c.IsFlagged) == cell.AdjacentMines)
                {
                    foreach (var surroundingCell in surroundingCells)
                    {
                        if (!surroundingCell.IsFlagged)
                        {
                            NormalDigging(surroundingCell);
                        }
                    }
                }
            }
        }

        public void CheckGameWonAfterDig()
        {
            if (Game.IsGameOver) return;

            if (Game.TotalCells - Game.RevealedCells == Game.Mines)
            {
                WinGame();
            }
        }

        #endregion

        #region Flag

        public void FlagCell(Cell cell)
        {
            //if (IsGameOver || IsGameWon) return;

            if (cell.IsRevealed)
            {
                EasyFlagging(cell);
            }
            else
            {
                ToggleFlag(cell);
            }
        }

        private void ToggleFlag(Cell cell)
        {
            cell.ToggleFlag();

            if (cell.IsFlagged)
            {
                Game.FlaggedCells++;
            }
            else
            {
                Game.FlaggedCells--;
            }

            CheckGameWonAfterFlag();

            InvokeAsync(StateHasChanged);
        }

        private void EasyFlagging(Cell cell)
        {
            if (cell.AdjacentMines > 0)
            {
                var surroundingCells = Game.GetSurroundingCells(cell.Row, cell.Column);
                if (surroundingCells.Count(c => !c.IsRevealed) == cell.AdjacentMines)
                {
                    foreach (var surroundingCell in surroundingCells)
                    {
                        if (!surroundingCell.IsRevealed && !surroundingCell.IsFlagged)
                        {
                            ToggleFlag(surroundingCell);
                        }
                    }
                }
            }
        }

        public void CheckGameWonAfterFlag()
        {
            if (Game.IsGameOver) return;

            if (Game.MinesLeftCounter == 0 && Game.AreAllFlagsPlacedCorrectly())
            {
                WinGame();
            }
        }

        #endregion

        private void WinGame()
        {
            StopTimer();
            Game.IsGameWon = true;
            InvokeAsync(StateHasChanged);
        }
    }
}
