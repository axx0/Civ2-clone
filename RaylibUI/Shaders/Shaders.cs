using Raylib_cs;

namespace RaylibUI;

public static class Shaders
{
    public static Shader Grayscale;

    public static void Load()
    {
        Grayscale = Raylib.LoadShader(
            "Shaders//base.vs",
            "Shaders//grayscale.fs"
        );
    }

    public static void Unload()
    {
        Raylib.UnloadShader(Grayscale);
    }
}
