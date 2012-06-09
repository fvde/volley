using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using ManhattanMorning.Controller;
using ManhattanMorning.Misc.Curves;
using ManhattanMorning.Model.GameObject;
using ManhattanMorning.Model;

namespace ManhattanMorning.Misc.Levels
{
    class Beach : Level
    {
        private Vector2 levelSize;

        /// <summary>
        /// Initializes the level
        /// </summary>
        public Beach()
            : base()
        {

            #region Static Levelproperties

            // Define name of the level
            levelProperties.Add("LevelName", "Beach");

            // Define the size of the level (in meters, has to have 16:9 ratio!)
            levelSize = new Vector2(12.8f, 7.2f);
            levelProperties.Add("LevelSize", levelSize);

            // Define the reset positions for ball, left team, right team
            // (in meters, origin is the upper left corner)
            levelProperties.Add("BallResetPosition", new Vector2(levelSize.X / 2 - ((Vector2)SettingsManager.Instance.get("ballSize")).X*0.5f, 1.0f));
            levelProperties.Add("LeftTeamResetPosition", new Vector2((levelSize.X / 4f) - ((float) SettingsManager.Instance.get("playerSize")) * 0.5f , 5.0f));
            levelProperties.Add("RightTeamResetPosition", new Vector2((levelSize.X / 4f) * 3 - ((float)SettingsManager.Instance.get("playerSize")) * 0.5f, 5.0f));

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
            
            //AllowedPowerUps.Add(PowerUpType.BallRain);
            //AllowedPowerUps.Add(PowerUpType.SuperBomb);
            //AllowedPowerUps.Add(PowerUpType.DoubleBall);
            AllowedPowerUps.Add(PowerUpType.InvertedControl);
            AllowedPowerUps.Add(PowerUpType.Jumpheight);
            AllowedPowerUps.Add(PowerUpType.Wind);

            AllowedPowerUps.Add(PowerUpType.Volcano);
            //AllowedPowerUps.Add(PowerUpType.SunsetSunrise);
            //AllowedPowerUps.Add(PowerUpType.SwitchStones);            

            #endregion


            #region Define PassivObjects

            // Define all passive objects
            // Passive objects are all objects which have no influence in the actual game
            // -> just visuals
            // you can also add an animation and/or an animation path to these objects
            
            //size, position 12,8 * 7,2
            levelObjectsList.Add(new PassiveObject("Background", "Background", levelSize, Vector2.Zero, 0));

            PassiveObject p = new PassiveObject("Volcano", "Volcano", levelSize, Vector2.Zero, 8);
            levelObjectsList.Add(p);

            
            PassiveObject sea = new PassiveObject("Sea", null, levelSize, Vector2.Zero, 9);
            sea.Animation = new SpriteAnimation("sea", 80, 12, 1280, 720, 3, 4, true);
            sea.Animation.Active = true;
            levelObjectsList.Add(sea);
            
            levelObjectsList.Add(new PassiveObject("Front", "front", levelSize, Vector2.Zero, 60));

            PassiveObject l = new PassiveObject("vulcL", "light", new Vector2(3.2f), new Vector2(9.7f, 4) - new Vector2(1.6f), 6);
            l.BlendColor = Color.OrangeRed;
            FadingAnimation f = new FadingAnimation(true, true, 0, true, 100);
            f.RandomFadeDuration = new Vector4(500, 800, 200, 450);
            l.FadingAnimation = f;
            levelObjectsList.Add(l);


            #endregion

        }



    }
}
