using System.Numerics;
using Raylib_CSharp.Transformations;

namespace Model.CityWindowModel;

public class ShieldProduction
{
    public string Type { get; set; }// = "Box";
    public Rectangle Box { get; set; }
    
    public Vector2 IconLocation;// = new Rectangle(510, 181, 47, 30);
    public Rectangle BuyButtonBounds { get; set; }// = new Dictionary<string, Rectangle>();
    public Rectangle ChangeButtonBounds { get; set; }// = new Dictionary<string, Rectangle>();
}