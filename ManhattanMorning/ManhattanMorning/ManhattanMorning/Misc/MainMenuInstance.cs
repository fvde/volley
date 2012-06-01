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
        /// Local list of all players of the left team
        /// (to initialize GameInstance)
        /// </summary>
        private List<PlayerRepresentationMainMenu> leftPlayers;

        /// <summary>
        /// Local list of all players of the right team
        /// (to initialize GameInstance)
        /// </summary>
        private List<PlayerRepresentationMainMenu> rightPlayers;

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
        private float[] yPositionsPlayerRepresentations2vs2 = { 0.22f, 0.37f, 0.52f, 0.67f };

        /// <summary>
        /// Size of a PlayerRepresentation (in percent of screen)
        /// </summary>
        private Vector2 sizePlayerRepresentation = new Vector2(0.09f, 0.22f);

        /// <summary>
        /// The distance between PlayerPicture and PlayerTitle in a PlayerRepresentation
        /// </summary>
        private float offsetPictureTitle = 0.11f;

        /// <summary>
        /// Array of all 4 available Player
        /// </summary>
        private PlayerRepresentationMainMenu[] availablePlayer;

        /// <summary>
        /// A list of all KI players
        /// (necessary when switching from select level back to team menu)
        /// </summary>
        private List<PlayerRepresentationMainMenu> kiPlayer;

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

        #endregion


        #region Initialization

        /// <summary>
        /// Initializes the main menu
        /// </summary>
        /// <param name="levels">Reference to the levels class to get all level previews</param>
        public MainMenuInstance(Levels.Levels levels)
        {
            //load intro
            introVideo = Game1.Instance.Content.Load<Video>(@"Videos\video2");
            // Save attributes
            this.levels = levels;
            
            // Load the objects
            menuObjectList = StorageManager.Instance.LoadMainMenuObjects();

            // Add the levelpreviews to the mainMenuObjectList
            foreach (LayerInterface levelPreview in levels.LevelPreviews)
                menuObjectList.Add(levelPreview);

            // Create the structure
            createMenuStructure();

            // set all necessary members
            selectedItem = 0;
            menuState = 0;
            previousMenuState = 0;
            previousMainScreenSelectedItem = 0;

            availablePlayer = new PlayerRepresentationMainMenu[4];
            availablePlayer[0] = new PlayerRepresentationMainMenu((MenuObject)menuObjectList.GetObjectByName("TeamMenu_Player1_Title"),
                (MenuObject)menuObjectList.GetObjectByName("TeamMenu_Player1_Picture"), new Gamepad(PlayerIndex.One, InputDevice.Gamepad, true),
                false, 1, new Vector2(xPositionNoTeam, yPositionsPlayerRepresentations2vs2[0]), sizePlayerRepresentation, offsetPictureTitle);
            availablePlayer[1] = new PlayerRepresentationMainMenu((MenuObject)menuObjectList.GetObjectByName("TeamMenu_Player2_Title"),
                (MenuObject)menuObjectList.GetObjectByName("TeamMenu_Player2_Picture"), new Gamepad(PlayerIndex.Two, InputDevice.Gamepad, true),
                false, 2, new Vector2(xPositionNoTeam, yPositionsPlayerRepresentations2vs2[1]), sizePlayerRepresentation, offsetPictureTitle);
            availablePlayer[2] = new PlayerRepresentationMainMenu((MenuObject)menuObjectList.GetObjectByName("TeamMenu_Player3_Title"),
                (MenuObject)menuObjectList.GetObjectByName("TeamMenu_Player3_Picture"), new Gamepad(PlayerIndex.Three, InputDevice.Gamepad, true),
                false, 3, new Vector2(xPositionNoTeam, yPositionsPlayerRepresentations2vs2[2]), sizePlayerRepresentation, offsetPictureTitle);
            availablePlayer[3] = new PlayerRepresentationMainMenu((MenuObject)menuObjectList.GetObjectByName("TeamMenu_Player4_Title"),
                (MenuObject)menuObjectList.GetObjectByName("TeamMenu_Player4_Picture"), new Gamepad(PlayerIndex.Four, InputDevice.Gamepad, true),
                false, 4, new Vector2(xPositionNoTeam, yPositionsPlayerRepresentations2vs2[3]), sizePlayerRepresentation, offsetPictureTitle);

            leftPlayers = new List<PlayerRepresentationMainMenu>();

            rightPlayers = new List<PlayerRepresentationMainMenu>();

            levelName = "";

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
                updateTeamplayMenu(gamepadArray);
            }

            manageOverlays(gameTime);

        }

        /// <summary>
        /// Checks if gamepads are connected/disconnected now
        /// and updates the teamplayMenu according to these changes
        /// </summary>
        /// <param name="connectedGamePads">Array with all gamepads, independent from
        /// being connected or disconnected</param>
        private void updateTeamplayMenu(Gamepad[] gamepadArray)
        {

            // When in teamplay 1vs1 menu
            if (menuState == 1)
            {

                List<int> indexesToBeSet = new List<int>();
                indexesToBeSet.Add(1);
                indexesToBeSet.Add(2);

                // Update player textures (Gamepad, Keyboard, KI)

                // 1. Set gamepads
                if (gamepadArray[0].IsConnected)
                {
                    // Set texture
                    availablePlayer[0].PlayerPicture.Texture =
                        StorageManager.Instance.getTextureByName("texture_TeamMenu_Gamepad");
                    // Set device and index
                    availablePlayer[0].InputDevice = new Gamepad(PlayerIndex.One, InputDevice.Gamepad, true);
                    availablePlayer[0].KI = false;

                    indexesToBeSet.Remove(1);
                }

                if (gamepadArray[1].IsConnected)
                {
                    // Set texture
                    availablePlayer[1].PlayerPicture.Texture =
                        StorageManager.Instance.getTextureByName("texture_TeamMenu_Gamepad");
                    // Set device and index
                    availablePlayer[1].InputDevice = new Gamepad(PlayerIndex.Two, InputDevice.Gamepad, true);
                    availablePlayer[1].KI = false;

                    indexesToBeSet.Remove(2);
                }

                // 2. Set Keyboards (Windows)
#if WINDOWS

                for (int i = 0; i < 2; i++)
                {
                    if (indexesToBeSet.Count > 0)
                    {
                        // Set texture
                        availablePlayer[indexesToBeSet.First() - 1].PlayerPicture.Texture =
                            StorageManager.Instance.getTextureByName("texture_TeamMenu_Keyboard");
                        // Set device and index
                        PlayerIndex playerIndex;
                        if (i == 0)
                            playerIndex = PlayerIndex.One;
                        else
                            playerIndex = PlayerIndex.Two;
                        availablePlayer[indexesToBeSet.First() - 1].InputDevice = new Gamepad(playerIndex, InputDevice.Keyboard, true);
                        availablePlayer[indexesToBeSet.First() - 1].KI = false;

                        indexesToBeSet.Remove(indexesToBeSet.First());
                    }
                }

#endif

                // 3. Set KI 

                while (indexesToBeSet.Count > 0)
                {
                    // Set texture
                    availablePlayer[indexesToBeSet.First() - 1].PlayerPicture.Texture =
                        StorageManager.Instance.getTextureByName("texture_TeamMenu_KI");

                    // Set device and index
                    PlayerIndex playerIndex;
                    if (indexesToBeSet.First() == 0)
                        playerIndex = PlayerIndex.One;
                    else
                        playerIndex = PlayerIndex.Two;

                    availablePlayer[indexesToBeSet.First() - 1].InputDevice = new Gamepad(playerIndex, InputDevice.Gamepad, true);

                    indexesToBeSet.Remove(indexesToBeSet.First());
                }

            }

            // When in teamplay 2vs2 menu
            else if (menuState == 2)
            {

                List<int> indexesToBeSet = new List<int>();
                indexesToBeSet.Add(1);
                indexesToBeSet.Add(2);
                indexesToBeSet.Add(3);
                indexesToBeSet.Add(4);

                // Update player textures (Gamepad, Keyboard, KI)

                // 1. Set gamepads
                if (gamepadArray[0].IsConnected)
                {
                    // Set texture
                    availablePlayer[0].PlayerPicture.Texture =
                        StorageManager.Instance.getTextureByName("texture_TeamMenu_Gamepad");
                    // Set device and index
                    availablePlayer[0].InputDevice = new Gamepad(PlayerIndex.One, InputDevice.Gamepad, true);
                    availablePlayer[0].KI = false;

                    indexesToBeSet.Remove(1);
                }

                if (gamepadArray[1].IsConnected)
                {
                    // Set texture
                    availablePlayer[1].PlayerPicture.Texture =
                        StorageManager.Instance.getTextureByName("texture_TeamMenu_Gamepad");
                    // Set device and index
                    availablePlayer[1].InputDevice = new Gamepad(PlayerIndex.Two, InputDevice.Gamepad, true);
                    availablePlayer[1].KI = false;

                    indexesToBeSet.Remove(2);
                }

                if (gamepadArray[2].IsConnected)
                {
                    // Set texture
                    availablePlayer[2].PlayerPicture.Texture =
                        StorageManager.Instance.getTextureByName("texture_TeamMenu_Gamepad");
                    // Set device and index
                    availablePlayer[2].InputDevice = new Gamepad(PlayerIndex.Three, InputDevice.Gamepad, true);
                    availablePlayer[2].KI = false;

                    indexesToBeSet.Remove(3);
                }

                if (gamepadArray[3].IsConnected)
                {
                    // Set texture
                    availablePlayer[3].PlayerPicture.Texture =
                        StorageManager.Instance.getTextureByName("texture_TeamMenu_Gamepad");
                    // Set device and index
                    availablePlayer[3].InputDevice = new Gamepad(PlayerIndex.Four, InputDevice.Gamepad, true);
                    availablePlayer[3].KI = false;

                    indexesToBeSet.Remove(4);
                }

                // 2. Set Keyboards (Windows)
#if WINDOWS

                for (int i = 0; i < 2; i++)
                {
                    if (indexesToBeSet.Count > 0)
                    {
                        // Set texture
                        availablePlayer[indexesToBeSet.First() - 1].PlayerPicture.Texture =
                            StorageManager.Instance.getTextureByName("texture_TeamMenu_Keyboard");
                        // Set device and index
                        PlayerIndex playerIndex;
                        if (i == 0)
                            playerIndex = PlayerIndex.One;
                        else
                            playerIndex = PlayerIndex.Two;
                        availablePlayer[indexesToBeSet.First() - 1].InputDevice = new Gamepad(playerIndex, InputDevice.Keyboard, true);
                        availablePlayer[indexesToBeSet.First() - 1].KI = false;

                        indexesToBeSet.Remove(indexesToBeSet.First());
                    }
                }

#endif

                // 3. Set KI 

                while (indexesToBeSet.Count > 0)
                {
                    // Set texture
                    availablePlayer[indexesToBeSet.First() - 1].PlayerPicture.Texture =
                        StorageManager.Instance.getTextureByName("texture_TeamMenu_KI");

                    // Set device and index
                    PlayerIndex playerIndex;
                    if (indexesToBeSet.First() == 0)
                        playerIndex = PlayerIndex.One;
                    else if (indexesToBeSet.First() == 1)
                        playerIndex = PlayerIndex.Two;
                    else if (indexesToBeSet.First() == 2)
                        playerIndex = PlayerIndex.Three;
                    else
                        playerIndex = PlayerIndex.Four;

                    availablePlayer[indexesToBeSet.First() - 1].InputDevice = new Gamepad(playerIndex, InputDevice.Gamepad, true);

                    indexesToBeSet.Remove(indexesToBeSet.First());
                }


            }



        }

        /// <summary>
        /// Triggers all necessary actions when the down button is pressed
        /// </summary>
        public void downButtonPressed()
        {
            
            // When in main screen
            if (menuState == 0)
            {

                SoundManager.Instance.playSoundEffect((int)Sound.menu_switch);

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
                    SoundManager.Instance.playSoundEffect((int)Sound.menu_switch);


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

                SoundManager.Instance.playSoundEffect((int)Sound.menu_switch);

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

                SoundManager.Instance.playSoundEffect((int)Sound.menu_switch);

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
                    SoundManager.Instance.playSoundEffect((int)Sound.menu_switch);

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

                SoundManager.Instance.playSoundEffect((int)Sound.menu_switch);

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

                SoundManager.Instance.playSoundEffect((int)Sound.menu_switch);

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
            // When in team menu 1vs1
            else if (menuState == 1)
            {

                SoundManager.Instance.playSoundEffect((int)Sound.menu_switch);

                // Save local reference
                PlayerRepresentationMainMenu player = getAvailablePlayer(device, deviceIndex);
                if (player == null)
                    return;

                // Move from the right to the middle
                if (rightPlayers.Contains(player))
                {
                    rightPlayers.Remove(player);
                    player.setXPosition(xPositionNoTeam);
                }
                // Move from the middle to the left
                else if (!leftPlayers.Contains(player))
                {
                    // if nobody else is in the left team
                    if (leftPlayers.Count < 1)
                    {
                        leftPlayers.Add(player);
                        player.setXPosition(xPositionLeftTeam);
                    }
                }

                timeSinceLastInput = 0;
            }
            // When in team menu 2vs2
            else if (menuState == 2)
            {

                SoundManager.Instance.playSoundEffect((int)Sound.menu_switch);

                // Save local reference
                PlayerRepresentationMainMenu player = getAvailablePlayer(device, deviceIndex);
                if (player == null)
                    return;

                // Move from the right to the middle
                if (rightPlayers.Contains(player))
                {
                    rightPlayers.Remove(player);
                    player.setXPosition(xPositionNoTeam);
                }
                // Move from the middle to the left
                else if (!leftPlayers.Contains(player))
                {
                    // if nobody else is in the left team
                    if (leftPlayers.Count < 2)
                    {
                        leftPlayers.Add(player);
                        player.setXPosition(xPositionLeftTeam);
                    }
                }

                timeSinceLastInput = 0;
            }
            // When in help
            else if (menuState == 3)
            {

                SoundManager.Instance.playSoundEffect((int)Sound.menu_switch);

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

                SoundManager.Instance.playSoundEffect((int)Sound.menu_switch);

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

                SoundManager.Instance.playSoundEffect((int)Sound.menu_switch);

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
            // When in team menu 1vs1
            else if (menuState == 1)
            {

                SoundManager.Instance.playSoundEffect((int)Sound.menu_switch);

                // Save local reference
                PlayerRepresentationMainMenu player = getAvailablePlayer(device, deviceIndex);
                if (player == null)
                    return;

                // Move from the left to the middle
                if (leftPlayers.Contains(player))
                {
                    leftPlayers.Remove(player);
                    player.setXPosition(xPositionNoTeam);
                }
                // Move from the middle to the right
                else if (!rightPlayers.Contains(player))
                {
                    // if nobody else is in the left team
                    if (rightPlayers.Count < 1)
                    {
                        rightPlayers.Add(player);
                        player.setXPosition(xPositionRightTeam);
                    }
                }

                timeSinceLastInput = 0;
            }
            // When in team menu 2vs2
            else if (menuState == 2)
            {

                SoundManager.Instance.playSoundEffect((int)Sound.menu_switch);

                // Save local reference
                PlayerRepresentationMainMenu player = getAvailablePlayer(device, deviceIndex);
                if (player == null)
                    return;

                // Move from the left to the middle
                if (leftPlayers.Contains(player))
                {
                    leftPlayers.Remove(player);
                    player.setXPosition(xPositionNoTeam);
                }
                // Move from the middle to the right
                else if (!rightPlayers.Contains(player))
                {
                    // if nobody else is in the left team
                    if (rightPlayers.Count < 2)
                    {
                        rightPlayers.Add(player);
                        player.setXPosition(xPositionRightTeam);
                    }
                }

                timeSinceLastInput = 0;
            }
            // When in help
            else if (menuState == 3)
            {

                SoundManager.Instance.playSoundEffect((int)Sound.menu_switch);

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

                SoundManager.Instance.playSoundEffect((int)Sound.menu_switch);

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
                SoundManager.Instance.playSoundEffect((int)Sound.menu_select);

                timeSinceLastInput = 0;
            }
            // When in team menu 1vs1
            else if (menuState == 1)
            {
                timeSinceLastInput = 0;

                if (((bool)SettingsManager.Instance.get("AllowKIOnlyGame") == false) &&
                    (leftPlayers.Count == 0) && (rightPlayers.Count == 0))
                {
                    ((MenuObject)menuObjectList.GetObjectByName("TeamMenuWarning_Overlay")).FadingAnimation = new FadingAnimation(false, true, 2000, true, 200);
                    SoundManager.Instance.playSoundEffect((int)Sound.menu_warning);
                    return;
                }
                // Switch to select level
                SoundManager.Instance.playSoundEffect((int)Sound.menu_select);
                previousMenuState = 1;
                switchMenuState(5);

            }
            // When in team menu 2vs2
            else if (menuState == 2)
            {
                timeSinceLastInput = 0;

                if (((bool)SettingsManager.Instance.get("AllowKIOnlyGame") == false) &&
                    (leftPlayers.Count == 0) && (rightPlayers.Count == 0))
                {
                    ((MenuObject)menuObjectList.GetObjectByName("TeamMenuWarning_Overlay")).FadingAnimation = new FadingAnimation(false, true, 2000, true, 200);
                    SoundManager.Instance.playSoundEffect((int)Sound.menu_warning);
                    return;
                }
                // Switch to select level
                SoundManager.Instance.playSoundEffect((int)Sound.menu_select);
                previousMenuState = 2;
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
                    SoundManager.Instance.playSoundEffect((int)Sound.menu_select);
                }
                else
                {
                    // Go back to main screen
                    switchMenuState(0);
                    SoundManager.Instance.playSoundEffect((int)Sound.menu_select);
                }

                timeSinceLastInput = 0;
            }
            // When in select level
            else if (menuState == 5)
            {

                // Start game
                SuperController.Instance.switchFromMainMenuToIngame(levelName, winCondition, leftPlayers, rightPlayers);
                SoundManager.Instance.playSoundEffect((int)Sound.menu_select);

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

                // Go back to main screen
                switchMenuState(0);
                timeSinceLastInput = 0;

            }
            // When in team menu 2vs2
            else if (menuState == 2)
            {

                // Go back to main screen
                switchMenuState(0);
                timeSinceLastInput = 0;

            }
            // When in help
            else if (menuState == 3)
            {

                // Go back to main screen
                switchMenuState(0);
                timeSinceLastInput = 0;

            }
            // When in really quit
            else if (menuState == 4)
            {

                // Go back to main screen
                switchMenuState(0);
                timeSinceLastInput = 0;

            }
            // When in select level
            else if (menuState == 5)
            {

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
            menuStructure[1, 0].Add(menuObjectList.GetObjectByName("TeamMenu_Player1_Title"));
            menuStructure[1, 0].Add(menuObjectList.GetObjectByName("TeamMenu_Player1_Picture"));
            menuStructure[1, 0].Add(menuObjectList.GetObjectByName("TeamMenu_Player2_Title"));
            menuStructure[1, 0].Add(menuObjectList.GetObjectByName("TeamMenu_Player2_Picture"));

            // Team menu 2vs2
            menuStructure[2, 0] = new List<LayerInterface>();
            menuStructure[2, 0].Add(menuObjectList.GetObjectByName("MainMenu_Background"));
            menuStructure[2, 0].Add(menuObjectList.GetObjectByName("TeamMenu_Background2_2vs2"));
            menuStructure[2, 0].Add(menuObjectList.GetObjectByName("TeamMenu_Player1_Title"));
            menuStructure[2, 0].Add(menuObjectList.GetObjectByName("TeamMenu_Player1_Picture"));
            menuStructure[2, 0].Add(menuObjectList.GetObjectByName("TeamMenu_Player2_Title"));
            menuStructure[2, 0].Add(menuObjectList.GetObjectByName("TeamMenu_Player2_Picture"));
            menuStructure[2, 0].Add(menuObjectList.GetObjectByName("TeamMenu_Player3_Title"));
            menuStructure[2, 0].Add(menuObjectList.GetObjectByName("TeamMenu_Player3_Picture"));
            menuStructure[2, 0].Add(menuObjectList.GetObjectByName("TeamMenu_Player4_Title"));
            menuStructure[2, 0].Add(menuObjectList.GetObjectByName("TeamMenu_Player4_Picture"));

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

            // First, make all objects invisible
            for (int i = 0; i < menuStructure.GetLength(0); i++)
                for (int j = 0; j < menuStructure.GetLength(1); j++)
                    if (menuStructure[i, j] != null)
                        foreach (LayerInterface menuObject in menuStructure[i, j])
                            ((DrawableObject)menuObject).Visible = false;

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
                leftPlayers = new List<PlayerRepresentationMainMenu>();
                rightPlayers = new List<PlayerRepresentationMainMenu>();


                int counter = 0;
                foreach (PlayerRepresentationMainMenu player in availablePlayer)
                {
                    // Set size and position of PlayerRepresentation
                    if (newState == 1)
                        player.setYPosition(yPositionsPlayerRepresentations1vs1[counter]);

                    if (newState == 2)
                        player.setYPosition(yPositionsPlayerRepresentations2vs2[counter]);
                    
                    player.setXPosition(xPositionNoTeam);
                    player.KI = false;

                    counter++;
                }

            }

            // When switching from team menu to select level fill with KI players
            if (newState == 5)
            {

                kiPlayer = new List<PlayerRepresentationMainMenu>();

                // If it's a 1vs1 game
                if (menuState == 1)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        if (availablePlayer[i].Position == new Vector2(xPositionNoTeam, yPositionsPlayerRepresentations1vs1[i]))
                        {
                            if (leftPlayers.Count < 1)
                            {
                                availablePlayer[i].KI = true;
                                leftPlayers.Add(availablePlayer[i]);
                                kiPlayer.Add(availablePlayer[i]);
                            }
                            else if (rightPlayers.Count < 1)
                            {
                                availablePlayer[i].KI = true;
                                rightPlayers.Add(availablePlayer[i]);
                                kiPlayer.Add(availablePlayer[i]);
                            }
                        }
                    }
                }
                // If it's a 2vs2 game
                else if (menuState == 2)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (availablePlayer[i].Position == new Vector2(xPositionNoTeam, yPositionsPlayerRepresentations2vs2[i]))
                        {
                            if (leftPlayers.Count < 2)
                            {
                                availablePlayer[i].KI = true;
                                leftPlayers.Add(availablePlayer[i]);
                                kiPlayer.Add(availablePlayer[i]);
                            }
                            else if (rightPlayers.Count < 2)
                            {
                                availablePlayer[i].KI = true;
                                rightPlayers.Add(availablePlayer[i]);
                                kiPlayer.Add(availablePlayer[i]);
                            }
                        }
                    }
                }

            }

            // When switching from select level back to team menu
            if ((menuState == 5) && ((newState == 1) || (newState == 2)))
            {

                // Deselect KI flag and remove it from team
                foreach (PlayerRepresentationMainMenu player in kiPlayer)
                {
                    player.KI = false;
                    if (leftPlayers.Contains(player))
                        leftPlayers.Remove(player);
                    else if (rightPlayers.Contains(player))
                        rightPlayers.Remove(player);
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
                    overlayObject.Animation = null;
                    overlayObject.Visible = false;
                    overlayObject = (MenuObject)menuObjectList.GetObjectByName("MainScreen_Overlay");
                    break;
                case 1:
                    overlayObject.Animation = null;
                    overlayObject.Visible = false;
                    overlayObject = (MenuObject)menuObjectList.GetObjectByName("TeamMenu_Overlay");
                    break;
                case 2:
                    overlayObject.Animation = null;
                    overlayObject.Visible = false;
                    overlayObject = (MenuObject)menuObjectList.GetObjectByName("TeamMenu_Overlay");
                    break;
                case 3:
                    overlayObject.Animation = null;
                    overlayObject.Visible = false;
                    overlayObject = (MenuObject)menuObjectList.GetObjectByName("Help_Overlay");
                    break;
                case 4:
                    overlayObject.Animation = null;
                    overlayObject.Visible = false;
                    overlayObject = (MenuObject)menuObjectList.GetObjectByName("ReallyQuit_Overlay");
                    break;
                case 5:
                    overlayObject.Animation = null;
                    overlayObject.Visible = false;
                    overlayObject = (MenuObject)menuObjectList.GetObjectByName("SelectLevel_Overlay");
                    break;
            }
            ((MenuObject)menuObjectList.GetObjectByName("TeamMenuWarning_Overlay")).FadingAnimation = null;
            ((MenuObject)menuObjectList.GetObjectByName("TeamMenuWarning_Overlay")).Visible = false;

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
        /// Gets the specified PlayerRepresentation from the available player
        /// </summary>
        /// <param name="device">The device of the player</param>
        /// <param name="deviceIndex">The index of the device</param>
        /// <returns>The PlayerRepresentation</returns>
        private PlayerRepresentationMainMenu getAvailablePlayer(InputDevice device, PlayerIndex deviceIndex)
        {

            for (int i = 0; i < availablePlayer.Length; i++)
                if ((device == availablePlayer[i].InputDevice.Device) && (deviceIndex == availablePlayer[i].InputDevice.Index))
                    return availablePlayer[i];

            // If PlayerRepresentation isn't found, return null
            return null;
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


        #endregion

    }

    /// <summary>
    /// A player representation for the main menu that stores all
    /// attributes that are needed.
    /// No actual player, just used in MainMenu
    /// </summary>
    public class PlayerRepresentationMainMenu
    {

        /// <param name="playerTitle">The object for the title texture</param>
        public MenuObject PlayerTitle;

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
        /// The position of the PlayerRepresentation
        /// </summary>
        public Vector2 Position;

        /// <summary>
        /// The size of the PlayerRepresentationMenu
        /// </summary>
        public Vector2 Size;

        /// <summary>
        /// The distance between picture and title in percent of screen
        /// </summary>
        private float offsetPictureTitle;

        /// <summary>
        /// Initializes a new PlayerRepresentation
        /// </summary>
        /// <param name="playerTitle">The object for the title texture</param>
        /// <param name="playerPicture">The object for the player Picture</param>
        /// <param name="inputDevice">The input device that is assigned to the player</param>
        /// <param name="ki">True if KI player</param>
        /// <param name="playerIndex">The index of the player (between 1 and 4)</param>
        /// <param name="position">The position of the PlayerRepresentation</param>
        /// <param name="size">The size of the PlayerRepresentation</param>
        /// <param name="offsetPictureTitle">The distance between picture and title in percent of screen</param>
        public PlayerRepresentationMainMenu(MenuObject playerTitle, MenuObject playerPicture, Gamepad inputDevice, 
            bool ki, int playerIndex, Vector2 position, Vector2 size, float offsetPictureTitle)
        {

            this.PlayerTitle = playerTitle;
            this.PlayerPicture = playerPicture;
            this.InputDevice = inputDevice;
            this.KI = ki;
            this.PlayerIndex = playerIndex;
            this.Position = position;
            this.Size = size;
            this.offsetPictureTitle = offsetPictureTitle;

            this.PlayerTitle.Position = new Vector2(position.X, position.Y + offsetPictureTitle);
            this.PlayerTitle.Size = new Vector2(size.X, 0.3f * size.Y);
            this.PlayerPicture.Position = new Vector2(position.X, position.Y);
            this.PlayerPicture.Size = new Vector2(size.X, 0.7f * size.Y);

        }

        /// <summary>
        /// Sets a new x-Position for the PlayerRepresentation
        /// </summary>
        /// <param name="newPosition">The x-value for the new position</param>
        public void setXPosition(float xPosition)
        {

            Position = new Vector2(xPosition, Position.Y);

            // Set the new position in the player representation textures
            PlayerTitle.Position = new Vector2(xPosition, PlayerTitle.Position.Y);
            PlayerPicture.Position = new Vector2(xPosition, PlayerPicture.Position.Y);

        }

        /// <summary>
        /// Sets a new y-Position for the PlayerRepresentation
        /// </summary>
        /// <param name="newPosition">The y-value for the new position</param>
        public void setYPosition(float yPosition)
        {

            Position = new Vector2(Position.X, yPosition);

            // Set the new position in the player representation textures
            PlayerTitle.Position = new Vector2(PlayerTitle.Position.X, yPosition + offsetPictureTitle);
            PlayerPicture.Position = new Vector2(PlayerPicture.Position.X, yPosition);

        }

    }

}
