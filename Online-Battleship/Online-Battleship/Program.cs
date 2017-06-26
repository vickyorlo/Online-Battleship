using System;
using System.Windows.Forms;

namespace Online_Battleship
{
    /// <summary>
    ///     Kinds of tiles on the battleship game board.
    /// </summary>
    public enum Square
    {
        /// <summary>
        ///     Water.
        /// </summary>
        Water,

        /// <summary>
        ///     Part of a ship.
        /// </summary>
        Ship,

        /// <summary>
        ///     A part of a ship that has been hit.
        /// </summary>
        Hit,

        /// <summary>
        ///     Water that has been shot at.
        /// </summary>
        Miss,

        /// <summary>
        ///     A part of a sunken ship.
        /// </summary>
        Sunk,

        /// <summary>
        ///     Water where ships cannot be placed.
        /// </summary>
        Forbidden,

        /// <summary>
        ///     Arrow pointing right for placing ships.
        /// </summary>
        ArrowRight,

        /// <summary>
        ///     Arrow pointing down for placing ships.
        /// </summary>
        ArrowDown,

        /// <summary>
        ///     Arrow pointing left for placing ships.
        /// </summary>
        ArrowLeft,

        /// <summary>
        ///     Arrow pointing up for placing ships.
        /// </summary>
        ArrowUp
    }


    public static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new BattleshipForm());
        }
    }
}