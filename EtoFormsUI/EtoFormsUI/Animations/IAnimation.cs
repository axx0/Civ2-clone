using Eto.Drawing;

namespace EtoFormsUI.Animations
{
    public interface IAnimation
    {
        bool Finished();
        int Width { get; }
        int Height { get; }
        
        double Interval { get; }
        Image CurrentFrame { get; }
        int YAdjustment { get; }
        int[] XY { get; }
        float GetXDrawOffset(int mapXpx, int startX);
        int GetYDrawOffset(int mapYpx, int startY);
        void Initialize();
    }
}