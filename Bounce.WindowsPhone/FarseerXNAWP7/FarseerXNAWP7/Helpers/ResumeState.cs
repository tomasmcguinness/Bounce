using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FarseerXNAWP7.Helpers
{
    public class ResumeState
    {
        public Vector2 CameraPosition { get; set; }
        public Vector2 BallPosition { get; set; }
        public Vector2 BackgroundPosition { get; set; }
        public int CurrentLevel { get; set; }
        public int DiamondsCollected { get; set; }
        public int DistanceTravelled { get; set; }
    }
}
