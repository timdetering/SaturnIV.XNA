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

namespace WindowsGame3
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

            settings.MaxParticles = 1000;

            settings.Duration = TimeSpan.FromSeconds(2);

            settings.DurationRandomness = 3.5f;

            settings.EmitterVelocitySensitivity = 0.1f;

            settings.MinHorizontalVelocity = 0;
            settings.MaxHorizontalVelocity = 1;

            settings.MinVerticalVelocity = 0;
            settings.MaxVerticalVelocity = 1;

            //settings.MinColor = new Color(64, 96, 128, 255);
            //settings.MaxColor = new Color(255, 255, 255, 128);
            settings.MinColor = new Color(255, 255, 255, 255);
            settings.MaxColor = new Color(255, 255, 255, 255);

            settings.MinRotateSpeed = -6;
            settings.MaxRotateSpeed = 4;

            settings.MinStartSize = 15;
            settings.MaxStartSize = 25;

            settings.MinEndSize = 10;
            settings.MaxEndSize = 20;
        }
    }
}
