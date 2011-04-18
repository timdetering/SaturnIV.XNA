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
        KeyboardState oldKeyboardState;
        KeyboardState currentKeyboardState;
        string textString;
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
                    0.0f,
                    RandomBetween(-1.0f, 1.0f));
            direction.Normalize();

            return direction;
        }


        //CheckForCollision give to object lists
        public bool CheckForCollision(GameTime gameTime, ref List<newShipStruct> thisShipList, ref List<weaponStruct> missileList, ref ExplosionClass ourExplosion)
        {
            for (int j = 0; j < thisShipList.Count; j++)
            {
                for (int i = 0; i < missileList.Count; i++)
                {
                    if (thisShipList[j].modelBoundingSphere.Contains(missileList[i].modelBoundingSphere) == ContainmentType.Contains
                        && missileList[i].distanceFromOrigin > 300)
                    {
                        Vector3 currentExpLocation = missileList[i].modelPosition;
                        missileList.Remove(missileList[i]);
                        ourExplosion.CreateExplosionVertices((float)gameTime.TotalGameTime.TotalMilliseconds,
                                                        currentExpLocation);
                        //thisShipList[j].objectArmorLvl -= (thisShipList[j].objectArmorFactor / 100) * missileList[i].damageFactor;
                        return true;
                    }
                }
            }
            return false;
        }

        //CheckForCollision given a single object and list of objects
        public bool CheckForCollision(GameTime gameTime, newShipStruct thisShip, ref List<weaponStruct> missileList, ref ExplosionClass ourExplosion)
        {
                for (int i = 0; i < missileList.Count; i++)
                {
                    if (thisShip.modelBoundingSphere.Contains(missileList[i].modelBoundingSphere) == ContainmentType.Contains
                        && missileList[i].distanceFromOrigin > 200)
                    {
                        Vector3 currentExpLocation = missileList[i].modelPosition;
                        missileList.Remove(missileList[i]);
                        ourExplosion.CreateExplosionVertices((float)gameTime.TotalGameTime.TotalMilliseconds,
                                                        currentExpLocation);
                        //thisShip.objectArmorLvl -= (thisShip.objectArmorFactor / 100) * missileList[i].damageFactor;
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

        public string UpdateInput()
        {
            oldKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
            textString = "";
            Keys[] pressedKeys;
            
            pressedKeys = currentKeyboardState.GetPressedKeys();
            //if (pressedKeys.Count() > 0)
            //    textString = pressedKeys[0].ToString();
            foreach (Keys key in pressedKeys)
            {
                if (oldKeyboardState.IsKeyUp(key))
                {
                    if (key == Keys.Back) // overflows
                        textString = textString.Remove(textString.Length - 1, 1);
                    else
                        if (key == Keys.Space)
                            textString = textString.Insert(textString.Length, " ");
                        else
                            if (key == Keys.Enter)
                                textString = textString.Insert(textString.Length, "\n");
                            else
                                textString += key.ToString();

                }
            }
                return textString;
        }

        public static BoundingBox updateBB(Vector3 min,Vector3 max,Vector3 position)
        {
            Vector3 min1 = Vector3.Transform(min, Matrix.CreateTranslation(position));
            Vector3 max1 = Vector3.Transform(max, Matrix.CreateTranslation(position));
            return new BoundingBox(min1, max1);
        }

        public static BoundingBox ComputeBoundingBox(Model _model,Vector3 position)
        {
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            Matrix[] bones = new Matrix[_model.Bones.Count];
            _model.CopyAbsoluteBoneTransformsTo(bones);

            List<Vector3> vertices = new List<Vector3>();

            foreach (ModelMesh mesh in _model.Meshes)
            {
                //get the transform of the current mesh
                Matrix transform = bones[mesh.ParentBone.Index];// *worldMatrix;

                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    //get the current mesh info
                    int stride = part.VertexStride;
                    int numVertices = part.NumVertices;
                    byte[] verticesData = new byte[stride * numVertices];

                    mesh.VertexBuffer.GetData(verticesData);

                    for (int i = 0; i < verticesData.Length; i += stride)
                    {
                        float x = BitConverter.ToSingle(verticesData, i);
                        float y = BitConverter.ToSingle(verticesData, i + sizeof(float));
                        float z = BitConverter.ToSingle(verticesData, i + 2 * sizeof(float));

                        Vector3 vector = new Vector3(x, y, z);
                        //apply transform to the current point
                        vector = Vector3.Transform(vector, transform);

                        vertices.Add(vector);

                        if (vector.X < min.X) min.X = vector.X;
                        if (vector.Y < min.Y) min.Y = vector.Y;
                        if (vector.Z < min.Z) min.Z = vector.Z;
                        if (vector.X > max.X) max.X = vector.X;
                        if (vector.Y > max.Y) max.Y = vector.Y;
                        if (vector.Z > max.Z) max.Z = vector.Z;
                    }
                }
            }

            //_VerticesCount = vertices.Count;
            //_Vertices = vertices.ToArray();
            min = Vector3.Transform(min, Matrix.CreateTranslation(position));
           max = Vector3.Transform(max, Matrix.CreateTranslation(position));
            return new BoundingBox(min, max);
        }
    }      
}
