
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using ManhattanMorning.Controller;
using ManhattanMorning.Misc.Curves;
using ManhattanMorning.Model.GameObject;
using Microsoft.Xna.Framework.Graphics;


namespace ManhattanMorning.Misc.Levels
{
    class Maya : Level
    {

        private Vector2 levelSize;

        /// <summary>
        /// Initializes the level
        /// </summary>
        public Maya()
            : base()
        {

            #region Static Levelproperties

            // Define name of the level
            levelProperties.Add("LevelName", "Maya");

            // Define the size of the level (in meters, has to have 16:9 ratio!)
            levelSize = new Vector2(12.8f, 7.2f);
            levelProperties.Add("LevelSize", levelSize);

            // Define the reset positions for ball, left team, right team
            // (in meters, origin is the upper left corner)
            levelProperties.Add("BallResetPosition", new Vector2(levelSize.X / 2 - ((Vector2)SettingsManager.Instance.get("ballSize")).X * 0.5f, 1.0f));
            levelProperties.Add("LeftTeamResetPosition", new Vector2((levelSize.X / 4f - ((float)SettingsManager.Instance.get("playerSize")) * 0.5f), 5.0f));
            levelProperties.Add("RightTeamResetPosition", new Vector2((levelSize.X / 4f * 3 - ((float)SettingsManager.Instance.get("playerSize")) * 0.5f), 5.0f));

            #endregion
        }

        override public void load()
        {

            #region Define net

            // Define net size and texture
            // it's centered automatically

            // Size of net (pixel) is always 28x280 px
            Vector2 screenResolution = (Vector2)SettingsManager.Instance.get("screenResolution");
            levelObjectsList.Add(new Net("net", new Vector2(((28 / screenResolution.X) * levelSize.X), ((280 / screenResolution.Y) * levelSize.Y))));

            #endregion


            #region Define ball

            // Define ball texture
            // This definition is used for all balls

            levelObjectsList.Add(new Ball("Ball"));

            #endregion


            #region Define powerups

            // Set allowed powerups

            AllowedPowerUps.Add(PowerUpType.BallRain);
            AllowedPowerUps.Add(PowerUpType.SuperBomb);
            AllowedPowerUps.Add(PowerUpType.DoubleBall);
            AllowedPowerUps.Add(PowerUpType.InvertedControl);
            AllowedPowerUps.Add(PowerUpType.Jumpheight);
            AllowedPowerUps.Add(PowerUpType.Wind);

            //AllowedPowerUps.Add(PowerUpType.Volcano);
            //AllowedPowerUps.Add(PowerUpType.SunsetSunrise);
            AllowedPowerUps.Add(PowerUpType.SwitchStones);   

            #endregion


            #region Define PassivObjects

            // Define all passive objects
            // Passive objects are all objects which have no influence in the actual game
            // -> just visuals
            // you can also add an animation and/or an animation path to these objects

            levelObjectsList.Add(new PassiveObject("Background", "background", levelSize, Vector2.Zero, 0));

            levelObjectsList.Add(new PassiveObject("Front", "front", levelSize, Vector2.Zero, 60));

            // add stone objects. they don't have any collision, this functionality is added by active stoneBlockers
            levelObjectsList.Add(new PassiveObject("stone0", new Vector2(0.95f, 0.95f), new Vector2(1.2f, 1.45f), 0.06f, 25, new SpriteAnimation("stone_animation", 60, 10, 150, 75, 5, 2, false, true, false)));
            levelObjectsList.Add(new PassiveObject("stone1", new Vector2(1.35f, 0.9f), new Vector2(3.05f, 1.425f), -0.06f, 25, new SpriteAnimation("stone_animation", 60, 10, 150, 75, 5, 2, false, true, false)));
            levelObjectsList.Add(new PassiveObject("stone2", new Vector2(1.35f, 0.9f), new Vector2(6.25f, 1.425f), -0.06f, 25, new SpriteAnimation("stone_animation", 60, 10, 150, 75, 5, 2, false, true, false)));
            levelObjectsList.Add(new PassiveObject("stone3", new Vector2(0.95f, 0.95f), new Vector2(8.55f, 1.55f), -0.06f, 25, new SpriteAnimation("stone_animation", 60, 10, 150, 75, 5, 2, false, true, false)));



            #endregion

        }

            

    }
}
