﻿namespace Minesweeper.Models
{
    public class Cell(int row, int column)
    {
        public int Row { get; set; } = row;
        public int Column { get; set; } = column;

        public bool? IsMine { get; set; }
        public int? AdjacentMines { get; set; } = null;

        public bool IsRevealed { get; set; } = false;
        public bool IsFlagged { get; set; } = false;

        public void ToggleFlag()
        {
            if (IsRevealed) return;
            IsFlagged = !IsFlagged;
        }

        public void Flag()
        {
            if (IsRevealed) return;
            IsFlagged = true;
        }

        public bool Reveal()
        {
            IsRevealed = true;
            return !(IsMine ?? false);
        }
    }
}