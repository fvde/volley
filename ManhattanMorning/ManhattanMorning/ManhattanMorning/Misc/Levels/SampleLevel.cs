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
using ManhattanMorning.Misc.Curves;
using ManhattanMorning.Model.GameObject;


namespace ManhattanMorning.Misc.Levels
{
    class SampleLevel : Level
    {
    

        /// <summary>
        /// Initializes the level
        /// </summary>
        public SampleLevel()
            : base()
        {

            #region Static Levelproperties

            // Define name of the level
            levelProperties.Add("LevelName", "Classic");

            // Define the size of the level (in meters, has to have 16:9 ratio!)
            Vector2 levelSize = new Vector2(12.8f, 7.2f);
            levelProperties.Add("LevelSize", levelSize);

            // Define the reset positions for ball, left team, right team
            // (in meters, origin is the upper left corner)
            levelProperties.Add("BallResetPosition", new Vector2(7.5f  , 1.0f));
            levelProperties.Add("LeftTeamResetPosition", new Vector2(2.0f, 5.0f));
            levelProperties.Add("RightTeamResetPosition", new Vector2(14.0f, 5.0f));

            #endregion


            #region Define net

            // Define net size and texture
            // it's centered automatically

            levelObjectsList.Add(new Net("Net", new Vector2(0.5f, 3f)));

            #endregion


            #region Define ball

            // Define ball texture
            // This definition is used for all balls

            levelObjectsList.Add(new Ball("Ball"));

            #endregion


            #region Define powerups


            #endregion


            #region Define PassivObjects

            // Define all passive objects
            // Passive objects are all objects which have no influence in the actual game
            // -> just visuals
            // you can also add an animation and/or an animation path to these objects

            levelObjectsList.Add(new PassiveObject("Background", "NewBackground", new Vector2(16f, 9f), Vector2.Zero, 0));


            // Passive Object with an animation and a path (you can also create objects that just have
            // an animation or a path):

            // 1. Create the animation
            SpriteAnimation animation = new SpriteAnimation("Bird", 80, 8, 70, 70, 3, 3, true);

            // 2. Create the path
            Curve2D curve = new Curve2D();
            curve.addPoint(0f, new Vector2(-1, 2));
            curve.addPoint(0.2f, new Vector2(3, 1));
            curve.addPoint(0.8f, new Vector2(9, 1.5f));
            curve.addPoint(1f, new Vector2(16, 0));

            PathAnimation path = new PathAnimation(curve, 10000f);

            // 3. Create the passive objects and insert animation and path
            levelObjectsList.Add(new PassiveObject("Bird", new Vector2(1f, 0.8f), new Vector2(2, 2), 30, animation, path));

            #endregion

        }

            

    }
}
