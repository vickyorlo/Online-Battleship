using System;
using System.ComponentModel;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace Online_Battleship
{
    /// <summary>
    ///     The main form including UI control and game logic.
    /// </summary>
    partial class OnlineBattleshipForm : Form
    {
        #region Fields and Properties

        /// <summary>
        ///     The size of the squares making up the game board in pixels
        /// </summary>
        private const int SquareSize = 25;

        /// <summary>
        ///     The distance from the left border of the form to the leftmost game board grid in pixels.
        /// </summary>
        private const int GridpaddingLeft = 50;

        /// <summary>
        ///     The distance from the top border of the form to the game board grids in pixels.
        /// </summary>
        private const int GridpaddingTop = 70;

        /// <summary>
        ///     The distance between the two game board grids in pixels.
        /// </summary>
        private const int GridpaddingCenter = 50;

        /// <summary>
        ///     The text displayed when the player wins and loses.
        /// </summary>
        private const string PlayerWon = "You win!", ComputerWon = "You lose!";

        /// <summary>
        ///     The lengths of the different ships
        /// </summary>
        private const int CarrierLength = 5, SubmarineLength = 4, CruiserLength = 3, PatrolboatLength = 2;

        /// <summary>
        ///     The game board panel containing the player's and enemies' ships.
        /// </summary>
        private BattleshipPanel playerField, enemyField;

        /// <summary>
        ///     The current game mode.
        /// </summary>
        private Mode gameMode;

        /// <summary>
        ///     Keeps track of how many of the players ships are currently on the board.
        /// </summary>
        private int shipsSetCount;

        /// <summary>
        ///     Keeps track of how many ships the player and enemy have lost.
        /// </summary>
        private int shipsLostEnemy, shipsLostPlayer;

        /// <summary>
        ///     Separate thread to wait on the other player's move.
        /// </summary>
        private BackgroundWorker backgWorker;

        /// <summary>
        ///     The TCP connection to send data over.
        /// </summary>
        private TcpClient client;

        /// <summary>
        ///     The stream where sending and receiving data happens.
        /// </summary>
        private NetworkStream stream;


        /// <summary>
        ///     Number of rows on the game board.
        /// </summary>
        public int Rows { get; set; }

        /// <summary>
        ///     Number of columns on the game board.
        /// </summary>
        public int Cols { get; set; }

        /// <summary>
        ///     A list of ships used in this game.
        /// </summary>
        public Ship[] Ships { private get; set; }

        /// <summary>
        ///     The total number of ships used in this game.
        /// </summary>
        public int NumberOfShips { get; set; }

        /// <summary>
        ///     Number of patrol-boats used in this game.
        /// </summary>
        public int NumberOfPatrolboats { get; set; }

        /// <summary>
        ///     Number of cruisers used in this game.
        /// </summary>
        public int NumberOfCruisers { get; set; }

        /// <summary>
        ///     Number of submarines used in this game.
        /// </summary>
        public int NumberOfSubmarines { get; set; }

        /// <summary>
        ///     Number of carriers used in this game.
        /// </summary>
        public int NumberOfCarriers { get; set; }

        /// <summary>
        ///     Game mode states.
        /// </summary>
        private enum Mode
        {
            /// <summary>
            ///     Placing ships on the game board.
            /// </summary>
            SettingShips,

            /// <summary>
            ///     We're playing!
            /// </summary>
            Playing,

            /// <summary>
            ///     The player has won.
            /// </summary>
            PlayerWon,

            /// <summary>
            ///     The computer has won.
            /// </summary>
            ComputerWon
        }

        /// <summary>
        ///     Used by the <see cref="Ships" /> property to hold data about the ships available to this game.
        /// </summary>
        public struct Ship
        {
            /// <summary>
            ///     The length of the ship in game board squares.
            /// </summary>
            public int Length;

            /// <summary>
            ///     The starting row for this ship if set. Null if not yet placed.
            /// </summary>
            public int? Row;

            /// <summary>
            ///     The starting column for this ship if set. Null if not yet placed.
            /// </summary>
            public int? Col;
        }

        #endregion

        /// <summary>
        ///     Initializes a new instance of the <see cref="OnlineBattleshipForm" /> class
        /// </summary>
        public OnlineBattleshipForm()
        {
            InitializeComponent();
            Rows = Cols = 10;
            NumberOfPatrolboats = 1;
            NumberOfCruisers = 1;
            NumberOfSubmarines = NumberOfCarriers = 0;
            NumberOfShips = NumberOfPatrolboats + NumberOfCruisers + NumberOfSubmarines + NumberOfCarriers;

            RestartGame();
        }

        /// <summary>
        ///     Reset everything needed to play the game.
        /// </summary>
        public void RestartGame()
        {
            lblSetShip.Visible = true;
            Controls.Remove(playerField);
            Controls.Remove(enemyField);
            ResetShips();

            var shipLength = new int[Ships.Length];
            var i = 0;

            foreach (var ship in Ships)
                shipLength[i++] = ship.Length;

            playerField = new BattleshipPanel(SquareSize);
            enemyField = new BattleshipPanel(SquareSize);

            shipsSetCount = 0;
            shipsLostEnemy = 0;
            shipsLostPlayer = 0;
            gameMode = Mode.SettingShips;
            lblGameOver.Visible = false;
            InitializeGameBoard();

            FormSize();
        }

        /// <summary>
        ///     Calculate and set the size of the window.
        /// </summary>
        public void FormSize()
        {
            Width = SquareSize * Cols * 2 + 168;
            Height = SquareSize * Rows + 158;
            Padding = new Padding(0, 0, 50, 50);
        }

        /// <summary>
        ///     Reset the Ships array to contain the right ships for the game
        /// </summary>
        private void ResetShips()
        {
            Ships = new Ship[NumberOfShips];
            var patrolboats = new Ship();
            var cruisers = new Ship();
            var submarines = new Ship();
            var carriers = new Ship();

            patrolboats.Length = PatrolboatLength;
            cruisers.Length = CruiserLength;
            submarines.Length = SubmarineLength;
            carriers.Length = CarrierLength;

            var tempShips = 0;

            for (var i = 0; i < NumberOfPatrolboats; i++)
            {
                Ships[tempShips] = patrolboats;
                tempShips++;
            }

            for (var i = 0; i < NumberOfCruisers; i++)
            {
                Ships[tempShips] = cruisers;
                tempShips++;
            }

            for (var i = 0; i < NumberOfSubmarines; i++)
            {
                Ships[tempShips] = submarines;
                tempShips++;
            }

            for (var i = 0; i < NumberOfCarriers; i++)
            {
                Ships[tempShips] = carriers;
                tempShips++;
            }
        }

        /// <summary>
        ///     Places one game board panel for the enemy, and one game board panel for the player.
        /// </summary>
        private void InitializeGameBoard()
        {
            // Create players panel
            playerField.Location = new Point(GridpaddingLeft, GridpaddingTop);
            playerField.Size = new Size(Rows * SquareSize, Cols * SquareSize);
            playerField.MouseClick += UpdateForm;
            Controls.Add(playerField);

            // Create enemy panel
            enemyField.Location = new Point(GridpaddingLeft + GridpaddingCenter + Cols * SquareSize, GridpaddingTop);
            enemyField.Size = new Size(Rows * SquareSize, Cols * SquareSize);
            enemyField.MouseClick += UpdateForm;
            Controls.Add(enemyField);
        }

        /// <summary>
        ///     Display a label telling who won and set gameMode accordingly.
        /// </summary>
        private void GameOver()
        {
            lblGameOver.Text = gameMode == Mode.PlayerWon ? PlayerWon : ComputerWon;
            lblGameOver.Left = Size.Width / 2 - lblGameOver.Size.Width / 2;
            lblGameOver.Top = Size.Height / 2 - lblGameOver.Size.Height / 2;
            Thread.Sleep(500);
            lblGameOver.Visible = true;
            textYourTurn.Enabled = false;
            client.GetStream().Close();
            client.Close();
        }

        /// <summary>
        ///     Handles player interaction with the playing fields.
        /// </summary>
        /// <param name="sender">A System.Object containing the sender date</param>
        /// <param name="e">A System.Windows.Forms.MouseEventArgs that contain the event data.</param>
        private void UpdateForm(object sender, MouseEventArgs e)
        {
            int row = e.Location.Y / enemyField.SquareHeight;
            int col = e.Location.X / enemyField.SquareWidth;

            if (sender.Equals(enemyField) && gameMode == Mode.Playing)
            {
                // Launch torpedos!
                var result = enemyField.PlayerShoot(col, row, stream);

                if (result == Square.Sunk)
                {
                    shipsLostEnemy++;
                    if (shipsLostEnemy == Ships.Length)
                    {
                        gameMode = Mode.PlayerWon;
                        GameOver();
                    }
                }
                else if (result != Square.Hit && result != Square.Forbidden)
                {
                    // Player missed. The turn is passed to the opponent.
                    // Disable the control so the player can't shoot. We enable it again when the backgroundworker finishes.

                    textYourTurn.Enabled = false;
                    enemyField.Enabled = false;

                    backgWorker = new BackgroundWorker();
                    backgWorker.DoWork += BackgroundWorker_DoWork;
                    backgWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
                    backgWorker.RunWorkerAsync();
                }
            }
            else if (sender.Equals(playerField) && gameMode == Mode.SettingShips)
            {
                //The player has clicked on their own grid and is trying to edit it.

                // Loop through the ships to find the first one that is not already on the board
                var length = 0;
                int shipArrayIndex;
                for (shipArrayIndex = 0; shipArrayIndex < Ships.Length; shipArrayIndex++)
                    if (Ships[shipArrayIndex].Row == null)
                    {
                        length = Ships[shipArrayIndex].Length;
                        break;
                    }

                //Try to manipulate the tile selected. If it's empty, placing a ship will be attempted.
                //If it is a ship, it will be removed. col and row will point at the ship removed.
                var result = playerField.ManualShipHandling(ref col, ref row, length);
                switch (result)
                {
                    case BattleshipPanel.ShipHandleReturn.ShipSet:
                        //Save the ship's position and if all ships are placed, enable the start game buttons
                        shipsSetCount++;
                        if (shipsSetCount == Ships.Length)
                        {
                            btnStartGame.Enabled = true;
                            btnJoinGame.Enabled = true;
                            lblSetShip.Visible = false;
                        }

                        Ships[shipArrayIndex].Row = row;
                        Ships[shipArrayIndex].Col = col;
                        break;

                    case BattleshipPanel.ShipHandleReturn.ShipRemoved:

                        // Disable start button and null the coordinates of the removed ship
                        shipsSetCount--;
                        btnStartGame.Enabled = false;
                        btnJoinGame.Enabled = false;
                        lblSetShip.Visible = true;
                        for (shipArrayIndex = 0; shipArrayIndex < Ships.Length; shipArrayIndex++)
                            if (Ships[shipArrayIndex].Row == row && Ships[shipArrayIndex].Col == col)
                            {
                                Ships[shipArrayIndex].Row = null;
                                Ships[shipArrayIndex].Col = null;
                            }
                        break;
                    case BattleshipPanel.ShipHandleReturn.PointSet:
                        break;
                    case BattleshipPanel.ShipHandleReturn.Failed:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }


        /// <summary>
        ///     Host a game session.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">Arguments passed.</param>
        private void BtnStartGame_Click(object sender, EventArgs e)
        {
            try
            {
                var listener = new TcpListener(IPAddress.Parse(textIPAddress.Text), (int)numericPort.Value);
                listener.Start();
                client = listener.AcceptTcpClient();
                stream = client.GetStream();
                listener.Stop();
            }
            catch (Exception)
            {
                MessageBox.Show("Cannot connect!");
                return;
            }


            gameMode = Mode.Playing;
            btnStartGame.Enabled = false;
            btnJoinGame.Enabled = false;
            textYourTurn.Enabled = true;
            playerField.ClearForbiddenSquares();
        }

        /// <summary>
        ///     Join an existing game session.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">Arguments passed.</param>
        private void btnJoinGame_Click(object sender, EventArgs e)
        {
            try
            {
                client = new TcpClient(textIPAddress.Text, (int)numericPort.Value);
                stream = client.GetStream();
            }
            catch (Exception)
            {
                MessageBox.Show("Cannot connect!");
                return;
            }


            gameMode = Mode.Playing;
            btnStartGame.Enabled = false;
            btnJoinGame.Enabled = false;
            playerField.ClearForbiddenSquares();

            //Someone has to be second, and it is the joining player.
            enemyField.Enabled = false;
            backgWorker = new BackgroundWorker();
            backgWorker.DoWork += BackgroundWorker_DoWork;
            backgWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            backgWorker.RunWorkerAsync();
        }


        /// <summary>
        ///     Start a new game and restart.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">Arguments passed.</param>
        private void NewGameItem_Click(object sender, EventArgs e)
        {
            RestartGame();
        }

        /// <summary>
        ///     Closes the program
        /// </summary>
        /// <param name="sender">A System.Object containing the sender data.</param>
        /// <param name="e">An System.EventArgs that contain the event data.</param>
        private void QuitItem_Click(object sender, EventArgs e)
        {
            Close();
        }


        /// <summary>
        ///     Let the network player do their turn.
        /// </summary>
        /// <param name="sender">Calling object.</param>
        /// <param name="e">Arguments passed.</param>
        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Square result;
                do
                {
                    var b = new byte[2];
                    stream.Read(b, 0, 2);

                    // We need to call ComputerShoot() from the UI thread as it changes the GUI
                    result = (Square)Invoke((Func<Square>)(() => playerField.EnemyShoot(b)));

                    b[0] = (byte)result;
                    b[1] = 255;
                    stream.Write(b, 0, 2);

                    if (result != Square.Sunk) continue;
                    shipsLostPlayer++;
                    if (shipsLostPlayer != Ships.Length) continue;
                    // Player has lost all her ships. This triggers a GameOver() call in bgWorker_RunWorkerCompleted.
                    e.Result = Mode.ComputerWon;
                    return;
                } while (result == Square.Hit || result == Square.Sunk || result == Square.Forbidden);
            }
            catch (Exception)
            {
                MessageBox.Show("Something bad happened! Your connection has been lost!");
                e.Result = Mode.ComputerWon;
            }
        }

        /// <summary>
        ///     Fires when computer is done shooting. Evaluates if computer won and enables computerField so
        ///     player can shoot again.
        /// </summary>
        /// <param name="sender">Calling object.</param>
        /// <param name="e">Arguments passed.</param>
        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Both player and computer are done shooting for now.
            enemyField.Enabled = true;
            textYourTurn.Enabled = true;

            // bgWorker_DoWork sets e.Result if computer won
            if (e.Result == null || (Mode)e.Result != Mode.ComputerWon) return;
            gameMode = Mode.ComputerWon;
            GameOver();
        }
    }
}