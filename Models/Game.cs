namespace Minesweeper.Models
{
    public class Game
    {
        private const int _maxRows = 40;
        private const int _maxColumns = 40;

        public int Rows { get; set; }
        public int Columns { get; set; }
        public int Mines { get; set; }

        public bool IsGameOver { get; set; }
        public bool IsGameWon { get; set; }

        public Cell[,]? Board { get; set; } = null;
        public bool IsBoardStarted { get; set; } = false;

        public int RevealedCells { get; set; } = 0;
        public int FlaggedCells { get; set; } = 0;

        public int TotalCells => Rows * Columns;
        public int MinesLeftCounter => Mines - FlaggedCells;

        #region CreateBoard
        public Game(int rows, int columns, int mines)
        {
            Rows = rows;
            Columns = columns;
            Mines = mines;

            if (!IsBoardValid())
            {
                throw new ArgumentException("Invalid board configuration");
            }

            CreateBoard();
        }


        private bool IsBoardValid()
        {
            return Rows > 0 && Columns > 0
                && Rows <= _maxRows && Columns <= _maxColumns
                && Mines > 0 && Mines <= TotalCells - 9;
        }

        private void CreateBoard()
        {
            Board = new Cell[Rows, Columns];
            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; column++)
                {
                    Board[row, column] = new Cell(row, column);
                }
            }
        }

        #endregion

        #region StartGame

        public void StartGame(int initialRow, int initialColumn)
        {
            if (Board == null)
            {
                throw new InvalidOperationException("Board is not initialized");
            }

            DesignateSafeClickZone(initialRow, initialColumn);
            PopulateMines();
            CalculateAdjacentMines();
            IsBoardStarted = true;
        }

        private void DesignateSafeClickZone(int initialRow, int initialColumn)
        {
            var surroundingCells = GetSurroundingCells(initialRow, initialColumn);

            foreach (var surroundingCell in surroundingCells)
            {
                surroundingCell.IsMine = false;
            }

            var cell = GetCell(initialRow, initialColumn);

            cell!.IsMine = false;
            cell.AdjacentMines = 0;
        }

        private void PopulateMines()
        {
            var minePositions = new HashSet<(int, int)>();

            while (minePositions.Count < Mines)
            {
                var minePosition = GetRandomMinePosition();

                if (minePositions.Contains((minePosition.Item1, minePosition.Item2))) continue;

                minePositions.Add(minePosition);
            }

            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; column++)
                {
                    var isMine = minePositions.Contains((row, column));

                    Board![row, column].IsMine = isMine;
                }
            }
        }

        private (int, int) GetRandomMinePosition()
        {
            var random = new Random();

            var position = (random.Next(Rows), random.Next(Columns));

            var cell = GetCell(position.Item1, position.Item2);
            if (cell == null) return GetRandomMinePosition();
            if (cell.IsMine.HasValue) return GetRandomMinePosition();

            return position;
        }

        private void CalculateAdjacentMines()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; column++)
                {
                    var cell = Board![row, column];

                    if (cell.IsMine == true) continue;

                    var surroundingCells = GetSurroundingCells(row, column);

                    cell.AdjacentMines = surroundingCells.Count(c => c.IsMine == true);
                }
            }
        }

        #endregion

        public Cell? GetCell(int row, int column)
        {
            if (column < 0 || row < 0 || column >= Columns || row >= Rows) return null;

            return Board![row, column];
        }

        public List<Cell> GetSurroundingCells(int row, int column)
        {
            var list = new List<Cell>(8);

            for (int r = row - 1; r <= row + 1; r++)
            {
                for (int c = column - 1; c <= column + 1; c++)
                {
                    if (r == row && c == column) continue; // Skip the center cell

                    var cell = GetCell(r, c);
                    if (cell != null)
                    {
                        list.Add(cell);
                    }
                }
            }

            return list;
        }

        public bool AreAllFlagsPlacedCorrectly()
        {
            if (MinesLeftCounter != 0) return false;

            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; column++)
                {
                    var cell = Board![row, column];
                    if (cell.IsFlagged && cell.IsMine == false)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }

}
