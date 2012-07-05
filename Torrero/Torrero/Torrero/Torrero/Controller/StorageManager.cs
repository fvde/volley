using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


using Torrero.Model;

namespace Torrero.Controller
{
    public class StorageManager
    {
        // All getters, setters.
        #region Properties

        #endregion

        // All members. They should always be private and provide a getter/setter if necessary.
        #region Members

        /// <summary>
        /// ContentManager loads all content.
        /// </summary>
        ContentManager contentManager;

        /// <summary>
        /// Saves textures which can be accessed from other controllers 
        /// (check StorageManager's methods)
        /// </summary>
        private static Dictionary<String, Texture2D> textures;

        #endregion

        // Object constructors.
        #region Initialization

        /// <summary>
        /// Create a new StorageManager.
        /// </summary>
        public StorageManager(ContentManager cm)
        {
            // Content-Manager von der Anwendung holen
            contentManager = cm;
            textures = new Dictionary<String, Texture2D>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads textures which are needed while runtime to create objects / replace textures
        /// </summary>
        public void loadTexturesForDictionary()
        {
            // clear Dictionary
            Texture2D obstacle = contentManager.Load<Texture2D>(@"Textures\Background\obstacle");
            Texture2D street = contentManager.Load<Texture2D>(@"Textures\Background\street");
            Texture2D player = contentManager.Load<Texture2D>(@"Textures\testgrid");
            Texture2D senorita = contentManager.Load<Texture2D>(@"Textures\senorita");

            // Streets
            Texture2D street_horizontal = contentManager.Load<Texture2D>(@"Textures\Background\Streets\street_horizontal");
            Texture2D street_vertical = contentManager.Load<Texture2D>(@"Textures\Background\Streets\street_vertical");
            Texture2D deadend_left = contentManager.Load<Texture2D>(@"Textures\Background\Streets\deadend_left");
            Texture2D deadend_right = contentManager.Load<Texture2D>(@"Textures\Background\Streets\deadend_right");
            Texture2D deadend_down = contentManager.Load<Texture2D>(@"Textures\Background\Streets\deadend_down");
            Texture2D crossing_right_down_left = contentManager.Load<Texture2D>(@"Textures\Background\Streets\crossing_right_down_left");
            Texture2D crossing_up_down_left = contentManager.Load<Texture2D>(@"Textures\Background\Streets\crossing_up_down_left");
            Texture2D crossing_up_right_down = contentManager.Load<Texture2D>(@"Textures\Background\Streets\crossing_up_right_down");
            Texture2D crossing_up_right_down_left = contentManager.Load<Texture2D>(@"Textures\Background\Streets\crossing_up_right_down_left");
            Texture2D crossing_up_right_left = contentManager.Load<Texture2D>(@"Textures\Background\Streets\crossing_up_right_left");
            Texture2D turn_down_left = contentManager.Load<Texture2D>(@"Textures\Background\Streets\turn_down_left");
            Texture2D turn_right_down = contentManager.Load<Texture2D>(@"Textures\Background\Streets\turn_right_down");
            Texture2D turn_up_left = contentManager.Load<Texture2D>(@"Textures\Background\Streets\turn_up_left");
            Texture2D turn_up_right = contentManager.Load<Texture2D>(@"Textures\Background\Streets\turn_up_right");

            saveTextureByName(obstacle, "obstacle");
            saveTextureByName(street, "street");
            saveTextureByName(street_horizontal, "street_horizontal");
            saveTextureByName(street_vertical, "street_vertical");
            saveTextureByName(deadend_left, "deadend_left");
            saveTextureByName(deadend_right, "deadend_right");
            saveTextureByName(deadend_down, "deadend_down");
            saveTextureByName(crossing_right_down_left, "crossing_right_down_left");
            saveTextureByName(crossing_up_down_left, "crossing_up_down_left");
            saveTextureByName(crossing_up_right_down, "crossing_up_right_down");
            saveTextureByName(crossing_up_right_down_left, "crossing_up_right_down_left");
            saveTextureByName(crossing_up_right_left, "crossing_up_right_left");
            saveTextureByName(turn_down_left, "turn_down_left");
            saveTextureByName(turn_right_down, "turn_right_down");
            saveTextureByName(turn_up_left, "turn_up_left");
            saveTextureByName(turn_up_right, "turn_up_right");
            saveTextureByName(player, "player");
            saveTextureByName(senorita, "senorita");
        }

        /// <summary>
        /// Returns the texture for a given name
        /// </summary>
        /// <param name="name">name of the texture</param>
        /// <returns>the texture</returns>
        public static Texture2D getTextureByName(String name)
        {
            return textures[name];
        }

        /// <summary>
        /// Checks if texture is already in dictionary
        /// if not, adds it
        /// </summary>
        /// <param name="texture">texture that should be saved</param>
        /// <param name="name">name of the texture</param>
        public void saveTextureByName(Texture2D texture, String name)
        {
            if (name.Length == 0)
                return;

            if (!textures.ContainsKey(name))
                textures.Add(name, texture);

            return;
        }

        #endregion
    }
}
