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

        public int RevealedCells { get; set; } = 0;
        public int TotalCells => Rows * Columns;


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
            return Rows > 0 && Columns > 0 && Mines > 0 && Mines <= TotalCells - 9;
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

        public void CreateGame(int initialRow, int initialColumn)
        {
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

        #endregion

        public int MineCounter()
        {
            if (Board == null)
            {
                throw new InvalidOperationException("Board is not initialized");
            }

            if (!IsBoardStarted) return Mines;

            var mineCount = Board.SelectMany(c => c).Count(c => c.IsMine == true);
            var flagCount = Board.SelectMany(c => c).Count(c => c.IsFlagged);

            return mineCount - flagCount;
        }

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
    }

}
