using System.Collections.Generic;
using Eto.Drawing;

namespace EtoFormsUI.Animations
{
    public abstract class BaseAnimation : IAnimation
    {
        private readonly Bitmap[] _frames;

        protected BaseAnimation(Bitmap[] frames, int width, int height, double interval, int[] locationXy, int yAdjustment = 0)
        {
            _frames = frames;
            Width = width;
            Height = height;
            Interval = interval;
            YAdjustment = yAdjustment;
            XY = locationXy;
        }

        public int[] XY { get; }
        public abstract float GetXDrawOffset(int mapXpx, int i);
        public abstract int GetYDrawOffset(int mapYpx, int startY);

        public abstract void Initialize();

        private int _currentFrame;

        public bool Finished()
        {
            _currentFrame++;
            return _currentFrame == _frames.Length;
        }

        public int Width { get; }

        public int Height { get; }
        public double Interval { get; }

        public Image CurrentFrame => _frames[_currentFrame];
        public int YAdjustment { get; }

        public void Reset()
        {
            _currentFrame = 0;
        }
    }
}