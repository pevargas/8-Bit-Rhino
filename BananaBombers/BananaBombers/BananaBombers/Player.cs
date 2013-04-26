using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics; // for Texture2D
using Microsoft.Xna.Framework; // for Vector2

namespace BananaBombers
{
    class Player
    {
        public Texture2D texture { get; set; } // sprite texture, read-only property
        public Vector2 position { get; set; } // sprite position on screen
        public Vector2 size { get; set; } // sprite size in pixels
        private Vector2 screenSize { get; set; } // screen size
        public Vector2 velocity { get; set; } // sprite velocity

        public Player(Texture2D newTexture, Vector2 newPosition, Vector2 newSize, int ScreenWidth, int ScreenHeight)
        {
            texture = newTexture;
            position = newPosition;
            size = newSize;
            screenSize = new Vector2(ScreenWidth, ScreenHeight);
        }

        public bool Move()
        {
            bool stop = false;

            // checking right boundary
            if (position.X + size.X + velocity.X > screenSize.X)
            {
                velocity = new Vector2(-velocity.X, velocity.Y);
                stop = true;
            }
            // checking bottom boundary
            if (position.Y + size.Y + velocity.Y > screenSize.Y)
            {
                velocity = new Vector2(velocity.X, -velocity.Y);
                stop = true;
            }
            // checking left boundary
            if (position.X + velocity.X < 0)
            {
                velocity = new Vector2(-velocity.X, velocity.Y);
                stop = true;
            }

            // checking top boundary
            if (position.Y + velocity.Y < 0)
            {
                velocity = new Vector2(velocity.X, -velocity.Y);
                stop = true;
            }

            // since we adjusted the velocity, just add it to the current position

            position += velocity;

            //tell the game whether monkey hit an edge and should stop
            return (stop);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }
    }
}
