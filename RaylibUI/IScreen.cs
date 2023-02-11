using Raylib_cs;

namespace RaylibUI;

public interface IScreen
{
    ScreenBackground GetBackground();
    void Draw(int screenWidth, int screenHeight);
}

public record ScreenBackground(Color background, Texture2D CentreImage);
