using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using ManhattanMorning.Controller;

namespace ManhattanMorning.Model.GameObject
{


    /// <summary>
    /// Indicates which kind of InputDevice the gamepad of a player actually is
    /// (Because for PCs it can also be the keyboard)
    /// </summary>
    public enum InputDevice
    {
        /// <summary>
        /// A Gamepad as input
        /// </summary>
        Gamepad,
        /// <summary>
        /// The keyboard as input
        /// </summary>
        Keyboard
    };

    public class Gamepad
    {

        #region Properties

        /// <summary>
        /// Indicates which Gamepad is assigned to this class
        /// Between PlayerIndex.One and PlayerIndex.Four
        /// </summary>
        public PlayerIndex Index { get { return index; } set { this.index = value; } }
        
        /// <summary>
        /// Indicates if this gamepad is connected
        /// </summary>
        public bool IsConnected { get { return isConnected; } set { this.isConnected = value; } }

        /// <summary>
        /// Gamepadstate from last Update call
        /// </summary>
        public GamePadState PreviousGamePadState { get { return previousGamePadState; } set { previousGamePadState = value; } }

        /// <summary>
        /// Current Gamepadstate
        /// </summary>
        public GamePadState CurrentGamePadState { get { return currentGamePadState; } set { currentGamePadState = value; } }

        /// <summary>
        /// Indicates which kind of input device the gamepad is
        /// </summary>
        public InputDevice Device { get { return device; } set { device = value; } }

        #endregion

        #region Members

        /// <summary>
        /// Indicates which Gamepad is assigned to this class
        /// Between PlayerIndex.One and PlayerIndex.Four
        /// </summary>
        private PlayerIndex index;

        /// <summary>
        /// Indicates if this gamepad is connected
        /// </summary>
        private bool isConnected;

        /// <summary>
        /// Gamepadstate from last Update call
        /// </summary>
        private GamePadState previousGamePadState;

        /// <summary>
        /// Current Gamepadstate
        /// </summary>
        private GamePadState currentGamePadState;

        /// <summary>
        /// Indicates which kind of input device the gamepad is
        /// </summary>
        private InputDevice device;

        #endregion


        #region Initialization

        /// <summary>
        /// Initializes a new Gamepad
        /// </summary>
        /// <param name="index">PlayerIndex, has to be between One and Four</param>
        /// <param name="device">Specifies the actual input device</param>
        /// <param name="isConnected">Indicates whether the Gamepad is connected at initalization time</param>
        public Gamepad(PlayerIndex index, InputDevice device, bool isConnected)
        {

            this.index = index;
            this.isConnected = isConnected;
            this.device = device;

        }


        #endregion

    }
}
