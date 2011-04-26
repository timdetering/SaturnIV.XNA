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
        public ProjectileTrailParticleSystem(Game game, ContentManager content)
            : base(game, content)
        { }


        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = "textures//smoke";

            settings.MaxParticles = 400;

            settings.Duration = TimeSpan.FromSeconds(2);

            settings.DurationRandomness = 3.5f;

            settings.EmitterVelocitySensitivity = 0.50f;

            settings.MinHorizontalVelocity = 0;
            settings.MaxHorizontalVelocity = 1;

            settings.MinVerticalVelocity = 0;
            settings.MaxVerticalVelocity = 1;

            settings.MinColor = new Color(64, 96, 128, 255);
            settings.MaxColor = new Color(255, 255, 255, 128);
            //settings.MinColor = new Color(255, 255, 255, 255);
            //settings.MaxColor = new Color(255, 255, 255, 255);

            settings.MinRotateSpeed = -2;
            settings.MaxRotateSpeed = 2;

            settings.MinStartSize = 5;
            settings.MaxStartSize = 15;

            settings.MinEndSize = 5;
            settings.MaxEndSize = 15;
        }
    }
}
