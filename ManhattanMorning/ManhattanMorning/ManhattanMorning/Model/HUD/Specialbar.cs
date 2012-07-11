using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ManhattanMorning.Misc;
using ManhattanMorning.Model;
using ManhattanMorning.Model.GameObject;
using ManhattanMorning.Controller;
using ManhattanMorning.Misc.Curves;

namespace ManhattanMorning.Model.HUD
{
    public class Specialbar: IObserver
    {



        #region Members

        /// <summary>
        /// Reference of the HUD element that represents the bar of the left specialbar
        /// </summary>
        private HUD specialbarBar_Left;

        /// <summary>
        /// Reference of the HUD element that represents the bar of the right specialbar
        /// </summary>
        private HUD specialbarBar_Right;

        /// <summary>
        /// Reference of the Scoreboard 
        /// (Necessary to calculate exact position for specialbars)
        /// </summary>
        private HUD scorebard;

        /// <summary>
        /// x-Size of the left (x) and right (y) specialbar
        /// </summary>
        private Vector2 specialbarSize;

        /// <summary>
        /// The target x-Size of the left(x) and right(y) specialbar after the animation finished
        /// </summary>
        private Vector2 specialbarTargetSize;

        /// <summary>
        /// original x-Size of the left (x) and right (y) specialbar
        /// </summary>
        private Vector2 originalSpecialbarSize;

        /// <summary>
        /// The filling value for left(x) and right(y) filling of the specialbar
        /// </summary>
        private Vector2 specialbarFillingValue;

        /// <summary>
        /// The target filling value for left(x) and right(y) filling of the specialbar after the animation finished
        /// </summary>
        private Vector2 specialbarTargetFillingValue;

        /// <summary>
        /// The filling value for left(x) and right(y) that triggers an new positive powerup
        /// and clears the filling
        /// </summary>
        private Vector2 specialbarMaxFillingValue;

        /// <summary>
        /// The maximal filling value for the initial specialbar size 
        /// </summary>
        private float initialMaxFillingValue;

        /// <summary>
        /// The gap size between filling tiles (in percent of screen, can also be negative)
        /// </summary>
        private float gapSizeBetweenFillingTiles;

        /// <summary>
        /// All the filling tiles of the left specialbar
        /// </summary>
        private Stack<HUD> specialbarFillingTiles_Left;

        /// <summary>
        /// All the filling tiles of the right specialbar
        /// </summary>
        private Stack<HUD> specialbarFillingTiles_Right;

        /// <summary>
        /// The speed whith which the specialbar changes its size (in percent per 10ms)
        /// </summary>
        float specialbarSizeChangeAnimationSpeed;

        /// <summary>
        /// The speed whith which the specialbar filling changes its size (in value per 10ms)
        /// </summary>
        float specialbarFillingChangeAnimationSpeed;

        /// <summary>
        /// The size of a tile
        /// </summary>
        Vector2 tileSize;

        /// <summary>
        /// Defines the filling value per filling tile
        /// </summary>
        float fillingValuePerTile;

        /// <summary>
        /// True if powerops and specialbar should be turned on
        /// </summary>
        bool powerupsOn;

        /// <summary>
        /// Moves starting point of left/right specialbar to right position
        /// </summary>
        private float specialbar_offset = 0.056f;

        /// <summary>
        /// The y-Position of every filling tile (in percent of screen)
        /// </summary>
        private float fillingTile_yPosition = 0.049f;

        #endregion


        #region Initialization

        /// <summary>
        /// Initializes a new Specialbar
        /// </summary>
        /// <param name="specialbarObjects">List with references to the specialbarObjects</param>
        public Specialbar(LayerList<HUD> specialbarObjects)
        {

            // Save all necessary parameters
            this.specialbarBar_Left = (HUD)specialbarObjects.GetObjectByName("specialbarBar_Left");
            this.specialbarBar_Right = (HUD)specialbarObjects.GetObjectByName("specialbarBar_Right");
            this.scorebard = (HUD)specialbarObjects.GetObjectByName("Scoreboard");

            powerupsOn = (bool)SettingsManager.Instance.get("enablePowerups");

            // Set all specialbar parameters
            initialMaxFillingValue = (float)SettingsManager.Instance.get("ValueToFillInitialSpecialbar");

            originalSpecialbarSize = (Vector2)SettingsManager.Instance.get("initialSpecialbarSize");
            setSpecialbarSize((Vector2)SettingsManager.Instance.get("initialSpecialbarSize"));
            specialbarTargetSize = new Vector2(specialbarSize.X, specialbarSize.Y);

            specialbarSizeChangeAnimationSpeed = (float)SettingsManager.Instance.get("specialbarSizeChangeAnimationSpeed");
            specialbarFillingChangeAnimationSpeed = (float)SettingsManager.Instance.get("specialbarFillingChangeAnimationSpeed");
            gapSizeBetweenFillingTiles = (float)SettingsManager.Instance.get("GapSizeBetweenFillingTiles");
            specialbarFillingValue = Vector2.Zero;
            specialbarTargetFillingValue = Vector2.Zero;

            specialbarFillingTiles_Left = new Stack<HUD>();
            specialbarFillingTiles_Right = new Stack<HUD>();

            // Set tile size
            int numberOfTiles = (int)SettingsManager.Instance.get("NumberOfSpecialBarFillingTilesToFillInitialSpecialbar");
            float xSize = (originalSpecialbarSize.X - ((float)numberOfTiles * gapSizeBetweenFillingTiles))/ (float)numberOfTiles;
            // Explanation for y-Size: X-Size * proportion texture * proportion Screen
            float ySize = xSize * 0.5556f * 1.7778f;
            tileSize = new Vector2(xSize, ySize);

            fillingValuePerTile = initialMaxFillingValue / (float)numberOfTiles;


        }

        #endregion


        #region Methods

        /// <summary>
        /// Necessary because it has to resize the bars and fillings in a sequence of loops
        /// so that it looks like an animation
        /// </summary>
        /// <param name="gameTime"></param>
        public void update(GameTime gameTime)
        {

            // Resize specialbar if necessary
            #region Specialbar

            // Left specialbar
            float newLeftValue = specialbarSize.X;
            if (specialbarSize.X < specialbarTargetSize.X)
            {
                // /10 because the speed is per 10ms
                newLeftValue = specialbarSize.X + (gameTime.ElapsedGameTime.Milliseconds / 10) *
                    specialbarSizeChangeAnimationSpeed * (specialbarTargetSize.X - specialbarSize.X);
                if ((newLeftValue > specialbarTargetSize.X) || ((specialbarTargetSize.X - specialbarSize.X) < 0.001f))
                    newLeftValue = specialbarTargetSize.X;
            }
            else if (specialbarSize.X > specialbarTargetSize.X)
            {
                // /10 because the speed is per 10ms
                newLeftValue = specialbarSize.X - (gameTime.ElapsedGameTime.Milliseconds / 10) *
                    specialbarSizeChangeAnimationSpeed * (specialbarSize.X - specialbarTargetSize.X);
                if ((newLeftValue < specialbarTargetSize.X) || ((specialbarSize.X - specialbarTargetSize.X) < 0.001f))
                    newLeftValue = specialbarTargetSize.X;
            }

            // Right specialbar
            float newRightValue = specialbarSize.Y;
            if (specialbarSize.Y < specialbarTargetSize.Y)
            {
                // /10 because the speed is per 10ms
                newRightValue = specialbarSize.Y + (gameTime.ElapsedGameTime.Milliseconds / 10) *
                    specialbarSizeChangeAnimationSpeed * (specialbarTargetSize.Y - specialbarSize.Y);
                if ((newRightValue > specialbarTargetSize.Y) || ((specialbarTargetSize.Y - specialbarSize.Y) < 0.001f))
                    newRightValue = specialbarTargetSize.Y;
            }
            else if (specialbarSize.Y > specialbarTargetSize.Y)
            {
                // /10 because the speed is per 10ms
                newRightValue = specialbarSize.Y - (gameTime.ElapsedGameTime.Milliseconds / 10) *
                    specialbarSizeChangeAnimationSpeed * (specialbarSize.Y - specialbarTargetSize.Y);
                if ((newRightValue < specialbarTargetSize.Y) || ((specialbarSize.Y - specialbarTargetSize.Y) < 0.001f))
                    newRightValue = specialbarTargetSize.Y;
            }

            if ((newLeftValue != specialbarSize.X) || (newRightValue != specialbarSize.Y))
                setSpecialbarSize(new Vector2(newLeftValue, newRightValue));

            #endregion


            // Resize specialbarFilling if necessary
            #region SpecialbarFilling

            // Left specialbarFilling
            newLeftValue = specialbarFillingValue.X;
            if (specialbarFillingValue.X < specialbarTargetFillingValue.X)
            {
                // /10 because the speed is per 10ms
                newLeftValue = specialbarFillingValue.X + (gameTime.ElapsedGameTime.Milliseconds / 10) *
                    specialbarFillingChangeAnimationSpeed;
                if (newLeftValue > specialbarTargetFillingValue.X)
                    newLeftValue = specialbarTargetFillingValue.X;
            }
            else if (specialbarFillingValue.X > specialbarTargetFillingValue.X)
            {
                // /10 because the speed is per 10ms
                newLeftValue = specialbarFillingValue.X - (gameTime.ElapsedGameTime.Milliseconds / 10) *
                    specialbarFillingChangeAnimationSpeed;
                if (newLeftValue < specialbarTargetFillingValue.X)
                    newLeftValue = specialbarTargetFillingValue.X;
            }

            // Right specialbarFilling
            newRightValue = specialbarFillingValue.Y;
            if (specialbarFillingValue.Y < specialbarTargetFillingValue.Y)
            {
                // /10 because the speed is per 10ms
                newRightValue = specialbarFillingValue.Y + (gameTime.ElapsedGameTime.Milliseconds / 10) *
                    specialbarFillingChangeAnimationSpeed;
                if (newRightValue > specialbarTargetFillingValue.Y)
                    newRightValue = specialbarTargetFillingValue.Y;
            }
            else if (specialbarFillingValue.Y > specialbarTargetFillingValue.Y)
            {
                // /10 because the speed is per 10ms
                newRightValue = specialbarFillingValue.Y - (gameTime.ElapsedGameTime.Milliseconds / 10) *
                    specialbarFillingChangeAnimationSpeed;
                if (newRightValue < specialbarTargetFillingValue.Y)
                    newRightValue = specialbarTargetFillingValue.Y;
            }

            if ((newLeftValue != specialbarFillingValue.X) || (newRightValue != specialbarFillingValue.Y))
                setSpecialbarFillingValue(new Vector2(newLeftValue, newRightValue));

            #endregion

        }

        /// <summary>
        /// Takes the score and processes the new length of left and right specialbar
        /// If you want to adjust the intensity of the resize, change it in the SettingsManager
        /// </summary>
        /// <param name="newScore">The new score</param>
        public void processNewSpecialbarSize(Vector2 newScore)
        {
            
            if (!powerupsOn)
                return;

            // Get intensity
            float intensity = (float)SettingsManager.Instance.get("resizeIntensitySpecialbar");
            // calculate logarithmic offset
            float offset = (float)Math.Log(1 + Math.Abs(newScore.X - newScore.Y)) * (0.01f * intensity);

            if (newScore.X < newScore.Y)
            {
                // right team is ahead
                // => add it to the right, substract it from the left
                specialbarTargetSize = new Vector2(originalSpecialbarSize.X - offset, originalSpecialbarSize.Y + offset);
            }
            else
            {
                // left team is ahead or tie
                // => add it to the left, subtract it from the right
                specialbarTargetSize = new Vector2(originalSpecialbarSize.X + offset, originalSpecialbarSize.Y - offset);
            }

        }

        /// <summary>
        /// Changes the filling of the specialbar and takes action if it's full
        /// </summary>
        /// <param name="offset">The increase/decrease value for the left(x) and right(y) filling of the bar</param>
        public void processNewSpecialbarFilling(Vector2 offset)
        {

            if (!powerupsOn)
                return;

            // Add offset to targetValue
            specialbarTargetFillingValue += offset;

            // Make sure that targetValue isn't negative
            if (specialbarTargetFillingValue.X < 0)
                specialbarTargetFillingValue = new Vector2(0, specialbarTargetFillingValue.Y);
            if (specialbarTargetFillingValue.Y < 0)
                specialbarTargetFillingValue = new Vector2(specialbarTargetFillingValue.X, 0);
            
        }

        #endregion

        #region HelperMethods

        /// <summary>
        /// Sets the specialbar x-Size for left(x) and right(y) specialbar 
        /// </summary>
        /// <param name="size">x-Size for left(x) and right(y) specialbar </param>
        private void setSpecialbarSize(Vector2 size)
        {
            // save size and make sure that it's not a negative size
            specialbarSize = size;
            if (specialbarSize.X < 0)
                specialbarSize = new Vector2(0, specialbarSize.Y);
            if (specialbarSize.Y < 0)
                specialbarSize = new Vector2(specialbarSize.X, 0);


            int pixelXSize_Screen = (int)((Vector2)SettingsManager.Instance.get("screenResolution")).X;
            int pixelYSize_Screen = (int)((Vector2)SettingsManager.Instance.get("screenResolution")).Y;

            // Set left specialbar
            float xPosition = scorebard.Position.X - specialbarSize.X + specialbar_offset;
            specialbarBar_Left.Position = new Vector2(xPosition, specialbarBar_Left.Position.Y);
            specialbarBar_Left.Size = new Vector2(specialbarSize.X, specialbarBar_Left.Size.Y);
            specialbarBar_Left.SourceRectangle = new Rectangle(0, 0, (int)(specialbarBar_Left.Texture.Height / (pixelYSize_Screen * specialbarBar_Left.Size.Y) * pixelXSize_Screen * specialbarBar_Left.Size.X), specialbarBar_Left.Texture.Height);

            // Set right specialbar
            xPosition = scorebard.Position.X + scorebard.Size.X - specialbar_offset;
            specialbarBar_Right.Position = new Vector2(xPosition, specialbarBar_Right.Position.Y);
            specialbarBar_Right.Size = new Vector2(specialbarSize.Y, specialbarBar_Right.Size.Y);
            specialbarBar_Right.SourceRectangle = new Rectangle(0, 0, (int)(specialbarBar_Left.Texture.Height / (pixelYSize_Screen * specialbarBar_Right.Size.Y) * pixelXSize_Screen * specialbarBar_Right.Size.X), specialbarBar_Left.Texture.Height);

            // Set new max filling value
            // -1.5f * fillingValuePerTile, because otherwise the new MaxValue would be too big
            float newMaxFillingValueX = (size.X / originalSpecialbarSize.X) * initialMaxFillingValue -1.7f * fillingValuePerTile;
            float newMaxFillingValueY = (size.Y / originalSpecialbarSize.Y) * initialMaxFillingValue -1.7f * fillingValuePerTile;
            setSpecialbarMaxFillingValue(new Vector2(newMaxFillingValueX, newMaxFillingValueY));

        }

        /// <summary>
        /// Sets the specialbarFilling value for left(x) and right(y) specialbar 
        /// </summary>
        /// <param name="newValue">Value for left(x) and right(y) specialbar </param>
        private void setSpecialbarFillingValue(Vector2 newValue)
        {
            // Make sure that value isn't negative
            if (newValue.X < 0)
                newValue = new Vector2(0, newValue.Y);
            if (newValue.Y < 0)
                newValue = new Vector2(newValue.X, 0);

            // Set new value
            specialbarFillingValue = newValue;

            // Adjust number of tiles
            // Increase when necessary
            while (getNumberOfTilesForFillingValue(newValue.X) > specialbarFillingTiles_Left.Count)
                specialbarFillingTiles_Left.Push(createNewFillingTile(1));
            while (getNumberOfTilesForFillingValue(newValue.Y) > specialbarFillingTiles_Right.Count)
                specialbarFillingTiles_Right.Push(createNewFillingTile(2));

            // Decrease when necessary
            while (getNumberOfTilesForFillingValue(newValue.X) < specialbarFillingTiles_Left.Count)
                SuperController.Instance.GameInstance.GameObjects.Remove(specialbarFillingTiles_Left.Pop());
            while (getNumberOfTilesForFillingValue(newValue.Y) < specialbarFillingTiles_Right.Count)
                SuperController.Instance.GameInstance.GameObjects.Remove(specialbarFillingTiles_Right.Pop());

            // Notify particle systems
            // For left bar
            if (specialbarFillingTiles_Left.Count == 0)
                ParticleSystemsManager.Instance.resizeSpecialbarEmitter(Vector2.Zero, Vector2.Zero, 1);
            else
            {
                Vector2 size = new Vector2((specialbarFillingTiles_Left.Last().Position.X + tileSize.X) - specialbarFillingTiles_Left.First().Position.X,
                    tileSize.Y);
                Vector2 position = new Vector2(specialbarFillingTiles_Left.First().Position.X, specialbarFillingTiles_Left.First().Position.Y);
                ParticleSystemsManager.Instance.resizeSpecialbarEmitter(size, position, 1);
            }
            // For right bar
            if (specialbarFillingTiles_Right.Count == 0)
                ParticleSystemsManager.Instance.resizeSpecialbarEmitter(Vector2.Zero, Vector2.Zero, 2);
            else
            {
                Vector2 size = new Vector2((specialbarFillingTiles_Right.First().Position.X + tileSize.X) - specialbarFillingTiles_Right.Last().Position.X,
                    tileSize.Y);
                Vector2 position = new Vector2(specialbarFillingTiles_Right.Last().Position.X,
                    specialbarFillingTiles_Right.Last().Position.Y);
                ParticleSystemsManager.Instance.resizeSpecialbarEmitter(size, position, 2);
            }

            // Check if value is bigger than maxValue
            if (specialbarFillingValue.X > specialbarMaxFillingValue.X)
                clearSpecialbarFilling(1);
            if (specialbarFillingValue.Y > specialbarMaxFillingValue.Y)
                clearSpecialbarFilling(2);

        }

        /// <summary>
        /// Sets a new max value for the left and right filling of the specialbar
        /// </summary>
        /// <param name="maxValue">max value for left(x) and right(Y) filling</param>
        private void setSpecialbarMaxFillingValue(Vector2 maxValue)
        {

            specialbarMaxFillingValue = maxValue;

            // Make sure that the max values aren't negative
            if (specialbarMaxFillingValue.X < 0)
                specialbarMaxFillingValue = new Vector2(0, specialbarMaxFillingValue.Y);
            if (specialbarMaxFillingValue.Y < 0)
                specialbarMaxFillingValue = new Vector2(specialbarMaxFillingValue.X, 0);

            // Check if there's enough filling that a powerup has to be spawned
            if (specialbarFillingValue.X > specialbarMaxFillingValue.X)
                clearSpecialbarFilling(1);
            if (specialbarFillingValue.Y > specialbarMaxFillingValue.Y)
                clearSpecialbarFilling(2);

        }

        /// <summary>
        /// Clears the filling of the left or right specialbar when it's full
        /// </summary>
        /// <param name="team">1 = left bar, 2 = right bar</param>
        private void clearSpecialbarFilling(int team)
        {
            // Clear filling

            if (team == 1)
            {
                // Clear left bar
                specialbarFillingValue = new Vector2(0, specialbarFillingValue.Y);
                specialbarTargetFillingValue = new Vector2(0, specialbarTargetFillingValue.Y);
                while (specialbarFillingTiles_Left.Count > 0)
                    SuperController.Instance.GameInstance.GameObjects.Remove(specialbarFillingTiles_Left.Pop());
                ParticleSystemsManager.Instance.resizeSpecialbarEmitter(Vector2.Zero, Vector2.Zero, 1);

                // Animate left bar
                specialbarBar_Left.ScalingAnimation = new ScalingAnimation(false, true, 0, true, 500);
                specialbarBar_Left.ScalingAnimation.ScalingRange = new Vector2(1, 1.3f);

                SoundManager.Instance.playIngameSoundEffect((int)IngameSound.SpecialbarFull);
            }
            else
            {
                // Clear right bar
                specialbarFillingValue = new Vector2(specialbarFillingValue.X, 0);
                specialbarTargetFillingValue = new Vector2(specialbarTargetFillingValue.X, 0);
                while (specialbarFillingTiles_Right.Count > 0)
                    SuperController.Instance.GameInstance.GameObjects.Remove(specialbarFillingTiles_Right.Pop());
                ParticleSystemsManager.Instance.resizeSpecialbarEmitter(Vector2.Zero, Vector2.Zero, 2);

                // Animate right bar
                specialbarBar_Right.ScalingAnimation = new ScalingAnimation(false, true, 0, true, 500);
                specialbarBar_Right.ScalingAnimation.ScalingRange = new Vector2(1, 1.3f);

                SoundManager.Instance.playIngameSoundEffect((int)IngameSound.SpecialbarFull);
            }



            // Spawn powerup
            SuperController.Instance.SpawnPositivePowerup(team);
        }

        /// <summary>
        /// Creates a new filling tile 
        /// </summary>
        /// <returns>The new tile (HUD element)</returns>
        private HUD createNewFillingTile(int team)
        {

            Texture2D texture;
            if (team == 1)
                texture = StorageManager.Instance.getTextureByName("specialbar_fillingRectTexture_left");
            else
                texture = StorageManager.Instance.getTextureByName("specialbar_fillingRectTexture_right");

            // Calculate new position
            Vector2 position = new Vector2(0, fillingTile_yPosition);
            if (team == 1)
            {
                if (specialbarFillingTiles_Left.Count == 0)
                    position = new Vector2((scorebard.Position.X - tileSize.X + specialbar_offset - 0.4f * tileSize.X), position.Y);
                else
                    position = new Vector2((specialbarFillingTiles_Left.First().Position.X - tileSize.X - gapSizeBetweenFillingTiles), position.Y);
            }
            else
            {
                if (specialbarFillingTiles_Right.Count == 0)
                    position = new Vector2((scorebard.Position.X + scorebard.Size.X - specialbar_offset + 0.4f * tileSize.X), position.Y);
                else
                    position = new Vector2((specialbarFillingTiles_Right.First().Position.X + tileSize.X + gapSizeBetweenFillingTiles), position.Y);
            }
                
            HUD tile = new HUD("FillingTile", true, texture, null, tileSize, position, 75, MeasurementUnit.PercentOfScreen);
            SuperController.Instance.GameInstance.GameObjects.Add(tile);
            return tile;
        }

        /// <summary>
        /// Returns the number of tiles for a given filling value
        /// </summary>
        /// <param name="value">the given filling value</param>
        /// <returns>The number of tiles</returns>
        private int getNumberOfTilesForFillingValue(float value)
        {

            return (int)Math.Floor(value / fillingValuePerTile);

        }


        /// <summary>
        /// Implemented, because this class is an observer
        /// Everytime an observed object has changed, this class is called
        /// </summary>
        /// <param name=“observableObject”>the object which changed</param>
        public void notify(ObservableObject observableObject)
        {

            //Check if we are Dealing with the right ObservableObject
            if (observableObject is SettingsManager)
            {

            }
        }

        #endregion
    }
}
