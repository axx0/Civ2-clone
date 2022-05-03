using Civ2engine.MapObjects;
using Civ2engine.Terrains;
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
        Tile Location { get; }
        float GetXDrawOffset(int mapXpx, int startX);
        int GetYDrawOffset(int mapYpx, int startY);
        void Initialize();
    }
}