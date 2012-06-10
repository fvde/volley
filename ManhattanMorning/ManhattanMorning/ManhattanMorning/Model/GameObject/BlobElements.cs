using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using ManhattanMorning.Controller;
using ManhattanMorning.Misc;

namespace ManhattanMorning.Model.GameObject
{
    /// <summary>
    /// Represents the player who is controlled by human or KI
    /// </summary>
    public class BlobElements
    {

        #region Properties

        /// <summary>
        /// Every player possesses a gamepad, irregardless if it's connected at all or a KI player
        /// </summary>
        public List<Body> BlobElementBodies { get { return blobElementBodies; } set { blobElementBodies = value; } }

        /// <summary>
        /// Size of all the blob Elements.
        /// </summary>
        public Vector2 Size { get { return this.size; } set { this.size = value; } }

        /// <summary>
        /// Textures of all the blob elements.
        /// </summary>
        public Texture2D Texture { get { return this.texture; } set { texture = value; } }

        #endregion

        #region Members

        /// <summary>
        /// Bodies of the blob
        /// </summary>
        private List<Body> blobElementBodies;

        /// <summary>
        /// Size of all Blob elements
        /// </summary>
        private Vector2 size;

        /// <summary>
        /// Texture of all Body elements
        /// </summary>
        private Texture2D texture;


        #endregion


        #region Initialization

        /// <summary>
        /// Initializes a new bodyElements
        /// </summary>
        /// <param name="blobElementBodies">A list of all the bodies linked to the main blob.</param>
        /// <param name="size">Size of all the bodies</param>
        /// <param name="texture">Texture of all the bodies</param>
        public BlobElements(List<Body> blobElementBodies, Vector2 size, Texture2D texture)
        {
            this.blobElementBodies = blobElementBodies;
            this.size = size;
            this.texture = texture;
        }


        #endregion


    }

}

