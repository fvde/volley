
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using ManhattanMorning.Controller;
using ManhattanMorning.Misc.Curves;
using ManhattanMorning.Model.GameObject;
using ManhattanMorning.Model;
using ManhattanMorning.View;
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

            //AllowedPowerUps.Add(PowerUpType.BallRain);
            //AllowedPowerUps.Add(PowerUpType.SuperBomb);
            //AllowedPowerUps.Add(PowerUpType.DoubleBall);
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
            PassiveObject stone0 = new PassiveObject("stone0", "stone0", new Vector2(96, 87) / 100, new Vector2(120, 148) / 100, 25);
            PassiveObject stone1 = new PassiveObject("stone1", "stone1", new Vector2(88, 81) / 100, new Vector2(357, 138) / 100, 25);
            PassiveObject stone2 = new PassiveObject("stone2", "stone2", new Vector2(69, 72) / 100, new Vector2(685, 146) / 100, 25);
            PassiveObject stone3 = new PassiveObject("stone3", "stone3", new Vector2(98, 84) / 100, new Vector2(852, 151) / 100, 25);

            Light l0 = new Light("stoneL", StorageManager.Instance.getTextureByName("stone_light"), stone0.Size * 1.1f, stone0.Position - stone0.Size / 2, Color.Moccasin, true, null);
            int time = (int)SettingsManager.Instance.get("switchStonesEffectDuration") - 2000;
            FadingAnimation fading0 = new FadingAnimation(false, true, time, false, 1000);
            l0.FadingAnimation = fading0;
            l0.Visible = false;
            ScalingAnimation s0 = new ScalingAnimation(false, false, 0, false, 1500);
            l0.ScalingAnimation = s0;
            stone0.ScalingAnimation = s0;

            Light l1 = new Light("stoneL", StorageManager.Instance.getTextureByName("stone_light"), stone1.Size * 1.1f, stone1.Position - stone1.Size / 2, Color.Moccasin, true, null);
            FadingAnimation fading1 = new FadingAnimation(false, true, time, false, 1000);
            l1.FadingAnimation = fading1;
            l1.Visible = false;
            ScalingAnimation s1 = new ScalingAnimation(false, false, 0, false, 1500);
            l1.ScalingAnimation = s1;
            stone1.ScalingAnimation = s1;

            Light l2 = new Light("stoneL", StorageManager.Instance.getTextureByName("stone_light"), stone2.Size * 1.1f, stone2.Position - stone2.Size / 2, Color.Moccasin, true, null);
            FadingAnimation fading2 = new FadingAnimation(false, true, time, false, 1000);
            l2.FadingAnimation = fading2;
            l2.Visible = false;
            ScalingAnimation s2 = new ScalingAnimation(false, false, 0, false, 1500);
            l2.ScalingAnimation = s2;
            stone2.ScalingAnimation = s2;

            Light l3 = new Light("stoneL", StorageManager.Instance.getTextureByName("stone_light"), stone3.Size * 1.1f, stone3.Position - stone3.Size / 2, Color.Moccasin, true, null);
            FadingAnimation fading3 = new FadingAnimation(false, true, time, false, 1000);
            l3.FadingAnimation = fading3;
            l3.Visible = false;
            ScalingAnimation s3 = new ScalingAnimation(false, false, 0, false, 1500);
            l3.ScalingAnimation = s3;
            stone3.ScalingAnimation = s3;

            stone0.attachObject(l0);
            stone1.attachObject(l1);
            stone2.attachObject(l2);
            stone3.attachObject(l3);

            levelObjectsList.Add(l0);
            levelObjectsList.Add(l1);
            levelObjectsList.Add(l2);
            levelObjectsList.Add(l3);

            levelObjectsList.Add(stone0);
            levelObjectsList.Add(stone1);
            levelObjectsList.Add(stone2);
            levelObjectsList.Add(stone3);
             

            #endregion

        }

            

    }
}
