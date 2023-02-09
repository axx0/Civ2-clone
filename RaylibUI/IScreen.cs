using Raylib_cs;

namespace RaylibUI;

public interface IScreen
{
    void Draw();
    ScreenBackground GetBackground();
}

public record ScreenBackground(Color background, Texture2D CentreImage);
