﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SaturnIV
{
    [Serializable]
    public struct saveObject
    {
        public string shipType;
        public string shipName;
        public Vector3 shipPosition;
        public Vector3 shipDirection;

    }

    [Serializable]
    public struct randomNames
    {
        
    }

}
