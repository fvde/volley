using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace ManhattanMorning.Model.GameObject
{
    class RibbonParticle
    {
        #region Private Structures
        private struct TrailNode
        {
            public Vector2 Position;
            public Vector2 Velocity;
        }
        #endregion

        #region Members

        private int numberOfTrailNodes;
        private TrailNode[] trailNodes;
        private Vector2 position;
        private Texture2D particleTexture;
        private float startScale;
        private float endScale;

        #endregion

        #region Properties
        #endregion


        #region initialisation
        /// <summary>
        /// Initializes a new ribbon particle
        /// </summary>
        public RibbonParticle(Vector2 position)
        {
            this.startScale = 1.0f;
            this.endScale = 0.3f;


            this.position = position;

            trailNodes = new TrailNode[10];

            for (int i = 0; i < 10; i++)
            {
                TrailNode n = new TrailNode();
                n.Position = this.position;
                trailNodes[i] = n;
            }
        }
        #endregion

        #region Methods

        public void updateNodes(GameTime time)
        {


        }

        #endregion







    }
}
