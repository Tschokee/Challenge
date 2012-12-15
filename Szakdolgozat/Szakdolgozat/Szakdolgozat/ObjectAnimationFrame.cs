using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Szakdolgozat
{
    class ObjectAnimationFrame
    {
        public Vector3 Position { get; private set; }
        public Vector3 Rotation { get; private set; }
        public TimeSpan Time { get; private set; }
        public ObjectAnimationFrame(Vector3 Position, Vector3 Rotation, TimeSpan Time)
        {
            this.Position = Position;
            this.Rotation = Rotation;
            this.Time = Time;
        }
    }
}
