using System;
using System.Collections.Generic;
using System.Drawing;

namespace Online_Battleship
{
    /// <summary>
    ///     Class that facilitates shooting
    /// </summary>
    public static class Player
    {
        /// <summary>
        ///     Tests if shot is a hit, miss, sunk or forbidden.
        /// </summary>
        /// <param name="grid">Game board grid.</param>
        /// <param name="row">Row to hit.</param>
        /// <param name="col">Column to hit.</param>
        /// <returns>Returns if the shot is a hit, a miss, a sinking hit or is forbidden.</returns>
        public static Square LaunchAtTarget(Square[,] grid, int row, int col)
        {
            // Is it a valid shot? 
            switch (grid[row, col])
            {
                case Square.Hit:
                case Square.Miss:
                case Square.Sunk:
                    return Square.Forbidden;
                case Square.Ship:
                    grid[row, col] = Square.Hit;

                    // Is it sunk?
                    if (IsSunk(grid, row, col))
                        return Square.Sunk;
                    return Square.Hit;
            }

            grid[row, col] = Square.Miss;
            return Square.Miss;
        }

        /// <summary>
        ///     Test if ship that was hit at row, col is sunk.
        /// </summary>
        /// <param name="grid">Game board grid.</param>
        /// <param name="row">Row to hit.</param>
        /// <param name="col">Column to hit.</param>
        /// <returns>Returns true if the ship was sunk.</returns>
        private static bool IsSunk(Square[,] grid, int row, int col)
        {
            bool[] testDir = {true, true, true, true};
            var hitList = new List<Point> {new Point(col, row)}; //The coordinates are a hit, so add them 


            // We have to check the whole width of the grid
            int arrayLen = grid.GetLength(0);

            for (var i = 1; i < arrayLen; i++)
            {
                // 0 = up, 1 = down, 2 = left and 3 = right
                for (var dir = 0; dir < 4; dir++)
                {
                    int newRow = dir == 0 && testDir[0] ? row - i : row;
                    newRow = dir == 1 && testDir[1] ? row + i : newRow;
                    int newCol = dir == 2 && testDir[2] ? col - i : col;
                    newCol = dir == 3 && testDir[3] ? col + i : newCol;

                    if (!testDir[dir]) continue;

                    switch (GetSquare(grid, newRow, newCol))
                    {
                        case Square.Hit:
                            testDir[dir] = true;

                            // If it's a hit, add it to the hit list and try the next one
                            hitList.Add(new Point(newCol, newRow));
                            break;
                        case Square.Ship:
                            //If it's a Ship, it's not sunk yet.
                            return false;
                        default:
                            testDir[dir] = false;
                            break;
                    }
                }

                // If no directions are to be tested next loop, then break.
                if (testDir[0] == false &&
                    testDir[1] == false &&
                    testDir[2] == false &&
                    testDir[3] == false)
                    break;
            }

            // If no Square.Ship was found next to any Square.Hit on this ship, it was sunk.
            // Rewrite grid and return true
            foreach (var hit in hitList)
                grid[hit.Y, hit.X] = Square.Sunk;

            return true;
        }

        /// <summary>
        ///     Check which Square value is at grid[row, col]. Also test if we're in the range of grid.
        /// </summary>
        /// <param name="grid">Game board grid.</param>
        /// <param name="row">Row of the grid.</param>
        /// <param name="col">Column of the grid.</param>
        /// <returns>Returns the Square at grid[row, col] or Forbidden if out of bounds.</returns>
        private static Square GetSquare(Square[,] grid, int row, int col)
        {
            try
            {
                return grid[row, col];
            }
            catch (IndexOutOfRangeException)
            {
                //If we go out of bounds, it's not a valid square.
                return Square.Forbidden;
            }
        }
    }
}