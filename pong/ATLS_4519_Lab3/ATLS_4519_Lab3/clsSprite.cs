/* File:    clsSprite.cs                    Feb 2013
 * Author:  JKB                             University of Colorado Boulder
 * Student: Patrick Vargas                  patrick.vargas@colorado.edu
 * Description: 
 *      Class to provide sprite.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics; //   for Texture2D
using Microsoft.Xna.Framework;          //  for Vector2

namespace ATLS_4519_Lab3
{
    class clsSprite
    {
        public Texture2D texture { get; set; }   // sprite texture, read-only property
        public Vector2 position { get; set; }    //  sprite position on screen
        public Vector2 size { get; set; }        //  sprite size in pixels
        public Vector2 velocity { get; set; }    //  sprite velocity
        private Vector2 screenSize { get; set; } //  screen size
        public Vector2 center { get { return position + (size / 2); } } //  sprite center
        public float radius { get { return size.X / 2; } }              //  sprite radius
        public int[] score;

        public clsSprite(Texture2D newTexture, Vector2 newPosition, Vector2 newSize, int ScreenWidth, int ScreenHeight)
        {
            texture = newTexture;
            position = newPosition;
            size = newSize;
            screenSize = new Vector2(ScreenWidth, ScreenHeight);
            score = new int[] { 0, 0 }; // Player = 0, Opponent = 1
        }
        
        public bool CircleCollides(clsSprite otherSprite)
        {
            //  Check if two circle sprites collided
            return (Vector2.Distance(this.center, otherSprite.center) < this.radius +
            otherSprite.radius);
        }

        public int Collides(clsSprite otherSprite)
        {
            int hit = 0;
            // check if two sprites intersect
            if (this.position.X + this.size.X > otherSprite.position.X &&
                this.position.X < otherSprite.position.X + otherSprite.size.X &&
                this.position.Y + this.size.Y > otherSprite.position.Y &&
                this.position.Y < otherSprite.position.Y + otherSprite.size.Y) {
                if (this.position.Y < otherSprite.position.Y + 6 ||
                    this.position.Y > otherSprite.position.Y + otherSprite.size.Y - 6)
                    hit = 2;
                else
                    hit = 1;
            }
            return hit;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }

        public Vector2 Midpoint(clsSprite other) {
            return new Vector2((this.position.X + other.position.X) / 2,
                (this.position.Y + other.position.Y) / 2);
        }

        public int Move()
        {
            int hitWall = 0;
            //  if we move out of the screen, invert velocity
            //  checking right boundary (player)
            if (position.X + size.X + velocity.X > screenSize.X) {
                velocity = new Vector2(-velocity.X, velocity.Y);
                hitWall = 2;
                ++score[1];
            }
            //  checking bottom boundary
            if (position.Y + size.Y + velocity.Y > screenSize.Y) {
                velocity = new Vector2(velocity.X, -velocity.Y);
                hitWall = 1;
            }
            //  checking left boundary (opponent)
            if (position.X + velocity.X < 0) {
                velocity = new Vector2(-velocity.X, velocity.Y);
                hitWall = 3;
                ++score[0];
            }
            //  checking top boundary
            if (position.Y + velocity.Y < 0) {
                velocity = new Vector2(velocity.X, -velocity.Y);
                hitWall = 1;
            }

            //  since we adjusted the velocity, just add it to the current position
            position += velocity;
            return hitWall;
        }
    }
}