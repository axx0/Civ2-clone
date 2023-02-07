
using System.Numerics;
using ImGuiNET;
using Raylib_cs;

namespace RaylibUI;

public class ImagePanel
{
    private Image _bmp;
    private readonly Vector2 _size;
    private readonly Texture2D _texture;

    public ImagePanel(string name, Image bmp, Vector2 location)
    {
        _bmp = bmp;
        Key = name;
        Location = location;

        _size = new Vector2(_bmp.width, _bmp.height);
        _texture = Raylib.LoadTextureFromImage(bmp);
    }

    public string Key { get; }
    public Vector2 Location { get; set; }

    public void Draw()
    {
        // unsafe
        // {
            Raylib.DrawTexture(_texture, (int)Location.X, (int)Location.Y,Color.WHITE);
        //     ImGui.Begin(Key);
        //     ImGui.GetWindowDrawList().AddImage(&_texture);
        //     //ImGui.Image((IntPtr)_bmp.data,_size);
        // }
    }
}