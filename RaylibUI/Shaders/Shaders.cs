using Raylib_CSharp.Shaders;

namespace RaylibUI;

public static class Shaders
{
    public static Shader Grayscale;

    public static void Load()
    {
        Grayscale = Shader.Load(
            "Shaders//base.vs",
            "Shaders//grayscale.fs"
        );
    }

    public static void Unload()
    {
        Grayscale.Unload();
    }
}
