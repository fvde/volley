// HOW TO CREATE A LEVEL

// 1. Create a Class with the same name as the level in Misc/Levels
// 2. Copy Paste this class and adjust it to the new name
// 3. Adjust/Add objects as you need

// 4. Create a folder for the textures in ManhattanMorningContent/Textures/Levels
//    with the same name as the level
// 5. Add all textures to this folder
// 6. Add a texture called LevelPreview to the folder which will be shown in main menu

// 7. Register the level in the Levels.cs initialization

// !Don't forget to add the class, the content folder and the the textures to the repository!

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
    class Forest : Level
    {

        private Vector2 levelSize;

        /// <summary>
        /// Initializes the level
        /// </summary>
        public Forest()
            : base()
        {

            #region Static Levelproperties

            // Define name of the level
            levelProperties.Add("LevelName", "Forest");

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
            AllowedPowerUps.Add(PowerUpType.SunsetSunrise);
            //AllowedPowerUps.Add(PowerUpType.SwitchStones);   

            #endregion


            #region Define PassivObjects

            // Define all passive objects
            // Passive objects are all objects which have no influence in the actual game
            // -> just visuals
            // you can also add an animation and/or an animation path to these objects

            levelObjectsList.Add(new PassiveObject("Background", "Background", levelSize, Vector2.Zero, 0));

            levelObjectsList.Add(new PassiveObject("Middle", "Middle", levelSize, Vector2.Zero, 10));

            levelObjectsList.Add(new PassiveObject("Front", "Front", levelSize, Vector2.Zero, 60));
            
            List<Vector2> fireflyWaypointList = new List<Vector2>();
            fireflyWaypointList.Add(new Vector2(0, 2));
            fireflyWaypointList.Add(new Vector2(1, 1.8f));
            fireflyWaypointList.Add(new Vector2(7.5f, 2f));
            fireflyWaypointList.Add(new Vector2(8.2f, 3.6f));
            fireflyWaypointList.Add(new Vector2(8.5f, 4.2f));
            fireflyWaypointList.Add(new Vector2(8f, 4.7f));
            fireflyWaypointList.Add(new Vector2(7.5f, 4.3f));
            fireflyWaypointList.Add(new Vector2(6.5f, 3.4f));
            fireflyWaypointList.Add(new Vector2(9.1f, 3.4f));
            fireflyWaypointList.Add(new Vector2(10.2f, 4.5f));
            fireflyWaypointList.Add(new Vector2(11.1f, 5.1f));
            fireflyWaypointList.Add(new Vector2(10.1f, 6.3f));
            fireflyWaypointList.Add(new Vector2(12.8f, 5.2f));
            BezierBatch curve = new BezierBatch(fireflyWaypointList);            
            PathAnimation anim = new PathAnimation(curve, 20000);
            anim.Looping = true;
            Light l = new Light("firefly1", "light", new Vector2(0.11f), fireflyWaypointList.First(), Color.Orange, 55);
            l.PathAnimation = anim;
            levelObjectsList.Add(l);

            anim = new PathAnimation(curve, 32000);
            anim.Looping = true;
            l = new Light("firefly2", "light", new Vector2(0.1f), fireflyWaypointList.First(), Color.Yellow, 55);
            l.PathAnimation = anim;
            levelObjectsList.Add(l);

            fireflyWaypointList = new List<Vector2>();
            fireflyWaypointList.Add(new Vector2(0, 5));
            fireflyWaypointList.Add(new Vector2(0.38f, 5.6f));
            fireflyWaypointList.Add(new Vector2(3.22f, 3.2f));
            fireflyWaypointList.Add(new Vector2(3.28f, 2.37f));
            fireflyWaypointList.Add(new Vector2(3.76f, 1.64f));
            fireflyWaypointList.Add(new Vector2(4.69f, 1.25f));
            fireflyWaypointList.Add(new Vector2(5.2f, 2.18f));
            fireflyWaypointList.Add(new Vector2(5.49f, 3.11f));
            fireflyWaypointList.Add(new Vector2(3.43f, 4.94f));
            fireflyWaypointList.Add(new Vector2(4.64f, 5.41f));
            fireflyWaypointList.Add(new Vector2(5.38f, 5.85f));
            fireflyWaypointList.Add(new Vector2(7.93f, 6.14f));
            fireflyWaypointList.Add(new Vector2(8.28f, 5.52f));
            fireflyWaypointList.Add(new Vector2(8.98f, 4.74f));
            fireflyWaypointList.Add(new Vector2(8.55f, 2.95f));
            fireflyWaypointList.Add(new Vector2(9.56f, 2.71f));
            fireflyWaypointList.Add(new Vector2(10.8f, 2.47f));
            fireflyWaypointList.Add(new Vector2(11.23f, 3.5f));
            fireflyWaypointList.Add(new Vector2(12.8f, 1.7f));

            curve = new BezierBatch(fireflyWaypointList);
            anim = new PathAnimation(curve, 25000);
            anim.Looping = true;
            l = new Light("firefly3", "light", new Vector2(0.1f), fireflyWaypointList.First(), Color.Orange, 55);
            l.PathAnimation = anim;
            levelObjectsList.Add(l);

            anim = new PathAnimation(curve, 22000);
            anim.Looping = true;
            l = new Light("firefly4", "light", new Vector2(0.08f), fireflyWaypointList.First(), Color.Yellow, 55);
            l.PathAnimation = anim;
            levelObjectsList.Add(l);

            fireflyWaypointList = new List<Vector2>();
            fireflyWaypointList.Add(new Vector2(8.42f, 0f));
            fireflyWaypointList.Add(new Vector2(7.85f, 1.07f));
            fireflyWaypointList.Add(new Vector2(7.67f, 2.22f));
            fireflyWaypointList.Add(new Vector2(6.8f, 1.59f));
            fireflyWaypointList.Add(new Vector2(6.2f, 1f));
            fireflyWaypointList.Add(new Vector2(4.92f, 1.75f));
            fireflyWaypointList.Add(new Vector2(5.81f, 3f));
            fireflyWaypointList.Add(new Vector2(6.46f, 4f));
            fireflyWaypointList.Add(new Vector2(4.6f, 5.5f));
            fireflyWaypointList.Add(new Vector2(3.63f, 5.12f));
            fireflyWaypointList.Add(new Vector2(2.37f, 4.79f));
            fireflyWaypointList.Add(new Vector2(2.82f, 3.69f));
            fireflyWaypointList.Add(new Vector2(3.73f, 4.43f));
            fireflyWaypointList.Add(new Vector2(4.8f, 5.53f));
            fireflyWaypointList.Add(new Vector2(6f, 4.25f));
            fireflyWaypointList.Add(new Vector2(6.94f, 5.54f));
            fireflyWaypointList.Add(new Vector2(7.47f, 6.78f));
            fireflyWaypointList.Add(new Vector2(10.76f, 5.31f));
            fireflyWaypointList.Add(new Vector2(10.8f, 7.2f));

            curve = new BezierBatch(fireflyWaypointList);
            anim = new PathAnimation(curve, 17000);
            anim.Looping = true;
            l = new Light("firefly5", "light", new Vector2(0.1f), fireflyWaypointList.First(), Color.Yellow, 55);
            l.PathAnimation = anim;
            levelObjectsList.Add(l);

            #endregion

        }



    }
}
