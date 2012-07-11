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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Torrero.Model
{
    public class Bulls : MoveableGameObject
    {
        // All getters, setters.
        #region Properties

        #endregion

        // All members. They should always be private and provide a getter/setter if necessary.
        #region Members

        #endregion

        // Object constructors.
        #region Initialization

        public Bulls(Vector2 position, Vector2 size, Texture2D texture)
            : base(position, size, texture)
        {
        }

        #endregion
    }
}
