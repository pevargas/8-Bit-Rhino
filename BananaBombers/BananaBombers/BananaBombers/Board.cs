#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics; // For Texture 2D
using Microsoft.Xna.Framework;          // For Vector 2
#endregion

namespace BananaBombers
{
    class Board
    {
        #region Member Variables
        public Texture2D texture { get; set; } // Board Texture
        public Vector2 position  { get; set; } // Position on screen
        public Vector2 size      { get; set; } // Size in pixels 
        #endregion

        #region Constructor
        public Board(Texture2D newTexture, Vector2 newPosition, Vector2 newSize)
        {
            texture = newTexture;
            position = newPosition;
            size = newSize;
        }
        #endregion

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }

    }
}
