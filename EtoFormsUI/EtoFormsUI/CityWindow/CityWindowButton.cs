using Eto.Drawing;
using EtoFormsUIExtensionMethods;

namespace EtoFormsUI
{
    public class CityWindowButton : Civ2button
    {
        public CityWindowButton(string text, int zoom) : base("Buy", 20, 20, new Font("Arial", 10))
        {
            Text = text;
            BackgroundImage = CityImages.Exit.Resize(zoom);
        }
    }
}
