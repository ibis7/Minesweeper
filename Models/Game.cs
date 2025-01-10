namespace Minesweeper.Models
{
    public class Game
    {
        public int Rows { get; set; }
        public int Columns { get; set; }
        public int Mines { get; set; }

        public bool IsGameOver { get; set; }
        public bool IsGameWon { get; set; }

        public Cell[][]? Board { get; set; } = null;
        public bool IsBoardStarted { get; set; } = false;

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

        #region CreateGame

        private bool IsBoardValid()
        {
            return Rows > 0 && Columns > 0 && Mines > 0 && Mines <= (Rows * Columns) - 9;
        }

        private void CreateBoard()
        {
            Board = new Cell[Rows][];
            for (int row = 0; row < Rows; row++)
            {
                Board[row] = new Cell[Columns];
                for (int column = 0; column < Columns; column++)
                {
                    Board[row][column] = new Cell(row, column);
                }
            }
        }

        private void CreateGame(int initialRow, int initialColumn)
        {
            DesignateSafeClickZone(initialRow, initialColumn);
            PopulateMines();
            CalculateAdjacentMines();
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
            if (Board == null)
            {
                throw new InvalidOperationException("Board is not initialized");
            }

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

                    Board[row][column].IsMine = isMine;
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
            if (Board == null)
            {
                throw new InvalidOperationException("Board is not initialized");
            }

            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; column++)
                {
                    var cell = Board[row][column];

                    if (cell.IsMine == true) continue;

                    var surroundingCells = GetSurroundingCells(row, column);

                    cell.AdjacentMines = surroundingCells.Count(c => c.IsMine == true);
                }
            }
        }

        private void CheckGameWon()
        {
            if (Board == null)
            {
                throw new InvalidOperationException("Board is not initialized");
            }

            if (IsGameOver) return;

            var totalCells = Rows * Columns;
            var unrevealedCells = Board.SelectMany(c => c).Count(c => !c.IsRevealed);

            if (unrevealedCells == Mines || unrevealedCells == totalCells - Mines)
            {
                IsGameWon = true;
            }
        }

        #endregion

        public Cell? GetCell(int row, int column)
        {
            if (Board == null)
            {
                throw new InvalidOperationException("Board is not initialized");
            }

            if (column < 0 || row < 0 || column >= Columns || row >= Rows) return null;

            return Board[row][column];
        }

        public List<Cell> GetSurroundingCells(int row, int column)
        {
            var list = new List<Cell>();

            void AddCellIfNotNull(int r, int c)
            {
                var cell = GetCell(r, c);
                if (cell != null)
                {
                    list.Add(cell);
                }
            }

            AddCellIfNotNull(row - 1, column - 1);
            AddCellIfNotNull(row - 1, column);
            AddCellIfNotNull(row - 1, column + 1);

            AddCellIfNotNull(row, column - 1);
            AddCellIfNotNull(row, column + 1);

            AddCellIfNotNull(row + 1, column - 1);
            AddCellIfNotNull(row + 1, column);
            AddCellIfNotNull(row + 1, column + 1);

            return list;
        }

        public void RevealCell(Cell cell)
        {
            if (IsGameOver || IsGameWon) return;

            if (cell == null || cell.IsRevealed || cell.IsFlagged) return;

            if (!IsBoardStarted)
            {
                CreateGame(cell.Row, cell.Column);
                IsBoardStarted = true;
            }

            var success = cell.Reveal();

            if (!success)
            {
                IsGameOver = true;
            }

            if (cell.AdjacentMines == 0)
            {
                var surroundingCells = GetSurroundingCells(cell.Row, cell.Column);
                foreach (var c in surroundingCells)
                {
                    RevealCell(c);
                }
            }

            CheckGameWon();
        }

        public void RevealCell(int row, int column)
        {
            if (IsGameOver || IsGameWon) return;

            if (Board == null)
            {
                throw new InvalidOperationException("Board is not initialized");
            }

            var cell = GetCell(row, column);

            if (cell == null || cell.IsRevealed || cell.IsFlagged) return;

            if (!IsBoardStarted)
            {
                CreateGame(row, column);
                IsBoardStarted = true;
            }

            var success = cell.Reveal();

            if (!success)
            {
                IsGameOver = true;
            }

            if (cell.AdjacentMines == 0)
            {
                var surroundingCells = GetSurroundingCells(row, column);
                foreach (var c in surroundingCells)
                {
                    RevealCell(c);
                }
            }

            CheckGameWon();
        }

        public void FlagCell(Cell cell)
        {
            cell.ToggleFlag();
        }

        public void FlagCell(int row, int column)
        {
            if (Board == null)
            {
                throw new InvalidOperationException("Board is not initialized");
            }

            var cell = GetCell(row, column);

            if (cell == null || cell.IsRevealed) return;

            cell.ToggleFlag();
        }
    }

}
