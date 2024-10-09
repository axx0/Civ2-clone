using Raylib_CSharp.Colors;
using System.Numerics;

namespace Model.ImageSets;

public class UnitShield
{
    /// <summary>
    /// Drawing offset of shield in unit rectangle
    /// </summary>
    public Vector2 Offset { get; set; }

    /// <summary>
    /// Distance between stacks and shield
    /// </summary>
    public Vector2 StackingOffset { get; set; }

    /// <summary>
    /// Distance between shadow & shield/stack
    /// </summary>
    public Vector2 ShadowOffset { get; set; }

    public bool DrawShadow { get; set; }

    /// <summary>
    /// Drawing offset of HPbar in unit rectangle
    /// </summary>
    public Vector2 HPbarOffset { get; set; }
    
    public Vector2 HPbarSize { get; set; }
    
    public Color[] HPbarColours { get; set; } = new Color[3];

    /// <summary>
    /// Size of health bar when colours are switched
    /// </summary>
    public int[] HPbarSizeForColours { get; set; } = new int[2];

    /// <summary>
    /// Drawing offset of order key in unit rectangle
    /// </summary>
    public Vector2 OrderOffset { get; set; }

    public int OrderTextHeight { get; set; }

    public bool ShieldInFrontOfUnit { get; set; }
}
