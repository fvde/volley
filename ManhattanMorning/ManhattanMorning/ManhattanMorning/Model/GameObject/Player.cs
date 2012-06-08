using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ManhattanMorning.Misc;
using FarseerPhysics.Dynamics;
using ManhattanMorning.Controller;

namespace ManhattanMorning.Model.GameObject
{
    /// <summary>
    /// Defines the status of a player
    /// </summary>
    public enum PlayerStatus
    {
        /// <summary>
        /// Player is controlled by Gamepad
        /// </summary>
        HumanPlayer,
        /// <summary>
        /// Player is controlled by KI
        /// </summary>
        KIPlayer
    }
    

    /// <summary>
    /// Defines the effects a player can
    /// be influenced by
    /// </summary>
    public enum PlayerFlag
    {
        // player controls are inverted
        InvertedControl,
        // player can jump twice as high
        DoubleJump,
        // player can jump half as high
        // as normal
        HalfJump,
        // player is stunned
        Stunned,
        // player is on the wrong half
        // of the playing field
        SwitchedPosition,

        /// <summary>
        /// Player touched the net, his hand is stunned for a short amount of time
        /// </summary>
        HandStunned
    }
    /// <summary>
    /// Represents the player who is controlled by human or KI
    /// </summary>
    public class Player : ActiveObject
    {

        #region Properties

        /// <summary>
        /// The actual input device of the player (Gamepad/Keyboard with index)
        /// </summary>
        public Gamepad InputDevice { get { return inputDevice; } }

        /// <summary>
        /// Impulse which represents the stick amplitude for moving
        /// X, Y Component's range: between -1 and 1
        /// </summary>
        public Vector2 MovingImpulse { get { return movingImpulse; } set { movingImpulse = value; } }

        /// <summary>
        /// Amplitude which represents the stick amplitude for player's hand
        /// </summary>
        public Vector2 HandAmplitude { get { return handAmplitude; } set { handAmplitude = value; } }

        /// <summary>
        /// Boolean that indicates if player has to do a simpleJump
        /// </summary>
        public Boolean SimpleJump { get { return simpleJump; } set { simpleJump = value; } }

        /// <summary>
        /// Indicates how many jumps are still possible before player has to touch the ground
        /// </summary>
        public int PossibleJumpsCounter { get { return possibleJumpsCounter; } set { possibleJumpsCounter = value; } }

        /// <summary>
        /// the body representing the players hand
        /// </summary>
        public Body HandBody;

        /// <summary>
        /// Defines team to which the player belongs (0 = no team, 1 = left team, 2 = right team)
        /// </summary>
        public int Team { get { return team; } set { team = value; } }

        /// <summary>
        /// Defines player's status
        /// </summary>
        public PlayerStatus Status { get { return status; } set { status = value; } }

        /// <summary>
        /// Contains all the effects
        /// the player is influenced just now
        /// </summary>
        public List<PlayerFlag> Flags { get { return flags; } set { flags = value; } }

        /// <summary>
        /// The index of the player (between 1 and 4)
        /// </summary>
        public int PlayerIndex { get { return playerIndex; } set { playerIndex = value; } }

        #endregion

        #region Members

        /// <summary>
        /// The actual input device of the player (Gamepad/Keyboard with index)
        /// </summary>
        private Gamepad inputDevice;

        /// <summary>
        /// impulse which represents the stick amplitude for moving 
        /// X,Y Components range: between -1 and 1
        /// </summary>
        private Vector2 movingImpulse;

        /// <summary>
        /// amplitude which represents the stick amplitude for player's hand
        /// </summary>
        private Vector2 handAmplitude;

        /// <summary>
        /// boolean that indicates if player has to do a simplejump
        /// </summary>
        private Boolean simpleJump;

        /// <summary>
        /// Indicates how many jumps are still possible before player has to touch the ground
        /// </summary>
        private int possibleJumpsCounter;

        /// <summary>
        /// Defines team to which the player belongs (0 = no team, 1 or 2)
        /// </summary>
        private int team;

        /// <summary>
        /// Defines player's status
        /// </summary>
        private PlayerStatus status;

        /// <summary>
        /// Contains all the effects
        /// the player is influenced just now
        /// </summary>
        private List<PlayerFlag> flags;

        /// <summary>
        /// The index of the player (between 1 and 4)
        /// </summary>
        private int playerIndex;

        #endregion


        #region Initialization

        /// <summary>
        /// Initializes a new player
        /// </summary>
        /// <param name="name">Name of the Object. use variable name, if not sure what to choose.</param>
        /// <param name="visible">Shall the object be visible? only set visible if object  either contains a texture or a animation</param>
        /// <param name="texture">The texture. if this object has no texture, set to null</param>
        /// <param name="animation">animation, if this object has no animation, set to null</param>
        /// <param name="Size">The size of the main body. depending on the measurementUnit either in Meters or in Pixel</param>
        /// <param name="handSize">The size of the hand. depending on the measurementUnit either in Meters or in Pixel</param> 
        /// <param name="Position">The upper left position. depending on the measurementUnit either in Meters or in Pixel </param>
        /// <param name="layer">In which layer should the object be drawn?</param>h
        /// <param name="measurementUnit">The measurement unit. </param>
        /// <param name="playerIndex">The Index of the player (between 1 and 4)</param>
        /// <param name="team">Defines team to which the player belongs (0 = no team, 1 or 2)</param>
        /// <param name="playerStatus">Defines status of player</param>
        /// <param name="inputDevice">Defines if player is controlled by keyboard or gamepad</param>
        /// <param name="inputIndex">Index of input device (between 1 and 4)</param>
        public Player(String name, bool visible, Texture2D texture, Texture2D shadowTexture, SpriteAnimation animation,Vector2 size, Vector2 position, int layer,
            int possibleJumpsCounter, MeasurementUnit measurementUnit,int playerIndex, int team, PlayerStatus playerStatus, Gamepad inputDevice)
            : base (name, visible, texture, shadowTexture, animation, size, position, layer, measurementUnit) 
        {
            // Initialize all standard member variables
            movingImpulse = new Vector2(0, 0);
            handAmplitude = new Vector2(0, 0);
            simpleJump = false;

            this.possibleJumpsCounter = possibleJumpsCounter;
            this.playerIndex = playerIndex;
            this.team = team;

            this.status = playerStatus;
            this.inputDevice = inputDevice;

            // Initliazie flag list
            flags = new List<PlayerFlag>();
        }




        #endregion


    }
}
