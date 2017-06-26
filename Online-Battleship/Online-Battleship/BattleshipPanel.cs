using System;
using System.Drawing;
using System.Net.Sockets;
using System.Windows.Forms;

namespace Online_Battleship
{
    /// <summary>
    ///     Class for drawing the playing field as a form control and handling events.
    /// </summary>
    public class BattleshipPanel : Panel
    {
        #region Fields and Properties

        /// <summary>
        ///     Constants for images.
        /// </summary>
        private const string ImagePath = @"images\",
            ForbiddenImage = "forbidden.png",
            MissImage = "miss.png",
            ShipImage = "ship.png",
            SunkImage = "sunk.png",
            WaterImage = "water.png",
            HitImage = "hit.png",
            RightArrowImage = "right.png",
            DownArrowImage = "down.png",
            LeftArrowImage = "left.png",
            UpArrowImage = "up.png";

        /// <summary>
        ///     Number of rows and columns in the play field
        /// </summary>
        private const int Rows = 10, Columns = 10;

        /// <summary>
        ///     The row and column where a ship is being placed.
        /// </summary>
        private int shipRow, shipCol;

        /// <summary>
        ///     The play field grid.
        /// </summary>
        private readonly Square[,] playField;

        /// <summary>
        ///     An instance of a ShipPositioner for placing ships on the grid.
        /// </summary>
        private readonly ShipPositioner shipPlacer = new ShipPositioner();

        /// <summary>
        ///     Whether the player is in the placing phase or not.
        /// </summary>
        private bool isPlacing;

        /// <summary>
        ///     Height of the individual squares.
        /// </summary>
        public int SquareHeight;

        /// <summary>
        ///     Width of the individual squares.
        /// </summary>
        public int SquareWidth;


        /// <summary>
        ///     Intermediary return value for the result of clicking on a square.
        /// </summary>
        public enum ShipHandleReturn
        {
            /// <summary>
            ///     A ship has been placed
            /// </summary>
            ShipSet,

            /// <summary>
            ///     Starting point for a ship has been set
            /// </summary>
            PointSet,

            /// <summary>
            ///     A ship has been removed
            /// </summary>
            ShipRemoved,

            /// <summary>
            ///     The method failed to set a starting point, place or remove a ship
            /// </summary>
            Failed
        }

        #endregion

        /// <summary>
        ///     Initializes a new instance of the <see cref="BattleshipPanel" /> class.
        /// </summary>
        public BattleshipPanel(int squareSize)
        {
            // Force it to redraw the field if the size of the panel changes
            ResizeRedraw = true;

            //A bunch of Windows Forms magic to make it flicker less
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint,
                true);

            playField = new Square[Rows, Columns];

            SquareHeight = squareSize;
            SquareWidth = squareSize;
        }

        /// <summary>
        ///     Manually take a shot on the enemy's grid through the internet.
        /// </summary>
        /// <param name="col">The column the player fires at.</param>
        /// <param name="row">The row the player fires at.</param>
        /// <param name="stream">The NetworkStream between players.</param>
        /// <returns>The Square type that has been shot.</returns>
        public Square PlayerShoot(int col, int row, NetworkStream stream)
        {
            Square hitType;
            try
            {
                var b = new byte[2];
                b[0] = (byte) row;
                b[1] = (byte) col;
                stream.Write(b, 0, 2);

                stream.Read(b, 0, 2);
                hitType = (Square) b[0];
                if (hitType != Square.Forbidden)
                    if (hitType == Square.Sunk) playField[row, col] = Square.Hit;
                    else playField[row, col] = hitType;
            }
            catch (Exception)
            {
                MessageBox.Show("Cannot connect to other player!");
                hitType = Square.Forbidden;
            }
            finally
            {
                // Force the panel to redraw the field to show any changes.
                Refresh();
            }
            return hitType;
        }

        /// <summary>
        ///     Method used when the computer chooses where to shoot in the grid.
        ///     Returns the type of square that the shot results in.
        /// </summary>
        /// <returns>The Square type that the shot results in.</returns>
        public Square EnemyShoot(byte[] b)
        {
            var hitType = Player.LaunchAtTarget(playField, b[0], b[1]);

            // Force the panel to redraw the field to show any changes.
            Refresh();
            return hitType;
        }

        /// <summary>
        ///     Method for placing ships on the playing field. Returns whether the
        ///     method successfully placed the ship. Row and col will be set to top-left
        ///     most coordinate of a removed or added ship.
        /// </summary>
        /// <param name="col">The column to place the ship.</param>
        /// <param name="row">The row to place the ship.</param>
        /// <param name="shipLength">The length of the ship</param>
        /// <returns>Whether the method successfully placed a ship.</returns>
        public ShipHandleReturn ManualShipHandling(ref int col, ref int row, int shipLength = 0)
        {
            var methodOutcome = ShipHandleReturn.Failed;

            // Check if the player have started placing a ship.
            if (isPlacing)
            {
                // The player has chosen an initial spot and is now choosing the orientation of the boat
                if (row == shipRow && col == shipCol)
                {
                    // Player pressed the same square again to stop the placing
                    // Leave placing mode and remove arrows
                    RemoveArrows();
                    isPlacing = false;
                    methodOutcome = ShipHandleReturn.Failed;
                }
                else if (playField[row, col] == Square.ArrowDown)
                {
                    // Remove the arrows and place the ship vertically below the chosen square
                    RemoveArrows();
                    shipPlacer.SetShip(playField, shipLength, ShipPositioner.Orientation.Vertical, shipRow, shipCol);
                    methodOutcome = ShipHandleReturn.ShipSet;
                }
                else if (playField[row, col] == Square.ArrowRight)
                {
                    // Remove the arrows and place the ship horizontally to the right of the chosen square
                    RemoveArrows();
                    shipPlacer.SetShip(playField, shipLength, ShipPositioner.Orientation.Horizontal, shipRow, shipCol);
                    methodOutcome = ShipHandleReturn.ShipSet;
                }
                else if (playField[row, col] == Square.ArrowUp)
                {
                    // Remove the arrows and place the ship vertically above the chosen square
                    RemoveArrows();

                    // The SetShip method can only place towards right or down so call it with coordinates on the top most side of the ship
                    shipRow = shipRow - shipLength + 1;
                    shipPlacer.SetShip(playField, shipLength, ShipPositioner.Orientation.Vertical, shipRow, shipCol);
                    methodOutcome = ShipHandleReturn.ShipSet;
                }
                else if (playField[row, col] == Square.ArrowLeft)
                {
                    // Remove the arrows and place the ship horizontally to the left of the chosen square
                    RemoveArrows();

                    // The SetShip method can only place towards right or down so call it with coordinates on the left most side of the ship
                    shipCol = shipCol - shipLength + 1;
                    shipPlacer.SetShip(playField, shipLength, ShipPositioner.Orientation.Horizontal, shipRow, shipCol);
                    methodOutcome = ShipHandleReturn.ShipSet;
                }
            }
            else if (playField[row, col] == Square.Ship)
            {
                // The player clicked an already placed ship
                // Find the top-left most square of the selected ship
                while (true)
                    if (row > 0 &&
                        playField[row - 1, col] == Square.Ship)
                        row--;
                    else if (col > 0 &&
                             playField[row, col - 1] == Square.Ship)
                        col--;
                    else
                        break;

                // Remove the chosen ship
                if (shipPlacer.RemoveShip(playField, row, col))
                    methodOutcome = ShipHandleReturn.ShipRemoved;
            }
            else if (shipLength != 0)
            {
                // The player is choosing the initial position for the ship, 
                // check for space to the right and below the chosen square
                if (shipPlacer.IsSettable(playField, shipLength, ShipPositioner.Orientation.Horizontal, row, col))
                {
                    // The ship fits to the right of the chosen spot
                    playField[row, col + 1] = Square.ArrowRight;
                    methodOutcome = ShipHandleReturn.PointSet;
                }

                if (shipPlacer.IsSettable(playField, shipLength, ShipPositioner.Orientation.Vertical, row, col))
                {
                    // The ship fits below the chosen spot
                    playField[row + 1, col] = Square.ArrowDown;
                    methodOutcome = ShipHandleReturn.PointSet;
                }

                // ShipPositioner.IsSettable can only check to the right and down so make sure the ship fits
                // with regards to the size of the field and then check if it's settable from the far side of the ship
                if (row - (shipLength - 1) >= 0 &&
                    shipPlacer.IsSettable(playField, shipLength, ShipPositioner.Orientation.Vertical,
                        row - shipLength + 1, col))
                {
                    // The ship fits to the above of the chosen spot.
                    playField[row - 1, col] = Square.ArrowUp;
                    methodOutcome = ShipHandleReturn.PointSet;
                }

                // ShipPositioner.IsSettable can only check to the right and down so make sure the ship fits
                // with regards to the size of the field and then check if it's settable from the far side of the ship
                if (col - (shipLength - 1) >= 0 &&
                    shipPlacer.IsSettable(playField, shipLength, ShipPositioner.Orientation.Horizontal, row,
                        col - shipLength + 1))
                {
                    // The ship fits to the left of the chosen spot.
                    playField[row, col - 1] = Square.ArrowLeft;
                    methodOutcome = ShipHandleReturn.PointSet;
                }
            }

            switch (methodOutcome)
            {
                case ShipHandleReturn.ShipSet:
                    // A ship has been placed or the player cancelled placement of a ship
                    isPlacing = false;
                    row = shipRow;
                    col = shipCol;
                    break;
                case ShipHandleReturn.PointSet:
                    // The player has started placing a ship and the class is now in placing mode,
                    // save the chosen starting position for the ship.
                    shipRow = row;
                    shipCol = col;
                    isPlacing = true;
                    break;
            }

            // Rebuild forbidden squares    
            shipPlacer.RemoveForbiddenSquares(playField);
            shipPlacer.SetForbiddenSquares(playField);
            // Force a redraw of the panel to show the changed squares
            Refresh();
            return methodOutcome;
        }

        /// <summary>
        ///     Clears all forbidden squares in preparation of starting the game
        /// </summary>
        public void ClearForbiddenSquares()
        {
            shipPlacer.RemoveForbiddenSquares(playField);
            Refresh();
        }

        /// <summary>
        ///     An event that fires when the panel is painted.
        /// </summary>
        /// <param name="paintEvent">A System.Windows.Form.PaintEventArgs that contain the event data.</param>
        protected override void OnPaint(PaintEventArgs paintEvent)
        {
            for (var row = 0; row < Rows; row++)
            {
                // Calculate where along the rows the image is placed
                int rowPlace = row * SquareHeight;
                for (var column = 0; column < Columns; column++)
                {
                    // Calculate where along the columns the image is placed
                    int columnPlace = column * SquareWidth;

                    // Paint the correct image depending on what kind of square is at the current coordinates
                    switch (playField[row, column])
                    {
                        case Square.Hit:
                            paintEvent.Graphics.DrawImageUnscaled(Image.FromFile(ImagePath + HitImage), columnPlace,
                                rowPlace);
                            break;
                        case Square.Miss:
                            paintEvent.Graphics.DrawImageUnscaled(Image.FromFile(ImagePath + MissImage), columnPlace,
                                rowPlace);
                            break;
                        case Square.Sunk:
                            paintEvent.Graphics.DrawImageUnscaled(Image.FromFile(ImagePath + SunkImage), columnPlace,
                                rowPlace);
                            break;
                        case Square.ArrowRight:
                            paintEvent.Graphics.DrawImageUnscaled(Image.FromFile(ImagePath + RightArrowImage), columnPlace,
                                rowPlace);
                            break;
                        case Square.ArrowDown:
                            paintEvent.Graphics.DrawImageUnscaled(Image.FromFile(ImagePath + DownArrowImage), columnPlace,
                                rowPlace);
                            break;
                        case Square.ArrowLeft:
                            paintEvent.Graphics.DrawImageUnscaled(Image.FromFile(ImagePath + LeftArrowImage), columnPlace,
                                rowPlace);
                            break;
                        case Square.ArrowUp:
                            paintEvent.Graphics.DrawImageUnscaled(Image.FromFile(ImagePath + UpArrowImage), columnPlace,
                                rowPlace);
                            break;
                        case Square.Forbidden:
                            paintEvent.Graphics.DrawImageUnscaled(Image.FromFile(ImagePath + ForbiddenImage), columnPlace,
                                rowPlace);
                            break;
                        case Square.Ship:
                            paintEvent.Graphics.DrawImageUnscaled(Image.FromFile(ImagePath + ShipImage), columnPlace,
                                rowPlace);
                            break;
                        case Square.Water:
                            paintEvent.Graphics.DrawImageUnscaled(Image.FromFile(ImagePath + WaterImage), columnPlace,
                                rowPlace);
                            break;
                        default:
                            paintEvent.Graphics.DrawImageUnscaled(Image.FromFile(ImagePath + WaterImage), columnPlace,
                                rowPlace);
                            break;
                    }
                }
            }

            base.OnPaint(paintEvent);
        }

        /// <summary>
        ///     Remove all arrows in the playing field
        /// </summary>
        private void RemoveArrows()
        {
            for (var row = 0; row < Rows; row++)
            for (var column = 0; column < Columns; column++)
                if (playField[row, column] == Square.ArrowDown ||
                    playField[row, column] == Square.ArrowRight ||
                    playField[row, column] == Square.ArrowUp ||
                    playField[row, column] == Square.ArrowLeft)
                    playField[row, column] = Square.Water;
        }

    }
}