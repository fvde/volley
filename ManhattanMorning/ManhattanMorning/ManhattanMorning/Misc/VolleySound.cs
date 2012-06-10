using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using ManhattanMorning.Controller;

namespace ManhattanMorning.Misc
{
    public class VolleySound
    {

        #region Properties

        public SoundEffectInstance SoundEffect
        {
            get { return soundEffect; }
            set { soundEffect = value; }
        }

        /// <summary>
        /// Returns the maximum volume of this sound.
        /// </summary>
        public float MaximumVolume
        {
            get { return maximumVolume; }
            set { maximumVolume = value; }
        }

        /// <summary>
        /// Retunrs the type of this sound.
        /// </summary>
        public IngameSound Type
        {
            get { return type; }
        }

        #endregion

        #region Members

        private IngameSound type;

        private float maximumVolume;

        private SoundEffectInstance soundEffect;

        #endregion


        #region Initialization

        /// <summary>
        /// Volley ingame sound constructor
        /// </summary>
        /// <param name="effect"></param>
        /// <param name="t"></param>
        /// <param name="max"></param>
        public VolleySound(SoundEffectInstance effect, IngameSound t, float max)
        {
            soundEffect = effect;
            type = t;
            maximumVolume = max;
        }

        #endregion
    }
}
