using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Szakdolgozat
{
    class KeyframedObjectAnimation
    {
        List<ObjectAnimationFrame> frames = new List<ObjectAnimationFrame>();
        bool loop;
        TimeSpan elapsedTime = TimeSpan.FromSeconds(0);
        public Vector3 Position { get; private set; }
        public Vector3 Rotation { get; private set; }

        public KeyframedObjectAnimation(List<ObjectAnimationFrame> Frames, bool Loop)
        {
            this.frames = Frames;
            this.loop = Loop;
            Position = Frames[0].Position;
            Rotation = Frames[0].Rotation;
        }

        public void Update(TimeSpan Elapsed)
        {
            // Update the time
            this.elapsedTime += Elapsed;
            TimeSpan totalTime = elapsedTime;
            TimeSpan end = frames[frames.Count - 1].Time;
            if (loop) // Loop around the total time if necessary
                while (totalTime > end)
                    totalTime -= end;
            else // Otherwise, clamp to the end values
            {
                Position = frames[frames.Count - 1].Position;
                Rotation = frames[frames.Count - 1].Rotation;
                return;
            }
            int i = 0;
            // Find the index of the current frame
            while (frames[i + 1].Time < totalTime)
                i++;
            // Find the time since the beginning of this frame
            totalTime -= frames[i].Time;
            // Find how far we are between the current and next frame (0 to 1)
            float amt = (float)((totalTime.TotalSeconds) /
            (frames[i + 1].Time - frames[i].Time).TotalSeconds);
            // Interpolate position and rotation values between frames
            Position = Vector3.Lerp(frames[i].Position, frames[i + 1].Position, amt);
            Rotation = Vector3.Lerp(frames[i].Rotation, frames[i + 1].Rotation, amt);
        }
    }
}
