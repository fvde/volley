using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Volley2.InOutput;

namespace Volley2.Objects
{
    public class Player
    {

        // Variables

        public enum Status { Gamepad, KI, InActive }

        private Status playerStatus;
        public Status PlayerStatus {
            get { return playerStatus; }
            set { this.playerStatus = value; }
        }

        public enum Team
        {
            Left = 0,
            Neutral = 1,
            Right = 2
        }

        private Team associatedTeam;
        public Team AssociatedTeam
        {
            get { return associatedTeam; }
            set { associatedTeam = value; }
        }

        private Texture2D graphics;
        public Texture2D Graphics
        {
            get { return graphics; }
            set { graphics = value; }
        }

        private Vector2 position;
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        // Stores position before last update
        private Vector2 lastPosition;
        public Vector2 LastPosition
        {
            get { return lastPosition; }
            set { lastPosition = value; }
        }

        private Vector2 size;
        public Vector2 Size
        {
            get { return size; }
            set { size = value; }
        }

        private Vector2 speed;
        public Vector2 Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        private Gamepad gamepad;
        public Gamepad Gamepad
        {
            get { return gamepad; }
            set { gamepad = value; }
        }

        private Hand hand;
        public Hand Hand
        {
            get { return hand; }
            set { hand = value; }
        }


        // Constructors

        public Player(Status status, PlayerIndex playerIndex, Team associatedTeam)
        {
            this.PlayerStatus = status;
            gamepad = new Gamepad(playerIndex);
            this.associatedTeam = associatedTeam;
            hand = new Hand();

            speed = new Vector2(0, 0);
        }


        // Methods

        public Rectangle destinationRectangle()
        {
            return new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
        }


    }
}
