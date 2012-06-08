using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

using ManhattanMorning.Model;
using ManhattanMorning.Controller;
using ManhattanMorning.Model.Menu;
using ManhattanMorning.Model.GameObject;
using ManhattanMorning.Misc.Logic;

namespace ManhattanMorning.Misc
{
    /// <summary>
    /// Reprensents the main menu
    /// </summary>
    class MainMenuInstance
    {

        #region Properties

        /// <summary>
        /// List of all IngameMenu objects
        /// </summary>
        public LayerList<LayerInterface> MenuObjectList
        {
            get { return this.menuObjectList; }
            set { this.menuObjectList = value; }
        }

        /// <summary>
        /// The Intro Video.
        /// </summary>
        public Video IntroVideo
        {
            get { return introVideo; }
            set { introVideo = value; }
        }

        #endregion

        #region Members

        /// <summary>
        /// List of all menu objects
        /// </summary>
        private LayerList<LayerInterface> menuObjectList;

        /// <summary>
        /// Indicates in which state the main menu is:
        /// 0: main screen
        /// 1: team menu 1 vs 1
        /// 2: team menu 2 vs 2
        /// 3: help
        /// 4: really quit
        /// 5: select level
        /// </summary>
        private int menuState;

        /// <summary>
        /// Saves references to all objects for the different states
        /// </summary>
        private List<LayerInterface>[,] menuStructure;

        /// <summary>
        /// Indicates the selected menu item (always start with 1!)
        /// </summary>
        private int selectedItem;

        /// <summary>
        /// Name of the selected level
        /// (to initialize GameInstance)
        /// </summary>
        private String levelName;

        /// <summary>
        /// Level's win condition
        /// (to initialize GameInstance)
        /// </summary>
        private WinCondition winCondition;

        /// <summary>
        /// Value of the win condition
        /// Represents be either the points or the minutes
        /// </summary>
        private int timePointsValue;

        /// <summary>
        /// Reference to the levels class
        /// Needed for levelpreviews which are stored in that class
        /// </summary>
        private Levels.Levels levels;

        /// <summary>
        /// Size of the levelpreview that is selected
        /// </summary>
        private Vector2 sizeSelectedLevelPreview = new Vector2(300f / 1280f, 300f / 720f);

        /// <summary>
        /// Position of the levelpreview that is selected
        /// </summary>
        private Vector2 positionSelectedLevelPreview = new Vector2(490f / 1280f, 210f / 720f);

        /// <summary>
        /// Index of the selected level preview
        /// </summary>
        private int selectedLevelPreviewIndex;

        /// <summary>
        /// Necessary to switch back from select level to the right team menu
        /// </summary>
        private int previousMenuState;

        /// <summary>
        /// Necessary to have the right item selected when switching back to main screen
        /// </summary>
        private int previousMainScreenSelectedItem;

        /// <summary>
        /// Position for the player icon in team menu when belonging to the left team
        /// </summary>
        private float xPositionLeftTeam = 0.20f;

        /// <summary>
        /// Position for the player icon in team menu when belonging to no team
        /// </summary>
        private float xPositionNoTeam = 0.455f;

        /// <summary>
        /// Position for the player icon in team menu when belonging to the right team
        /// </summary>
        private float xPositionRightTeam = 0.71f;

        /// <summary>
        /// yPositions for all PlayerRepresentations when playing 1vs1
        /// </summary>
        private float[] yPositionsPlayerRepresentations1vs1 = { 0.33f, 0.5f , 0f, 0f};

        /// <summary>
        /// yPositions for all PlayerRepresentations when playing 2vs2
        /// </summary>
        private float[] yPositionsPlayerRepresentations2vs2 = { 0.25f, 0.40f, 0.55f, 0.70f };

        /// <summary>
        /// The Intro Video.
        /// </summary>
        private Video introVideo;

        /// <summary>
        /// The time in MS since the last button was pressed
        /// Necessary to fade in button instructions at the right time
        /// </summary>
        private int timeSinceLastInput = 1;

        /// <summary>
        /// The timespan in MS after which no input leads to an overlay
        /// </summary>
        private int timeTillOverlayAppears = 2500;

        /// <summary>
        /// The overlayObject that could be shown in the current menuState
        /// </summary>
        private MenuObject overlayObject;

        /// <summary>
        /// Array with all possible player representations
        /// </summary>
        private PlayerRepresentationMainMenu[] playerArray = new PlayerRepresentationMainMenu[6];

        /// <summary>
        /// True if a 2vs2 game should be started
        /// </summary>
        private bool gameMode2vs2;

        /// <summary>
        /// List of all active player indexes
        /// </summary>
        private List<int> activePlayers = new List<int>();

        #endregion


        #region Initialization

        /// <summary>
        /// Initializes the main menu
        /// </summary>
        /// <param name="levels">Reference to the levels class to get all level previews</param>
        public MainMenuInstance(Levels.Levels levels)
        {

            // Save attributes
            this.levels = levels;

            // Load the objects
            menuObjectList = StorageManager.Instance.LoadMainMenuObjects();

            // Add the levelpreviews to the mainMenuObjectList
            foreach (LayerInterface levelPreview in levels.LevelPreviews)
                menuObjectList.Add(levelPreview);

            // Create the structure
            createMenuStructure();

            // Create all PlayerRepresentations:
            // 4 Gamepads
            for (int i = 0; i < 4; i++)
                playerArray[i] = new PlayerRepresentationMainMenu((MenuObject)menuObjectList.GetObjectByName("TeamMenu_Gamepad" + (i+1)),
                    InputManager.Instance.GamepadArray[i], false, i+1);
            // 2 Keyboards
            playerArray[4] = new PlayerRepresentationMainMenu((MenuObject)menuObjectList.GetObjectByName("TeamMenu_Keyboard1"),
                    new Gamepad(PlayerIndex.One, InputDevice.Keyboard, true), false, 1);
            playerArray[5] = new PlayerRepresentationMainMenu((MenuObject)menuObjectList.GetObjectByName("TeamMenu_Keyboard2"),
                    new Gamepad(PlayerIndex.Two, InputDevice.Keyboard, true), false, 2);

            // set all necessary members
            selectedItem = 0;
            menuState = 0;
            previousMenuState = 0;
            previousMainScreenSelectedItem = 0;

            levelName = "";

            // Play Intro
            activateIntro();

            activateMenu();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Displays the wright screen
        /// </summary>
        public void activateMenu()
        {

            // First, make all objects invisible
            for (int i = 0; i < menuStructure.GetLength(0); i++)
                for (int j = 0; j < menuStructure.GetLength(1); j++)
                    if (menuStructure[i, j] != null)
                        foreach (LayerInterface menuObject in menuStructure[i, j])
                            ((DrawableObject)menuObject).Visible = false;

            for (int i = 0; i < levels.LevelPreviews.Count; i++)
            {
                ((MenuButtonObject)levels.LevelPreviews[i]).Visible = false;
                ((MenuButtonObject)levels.LevelPreviews[i]).FadingAnimation = null;
            }

            menuState = 0;
            selectedItem = 0;
            overlayObject = (MenuObject)menuObjectList.GetObjectByName("MainScreen_Overlay");

            // Make all necessary menu objects visible
            for (int i = 0; i < menuStructure.GetLength(1); i++)
            {
                if (menuStructure[0, i] != null)
                {
                    foreach (LayerInterface menuObject in menuStructure[0, i])
                    {
                        ((DrawableObject)menuObject).Visible = true;
                        // Unhighlight all buttons
                        if (i > 0)
                            ((MenuButtonObject)menuObject).Selected = false;
                    }
                }
            }

        }

        /// <summary>
        /// Updates the main menu
        /// </summary>
        /// <param name="gameTime">The gametime</param>
        /// <param name="gamepadArray">Array with all gamepads, independent from
        /// being connected or disconnected</param>
        public void update(GameTime gameTime, Gamepad[] gamepadArray)
        {

            if ((menuState == 1) || (menuState == 2))
            {
                updateTeamMenu();
            }

            manageOverlays(gameTime);

        }

        /// <summary>
        /// Checks if gamepads are connected/disconnected now
        /// and updates the teamplayMenu according to these changes
        /// </summary>
        private void updateTeamMenu()
        {

            // List of all active players indexes
            activePlayers.Clear();

            // 1.Deactivate all players
            for (int i = 0; i < playerArray.Length; i++)
            {
                playerArray[i].Active = false;
                playerArray[i].PlayerPicture.Visible = false;
            }

            #region 2. Update Gamepads

            for (int i = 0; i < 4; i++)
            {

                // Check if gamepad that belongs to the player is connected
                if (InputManager.Instance.GamepadArray[i].IsConnected == true)
                {
                    // Check if there is still a player needed
                    if (((activePlayers.Count < 2) && (menuState == 1)) ||
                        (activePlayers.Count < 4) && (menuState == 2))
                    {
                        // Activate Player

                        // Check if player was already connected
                        // If not do the following
                        if (playerArray[i].Connected == false)
                        {
                            playerArray[i].Connected = true;
                            playerArray[i].Team = 0;
                            playerArray[i].setXPosition(xPositionNoTeam);

                        }

                        // If active, update y-Position and show picture
                        if (menuState == 1)
                            playerArray[i].setYPosition(yPositionsPlayerRepresentations1vs1[activePlayers.Count]);
                        else
                            playerArray[i].setYPosition(yPositionsPlayerRepresentations2vs2[activePlayers.Count]);

                        playerArray[i].PlayerPicture.Visible = true;
                        playerArray[i].Active = true;
                        activePlayers.Add(i+1);
                    }
                    else
                    {
                        // Deactivate Player
                        playerArray[i].Team = 0;
                        playerArray[i].setXPosition(xPositionNoTeam);
                    }

                }
                // If gamepad was connected and is no more connected, deactivate PlayerRepresentation
                else
                {
                    if (playerArray[i].Connected == true)
                    {
                        playerArray[i].Connected = false;
                        playerArray[i].Active = false;
                        playerArray[i].Team = 0;
                        playerArray[i].setXPosition(xPositionNoTeam);
                        playerArray[i].PlayerPicture.Visible = false;
                    }
                
                }
            

            }

            #endregion

            
            #region 3. Update Keyboards
            #if WINDOWS

            for (int i = 4; i < 6; i++)
            {

                // Check if there is still a player needed
                if (((activePlayers.Count < 2) && (menuState == 1)) ||
                    (activePlayers.Count < 4) && (menuState == 2))
                {
                        // Activate Player

                        // Check if player was already connected
                        // If not do the following
                        if (playerArray[i].Connected == false)
                        {
                            playerArray[i].Connected = true;
                            playerArray[i].Team = 0;
                            playerArray[i].setXPosition(xPositionNoTeam);

                        }

                        // If active, update y-Position and show picture
                        if (menuState == 1)
                            playerArray[i].setYPosition(yPositionsPlayerRepresentations1vs1[activePlayers.Count]);
                        else
                            playerArray[i].setYPosition(yPositionsPlayerRepresentations2vs2[activePlayers.Count]);

                        playerArray[i].PlayerPicture.Visible = true;
                        playerArray[i].Active = true;

                        // Set index by finding the first free one
                        int index = 0;
                        for (int j = 1; j < 5; j++)
                        {
                            if (!activePlayers.Contains(j))
                            {
                                index = j;
                                break;
                            }
                        }
                        playerArray[i].PlayerIndex = index;                      
                        activePlayers.Add(index);

                    }
                    else
                    {
                        // Deactivate Player
                        playerArray[i].Team = 0;
                        playerArray[i].setXPosition(xPositionNoTeam);
                        playerArray[i].Connected = false;
                    }
            
            

            }

            #endif
            #endregion
            
            #region 4. Show KI Gameobjects left and right

            int countLeftTeam = getNumberOfTeamMembers(1);
            int countRightTeam = getNumberOfTeamMembers(2);
            int kiCounter = 0;

            // First, hide all KIObjects
            for (int i = 1; i < 5; i++)
            {
                ((MenuObject)menuObjectList.GetObjectByName("TeamMenu_KI" + i)).Visible = false;
            }

            // Go through all player representations
            for (int i = 0; i < playerArray.Length; i++)
            {

                // If a player is active but has no team, show a KI player at the right side
                if ((playerArray[i].Active) && (playerArray[i].Team == 0))
                {
                    // Left
                    if (((countLeftTeam < 1) && (menuState == 1)) ||
                        ((countLeftTeam < 2) && (menuState == 2)))
                    {
                        countLeftTeam++;
                        kiCounter++;
                        MenuObject KIObject = (MenuObject)menuObjectList.GetObjectByName("TeamMenu_KI" + kiCounter);
                        KIObject.Visible = true;
                        KIObject.Position = new Vector2(xPositionLeftTeam, playerArray[i].PlayerPicture.Position.Y);
                    }
                    // Right
                    else
                    {
                        countRightTeam++;
                        kiCounter++;
                        MenuObject KIObject = (MenuObject)menuObjectList.GetObjectByName("TeamMenu_KI" + kiCounter);
                        KIObject.Visible = true;
                        KIObject.Position = new Vector2(xPositionRightTeam, playerArray[i].PlayerPicture.Position.Y);
                    }
                }

            }

            #endregion

            #region 4. Fill with Deactivated Gamepads and the belonging KI textures

            int limit = 0;
            if (menuState == 1)
                limit = 2;
            else
                limit = 4;

            // First, make them all invisible
            for (int i = 1; i < 5; i++)
            {

                MenuObject deactivatedGamepad = (MenuObject)menuObjectList.GetObjectByName("TeamMenu_Gamepad_Deactivated" + i);
                deactivatedGamepad.Visible = false;

            }

            // Get each position from down to the top where no gamepad is connected
            for (int i = limit; i > getNumberOfConnectedDevices(); i--)
            {

                // Get yPosition
                float yPosition;
                if (menuState == 1)
                    yPosition = yPositionsPlayerRepresentations1vs1[i - 1];
                else
                    yPosition = yPositionsPlayerRepresentations2vs2[i - 1];

                // Get the reference to the Gameobject
                MenuObject deactivatedGamepad = (MenuObject)menuObjectList.GetObjectByName("TeamMenu_Gamepad_Deactivated" + i);

                deactivatedGamepad.Visible = true;
                deactivatedGamepad.Position = new Vector2(xPositionNoTeam, yPosition);


                // Also show a KI texture on the right side
                // Left
                if (((countLeftTeam < 1) && (menuState == 1)) ||
                    ((countLeftTeam < 2) && (menuState == 2)))
                {
                    countLeftTeam++;
                    kiCounter++;
                    MenuObject KIObject = (MenuObject)menuObjectList.GetObjectByName("TeamMenu_KI" + kiCounter);
                    KIObject.Visible = true;
                    KIObject.Position = new Vector2(xPositionLeftTeam, deactivatedGamepad.Position.Y);
                }
                // Right
                else
                {
                    countRightTeam++;
                    kiCounter++;
                    MenuObject KIObject = (MenuObject)menuObjectList.GetObjectByName("TeamMenu_KI" + kiCounter);
                    KIObject.Visible = true;
                    KIObject.Position = new Vector2(xPositionRightTeam, deactivatedGamepad.Position.Y);
                }

            }

            #endregion

        }

        /// <summary>
        /// Triggers all necessary actions to show the intro
        /// </summary>
        public void activateIntro()
        {
            //((MenuObject)menuObjectList.GetObjectByName("Intro_GlassboxGames")).FadingAnimation = new FadingAnimation(false, true, 1000, true, 500);
            ((MenuObject)menuObjectList.GetObjectByName("Intro_GlassboxGames")).Visible = false;
            //((MenuObject)menuObjectList.GetObjectByName("Intro_IvorySound")).Visible = false;
            ((MenuObject)menuObjectList.GetObjectByName("Intro_IvorySound")).FadingAnimation = new FadingAnimation(false, true, 1000, false, 500);
            TaskManager.Instance.addTask(new Tasks.AnimationTask(2000, ((MenuObject)menuObjectList.GetObjectByName("Intro_IvorySound")).FadingAnimation));

        }

        /// <summary>
        /// Triggers all necessary actions when the down button is pressed
        /// </summary>
        public void downButtonPressed()
        {
            
            // When in main screen
            if (menuState == 0)
            {

                SoundManager.Instance.playMenuSoundEffect((int)MenuSound.Switch);

                // unhighlight all previous selected button items
                if (selectedItem > 0)
                    foreach (LayerInterface menuObject in menuStructure[menuState, selectedItem])
                    {
                        ((MenuButtonObject)menuObject).Selected = false;
                    }

                // select quit item
                selectedItem = 4;

                // highlight all items of the new selected button
                foreach (LayerInterface menuObject in menuStructure[menuState, selectedItem])
                {
                    ((MenuButtonObject)menuObject).Selected = true;
                }

                timeSinceLastInput = 0;
            }
            // When in team menu 1vs1
            else if (menuState == 1)
            {
                // Do nothing
            }
            // When in team menu 2vs2
            else if (menuState == 2)
            {
                // Do nothing
            }
            // When in help
            else if (menuState == 3)
            {
                // Do nothing
            }
            // When in really quit
            else if (menuState == 4)
            {

                if (selectedItem < 2)
                    SoundManager.Instance.playMenuSoundEffect((int)MenuSound.Switch);


                // unhighlight first item of previous selected button items
                ((MenuButtonObject)menuStructure[menuState, selectedItem][0]).Selected = false;

                // select next item
                selectedItem++;
                if (selectedItem > 2)
                    selectedItem = 2;

                // highlight first item of the new selected button
                ((MenuButtonObject)menuStructure[menuState, selectedItem][0]).Selected = true;

                timeSinceLastInput = 0;
            }
            // When in select level
            else if (menuState == 5)
            {

                SoundManager.Instance.playMenuSoundEffect((int)MenuSound.Switch);

                // Make all winning conditions invisible
                ((MenuObject)menuObjectList.GetObjectByName("SelectLevel_15pts")).Visible = false;
                ((MenuObject)menuObjectList.GetObjectByName("SelectLevel_30pts")).Visible = false;
                ((MenuObject)menuObjectList.GetObjectByName("SelectLevel_3min")).Visible = false;
                ((MenuObject)menuObjectList.GetObjectByName("SelectLevel_5min")).Visible = false;

                // select next item
                selectedItem++;
                if (selectedItem > 4)
                    selectedItem = 1;

                // Make correct condition visible
                switch (selectedItem)
                {
                    case 1:
                        ((MenuObject)menuObjectList.GetObjectByName("SelectLevel_15pts")).Visible = true;
                        winCondition = new ScoreLimit_WinCondition(15);
                        break;
                    case 2:
                        ((MenuObject)menuObjectList.GetObjectByName("SelectLevel_30pts")).Visible = true;
                        winCondition = new ScoreLimit_WinCondition(30);
                        break;
                    case 3:
                        ((MenuObject)menuObjectList.GetObjectByName("SelectLevel_3min")).Visible = true;
                        winCondition = new TimeLimit_WinCondition(new TimeSpan(0, 3, 0));
                        break;
                    case 4:
                        ((MenuObject)menuObjectList.GetObjectByName("SelectLevel_5min")).Visible = true;
                        winCondition = new TimeLimit_WinCondition(new TimeSpan(0, 5, 0));
                        break;
                }

                // Animate
                ((MenuObject)menuObjectList.GetObjectByName("SelectLevel_down_s")).FadingAnimation =
                    new FadingAnimation(false, true, 0, true, 200);

                timeSinceLastInput = 0;
            }


        }

        /// <summary>
        /// Triggers all necessary actions when the up button is pressed
        /// </summary>
        public void upButtonPressed()
        {

            // When in main screen
            if (menuState == 0)
            {

                SoundManager.Instance.playMenuSoundEffect((int)MenuSound.Switch);

                // unhighlight all previous selected button items
                if (selectedItem > 0)
                    foreach (LayerInterface menuObject in menuStructure[menuState, selectedItem])
                    {
                        ((MenuButtonObject)menuObject).Selected = false;
                    }

                // select help item
                selectedItem = 3;

                // highlight all items of the new selected button
                foreach (LayerInterface menuObject in menuStructure[menuState, selectedItem])
                {
                    ((MenuButtonObject)menuObject).Selected = true;
                }

                timeSinceLastInput = 0;
            }
            // When in team menu 1vs1
            else if (menuState == 1)
            {
                // Do nothing
            }
            // When in team menu 2vs2
            else if (menuState == 2)
            {
                // Do nothing
            }
            // When in help
            else if (menuState == 3)
            {
                // Do nothing
            }
            // When in really quit
            else if (menuState == 4)
            {

                if (selectedItem > 1)
                    SoundManager.Instance.playMenuSoundEffect((int)MenuSound.Switch);

                // unhighlight first item of previous selected button items
                ((MenuButtonObject)menuStructure[menuState, selectedItem][0]).Selected = false;

                // select next item
                selectedItem--;
                if (selectedItem < 1)
                    selectedItem = 1;

                // highlight first item of the new selected button
                ((MenuButtonObject)menuStructure[menuState, selectedItem][0]).Selected = true;

                timeSinceLastInput = 0;
            }
            // When in select level
            else if (menuState == 5)
            {

                SoundManager.Instance.playMenuSoundEffect((int)MenuSound.Switch);

                // If powerups are on
                if ((bool)SettingsManager.Instance.get("enablePowerups") == true)
                {

                    // Turn them off
                    SettingsManager.Instance.set("enablePowerups", false);
                    ((MenuObject)menuObjectList.GetObjectByName("SelectLevel_powerups")).Visible = false;
                    ((MenuObject)menuObjectList.GetObjectByName("SelectLevel_classic")).Visible = true;
                    ((MenuObject)menuObjectList.GetObjectByName("SelectLevel_up_s")).FadingAnimation =
                        new FadingAnimation(false, true, 0, true, 200);

                }
                else
                {

                    // Else turn them on
                    SettingsManager.Instance.set("enablePowerups", true);
                    ((MenuObject)menuObjectList.GetObjectByName("SelectLevel_powerups")).Visible = true;
                    ((MenuObject)menuObjectList.GetObjectByName("SelectLevel_classic")).Visible = false;
                    ((MenuObject)menuObjectList.GetObjectByName("SelectLevel_up_s")).FadingAnimation =
                        new FadingAnimation(false, true, 0, true, 200);

                }

                timeSinceLastInput = 0;
            }

        }

        /// <summary>
        /// Triggers all necessary actions when the left button is pressed
        /// </summary>
        /// <param name="device">The device that pressed the key</param>
        /// <param name="deviceIndex">The index of the device</param>
        public void leftButtonPressed(InputDevice device, PlayerIndex deviceIndex)
        {

            // When in main screen
            if (menuState == 0)
            {

                SoundManager.Instance.playMenuSoundEffect((int)MenuSound.Switch);

                // unhighlight all previous selected button items
                if (selectedItem > 0)
                    foreach (LayerInterface menuObject in menuStructure[menuState, selectedItem])
                    {
                        ((MenuButtonObject)menuObject).Selected = false;
                    }

                // select 1vs1 item
                selectedItem = 1;

                // highlight all items of the new selected button
                foreach (LayerInterface menuObject in menuStructure[menuState, selectedItem])
                {
                    ((MenuButtonObject)menuObject).Selected = true;
                }

                timeSinceLastInput = 0;
            }
            // When in team menu 
            else if ((menuState == 1) || (menuState == 2))
            {

                SoundManager.Instance.playMenuSoundEffect((int)MenuSound.Switch);

                PlayerRepresentationMainMenu player = getPlayerRepresentation(device, deviceIndex);
                if (player.Active)
                {
                    movePlayerRepresentationLeft(player);
                }

                timeSinceLastInput = 0;
            }
            // When in help
            else if (menuState == 3)
            {

                SoundManager.Instance.playMenuSoundEffect((int)MenuSound.Switch);

                // unhighlight previous selected button item
                ((MenuButtonObject)menuStructure[menuState, selectedItem][0]).Selected = false;

                // select left button item
                selectedItem--;
                if (selectedItem < 1)
                    selectedItem = 1;

                // highlight first item of the new selected button
                ((MenuButtonObject)menuStructure[menuState, selectedItem][0]).Selected = true;

                // Hide all boxes
                foreach (LayerInterface menuObject in menuStructure[menuState, 0])
                {
                    ((MenuObject)menuObject).Visible = false;
                }

                // Make background and right one visible
                ((MenuObject)menuObjectList.GetObjectByName("MainMenu_Background")).Visible = true;
                switch (selectedItem)
                {
                    case 1:
                        ((MenuObject)menuObjectList.GetObjectByName("Help_Box1")).Visible = true;
                        break;
                    case 2:
                        ((MenuObject)menuObjectList.GetObjectByName("Help_Box2")).Visible = true;
                        break;
                    case 3:
                        ((MenuObject)menuObjectList.GetObjectByName("Help_Box3")).Visible = true;
                        break;
                    case 4:
                        ((MenuObject)menuObjectList.GetObjectByName("Help_CreditsBox")).Visible = true;
                        break;
                }

                timeSinceLastInput = 0;
            }
            // When in really quit
            else if (menuState == 4)
            {
                // Do nothing
            }
            // When in select level
            else if (menuState == 5)
            {

                SoundManager.Instance.playMenuSoundEffect((int)MenuSound.Switch);

                // Switch to previous levelPreview
                selectedLevelPreviewIndex--;
                if (selectedLevelPreviewIndex < 0)
                    selectedLevelPreviewIndex = levels.LevelPreviews.Count - 1;

                selectLevelPreview(selectedLevelPreviewIndex);

                // Highlight Button
                ((MenuObject)menuObjectList.GetObjectByName("SelectLevel_left_s")).FadingAnimation =
                    new FadingAnimation(false, true, 0, true, 200);

                timeSinceLastInput = 0;
            }

        }

        /// <summary>
        /// Triggers all necessary actions when the right button is pressed
        /// </summary>
        /// <param name="device">The device that pressed the key</param>
        /// <param name="deviceIndex">The index of the device</param>
        public void rightButtonPressed(InputDevice device, PlayerIndex deviceIndex)
        {

            // When in main screen
            if (menuState == 0)
            {

                SoundManager.Instance.playMenuSoundEffect((int)MenuSound.Switch);

                // unhighlight all previous selected button items
                if (selectedItem > 0)
                    foreach (LayerInterface menuObject in menuStructure[menuState, selectedItem])
                    {
                        ((MenuButtonObject)menuObject).Selected = false;
                    }

                // select 2vs2 item
                selectedItem = 2;

                // highlight all items of the new selected button
                foreach (LayerInterface menuObject in menuStructure[menuState, selectedItem])
                {
                    ((MenuButtonObject)menuObject).Selected = true;
                }

                timeSinceLastInput = 0;
            }
            // When in team menu 
            else if ((menuState == 1) || (menuState == 2))
            {

                SoundManager.Instance.playMenuSoundEffect((int)MenuSound.Switch);

                PlayerRepresentationMainMenu player = getPlayerRepresentation(device, deviceIndex);
                if (player.Active)
                {
                    movePlayerRepresentationRight(player);
                }

                timeSinceLastInput = 0;
            }
            // When in help
            else if (menuState == 3)
            {

                SoundManager.Instance.playMenuSoundEffect((int)MenuSound.Switch);

                // unhighlight previous selected button item
                ((MenuButtonObject)menuStructure[menuState, selectedItem][0]).Selected = false;

                // select left button item
                selectedItem++;
                if (selectedItem > 4)
                    selectedItem = 4;

                // highlight first item of the new selected button
                ((MenuButtonObject)menuStructure[menuState, selectedItem][0]).Selected = true;

                // Hide all boxes
                foreach (LayerInterface menuObject in menuStructure[menuState, 0])
                {
                    ((MenuObject)menuObject).Visible = false;
                }

                // Make background and right one visible
                ((MenuObject)menuObjectList.GetObjectByName("MainMenu_Background")).Visible = true;
                switch (selectedItem)
                {
                    case 1:
                        ((MenuObject)menuObjectList.GetObjectByName("Help_Box1")).Visible = true;
                        break;
                    case 2:
                        ((MenuObject)menuObjectList.GetObjectByName("Help_Box2")).Visible = true;
                        break;
                    case 3:
                        ((MenuObject)menuObjectList.GetObjectByName("Help_Box3")).Visible = true;
                        break;
                    case 4:
                        ((MenuObject)menuObjectList.GetObjectByName("Help_CreditsBox")).Visible = true;
                        break;
                }

                timeSinceLastInput = 0;
            }
            // When in really quit
            else if (menuState == 4)
            {
                // Do nothing
            }
            // When in select level
            else if (menuState == 5)
            {

                SoundManager.Instance.playMenuSoundEffect((int)MenuSound.Switch);

                // Switch to next levelPreview
                selectedLevelPreviewIndex++;
                if (selectedLevelPreviewIndex > (levels.LevelPreviews.Count - 1))
                    selectedLevelPreviewIndex = 0;

                selectLevelPreview(selectedLevelPreviewIndex);

                // Highlight Button
                ((MenuObject)menuObjectList.GetObjectByName("SelectLevel_right_s")).FadingAnimation =
                    new FadingAnimation(false, true, 0, true, 200);
            }

            timeSinceLastInput = 0;
        }

        /// <summary>
        /// Triggers all necessary actions when the enter button is pressed
        /// </summary>
        public void enterButtonPressed()
        {

            // When in main screen
            if (menuState == 0)
            {

                // If nothing is selected, show help
                if (selectedItem == 0)
                {
                    return;
                }

                // Otherwise switch to the right state
                switchMenuState(selectedItem);
                SoundManager.Instance.playMenuSoundEffect((int)MenuSound.Select);

                timeSinceLastInput = 0;
            }
            // When in team menu 
            else if ((menuState == 1) || (menuState == 2))
            {
                timeSinceLastInput = 0;
                
                if (((bool)SettingsManager.Instance.get("AllowKIOnlyGame") == false) &&
                    (getNumberOfTeamMembers(1) == 0) && (getNumberOfTeamMembers(2) == 0))
                {
                    ((MenuObject)menuObjectList.GetObjectByName("TeamMenuWarning2_Overlay")).FadingAnimation = null;
                    ((MenuObject)menuObjectList.GetObjectByName("TeamMenuWarning2_Overlay")).Visible = false;
                    ((MenuObject)menuObjectList.GetObjectByName("TeamMenuWarning1_Overlay")).FadingAnimation = new FadingAnimation(false, true, 2000, true, 200);
                    SoundManager.Instance.playMenuSoundEffect((int)MenuSound.Warning);
                    return;
                }
                // Switch to select level
                SoundManager.Instance.playMenuSoundEffect((int)MenuSound.Select);
                previousMenuState = menuState;
                switchMenuState(5);

            }
            // When in help
            else if (menuState == 3)
            {

                // Select the next item:
                rightButtonPressed(InputDevice.Gamepad, PlayerIndex.One);

                timeSinceLastInput = 0;
            }
            // When in really quit
            else if (menuState == 4)
            {

                // If yes is selected
                if (selectedItem == 1)
                {
                    // Quit game
                    Game1.Instance.QuitGame();
                    SoundManager.Instance.playMenuSoundEffect((int)MenuSound.Select);
                }
                else
                {
                    // Go back to main screen
                    switchMenuState(0);
                    SoundManager.Instance.playMenuSoundEffect((int)MenuSound.Select);
                }

                timeSinceLastInput = 0;
            }
            // When in select level
            else if (menuState == 5)
            {

                SoundManager.Instance.playMenuSoundEffect((int)MenuSound.Select);
                // Start game
                startGame();

                timeSinceLastInput = 0;
            }

        }

        /// <summary>
        /// Triggers all necessary actions when the back button is pressed
        /// </summary>
        public void backButtonPressed()
        {

            // When in main screen
            if (menuState == 0)
            {
                // Do nothing
            }
            // When in team menu 1vs1
            else if (menuState == 1)
            {
                SoundManager.Instance.playMenuSoundEffect((int)MenuSound.SelectBack);

                // Go back to main screen
                switchMenuState(0);
                timeSinceLastInput = 0;

            }
            // When in team menu 2vs2
            else if (menuState == 2)
            {
                SoundManager.Instance.playMenuSoundEffect((int)MenuSound.SelectBack);

                // Go back to main screen
                switchMenuState(0);
                timeSinceLastInput = 0;

            }
            // When in help
            else if (menuState == 3)
            {
                SoundManager.Instance.playMenuSoundEffect((int)MenuSound.SelectBack);

                // Go back to main screen
                switchMenuState(0);
                timeSinceLastInput = 0;

            }
            // When in really quit
            else if (menuState == 4)
            {
                SoundManager.Instance.playMenuSoundEffect((int)MenuSound.SelectBack);

                // Go back to main screen
                switchMenuState(0);
                timeSinceLastInput = 0;

            }
            // When in select level
            else if (menuState == 5)
            {
                SoundManager.Instance.playMenuSoundEffect((int)MenuSound.SelectBack);

                // Go back to the right team menu
                switchMenuState(previousMenuState);
                timeSinceLastInput = 0;

            }

        }


        #endregion

        #region HelperMethods

        /// <summary>
        /// Fills the menu structure with content
        /// </summary>
        private void createMenuStructure()
        {

            // Explanation structure:
            // 1.Index: Menu state
            // 2.Index: 0: List of Menu_PassiveObjects which are always shown/never highlighted (e.g. background)
            //          all other indexes: List of Menu_Buttons which belong to a single selectable item
            menuStructure = new List<LayerInterface>[6, 6];

            // Main screen
            menuStructure[0, 0] = new List<LayerInterface>();
            menuStructure[0, 0].Add(menuObjectList.GetObjectByName("MainMenu_Background_Ball"));
            menuStructure[0, 1] = new List<LayerInterface>();
            menuStructure[0, 1].Add(menuObjectList.GetObjectByName("MainScreen_1vs1"));
            menuStructure[0, 2] = new List<LayerInterface>();
            menuStructure[0, 2].Add(menuObjectList.GetObjectByName("MainScreen_2vs2"));
            menuStructure[0, 3] = new List<LayerInterface>();
            menuStructure[0, 3].Add(menuObjectList.GetObjectByName("MainScreen_Help"));
            menuStructure[0, 4] = new List<LayerInterface>();
            menuStructure[0, 4].Add(menuObjectList.GetObjectByName("MainScreen_Exit"));

            // Team menu 1vs1
            menuStructure[1, 0] = new List<LayerInterface>();
            menuStructure[1, 0].Add(menuObjectList.GetObjectByName("MainMenu_Background"));
            menuStructure[1, 0].Add(menuObjectList.GetObjectByName("TeamMenu_Background2_1vs1"));
            menuStructure[1, 0].Add(menuObjectList.GetObjectByName("TeamMenu_Gamepad1"));
            menuStructure[1, 0].Add(menuObjectList.GetObjectByName("TeamMenu_Gamepad2"));
            menuStructure[1, 0].Add(menuObjectList.GetObjectByName("TeamMenu_Gamepad3"));
            menuStructure[1, 0].Add(menuObjectList.GetObjectByName("TeamMenu_Gamepad4"));
            menuStructure[1, 0].Add(menuObjectList.GetObjectByName("TeamMenu_Keyboard1"));
            menuStructure[1, 0].Add(menuObjectList.GetObjectByName("TeamMenu_Keyboard2"));
            menuStructure[1, 0].Add(menuObjectList.GetObjectByName("TeamMenu_Gamepad_Deactivated1"));
            menuStructure[1, 0].Add(menuObjectList.GetObjectByName("TeamMenu_Gamepad_Deactivated2"));
            menuStructure[1, 0].Add(menuObjectList.GetObjectByName("TeamMenu_Gamepad_Deactivated3"));
            menuStructure[1, 0].Add(menuObjectList.GetObjectByName("TeamMenu_Gamepad_Deactivated4"));
            menuStructure[1, 0].Add(menuObjectList.GetObjectByName("TeamMenu_KI1"));
            menuStructure[1, 0].Add(menuObjectList.GetObjectByName("TeamMenu_KI2"));
            menuStructure[1, 0].Add(menuObjectList.GetObjectByName("TeamMenu_KI3"));
            menuStructure[1, 0].Add(menuObjectList.GetObjectByName("TeamMenu_KI4"));

            // Team menu 2vs2
            menuStructure[2, 0] = new List<LayerInterface>();
            menuStructure[2, 0].Add(menuObjectList.GetObjectByName("MainMenu_Background"));
            menuStructure[2, 0].Add(menuObjectList.GetObjectByName("TeamMenu_Background2_2vs2"));
            menuStructure[2, 0].Add(menuObjectList.GetObjectByName("TeamMenu_Gamepad1"));
            menuStructure[2, 0].Add(menuObjectList.GetObjectByName("TeamMenu_Gamepad2"));
            menuStructure[2, 0].Add(menuObjectList.GetObjectByName("TeamMenu_Gamepad3"));
            menuStructure[2, 0].Add(menuObjectList.GetObjectByName("TeamMenu_Gamepad4"));
            menuStructure[2, 0].Add(menuObjectList.GetObjectByName("TeamMenu_Keyboard1"));
            menuStructure[2, 0].Add(menuObjectList.GetObjectByName("TeamMenu_Keyboard2"));
            menuStructure[2, 0].Add(menuObjectList.GetObjectByName("TeamMenu_Gamepad_Deactivated1"));
            menuStructure[2, 0].Add(menuObjectList.GetObjectByName("TeamMenu_Gamepad_Deactivated2"));
            menuStructure[2, 0].Add(menuObjectList.GetObjectByName("TeamMenu_Gamepad_Deactivated3"));
            menuStructure[2, 0].Add(menuObjectList.GetObjectByName("TeamMenu_Gamepad_Deactivated4"));
            menuStructure[2, 0].Add(menuObjectList.GetObjectByName("TeamMenu_KI1"));
            menuStructure[2, 0].Add(menuObjectList.GetObjectByName("TeamMenu_KI2"));
            menuStructure[2, 0].Add(menuObjectList.GetObjectByName("TeamMenu_KI3"));
            menuStructure[2, 0].Add(menuObjectList.GetObjectByName("TeamMenu_KI4"));

            // help
            menuStructure[3, 0] = new List<LayerInterface>();
            menuStructure[3, 0].Add(menuObjectList.GetObjectByName("MainMenu_Background"));
            menuStructure[3, 0].Add(menuObjectList.GetObjectByName("Help_Box1"));
            menuStructure[3, 0].Add(menuObjectList.GetObjectByName("Help_Box2"));
            menuStructure[3, 0].Add(menuObjectList.GetObjectByName("Help_Box3"));
            menuStructure[3, 0].Add(menuObjectList.GetObjectByName("Help_CreditsBox"));
            menuStructure[3, 1] = new List<LayerInterface>();
            menuStructure[3, 1].Add(menuObjectList.GetObjectByName("Help_Button1"));
            menuStructure[3, 2] = new List<LayerInterface>();
            menuStructure[3, 2].Add(menuObjectList.GetObjectByName("Help_Button2"));
            menuStructure[3, 3] = new List<LayerInterface>();
            menuStructure[3, 3].Add(menuObjectList.GetObjectByName("Help_Button3"));
            menuStructure[3, 4] = new List<LayerInterface>();
            menuStructure[3, 4].Add(menuObjectList.GetObjectByName("Help_Button4"));

            // ReallyQuit
            menuStructure[4, 0] = new List<LayerInterface>();
            menuStructure[4, 0].Add(menuObjectList.GetObjectByName("MainMenu_Background"));
            menuStructure[4, 0].Add(menuObjectList.GetObjectByName("ReallyQuit_Frame"));
            menuStructure[4, 0].Add(menuObjectList.GetObjectByName("ReallyQuit_Frame2"));
            menuStructure[4, 0].Add(menuObjectList.GetObjectByName("ReallyQuit_Head"));
            menuStructure[4, 1] = new List<LayerInterface>();
            menuStructure[4, 1].Add(menuObjectList.GetObjectByName("ReallyQuit_Yes"));
            menuStructure[4, 2] = new List<LayerInterface>();
            menuStructure[4, 2].Add(menuObjectList.GetObjectByName("ReallyQuit_No"));

            // Select level
            menuStructure[5, 0] = new List<LayerInterface>();
            menuStructure[5, 0].Add(menuObjectList.GetObjectByName("MainMenu_Background"));
            menuStructure[5, 0].Add(menuObjectList.GetObjectByName("SelectLevel_left"));
            menuStructure[5, 0].Add(menuObjectList.GetObjectByName("SelectLevel_right"));
            menuStructure[5, 0].Add(menuObjectList.GetObjectByName("SelectLevel_powerups"));
            menuStructure[5, 0].Add(menuObjectList.GetObjectByName("SelectLevel_classic"));
            menuStructure[5, 0].Add(menuObjectList.GetObjectByName("SelectLevel_15pts"));
            menuStructure[5, 0].Add(menuObjectList.GetObjectByName("SelectLevel_30pts"));
            menuStructure[5, 0].Add(menuObjectList.GetObjectByName("SelectLevel_3min"));
            menuStructure[5, 0].Add(menuObjectList.GetObjectByName("SelectLevel_5min"));

            foreach (String levelName in levels.ImplementedLevels().Keys)
                menuStructure[5, 0].Add(menuObjectList.GetObjectByName(levelName));


        }

        /// <summary>
        /// Triggers all necessary action when switching the states
        /// in main menu
        /// </summary>
        /// <param name="newState">The state to which should be switched</param>
        private void switchMenuState(int newState)
        {

            // First, make all objects invisible and disable a fading animation
            for (int i = 0; i < menuStructure.GetLength(0); i++)
                for (int j = 0; j < menuStructure.GetLength(1); j++)
                    if (menuStructure[i, j] != null)
                        foreach (LayerInterface menuObject in menuStructure[i, j])
                        {
                            ((DrawableObject)menuObject).Visible = false;
                            ((DrawableObject)menuObject).FadingAnimation = null;
                        }

            // Make all necessary menu objects visible
            for (int i = 0; i < menuStructure.GetLength(1); i++)
            {
                if (menuStructure[newState, i] != null)
                {
                    foreach (LayerInterface menuObject in menuStructure[newState, i])
                    {

                        // Special case select level menu: The different points options aren't
                        // visible in the beginning
                        if (!((menuObject.Name == "SelectLevel_15pts") || (menuObject.Name == "SelectLevel_30pts")
                            || (menuObject.Name == "SelectLevel_50pts")))
                        {
                            ((DrawableObject)menuObject).Visible = true;
                            // Unhighlight all buttons
                            if (i > 0)
                                ((MenuButtonObject)menuObject).Selected = false;
                        }

                    }
                }
            }

            // Save selected item when leaving main screen
            if (menuState == 0)
                previousMainScreenSelectedItem = selectedItem;

            // If in main screen or select level: Select item and highlight it
            if ((newState == 0) || (newState == 5))
            {
                if (newState == 0)
                {
                    selectedItem = previousMainScreenSelectedItem;

                    foreach (LayerInterface menuObject in menuStructure[newState, selectedItem])
                        ((MenuButtonObject)menuObject).Selected = true;
                }
                else
                {

                    // Select a default level randomly
                    Random random = new Random();
                    selectedLevelPreviewIndex = random.Next(levels.ImplementedLevels().Count);
                    selectLevelPreview(selectedLevelPreviewIndex);

                    // Highlight multiple options as default and set these options
                    winCondition = new ScoreLimit_WinCondition(15);
                    SettingsManager.Instance.set("enablePowerups", true);

                    selectedItem = 1;

                    // Make all MenuObjects that aren't selected invisible
                    ((MenuObject)menuObjectList.GetObjectByName("SelectLevel_15pts")).Visible = true;
                    ((MenuObject)menuObjectList.GetObjectByName("SelectLevel_30pts")).Visible = false;
                    ((MenuObject)menuObjectList.GetObjectByName("SelectLevel_3min")).Visible = false;
                    ((MenuObject)menuObjectList.GetObjectByName("SelectLevel_5min")).Visible = false;

                    ((MenuObject)menuObjectList.GetObjectByName("SelectLevel_classic")).Visible = false;


                }

            }

            // When switching from main screen to team menu reset all players
            if ((menuState == 0) && ((newState == 1) || (newState == 2)))
            {

                for (int i = 0; i < playerArray.Length; i++)
                {
                    playerArray[i].Team = 0;
                    playerArray[i].KI = false;
                    playerArray[i].Connected = false;
                }


            }

            // When switching from team menu to select level set gameMode
            if (newState == 5)
            {

               if (menuState == 1)
                   gameMode2vs2 = false;
               else
                   gameMode2vs2 = true;

            }

            // When switching from select level back to team menu
            if ((menuState == 5) && ((newState == 1) || (newState == 2)))
            {

                // Make sure that no levelpreview is fading any more
                for (int i = 0; i < levels.LevelPreviews.Count; i++)
                {
                    ((MenuButtonObject)levels.LevelPreviews[i]).Visible = false;
                    ((MenuButtonObject)levels.LevelPreviews[i]).FadingAnimation = null;

                }

            }

            // When switching to help
            if (newState == 3)
            {
                // Select first button
                selectedItem = 1;

                // Highlight first button
                foreach (LayerInterface menuObject in menuStructure[newState, selectedItem])
                    ((MenuButtonObject)menuObject).Selected = true;

                // Make all help boxes except first one invisible
                foreach (LayerInterface menuObject in menuStructure[newState, 0])
                    ((MenuObject)menuObject).Visible = false;

                ((MenuObject)menuObjectList.GetObjectByName("Help_Box1")).Visible = true;
                ((MenuObject)menuObjectList.GetObjectByName("MainMenu_Background")).Visible = true;

            }

            // When switching to ReallyQuit
            if (newState == 4)
            {
                selectedItem = 2;

                // Highlight first button
                foreach (LayerInterface menuObject in menuStructure[newState, selectedItem])
                    ((MenuButtonObject)menuObject).Selected = true;

            }

            // Get the reference to the right overlay
            switch (newState)
            {
                case 0:
                    overlayObject.FadingAnimation = null;
                    overlayObject.Visible = false;
                    overlayObject = (MenuObject)menuObjectList.GetObjectByName("MainScreen_Overlay");
                    break;
                case 1:
                    overlayObject.FadingAnimation = null;
                    overlayObject.Visible = false;
                    overlayObject = (MenuObject)menuObjectList.GetObjectByName("TeamMenu_Overlay");
                    break;
                case 2:
                    overlayObject.FadingAnimation = null;
                    overlayObject.Visible = false;
                    overlayObject = (MenuObject)menuObjectList.GetObjectByName("TeamMenu_Overlay");
                    break;
                case 3:
                    overlayObject.FadingAnimation = null;
                    overlayObject.Visible = false;
                    overlayObject = (MenuObject)menuObjectList.GetObjectByName("Help_Overlay");
                    break;
                case 4:
                    overlayObject.FadingAnimation = null;
                    overlayObject.Visible = false;
                    overlayObject = (MenuObject)menuObjectList.GetObjectByName("ReallyQuit_Overlay");
                    break;
                case 5:
                    overlayObject.FadingAnimation = null;
                    overlayObject.Visible = false;
                    overlayObject = (MenuObject)menuObjectList.GetObjectByName("SelectLevel_Overlay");
                    break;
            }

            ((MenuObject)menuObjectList.GetObjectByName("TeamMenuWarning1_Overlay")).FadingAnimation = null;
            ((MenuObject)menuObjectList.GetObjectByName("TeamMenuWarning1_Overlay")).Visible = false;
            ((MenuObject)menuObjectList.GetObjectByName("TeamMenuWarning2_Overlay")).FadingAnimation = null;
            ((MenuObject)menuObjectList.GetObjectByName("TeamMenuWarning2_Overlay")).Visible = false;

            animateTransition(menuState, newState);

            menuState = newState;

        }

        /// <summary>
        /// Selects the right level preview and shows it
        /// Also displayed the one left and right of the selected one
        /// </summary>
        /// <param name="levelName">The index of the selected level</param>
        private void selectLevelPreview(int levelIndex)
        {

            for (int i = 0; i < levels.LevelPreviews.Count; i++)
            {
                // Is selected level
                if (i == levelIndex)
                {
                    levelName = ((MenuButtonObject)levels.LevelPreviews[i]).Name;
                    ((MenuButtonObject)levels.LevelPreviews[i]).Visible = true;
                    ((MenuButtonObject)levels.LevelPreviews[i]).Size = sizeSelectedLevelPreview;
                    ((MenuButtonObject)levels.LevelPreviews[i]).Position = positionSelectedLevelPreview;

                    ((MenuButtonObject)levels.LevelPreviews[i]).FadingAnimation =
                        new FadingAnimation(false, false, 0, true, 500);
                }
                // Every other level
                else
                {
                    ((MenuButtonObject)levels.LevelPreviews[i]).Visible = false;
                    ((MenuButtonObject)levels.LevelPreviews[i]).FadingAnimation = null;
                }
            }


        }

        /// <summary>
        /// Sets the animations for the different transitions in MainMenu
        /// </summary>
        /// <param name="oldState">The previous state</param>
        /// <param name="newState">The new state</param>
        private void animateTransition(int oldState, int newState)
        {
            // TeamMenu -> SelectLevel
            if (((oldState == 1) || (oldState == 2)) && (newState == 5))
            {

            }

            // MainScreen -> ReallyQuit
            if ((oldState == 0)  && (newState == 4))
            {
                ((MenuObject)menuObjectList.GetObjectByName("ReallyQuit_Frame")).FadingAnimation =
                    new FadingAnimation(false, false, 0, true, 300);
                ((MenuObject)menuObjectList.GetObjectByName("ReallyQuit_Frame2")).FadingAnimation =
                    new FadingAnimation(false, false, 0, true, 300);
                ((MenuObject)menuObjectList.GetObjectByName("ReallyQuit_Head")).FadingAnimation =
                    new FadingAnimation(false, false, 0, true, 300);
                ((MenuObject)menuObjectList.GetObjectByName("ReallyQuit_Yes")).FadingAnimation =
                    new FadingAnimation(false, false, 0, true, 300);
                ((MenuObject)menuObjectList.GetObjectByName("ReallyQuit_No")).FadingAnimation =
                    new FadingAnimation(false, false, 0, true, 300);
            }

            // ReallyQuit -> MainScreen
            if ((oldState == 4) && (newState == 0))
            {
                ((MenuObject)menuObjectList.GetObjectByName("MainScreen_1vs1")).FadingAnimation =
                    new FadingAnimation(false, false, 0, true, 300);
                ((MenuObject)menuObjectList.GetObjectByName("MainScreen_2vs2")).FadingAnimation =
                    new FadingAnimation(false, false, 0, true, 300);
                ((MenuObject)menuObjectList.GetObjectByName("MainScreen_Help")).FadingAnimation =
                    new FadingAnimation(false, false, 0, true, 300);
                ((MenuObject)menuObjectList.GetObjectByName("MainScreen_Exit")).FadingAnimation =
                    new FadingAnimation(false, false, 0, true, 300);
            }
        }

        /// <summary>
        /// Checks if an overlay has to be shown
        /// </summary>
        /// <param name="gameTime">The GameTime</param>
        private void manageOverlays(GameTime gameTime)
        {
            // If there was an input, fade out the overlay
            if (timeSinceLastInput == 0)
            {
                if (overlayObject.Visible == true)
                {
                    overlayObject.FadingAnimation = new FadingAnimation(false, false, 0, true, 400);
                    overlayObject.FadingAnimation.Inverted = true;
                    timeSinceLastInput = 1;
                }
            }

            // Increment time since last input
            timeSinceLastInput += gameTime.ElapsedGameTime.Milliseconds;

            // Check if an overlay should be shown
            if (timeSinceLastInput > timeTillOverlayAppears)
            {
                if (overlayObject.Visible == false)
                    overlayObject.FadingAnimation = new FadingAnimation(false, false, 0, true, 400);

            }

        }


        /// <summary>
        /// Takes the player from the playerArray and create lists with the correct
        /// PlayerRepresentations for the GameInstance
        /// Then it triggers the game start in SuperController
        /// </summary>
        private void startGame()
        {
            List<int> possibleIndexes = new List<int>();
            for (int i = 1; i < 5; i++)
                possibleIndexes.Add(i);
            List<PlayerRepresentationMainMenu> leftPlayers = new List<PlayerRepresentationMainMenu>();
            List<PlayerRepresentationMainMenu> rightPlayers = new List<PlayerRepresentationMainMenu>();

            // Go through all playerRepresentations and if they're active and belong to one
            // team, put them in the list
            for (int i = 0; i < playerArray.Length; i++)
            {

                if (playerArray[i].Active && (playerArray[i].Team != 0))
                {

                    if (playerArray[i].Team == 1)
                    {

                        leftPlayers.Add(playerArray[i]);
                        possibleIndexes.Remove(playerArray[i].PlayerIndex);
                    }
                    else
                    {
                        rightPlayers.Add(playerArray[i]);
                        possibleIndexes.Remove(playerArray[i].PlayerIndex);
                    }

                }

            }

            // Fill with KI players
            for (int i = 0; i < 4; i++)
            {

                if (((leftPlayers.Count < 1) && (!gameMode2vs2)) ||
                    ((leftPlayers.Count < 2) && (gameMode2vs2)))
                {
                    int index = possibleIndexes.First();
                    leftPlayers.Add(new PlayerRepresentationMainMenu((MenuObject)menuObjectList.GetObjectByName("TeamMenu_KI1"), 
                        new Gamepad(PlayerIndex.Four, InputDevice.Gamepad, false), true, index));
                    possibleIndexes.Remove(index);
                }
                else if (((rightPlayers.Count < 1) && (!gameMode2vs2)) ||
                    ((rightPlayers.Count < 2) && (gameMode2vs2)))
                {
                    int index = possibleIndexes.First();
                    rightPlayers.Add(new PlayerRepresentationMainMenu((MenuObject)menuObjectList.GetObjectByName("TeamMenu_KI1"), 
                        new Gamepad(PlayerIndex.Four, InputDevice.Gamepad, false), true, index));
                    possibleIndexes.Remove(index);
                }
            }

            // Start game
            SuperController.Instance.switchFromMainMenuToIngame(levelName, winCondition, leftPlayers, rightPlayers);

        }

        /// <summary>
        /// Returns the fitting PlayerRepresentation
        /// </summary>
        /// <param name="device">The device</param>
        /// <param name="deviceIndex">The index of the device</param>
        /// <returns>The PlayerRepresentation or null if it doesn't exist</returns>
        private PlayerRepresentationMainMenu getPlayerRepresentation(InputDevice device, PlayerIndex deviceIndex)
        {

            PlayerRepresentationMainMenu player = null;

            // Go through all player and check if there is the right one
            for (int i = 0; i < playerArray.Length; i++)
                if ((playerArray[i].InputDevice.Device == device) && (playerArray[i].InputDevice.Index == deviceIndex))
                {
                    player = playerArray[i];
                    break;
                }

            return player;

        }

        /// <summary>
        /// Returns the number of PlayerRepresentations in one team
        /// </summary>
        /// <param name="team"></param>
        /// <returns></returns>
        private int getNumberOfTeamMembers(int team)
        {
            int number = 0;

            for (int i = 0; i < playerArray.Length; i++)
                if (playerArray[i].Team == team)
                    number++;

            return number;

        }

        /// <summary>
        /// Returns the number of connected devices
        /// </summary>
        /// <returns>The number of devices</returns>
        private int getNumberOfConnectedDevices()
        {

            int count = 0;

            // Go through the 4 gamepads and check if they're connected
            for (int i = 0; i < 4; i++)
            {
                if (playerArray[i].InputDevice.IsConnected == true)
                    count++;
            }

            // If it's a PC the keyboard is connected for sure
            // -> 2 Devices
            #if WINDOWS
            count += 2;
            #endif

            return count;
        }

        /// <summary>
        /// Moves the given PlayerRepresentation one position left
        /// </summary>
        /// <param name="player">The PlayerRepresentation that should be moved</param>
        private void movePlayerRepresentationLeft(PlayerRepresentationMainMenu player)
        {

            // From the right to the middle
            if (player.Team == 2)
            {
                player.Team = 0;
                player.setXPosition(xPositionNoTeam);
            }
            // From the middle to the left
            else if (player.Team == 0)
            {
                // Check if it can move left, otherwise show warning
                if (((menuState == 1) && (getNumberOfTeamMembers(1) < 1)) ||
                    ((menuState == 2) && (getNumberOfTeamMembers(1) < 2)))
                {
                    player.Team = 1;
                    player.setXPosition(xPositionLeftTeam);
                }
                else
                {
                    ((MenuObject)menuObjectList.GetObjectByName("TeamMenuWarning1_Overlay")).FadingAnimation = null;
                    ((MenuObject)menuObjectList.GetObjectByName("TeamMenuWarning1_Overlay")).Visible = false;
                    ((MenuObject)menuObjectList.GetObjectByName("TeamMenuWarning2_Overlay")).FadingAnimation = new FadingAnimation(false, true, 2000, true, 200);
                    SoundManager.Instance.playMenuSoundEffect((int)MenuSound.Warning);
                }
            }

        }


        /// <summary>
        /// Moves the given PlayerRepresentation one position right
        /// </summary>
        /// <param name="player">The PlayerRepresentation that should be moved</param>
        private void movePlayerRepresentationRight(PlayerRepresentationMainMenu player)
        {

            // From the left to the middle
            if (player.Team == 1)
            {
                player.Team = 0;
                player.setXPosition(xPositionNoTeam);
            }
            // From the middle to the right
            else if (player.Team == 0)
            {
                // Check if it can move right, otherwise show warning
                if (((menuState == 1) && (getNumberOfTeamMembers(2) < 1)) ||
                    ((menuState == 2) && (getNumberOfTeamMembers(2) < 2)))
                {
                    player.Team = 2;
                    player.setXPosition(xPositionRightTeam);
                }
                else
                {
                    ((MenuObject)menuObjectList.GetObjectByName("TeamMenuWarning1_Overlay")).FadingAnimation = null;
                    ((MenuObject)menuObjectList.GetObjectByName("TeamMenuWarning1_Overlay")).Visible = false;
                    ((MenuObject)menuObjectList.GetObjectByName("TeamMenuWarning2_Overlay")).FadingAnimation = new FadingAnimation(false, true, 2000, true, 200);
                    SoundManager.Instance.playMenuSoundEffect((int)MenuSound.Warning);
                }
            }

        }

        #endregion

    }

    /// <summary>
    /// A player representation for the main menu that stores all
    /// attributes that are needed.
    /// No actual player, just used in MainMenu
    /// </summary>
    public class PlayerRepresentationMainMenu
    {

        /// <param name="playerPicture">The object for the player Picture</param>
        public MenuObject PlayerPicture;

        /// <param name="inputDevice">The input device that is assigned to the player</param>
        public Gamepad InputDevice;

        /// <param name="ki">True if KI player</param>
        public bool KI;

        /// <summary>
        /// The index of the player (between 1 and 4)
        /// </summary>
        public int PlayerIndex;

        /// <summary>
        /// Defines if the PlayerRepresentation is an active player at the moment
        /// </summary>
        public bool Active;

        /// <summary>
        /// The team to which the PlayerRepresentation belongs
        /// (0: no team)
        /// </summary>
        public int Team;

        /// <summary>
        /// True if the PlayerRepresentation's device was connected in the last loop
        /// </summary>
        public bool Connected;

        /// <summary>
        /// Initializes a new PlayerRepresentation
        /// </summary>
        /// <param name="playerPicture">The MenuObject that represents the PlayerIcon</param>
        /// <param name="inputDevice">The input device</param>
        /// <param name="ki">True if it's a KI player</param>
        /// <param name="playerIndex">The index of the player</param>
        public PlayerRepresentationMainMenu(MenuObject playerPicture, Gamepad inputDevice, 
            bool ki, int playerIndex)
        {

            this.PlayerPicture = playerPicture;
            this.InputDevice = inputDevice;
            this.KI = ki;
            this.PlayerIndex = playerIndex;

            Team = 0;
            Connected = false;


        }

        /// <summary>
        /// Sets a new x-Position for the PlayerRepresentation
        /// </summary>
        /// <param name="newPosition">The x-value for the new position</param>
        public void setXPosition(float xPosition)
        {
            
            PlayerPicture.Position = new Vector2(xPosition, PlayerPicture.Position.Y);

        }

        /// <summary>
        /// Sets a new y-Position for the PlayerRepresentation
        /// </summary>
        /// <param name="newPosition">The y-value for the new position</param>
        public void setYPosition(float yPosition)
        {

            PlayerPicture.Position = new Vector2(PlayerPicture.Position.X, yPosition);

        }


    }

}
