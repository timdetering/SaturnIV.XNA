﻿using System;
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
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace SaturnIV
{
    class HelperClass
    {
        private float _FPS = 0f, _TotalTime = 0f, _DisplayFPS = 0f;
        Random rand = new Random();
        /// <summary>
        /// Returns a number between two values.
        /// </summary>
        /// <param name="min">Lower bound value</param>
        /// <param name="max">Upper bound value</param>
        /// <returns>Random number between bounds.</returns>
        public static float RandomBetween(double min, double max)
        {
            Random random = new Random();
            return (float)(min + (float)random.NextDouble() * (max - min));
        }

        public static Vector3 RandomPosition(float minBoxPos, float maxBoxPos)
        {
            Random random = new Random();

            return new Vector3(
                     RandomBetween(minBoxPos, maxBoxPos),
                     0.0f,
                     RandomBetween(minBoxPos, maxBoxPos));
        }

        public static Vector3 RandomDirection()
        {
            Random random = new Random();

            Vector3 direction = new Vector3(
                    RandomBetween(-1.0f, 1.0f),
                    RandomBetween(-1.0f, 1.0f),
                    RandomBetween(-1.0f, 1.0f));
            direction.Normalize();

            return direction;
        }


        

        public bool CheckForCollision(GameTime gameTime, List<newShipStruct> shipList, List<weaponStruct> missileBSList, 
                                       ref List<weaponStruct> missileList, ref ExplosionClass ourExplosion)
        {
            foreach (newShipStruct ship in shipList)
            {
                foreach (weaponStruct missile in missileBSList)
                {
                    if (ship.modelBoundingSphere.Contains(missile.modelBoundingSphere) == ContainmentType.Contains 
                        && missile.distanceFromOrigin > 200)
                    {
                        Vector3 currentExpLocation = missile.modelPosition;
                        missileList.Remove(missile);
                        ourExplosion.CreateExplosionVertices((float)gameTime.TotalGameTime.TotalMilliseconds,
                                                        currentExpLocation);
                        ship.objectArmorLvl -= (ship.objectArmorFactor / 100) * missile.damageFactor;
                        return true;
                    }
                }
            }
            return false;
        }

        public bool CheckForCollision(GameTime gameTime, newShipStruct player, List<weaponStruct> missileBSList,
                               ref List<weaponStruct> missileList, ref ExplosionClass ourExplosion)
        {
                foreach (weaponStruct missile in missileBSList)
                {
                    //if (player.modelBoundingSphere.Intersects(missile.modelBoundingSphere))
                    if (player.modelBoundingSphere.Contains(missile.modelBoundingSphere) == ContainmentType.Contains
                        && missile.distanceFromOrigin > 200)
                    {
                        Vector3 currentExpLocation = missile.modelPosition;
                        missileList.Remove(missile);
                        ourExplosion.CreateExplosionVertices((float)gameTime.TotalGameTime.TotalMilliseconds,
                                                        currentExpLocation);
                        player.objectArmorLvl -= (player.objectArmorFactor / 100) * missile.damageFactor;
                        return true;
                    }
            }
            return false;
        }

        public bool CheckForCollision(GameTime gameTime, NPCManager myShip, List<NPCManager> shipList2)
        {
                foreach (NPCManager ship2 in shipList2)
                {
                    if (myShip.modelBoundingSphere.Intersects(ship2.modelBoundingSphere))
                    {
                        return true;
                    }
                }
            return false;
        }

        public void DrawFPS(GameTime gameTime, GraphicsDevice device, SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            // Calculate the Frames Per Second
            float ElapsedTime = (float)gameTime.ElapsedRealTime.TotalSeconds;
            _TotalTime += ElapsedTime;

            if (_TotalTime >= 1)
            {
                _DisplayFPS = _FPS;
                _FPS = 0;
                _TotalTime = 0;
            }
            _FPS += 1;

            // Format the string appropriately
            string FpsText = _DisplayFPS.ToString() + " FPS";
            Vector2 FPSPos = new Vector2((device.Viewport.Width - spriteFont.MeasureString(FpsText).X) - 15, 10);
            spriteBatch.Begin();
            spriteBatch.DrawString(spriteFont, FpsText, FPSPos, Color.White);
            spriteBatch.End();
        }

    }

   
        
}
