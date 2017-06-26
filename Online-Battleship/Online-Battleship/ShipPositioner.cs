using System;

namespace Online_Battleship
{
    /// <summary>
    ///     Class that allows placing ships in only valid positions.
    /// </summary>
    public class ShipPositioner
    {
        /// <summary>
        ///     Orientation of the ship.
        /// </summary>
        public enum Orientation
        {
            /// <summary>
            ///     Vertical orientation.
            /// </summary>
            Vertical,

            /// <summary>
            ///     Horizontal orientation.
            /// </summary>
            Horizontal
        }


        /// <summary>
        ///     Add a ship to the playing field at given coordinates.
        /// </summary>
        /// <param name="grid">The playing field grid.</param>
        /// <param name="shipLength">Length of the ship to place.</param>
        /// <param name="orientation">Orientation of the ship to place.</param>
        /// <param name="row">Topmost row.</param>
        /// <param name="col">Leftmost column.</param>
        /// <returns>False if the ship cannot be placed. True otherwise.</returns>
        public bool SetShip(Square[,] grid, int shipLength, Orientation orientation, int row, int col)
        {
            // Is there a channel for the ship?
            if (!IsSettable(grid, shipLength, orientation, row, col))
                return false;

            // Commit to grid
            for (var i = 0; i < shipLength; i++)
                switch (orientation)
                {
                    case Orientation.Horizontal:
                        grid[row, col + i] = Square.Ship;
                        break;
                    case Orientation.Vertical:
                        grid[row + i, col] = Square.Ship;
                        break;
                    default:
                        return false;
                }
            return true;
        }

        /// <summary>
        ///     Checks if ship can be set. Used for choosing which arrows to display.
        /// </summary>
        /// <param name="grid">Game board grid.</param>
        /// <param name="shipLength">Length of the ship to place.</param>
        /// <param name="orientation">Orientation of the ship to place.</param>
        /// <param name="row">Topmost row.</param>
        /// <param name="col">Leftmost column.</param>
        /// <returns>False if the ship cannot be placed. True otherwise.</returns>
        public bool IsSettable(Square[,] grid, int shipLength, Orientation orientation, int row, int col)
        {
            try
            {
                if (grid[row, col] == Square.Water)
                {
                    switch (orientation)
                    {
                        case Orientation.Vertical:
                            for (var i = 1; i < shipLength; i++)
                                if (grid[row + i, col] != Square.Water) return false;
                            break;
                        case Orientation.Horizontal:
                            for (var i = 1; i < shipLength; i++)
                                if (grid[row, col + i] != Square.Water) return false;
                            break;
                    }
                    return true;
                }
            }

            // If at any point the index goes out of bounds, obviously we can't put a ship there
            catch (IndexOutOfRangeException)
            {
                return false;
            }
            return false;
        }

        /// <summary>
        ///     Remove a ship from the playing field at given coordinates.
        /// </summary>
        /// <param name="grid">The playing field grid.</param>
        /// <param name="row">Least significant row.</param>
        /// <param name="col">Least significant column.</param>
        /// <returns>True if a ship was removed successfully.</returns>
        public bool RemoveShip(Square[,] grid, int row, int col)
        {
            // If this is not the uppermost, leftmost (row, col) on the ship, 
            // or if this is not a ship at all, return false.
            if (grid[row, col] != Square.Ship ||
                col - 1 >= 0 && row - 1 >= 0 &&
                (grid[row - 1, col] == Square.Ship || grid[row, col - 1] == Square.Ship))
                return false;

            // Remove adjacent ship squares.
            if (col + 1 < grid.GetLength(0) && grid[row, col + 1] == Square.Ship)
                for (int c = col; c < grid.GetLength(1); c++)
                    if (grid[row, c] == Square.Ship)
                        grid[row, c] = Square.Water;
                    else
                        break;
            else
                for (int r = row; r < grid.GetLength(0); r++)
                    if (grid[r, col] == Square.Ship)
                        grid[r, col] = Square.Water;
                    else
                        break;

            return true;
        }

        /// <summary>
        ///     Set squares surrounding ships to Square.Forbidden.
        /// </summary>
        /// <param name="grid">The playing field grid.</param>
        public void SetForbiddenSquares(Square[,] grid)
        {
            var tempGrid = new Square[grid.GetLength(0) + 2, grid.GetLength(1) + 2];
            for (var row = 0; row < grid.GetLength(0); row++)
            for (var col = 0; col < grid.GetLength(1); col++)
                tempGrid[row + 1, col + 1] = grid[row, col];

            // Anything around a ship is forbidden
            for (var row = 0; row < tempGrid.GetLength(0); row++)
            for (var col = 0; col < tempGrid.GetLength(1); col++)
                if (tempGrid[row, col] == Square.Ship)
                {
                    tempGrid[row - 1, col - 1] = tempGrid[row - 1, col - 1] == Square.Ship
                        ? Square.Ship
                        : Square.Forbidden;
                    tempGrid[row - 1, col] = tempGrid[row - 1, col] == Square.Ship
                        ? Square.Ship
                        : Square.Forbidden;
                    tempGrid[row - 1, col + 1] = tempGrid[row - 1, col + 1] == Square.Ship
                        ? Square.Ship
                        : Square.Forbidden;
                    tempGrid[row, col - 1] = tempGrid[row, col - 1] == Square.Ship
                        ? Square.Ship
                        : Square.Forbidden;
                    tempGrid[row, col + 1] = tempGrid[row, col + 1] == Square.Ship
                        ? Square.Ship
                        : Square.Forbidden;
                    tempGrid[row + 1, col - 1] = tempGrid[row + 1, col - 1] == Square.Ship
                        ? Square.Ship
                        : Square.Forbidden;
                    tempGrid[row + 1, col] = tempGrid[row + 1, col] == Square.Ship
                        ? Square.Ship
                        : Square.Forbidden;
                    tempGrid[row + 1, col + 1] = tempGrid[row + 1, col + 1] == Square.Ship
                        ? Square.Ship
                        : Square.Forbidden;
                }

            // Commit to grid.
            for (var row = 0; row < grid.GetLength(0); row++)
            for (var col = 0; col < grid.GetLength(1); col++)
                grid[row, col] = tempGrid[row + 1, col + 1];
        }

        /// <summary>
        ///     Change all Square.Forbidden in the <paramref name="grid" /> to Square.Water.
        /// </summary>
        /// <param name="grid">The playing field grid.</param>
        public void RemoveForbiddenSquares(Square[,] grid)
        {
            for (var row = 0; row < grid.GetLength(0); row++)
            for (var col = 0; col < grid.GetLength(1); col++)
                if (grid[row, col] == Square.Forbidden)
                    grid[row, col] = Square.Water;
        }
    }
}