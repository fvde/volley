using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace ManhattanMorning.Misc.Curves
{
    public abstract class Manipulator
    {
        public abstract bool IsActive { get; }
        abstract public void handleInteraction(MouseState mouseState);
    }
}
