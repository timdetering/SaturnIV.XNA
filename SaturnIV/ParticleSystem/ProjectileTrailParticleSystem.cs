#region File Description
//-----------------------------------------------------------------------------
// ProjectileTrailParticleSystem.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace SaturnIV
{
    /// <summary>
    /// Custom particle system for leaving smoke trails behind the rocket projectiles.
    /// </summary>
    class ProjectileTrailParticleSystem : ParticleSystem
    {
        public static Color color;
        public ProjectileTrailParticleSystem(Game game, ContentManager content)
            : base(game, content)
        {
        }

        public void initColor(Color color, ParticleSettings settings)
        {
            settings.MaxColor = color;
            settings.MinColor = color;
        }
        
        protected override void InitializeSettings(ParticleSettings settings)
        {            
            settings.TextureName = "textures//smoke";

            settings.MaxParticles = 30000;

            settings.Duration = TimeSpan.FromSeconds(2.0);

            settings.DurationRandomness = 0.5f;

            settings.EmitterVelocitySensitivity = 0.2f;

            settings.MinHorizontalVelocity = 20;
            settings.MaxHorizontalVelocity = 31;

            settings.MinVerticalVelocity = -2;
            settings.MaxVerticalVelocity = 2;

            settings.MinColor = Color.White;
            settings.MaxColor = Color.Blue;

            settings.MinRotateSpeed = 40;
            settings.MaxRotateSpeed = 60;

            settings.MinStartSize = 850;
            settings.MaxStartSize = 950;

            settings.MinEndSize = 2150;
            settings.MaxEndSize = 2250;
        }
    }
}
