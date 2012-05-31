using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ManhattanMorning.Model.ParticleSystem
{
    public class ParticleSystem : LayerInterface
    {

        #region Members

        /// <summary>
        /// The Emitters Linked to this system
        /// </summary>
        private List<Emitter> emitterList;

        /// <summary>
        /// The Orbitters Linked to this system
        /// </summary>
        private List<Orbitter> orbitterList;

        //List of General Forces in the System like Gravity and Wind
        private List<Vector2> forces;

        /// <summary>
        /// Layer Position of this Object between 0 and 100. 100 being on top.
        /// </summary>
        private int layer;

        #endregion

        #region Properties

        /// <summary>
        /// List of all Emitters within this Particle System
        /// </summary>
        public List<Emitter> EmitterList
        {
            get {return this.emitterList; }
        }

        /// <summary>
        /// The Orbitters Linked to this system
        /// </summary>
        public List<Orbitter> OrbitterList
        {
            get { return orbitterList; }
        }

        /// <summary>
        /// List of all the forces in the Particle System, that affect all the Particles. (Wind, Gravity etc)
        /// </summary>
        public List<Vector2> Forces { get { return this.forces; } set { this.forces = value; } }

        public BlendState SystemBlendState { get; set; }

        public int Layer { get { return layer; } set { layer = value; } }

        public string Name
        {
            get { return ""; }
        }

        #endregion


        #region Initialization


        /// <summary>
        /// Creates a new Particle System with no Emitters
        /// </summary>
        public ParticleSystem(int layer)
        {
            SystemBlendState = BlendState.AlphaBlend;
            emitterList = new List<Emitter>();
            orbitterList = new List<Orbitter>();
            forces = new List<Vector2>();

            this.layer = layer;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Update the whole Particle System. With all Particles.
        /// </summary>
        /// <param name="time">The current GameTime</param>
        public void update(GameTime time)
        {
            //Update every emitter
            foreach (Emitter e in this.emitterList)
            {
                e.update(time, forces);
            }
            foreach (Orbitter o in this.orbitterList)
            {
                if(o.Visible)
                    o.update(time);
            }
        }

        /// <summary>
        /// Add a new emitter to the Particle System
        /// </summary>
        /// <param name="e">The new Emitter</param>
        public void addEmitter(Emitter e)
        {
            this.emitterList.Add(e);
            e.ParentParticleSystem = this;
        }

        /// <summary>
        /// Remove an emitter from the Particle System
        /// </summary>
        /// <param name="e">The Emitter to be removed</param>
        public void removeEmitter(Emitter e)
        {
            this.emitterList.Remove(e);
            e.ParentParticleSystem = null;
        }

        /// <summary>
        /// Add a new emitter to the Particle System
        /// </summary>
        /// <param name="o">The new Emitter</param>
        public void addOrbitter(Orbitter o)
        {
            this.orbitterList.Add(o);
        }

        /// <summary>
        /// Remove an emitter from the Particle System
        /// </summary>
        /// <param name="o">The Emitter to be removed</param>
        public void removeOrbitter(Orbitter o)
        {
            this.orbitterList.Remove(o);
        }



        #endregion

    }
}
