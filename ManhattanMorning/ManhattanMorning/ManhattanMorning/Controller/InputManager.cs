using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using ManhattanMorning.Misc;
using ManhattanMorning.Misc.Logic;
using ManhattanMorning.Model.GameObject;

namespace ManhattanMorning.Controller
{

    /// <summary>
    /// Handles gamepad, keyboard and KI input
    /// Handles gamepad rumble
    /// </summary>
    class InputManager : IObserver, IController
    {

        #region Properties

        /// <summary>
        /// Returns an Instance of the StorageManager
        /// </summary>
        public static InputManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new InputManager();
                }
                return instance;
            }
        }

        /// <summary>
        /// Array of all gamepads, independent of being connected or not
        /// </summary>
        public Gamepad[] GamepadArray
        {
            get { return gamepadArray; }
            set { gamepadArray = value; }
        }


        #endregion

        #region Members

        /// <summary>
        /// Necessary Instance for Singleton Pattern
        /// </summary>
        private static InputManager instance;

        /// <summary>
        /// KeyboardState necessary for playtesting with keyboard
        /// </summary>
        private KeyboardState currentKeyboardState;

        /// <summary>
        /// Previous keyboardSate necessary for playtesting with keyboard
        /// </summary>
        private KeyboardState previousKeyboardState;

        /// <summary>
        /// GameState that has to be saved, because you switch back
        /// to it when closing ingame menus
        /// </summary>
        private object[] gameStateBackup = new object[3];

        /// <summary>
        /// Array of all gamepads, independent of being connected or not
        /// </summary>
        private Gamepad[] gamepadArray;

        /// <summary>
        /// List of all players ingame
        /// </summary>
        private LayerList<Player> playerList;

        /// <summary>
        /// The waiting time until the player is allowed to make the next jump (in ms)
        /// (Loaded from SettingsManager)
        /// </summary>
        private int timeBetweenJumps;

        /// <summary>
        /// The time until the next item in a scrollable list is selected (in ms)
        /// (Loaded from SettingsManager)
        /// </summary>
        private int scrollWaitingTime;

        /// <summary>
        /// Stores for every player the time (in ms) until it can make the next jump
        /// </summary>
        private int[] timeTillNextJump;

        /// <summary>
        /// Stores the time until it scrolls to the next item
        /// </summary>
        private int timeTillNextScrollAction;

        /// <summary>
        /// The remaining duration of the rumbling action
        /// </summary>
        private int rumbling_remainingDuration;

        /// <summary>
        /// The intensity of the rumbling (0.0f - 1.0f)
        /// X = left motor, Y = right motor
        /// </summary>
        private Vector2 rumbling_intensity;

        #endregion


        #region Initialization

        /// <summary>
        /// Initializes InputManager
        /// hidden because of Singleton Pattern
        /// </summary>
        private InputManager()
        {

            currentKeyboardState = Keyboard.GetState();

            //get default values from SettingsManager
            SettingsManager.Instance.registerObserver(this);

            timeBetweenJumps = (int)SettingsManager.Instance.get("timeBetweenJumps");
            scrollWaitingTime = (int)SettingsManager.Instance.get("scrollWaitingTime");

            timeTillNextJump = new int[4];
            timeTillNextScrollAction = 0;

            // Initialize gamepad Array
            gamepadArray = new Gamepad[4];
            gamepadArray[0] = new Gamepad(PlayerIndex.One, InputDevice.Gamepad, false);
            gamepadArray[1] = new Gamepad(PlayerIndex.Two, InputDevice.Gamepad, false);
            gamepadArray[2] = new Gamepad(PlayerIndex.Three, InputDevice.Gamepad, false);
            gamepadArray[3] = new Gamepad(PlayerIndex.Four, InputDevice.Gamepad, false);


        }

        /// <summary>
        /// Necessary for interface, but mustn't be called
        /// </summary>
        public void initialize()
        {
            throw new Exception("Wrong InputManager initialization");
        }

        /// <summary>
        /// Initializes the Controller to handle a new GameInstance
        /// </summary>
        /// <param name="playerList">List with all players of the level</param>
        public void initialize(LayerList<Player> playerList)
        {

            this.playerList = playerList;

            rumbling_intensity = Vector2.Zero;
            rumbling_remainingDuration = 0;

        }

        #endregion


        #region Methods

        /// <summary>
        /// Updates internal variables 
        /// </summary>
        /// <param name="gameTime">The time since the last update</param>
        public void update(GameTime gameTime)
        {
            
            // update the times till next jump
            for (int i = 0; i < 4; i++)
            {
                timeTillNextJump[i] -= gameTime.ElapsedGameTime.Milliseconds;
                if (timeTillNextJump[i] < 0)
                    timeTillNextJump[i] = 0;
            }

            // update the scroll waiting time
            timeTillNextScrollAction -= gameTime.ElapsedGameTime.Milliseconds;
            if (timeTillNextScrollAction < 0)
                timeTillNextScrollAction = 0;

            handleRumbling(gameTime);
        }

        /// <summary>
        /// Updates Ingame input
        /// </summary>
        /// <param name="gameTime">the time since the last update</param>
        /// <param name="playerList">List of all players which belog to the level</param>
        public void updateIngameInput(GameTime gameTime, LayerList<Player> playerList)
        {
            this.playerList = playerList;

            if (playerList != null && playerList.Count > 0)
            {
                setHandAmplitude(1, Vector2.Zero);
                setHandAmplitude(2, Vector2.Zero);
                setHandAmplitude(3, Vector2.Zero);
                setHandAmplitude(4, Vector2.Zero);
            }


            // Update keyboardstates
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            //  check if new gamepads are connected/disconnected and update their states
            updateGamepads();

            handleIngameInput();

            
        }

        /// <summary>
        /// Updates MainMenu input
        /// </summary>
        /// <param name="gameTime">the time since the last update</param>
        public void updateMainMenuInput(GameTime gameTime)
        {
            // Update keyboardstates
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            //  check if new gamepads are connected/disconnected and update their states
            updateGamepads();

            handleMainMenuInput();
        }

        /// <summary>
        /// Updates IngameMenu input
        /// </summary>
        /// <param name="gameTime">the time since the last update</param>
        public void updateIngameMenuInput(GameTime gameTime)
        {

            // Update keyboardstates
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            //  check if new gamepads are connected/disconnected and update their states
            updateGamepads();

            handleIngameMenuInput();
        }

        /// <summary>
        /// Sets the movement for a specified player
        /// </summary>
        /// <param name="playerIndex">Player that should move</param>
        /// <param name="direction">direction of the movement (x and y both between -1, 1)</param>
        public void setMovement(int playerIndex, Vector2 direction)
        {

            // limit direction to -1, 1
            if (direction.Length() > 1)
                direction.Normalize();

            // Get the player
            Player player = getPlayer(playerIndex);

            // if player is not in gameobjects, cancel movement
            if (player == null)
                return;

            // set movement for player
            player.MovingImpulse = direction;

        }

        /// <summary>
        /// Sets the hand position for a specified player
        /// </summary>
        /// <param name="playerIndex">Player who's hand position should be changed</param>
        /// <param name="amplitude">hand's position relative to player (x and y both between -1, 1)</param>
        public void setHandAmplitude(int playerIndex, Vector2 amplitude)
        {

            // limit hand amplitude to -1, 1
            if (amplitude.Length() > 1)
                amplitude.Normalize();

            // Get the player
            Player player = getPlayer(playerIndex);

            // if player is not in gameobjects, cancel hand amplitude
            if (player == null)
                return;

            // set hand amplitude
            if (!Physics.Instance.playerHandIsOutOfRange(player))
            {
                player.HandAmplitude = amplitude;
            }

        }

        /// <summary>
        /// Causes player to jump if possible
        /// </summary>
        /// <param name="playerIndex">Player that should jump</param>
        public void triggerJump(int playerIndex)
        {

            // Get the player
            Player player = getPlayer(playerIndex);

            // if player is not in gameobjects, cancel jump
            if (player == null)
                return;

            // if player still hast to wait for the next jump, cancel jump
            if (timeTillNextJump[playerIndex - 1] > 0)
                return;

            // there have to be jumps "left over"
            if (player.PossibleJumpsCounter > 0)
            {
                // decrease possible jumps
                player.PossibleJumpsCounter--;

                // set wating time for next jump
                timeTillNextJump[playerIndex - 1] = timeBetweenJumps;

                // set simpleJump boolean in player
                player.SimpleJump = true;
            }

        }

        /// <summary>
        /// Quits the whole game
        /// </summary>
        public void quitGame()
        {
            Game1.Instance.QuitGame();
        }

        /// <summary>
        /// Implemented, because this class is an observer
        /// Everytime an observed object has changed, this class is called
        /// </summary>
        /// <param name="observableObject">the object which changed</param>
        public void notify(ObservableObject observableObject)
        {
            Logger.Instance.log(Sender.InputManager, this.ToString() + " received a notification", PriorityLevel.Priority_2);

            //Check if we are Dealing with the right ObservableObject
            if (observableObject is SettingsManager)
            {

                timeBetweenJumps = (int)SettingsManager.Instance.get("timeBetweenJumps");
                scrollWaitingTime = (int)SettingsManager.Instance.get("scrollWaitingTime");

            }
        }


        /// <summary>
        /// Sets rumbling for every connected gamepad
        /// </summary>
        /// <param name="duration">The duration of the rumbling in MS</param>
        /// <param name="intensity">The intensity (0.0f - 1.0f) for left(X) and right(Y) motor</param>
        public void setRumble(int duration, Vector2 intensity)
        {

            rumbling_intensity = intensity;
            rumbling_remainingDuration = duration;

        }

        #endregion



        #region HelperMethods

        /// <summary>
        /// Responsible for the input handling ingame
        /// </summary>
        private void handleIngameInput()
        {
            #region For Debugging

            // Allow keyboard input when no human player is involved
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Game1.Instance.QuitGame();

            #endregion

            // go through all players in Level and process input if it's a human player
            foreach (Player player in playerList)
            {
                if (player.Status == PlayerStatus.HumanPlayer)
                {

                    // Check if the actual input device is a gamepad
                    if (player.InputDevice.Device == InputDevice.Gamepad)
                    {
                        #region Gamepad

                        // Open ingame menu
                        if ((player.InputDevice.CurrentGamePadState.Buttons.Start == ButtonState.Pressed) &&
                            (player.InputDevice.PreviousGamePadState.Buttons.Start != ButtonState.Pressed))
                            SuperController.Instance.switchFromIngameToIngameMenu(-1);


                        // Movement, Hand and Jump

                        // set moving impulse by taking value of left stick
                        if (player.MovingImpulse.X != player.InputDevice.CurrentGamePadState.ThumbSticks.Left.X ||
                            player.MovingImpulse.Y != -player.InputDevice.CurrentGamePadState.ThumbSticks.Left.Y)
                        {
                            if (!player.Flags.Contains(PlayerFlag.InvertedControl))
                                setMovement(player.PlayerIndex, new Vector2(player.InputDevice.CurrentGamePadState.ThumbSticks.Left.X,
                                    -player.InputDevice.CurrentGamePadState.ThumbSticks.Left.Y));
                            else
                                setMovement(player.PlayerIndex, new Vector2(-player.InputDevice.CurrentGamePadState.ThumbSticks.Left.X,
                                    player.InputDevice.CurrentGamePadState.ThumbSticks.Left.Y));

                        }

                        // set hand amplitude by taking value of right stick
                        if (player.HandAmplitude.X != player.InputDevice.CurrentGamePadState.ThumbSticks.Right.X ||
                            player.HandAmplitude.Y != -player.InputDevice.CurrentGamePadState.ThumbSticks.Right.Y)
                        {

                            setHandAmplitude(player.PlayerIndex, new Vector2(player.InputDevice.CurrentGamePadState.ThumbSticks.Right.X,
                                -player.InputDevice.CurrentGamePadState.ThumbSticks.Right.Y));

                        }

                        // Use the player's hand by pressing A
                        /*
                        if ((player.InputDevice.CurrentGamePadState.Buttons.A == ButtonState.Pressed) && (SuperController.Instance.getNearestBall(player.Position) != null))
                            setHandAmplitude(player.PlayerIndex, SuperController.Instance.getNearestBall(player.Position).Body.Position - player.HandBody.Position);
                        else
                            setHandAmplitude(player.PlayerIndex, Vector2.Zero);
                        */

                        //recognize Jump
                        if (!player.Flags.Contains(PlayerFlag.InvertedControl))
                        {
                            if (player.InputDevice.CurrentGamePadState.ThumbSticks.Left.Y > 0.4f)
                            {
                                triggerJump(player.PlayerIndex);
                            }
                        }
                        else
                        {
                            if (player.InputDevice.CurrentGamePadState.ThumbSticks.Left.Y < -0.4f)
                            {
                                triggerJump(player.PlayerIndex);
                            }
                        }

                        #endregion
                    }
                    // Check if the actual input device is keyboard1
                    else if ((player.InputDevice.Device == InputDevice.Keyboard) && (player.InputDevice.Index == PlayerIndex.One))
                    {
                        #region Keyboard1

                        // Quit game
                        if (currentKeyboardState.IsKeyDown(Keys.Escape) && (!previousKeyboardState.IsKeyDown(Keys.Escape)))
                            quitGame();

                        // Open ingame menu
                        if (currentKeyboardState.IsKeyDown(Keys.Back) && (!previousKeyboardState.IsKeyDown(Keys.Back)))
                            SuperController.Instance.switchFromIngameToIngameMenu(-1);

                        // Use the player's hand
                        if (currentKeyboardState.IsKeyDown(Keys.Enter) && (SuperController.Instance.getNearestBall(player.Position) != null))
                            setHandAmplitude(player.PlayerIndex, SuperController.Instance.getNearestBall(player.Position).Body.Position - player.HandBody.Position);
                        else
                            setHandAmplitude(player.PlayerIndex, Vector2.Zero);

                        // Check inverted controls
                        if (!player.Flags.Contains(PlayerFlag.InvertedControl))
                        {
                            // No inverted controls

                            // Movement
                            Vector2 tempMovementVector = Vector2.Zero;

                            if (currentKeyboardState.IsKeyDown(Keys.Down))
                                tempMovementVector += new Vector2(0, 1);
                            if (currentKeyboardState.IsKeyDown(Keys.Left))
                                tempMovementVector += new Vector2(-1, 0);
                            if (currentKeyboardState.IsKeyDown(Keys.Right))
                                tempMovementVector += new Vector2(1, 0);

                            setMovement(player.PlayerIndex, tempMovementVector);

                            // Jump
                            if (currentKeyboardState.IsKeyDown(Keys.Up))
                                triggerJump(player.PlayerIndex);

                        }
                        else
                        {
                            // Inverted controls

                            // Movement
                            Vector2 tempMovementVector = Vector2.Zero;

                            if (currentKeyboardState.IsKeyDown(Keys.Up))
                                tempMovementVector += new Vector2(0, 1);
                            if (currentKeyboardState.IsKeyDown(Keys.Right))
                                tempMovementVector += new Vector2(-1, 0);
                            if (currentKeyboardState.IsKeyDown(Keys.Left))
                                tempMovementVector += new Vector2(1, 0);

                            setMovement(player.PlayerIndex, tempMovementVector);

                            // Jump
                            if (currentKeyboardState.IsKeyDown(Keys.Down))
                                triggerJump(player.PlayerIndex);

                        }

                        #endregion
                    }
                    // Check if the actual input device is the keyboard2
                    else if ((player.InputDevice.Device == InputDevice.Keyboard) && (player.InputDevice.Index == PlayerIndex.Two))
                    {
                        #region Keyboard2

                        // Quit game
                        if (currentKeyboardState.IsKeyDown(Keys.Escape) && (!previousKeyboardState.IsKeyDown(Keys.Escape)))
                            quitGame();

                        // Open ingame menu
                        if (currentKeyboardState.IsKeyDown(Keys.Back) && (!previousKeyboardState.IsKeyDown(Keys.Back)))
                            SuperController.Instance.switchFromIngameToIngameMenu(-1);

                        // Use the player's hand
                        if (currentKeyboardState.IsKeyDown(Keys.Space) && (SuperController.Instance.getNearestBall(player.Position) != null))
                            setHandAmplitude(player.PlayerIndex, SuperController.Instance.getNearestBall(player.Position).Body.Position - player.HandBody.Position);
                        else
                            setHandAmplitude(player.PlayerIndex, Vector2.Zero);


                        // Check inverted controls
                        if (!player.Flags.Contains(PlayerFlag.InvertedControl))
                        {
                            // No inverted controls

                            // Movement
                            Vector2 tempMovementVector = Vector2.Zero;

                            if (currentKeyboardState.IsKeyDown(Keys.S))
                                tempMovementVector += new Vector2(0, 1);
                            if (currentKeyboardState.IsKeyDown(Keys.A))
                                tempMovementVector += new Vector2(-1, 0);
                            if (currentKeyboardState.IsKeyDown(Keys.D))
                                tempMovementVector += new Vector2(1, 0);

                            setMovement(player.PlayerIndex, tempMovementVector);

                            // Jump
                            if (currentKeyboardState.IsKeyDown(Keys.W))
                                triggerJump(player.PlayerIndex);

                        }
                        else
                        {
                            // Inverted controls

                            // Movement
                            Vector2 tempMovementVector = Vector2.Zero;

                            if (currentKeyboardState.IsKeyDown(Keys.W))
                                tempMovementVector += new Vector2(0, 1);
                            if (currentKeyboardState.IsKeyDown(Keys.D))
                                tempMovementVector += new Vector2(-1, 0);
                            if (currentKeyboardState.IsKeyDown(Keys.A))
                                tempMovementVector += new Vector2(1, 0);

                            setMovement(player.PlayerIndex, tempMovementVector);

                            // Jump
                            if (currentKeyboardState.IsKeyDown(Keys.S))
                                triggerJump(player.PlayerIndex);

                        }

                        #endregion
                    }

                }

            }

        }

        /// <summary>
        /// Responsible for the input handling in the main menu
        /// </summary>
        private void handleMainMenuInput()
        {

           #region Gamepads

            // go through all gamepads
            foreach (Gamepad gamepad in gamepadArray)
            {
                if (gamepad.IsConnected)
                {

                    // "Up" on DPad or left/right analog stick pressed

                    if (gamepad.CurrentGamePadState.DPad.Up == ButtonState.Pressed)
                    {
                        if (gamepad.PreviousGamePadState.DPad.Up == ButtonState.Pressed)
                        {
                            if (timeTillNextScrollAction == 0)
                            {
                                SuperController.Instance.MainMenuInstance.upButtonPressed();
                                timeTillNextScrollAction = scrollWaitingTime;
                            }
                        }
                        else
                        {
                            SuperController.Instance.MainMenuInstance.upButtonPressed();
                            timeTillNextScrollAction = scrollWaitingTime;
                        }
                    }

                    if (gamepad.CurrentGamePadState.ThumbSticks.Left.Y > 0.5f)
                    {
                        if (gamepad.PreviousGamePadState.ThumbSticks.Left.Y > 0.5f)
                        {
                            if (timeTillNextScrollAction == 0)
                            {
                                SuperController.Instance.MainMenuInstance.upButtonPressed();
                                timeTillNextScrollAction = scrollWaitingTime;
                            }
                        }
                        else
                        {
                            SuperController.Instance.MainMenuInstance.upButtonPressed();
                            timeTillNextScrollAction = scrollWaitingTime;
                        }
                    }

                    if (gamepad.CurrentGamePadState.ThumbSticks.Right.Y > 0.5f)
                    {
                        if (gamepad.PreviousGamePadState.ThumbSticks.Right.Y > 0.5f)
                        {
                            if (timeTillNextScrollAction == 0)
                            {
                                SuperController.Instance.MainMenuInstance.upButtonPressed();
                                timeTillNextScrollAction = scrollWaitingTime;
                            }
                        }
                        else
                        {
                            SuperController.Instance.MainMenuInstance.upButtonPressed();
                            timeTillNextScrollAction = scrollWaitingTime;
                        }
                    }


                    // "Down" on DPad or left/right analog stick pressed
                    if (gamepad.CurrentGamePadState.DPad.Down == ButtonState.Pressed)
                    {
                        if (gamepad.PreviousGamePadState.DPad.Down == ButtonState.Pressed)
                        {
                            if (timeTillNextScrollAction == 0)
                            {
                                SuperController.Instance.MainMenuInstance.downButtonPressed();
                                timeTillNextScrollAction = scrollWaitingTime;
                            }
                        }
                        else
                        {
                            SuperController.Instance.MainMenuInstance.downButtonPressed();
                            timeTillNextScrollAction = scrollWaitingTime;
                        }
                    }

                    if (gamepad.CurrentGamePadState.ThumbSticks.Left.Y < -0.5f)
                    {
                        if (gamepad.PreviousGamePadState.ThumbSticks.Left.Y < -0.5f)
                        {
                            if (timeTillNextScrollAction == 0)
                            {
                                SuperController.Instance.MainMenuInstance.downButtonPressed();
                                timeTillNextScrollAction = scrollWaitingTime;
                            }
                        }
                        else
                        {
                            SuperController.Instance.MainMenuInstance.downButtonPressed();
                            timeTillNextScrollAction = scrollWaitingTime;
                        }
                    }

                    if (gamepad.CurrentGamePadState.ThumbSticks.Right.Y < -0.5f)
                    {
                        if (gamepad.PreviousGamePadState.ThumbSticks.Right.Y < -0.5f)
                        {
                            if (timeTillNextScrollAction == 0)
                            {
                                SuperController.Instance.MainMenuInstance.downButtonPressed();
                                timeTillNextScrollAction = scrollWaitingTime;
                            }
                        }
                        else
                        {
                            SuperController.Instance.MainMenuInstance.downButtonPressed();
                            timeTillNextScrollAction = scrollWaitingTime;
                        }
                    }


                    // "Left" on DPad or left/right analog stick pressed
                    if ((gamepad.CurrentGamePadState.DPad.Left == ButtonState.Pressed) &&
                        (gamepad.PreviousGamePadState.DPad.Left != ButtonState.Pressed))
                        SuperController.Instance.MainMenuInstance.leftButtonPressed(InputDevice.Gamepad, gamepad.Index);
                    if ((gamepad.CurrentGamePadState.ThumbSticks.Left.X < -0.5f) &&
                        (gamepad.PreviousGamePadState.ThumbSticks.Left.X > -0.5f))
                        SuperController.Instance.MainMenuInstance.leftButtonPressed(InputDevice.Gamepad, gamepad.Index);
                    if ((gamepad.CurrentGamePadState.ThumbSticks.Right.X < -0.5f) &&
                        (gamepad.PreviousGamePadState.ThumbSticks.Right.X > -0.5f))
                        SuperController.Instance.MainMenuInstance.leftButtonPressed(InputDevice.Gamepad, gamepad.Index);

                    // "Right" on DPad or left/right analog stick pressed
                    if ((gamepad.CurrentGamePadState.DPad.Right == ButtonState.Pressed) &&
                        (gamepad.PreviousGamePadState.DPad.Right != ButtonState.Pressed))
                        SuperController.Instance.MainMenuInstance.rightButtonPressed(InputDevice.Gamepad, gamepad.Index);
                    if ((gamepad.CurrentGamePadState.ThumbSticks.Left.X > 0.5f) &&
                        (gamepad.PreviousGamePadState.ThumbSticks.Left.X < 0.5f))
                        SuperController.Instance.MainMenuInstance.rightButtonPressed(InputDevice.Gamepad, gamepad.Index);
                    if ((gamepad.CurrentGamePadState.ThumbSticks.Right.X > 0.5f) &&
                        (gamepad.PreviousGamePadState.ThumbSticks.Right.X < 0.5f))
                        SuperController.Instance.MainMenuInstance.rightButtonPressed(InputDevice.Gamepad, gamepad.Index);

                    // "A" pressed
                    if ((gamepad.CurrentGamePadState.Buttons.A == ButtonState.Pressed) &&
                        (gamepad.PreviousGamePadState.Buttons.A != ButtonState.Pressed))
                        SuperController.Instance.MainMenuInstance.enterButtonPressed();

                    // "B" pressed
                    if ((gamepad.CurrentGamePadState.Buttons.B == ButtonState.Pressed) &&
                        (gamepad.PreviousGamePadState.Buttons.B != ButtonState.Pressed))
                        SuperController.Instance.MainMenuInstance.backButtonPressed();

                    
                }
            }

            #endregion

           #region Keyboard

            // Quit game
            if (currentKeyboardState.IsKeyDown(Keys.Escape) && (!previousKeyboardState.IsKeyDown(Keys.Escape)))
                Game1.Instance.QuitGame();

            // "Enter" button pressed
            if (currentKeyboardState.IsKeyDown(Keys.Enter) && (!previousKeyboardState.IsKeyDown(Keys.Enter)))
                SuperController.Instance.MainMenuInstance.enterButtonPressed();

            // "Backspace" button pressed
            if (currentKeyboardState.IsKeyDown(Keys.Back) && (!previousKeyboardState.IsKeyDown(Keys.Back)))
                SuperController.Instance.MainMenuInstance.backButtonPressed();

            #region Keyboard1 (Arrow-Keys)

            // "Up" button pressed
            if (currentKeyboardState.IsKeyDown(Keys.Up))
            {
                if (previousKeyboardState.IsKeyDown(Keys.Up))
                {
                    if (timeTillNextScrollAction == 0)
                    {
                        SuperController.Instance.MainMenuInstance.upButtonPressed();
                        timeTillNextScrollAction = scrollWaitingTime;
                    }
                }
                else
                {
                    SuperController.Instance.MainMenuInstance.upButtonPressed();
                    timeTillNextScrollAction = scrollWaitingTime;
                }
            }


            // "Down" button pressed
            if (currentKeyboardState.IsKeyDown(Keys.Down))
            {
                if (previousKeyboardState.IsKeyDown(Keys.Down))
                {
                    if (timeTillNextScrollAction == 0)
                    {
                        SuperController.Instance.MainMenuInstance.downButtonPressed();
                        timeTillNextScrollAction = scrollWaitingTime;
                    }
                }
                else
                {
                    SuperController.Instance.MainMenuInstance.downButtonPressed();
                    timeTillNextScrollAction = scrollWaitingTime;
                }
            }

            // "Left" button pressed
            if (currentKeyboardState.IsKeyDown(Keys.Left) && (!previousKeyboardState.IsKeyDown(Keys.Left)))
                SuperController.Instance.MainMenuInstance.leftButtonPressed(InputDevice.Keyboard, PlayerIndex.One);

            // "Right" button pressed
            if (currentKeyboardState.IsKeyDown(Keys.Right) && (!previousKeyboardState.IsKeyDown(Keys.Right)))
                SuperController.Instance.MainMenuInstance.rightButtonPressed(InputDevice.Keyboard, PlayerIndex.One);

            #endregion

            #region Keyboard2 (WASD)

            // "Up" button pressed
            if (currentKeyboardState.IsKeyDown(Keys.W))
            {
                if (previousKeyboardState.IsKeyDown(Keys.W))
                {
                    if (timeTillNextScrollAction == 0)
                    {
                        SuperController.Instance.MainMenuInstance.upButtonPressed();
                        timeTillNextScrollAction = scrollWaitingTime;
                    }
                }
                else
                {
                    SuperController.Instance.MainMenuInstance.upButtonPressed();
                    timeTillNextScrollAction = scrollWaitingTime;
                }
            }


            // "Down" button pressed
            if (currentKeyboardState.IsKeyDown(Keys.S))
            {
                if (previousKeyboardState.IsKeyDown(Keys.S))
                {
                    if (timeTillNextScrollAction == 0)
                    {
                        SuperController.Instance.MainMenuInstance.downButtonPressed();
                        timeTillNextScrollAction = scrollWaitingTime;
                    }
                }
                else
                {
                    SuperController.Instance.MainMenuInstance.downButtonPressed();
                    timeTillNextScrollAction = scrollWaitingTime;
                }
            }

            // "Left" button pressed
            if (currentKeyboardState.IsKeyDown(Keys.A) && (!previousKeyboardState.IsKeyDown(Keys.A)))
                SuperController.Instance.MainMenuInstance.leftButtonPressed(InputDevice.Keyboard, PlayerIndex.Two);

            // "Right" button pressed
            if (currentKeyboardState.IsKeyDown(Keys.D) && (!previousKeyboardState.IsKeyDown(Keys.D)))
                SuperController.Instance.MainMenuInstance.rightButtonPressed(InputDevice.Keyboard, PlayerIndex.Two);

            #endregion

           #endregion

        }

        /// <summary>
        /// Responsible for the input handling in the ingame menu
        /// </summary>
        private void handleIngameMenuInput()
        {

            #region Gamepads

            // go through all gamepads
            foreach (Gamepad gamepad in gamepadArray)
            {
                if (gamepad.IsConnected)
                {

                    // Back button pressed
                    if ((gamepad.CurrentGamePadState.Buttons.B == ButtonState.Pressed) &&
                        (gamepad.PreviousGamePadState.Buttons.B != ButtonState.Pressed))
                        SuperController.Instance.IngameMenuInstance.backButtonPressed();
                    if ((gamepad.CurrentGamePadState.Buttons.Start == ButtonState.Pressed) &&
                        (gamepad.PreviousGamePadState.Buttons.Start != ButtonState.Pressed))
                        SuperController.Instance.IngameMenuInstance.backButtonPressed();

                    // "Up" on DPad or left/right analog stick pressed

                    if (gamepad.CurrentGamePadState.DPad.Up == ButtonState.Pressed)
                    {
                        if (gamepad.PreviousGamePadState.DPad.Up == ButtonState.Pressed)
                        {
                            if (timeTillNextScrollAction == 0)
                            {
                                SuperController.Instance.IngameMenuInstance.upButtonPressed();
                                timeTillNextScrollAction = scrollWaitingTime;
                            }
                        }
                        else
                        {
                            SuperController.Instance.IngameMenuInstance.upButtonPressed();
                            timeTillNextScrollAction = scrollWaitingTime;
                        }
                    }

                    if (gamepad.CurrentGamePadState.ThumbSticks.Left.Y > 0.5f)
                    {
                        if (gamepad.PreviousGamePadState.ThumbSticks.Left.Y > 0.5f)
                        {
                            if (timeTillNextScrollAction == 0)
                            {
                                SuperController.Instance.IngameMenuInstance.upButtonPressed();
                                timeTillNextScrollAction = scrollWaitingTime;
                            }
                        }
                        else
                        {
                            SuperController.Instance.IngameMenuInstance.upButtonPressed();
                            timeTillNextScrollAction = scrollWaitingTime;
                        }
                    }

                    if (gamepad.CurrentGamePadState.ThumbSticks.Right.Y > 0.5f)
                    {
                        if (gamepad.PreviousGamePadState.ThumbSticks.Right.Y > 0.5f)
                        {
                            if (timeTillNextScrollAction == 0)
                            {
                                SuperController.Instance.IngameMenuInstance.upButtonPressed();
                                timeTillNextScrollAction = scrollWaitingTime;
                            }
                        }
                        else
                        {
                            SuperController.Instance.IngameMenuInstance.upButtonPressed();
                            timeTillNextScrollAction = scrollWaitingTime;
                        }
                    }


                    // "Down" on DPad or left/right analog stick pressed
                    if (gamepad.CurrentGamePadState.DPad.Down == ButtonState.Pressed)
                    {
                        if (gamepad.PreviousGamePadState.DPad.Down == ButtonState.Pressed)
                        {
                            if (timeTillNextScrollAction == 0)
                            {
                                SuperController.Instance.IngameMenuInstance.downButtonPressed();
                                timeTillNextScrollAction = scrollWaitingTime;
                            }
                        }
                        else
                        {
                            SuperController.Instance.IngameMenuInstance.downButtonPressed();
                            timeTillNextScrollAction = scrollWaitingTime;
                        }
                    }

                    if (gamepad.CurrentGamePadState.ThumbSticks.Left.Y < -0.5f)
                    {
                        if (gamepad.PreviousGamePadState.ThumbSticks.Left.Y < -0.5f)
                        {
                            if (timeTillNextScrollAction == 0)
                            {
                                SuperController.Instance.IngameMenuInstance.downButtonPressed();
                                timeTillNextScrollAction = scrollWaitingTime;
                            }
                        }
                        else
                        {
                            SuperController.Instance.IngameMenuInstance.downButtonPressed();
                            timeTillNextScrollAction = scrollWaitingTime;
                        }
                    }

                    if (gamepad.CurrentGamePadState.ThumbSticks.Right.Y < -0.5f)
                    {
                        if (gamepad.PreviousGamePadState.ThumbSticks.Right.Y < -0.5f)
                        {
                            if (timeTillNextScrollAction == 0)
                            {
                                SuperController.Instance.IngameMenuInstance.downButtonPressed();
                                timeTillNextScrollAction = scrollWaitingTime;
                            }
                        }
                        else
                        {
                            SuperController.Instance.IngameMenuInstance.downButtonPressed();
                            timeTillNextScrollAction = scrollWaitingTime;
                        }
                    }

                    // Enter button pressed
                    if ((gamepad.CurrentGamePadState.Buttons.A == ButtonState.Pressed) &&
                        (gamepad.PreviousGamePadState.Buttons.A != ButtonState.Pressed))
                    {
                        SuperController.Instance.IngameMenuInstance.enterButtonPressed();
                    }

                    // Revanche button pressed
                    if ((gamepad.CurrentGamePadState.Buttons.Y == ButtonState.Pressed) &&
                        (gamepad.PreviousGamePadState.Buttons.Y != ButtonState.Pressed))
                    {
                        SuperController.Instance.IngameMenuInstance.revancheButtonPressed();
                    }

                }

            }

            #endregion

            #region Keyboard

            // Quit game
            if (currentKeyboardState.IsKeyDown(Keys.Escape) && (!previousKeyboardState.IsKeyDown(Keys.Escape)))
                Game1.Instance.QuitGame();

            // Back button pressed
            if (currentKeyboardState.IsKeyDown(Keys.Back) && (!previousKeyboardState.IsKeyDown(Keys.Back)))
                SuperController.Instance.IngameMenuInstance.backButtonPressed();

            // "Up" button pressed
            if (currentKeyboardState.IsKeyDown(Keys.Up))
            {
                if (previousKeyboardState.IsKeyDown(Keys.Up))
                {
                    if (timeTillNextScrollAction == 0)
                    {
                        SuperController.Instance.IngameMenuInstance.upButtonPressed();
                        timeTillNextScrollAction = scrollWaitingTime;
                    }
                }
                else
                {
                    SuperController.Instance.IngameMenuInstance.upButtonPressed();
                    timeTillNextScrollAction = scrollWaitingTime;
                }
            }

            // "Down" button pressed
            if (currentKeyboardState.IsKeyDown(Keys.Down))
            {
                if (previousKeyboardState.IsKeyDown(Keys.Down))
                {
                    if (timeTillNextScrollAction == 0)
                    {
                        SuperController.Instance.IngameMenuInstance.downButtonPressed();
                        timeTillNextScrollAction = scrollWaitingTime;
                    }
                }
                else
                {
                    SuperController.Instance.IngameMenuInstance.downButtonPressed();
                    timeTillNextScrollAction = scrollWaitingTime;
                }
            }

            // Enter button pressed
            if (currentKeyboardState.IsKeyDown(Keys.Enter) && (!previousKeyboardState.IsKeyDown(Keys.Enter)))
            {
                SuperController.Instance.IngameMenuInstance.enterButtonPressed();
            }

            // Revanche
            if (currentKeyboardState.IsKeyDown(Keys.Y) && (!previousKeyboardState.IsKeyDown(Keys.Y)))
            {
                SuperController.Instance.IngameMenuInstance.revancheButtonPressed();
            }

            #endregion

        }

        /// <summary>
        /// Checks if one of the 4 gamepads is connected/disconnected now
        /// and updates their states
        /// </summary>
        private void updateGamepads()
        {

            // go through all possible gamepads
            for (int i = 0; i < gamepadArray.Length; i++)
            {
                
                if (GamePad.GetState(gamepadArray[i].Index).IsConnected)
                {
                    // Gamepad is connected
                    gamepadArray[i].IsConnected = true;

                    // update states
                    gamepadArray[i].PreviousGamePadState = gamepadArray[i].CurrentGamePadState;
                    gamepadArray[i].CurrentGamePadState = GamePad.GetState(gamepadArray[i].Index);

                }
                else
                {
                    // Gamepad is disconnected
                    gamepadArray[i].IsConnected = false;
                }

            }

        }

        /// <summary>
        /// Goes through all RumbleActions, updates them,
        /// lets rumble the gamepads
        /// </summary>
        /// <param name="gameTime"></param>
        private void handleRumbling(GameTime gameTime)
        {

            rumbling_remainingDuration -= gameTime.ElapsedGameTime.Milliseconds;

            if (rumbling_remainingDuration < 0)
            {
                rumbling_intensity = Vector2.Zero;
                rumbling_remainingDuration = 0;
            }

            // Let gamepads rumble
            GamePad.SetVibration(PlayerIndex.One, rumbling_intensity.X, rumbling_intensity.Y);
            GamePad.SetVibration(PlayerIndex.Two, rumbling_intensity.X, rumbling_intensity.Y);
            GamePad.SetVibration(PlayerIndex.Three, rumbling_intensity.X, rumbling_intensity.Y);
            GamePad.SetVibration(PlayerIndex.Four, rumbling_intensity.X, rumbling_intensity.Y);
            
        }

        /// <summary>
        /// Returns the specified player
        /// </summary>
        /// <param name="playerIndex">Index of the player which should be returned</param>
        /// <returns>the player with the specified index, if not in list returns a null player</returns>
        private Player getPlayer(int playerIndex)
        {
            Player player = null;

            // go through all player and check if the one with
            // the right index is in the list
            foreach (Player temp_player in playerList)
            {
                if (temp_player.PlayerIndex == playerIndex)
                {
                    player = temp_player;
                    break;
                }
            }

            return player;
        }


        #endregion


        /// <summary>
        /// Does all necessary action, to bring controller back to the state after initialization
        /// </summary>
        public void clear()
        {
            playerList = null;
            
        }

        /// <summary>
        /// Not possible for InputManager
        /// </summary>
        /// <param name="on">On if paused</param>
        public void pause(bool on)
        {
            throw new NotImplementedException();
        }


    }

}
