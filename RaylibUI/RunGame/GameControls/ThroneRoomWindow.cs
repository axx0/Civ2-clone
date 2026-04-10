using Civ2engine;
using Model;
using Model.Core;
using Model.Interface;
using Raylib_CSharp.Collision;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Rendering;
using RaylibUI.BasicTypes.Controls;
using RaylibUtils;
using System.Numerics;
using Civ2engine.IO;

namespace RaylibUI.RunGame.GameControls;

public class ThroneRoomWindow : FullscreenView
{
    private readonly Civilization _civ;
    private readonly IUserInterface _active;
    private const int PicWidth = 640;
    private const int PicHeight = 480;
    private readonly int _offsetX, _offsetY;
    private int _drawWidth;
    private bool _timeToUpgrade, _upgradeIntroScreen;
    private readonly LabelControl _chooseSectionlabel, _clickMouseLabel;
    private readonly List<LabelControl> _spontaneouslyLabels = [];
    private ThroneRoom? _oldRoom = null;
    private readonly System.Timers.Timer _timer = new();

    public ThroneRoomWindow(GameScreen gameScreen, bool timeToUpgrade = false) : base(gameScreen)
    {
        _active = gameScreen.MainWindow.ActiveInterface;
        _civ = gameScreen.Game.ActivePlayer.Civilization;
        _timeToUpgrade = timeToUpgrade;
        _offsetX = (Width - PicWidth) / 2;
        _offsetY = (Height - PicHeight) / 2;

        _timer.Interval = 10;
        _drawWidth = 0;
        _timer.Elapsed += (_, _) => 
        {
            _drawWidth += 5;
            if (_drawWidth >= PicWidth)
            {
                _timer.Stop();
                _timer.Dispose();
            }
        };

        // Which section shall we improve?
        var text = _active.GetDialog("ADDTOTHRONE").Text[0];
        _chooseSectionlabel = new LabelControl(this, text, true, 
            horizontalAlignment: Model.Controls.HorizontalAlignment.Center, font: Fonts.Arial,
            colorFront: Color.White, colorShadow: Color.Black, shadowOffset: new(-2, -2),
            fontSize: 26);
        _chooseSectionlabel.Width = PicWidth;
        _chooseSectionlabel.Location = new(_offsetX, _offsetY + 125);
        _chooseSectionlabel.Visible = false;
        Controls.Add(_chooseSectionlabel);

        _clickMouseLabel = new LabelControl(this, "(" + Labels.For(LabelIndex.Clickmousetocontinue) + ")", 
            true, horizontalAlignment: Model.Controls.HorizontalAlignment.Center, font: Fonts.Arial,
            colorFront: Color.White, colorShadow: Color.Black, shadowOffset: new(1, 1));
        _clickMouseLabel.Width = PicWidth;
        _clickMouseLabel.Location = new(_offsetX, _offsetY + PicHeight - _clickMouseLabel.Height);
        _clickMouseLabel.Visible = false;
        Controls.Add(_clickMouseLabel);

        // Spontaneously texts
        var textsWidth = 430;
        var _texts = _active.GetDialog("THRONE").Text;
        var texts = string.Empty;
        foreach (var txt in _texts)
            texts = texts + " " + txt;
        var wrappedTexts = DialogUtils.GetWrappedTexts(texts, textsWidth, Fonts.Arial, 26);
        int offsetY = _offsetY + 120;
        foreach (var txt in wrappedTexts)
        {
            var spontaneouslyLabel = new LabelControl(this, txt, true,
                horizontalAlignment: Model.Controls.HorizontalAlignment.Center, font: Fonts.Arial,
                colorFront: Color.White, colorShadow: Color.Black, shadowOffset: new(-2, -2),
                fontSize: 26);
            spontaneouslyLabel.Width = textsWidth;
            spontaneouslyLabel.Location = new(_offsetX + (PicWidth - textsWidth) / 2, offsetY);
            spontaneouslyLabel.Visible = timeToUpgrade;
            _spontaneouslyLabels.Add(spontaneouslyLabel);
            Controls.Add(spontaneouslyLabel);
            offsetY += spontaneouslyLabel.Height;
        }

        _upgradeIntroScreen = timeToUpgrade;
    }

    public override void MouseOutsideControls(Vector2 mousePos)
    {
        if (_timer.Enabled) return;

        if (Input.IsMouseButtonReleased(MouseButton.Left) &&
            ShapeHelper.CheckCollisionPointRec(mousePos, new(_offsetX, _offsetY, PicWidth, PicHeight)))
        {
            if (!_timeToUpgrade)
            {
                base.Close();
            }
            else if (_timeToUpgrade && _upgradeIntroScreen)
            {
                _upgradeIntroScreen = false;
            }
            else
            {
                _oldRoom = _civ.ThroneRoom.Clone();

                var pos = mousePos - new Vector2(_offsetX, _offsetY);

                if (Images.ExtractBitmap(
                    _active.PicSources["trWallBack_section"][0]).GetColor((int)pos.X, (int)pos.Y).A == 255
                    && _civ.ThroneRoom.WallBack < 4)
                {
                    _civ.ThroneRoom.WallBack++;
                    _timeToUpgrade = false;
                    _timer.Start();
                }
                else if (Images.ExtractBitmap(
                    _active.PicSources["trFloor_section"][0]).GetColor((int)pos.X, (int)pos.Y).A == 255 &&
                    _civ.ThroneRoom.Floor < 4)
                {
                    _civ.ThroneRoom.Floor++;
                    _timeToUpgrade = false;
                    _timer.Start();
                }
                else if (Images.ExtractBitmap(
                    _active.PicSources["trRug_section"][0]).GetColor((int)pos.X, (int)pos.Y).A == 255 &&
                    _civ.ThroneRoom.Rug < 4)
                {
                    _civ.ThroneRoom.Rug++;
                    _timeToUpgrade = false;
                    _timer.Start();
                }
                else if (Images.ExtractBitmap(
                    _active.PicSources["trWallFront_section"][0]).GetColor((int)pos.X, (int)pos.Y).A == 255 &&
                    _civ.ThroneRoom.WallFront < 4)
                {
                    _civ.ThroneRoom.WallFront++;
                    _timeToUpgrade = false;
                    _timer.Start();
                }
                else if (Images.ExtractBitmap(
                    _active.PicSources["trThroneDecor_section"][0]).GetColor((int)pos.X, (int)pos.Y).A == 255 &&
                    _civ.ThroneRoom.ThroneDecor < 4)
                {
                    _civ.ThroneRoom.ThroneDecor++;
                    _timeToUpgrade = false;
                    _timer.Start();
                }
                else if (Images.ExtractBitmap(
                    _active.PicSources["trColumnsBack_section"][0]).GetColor((int)pos.X, (int)pos.Y).A == 255 &&
                    _civ.ThroneRoom.ColumnsBack < 4)
                {
                    _civ.ThroneRoom.ColumnsBack++;
                    _timeToUpgrade = false;
                    _timer.Start();
                }
                else if (Images.ExtractBitmap(
                    _active.PicSources["trThrone_section"][0]).GetColor((int)pos.X, (int)pos.Y).A == 255 &&
                    _civ.ThroneRoom.Throne < 4)
                {
                    _civ.ThroneRoom.Throne++;
                    _timeToUpgrade = false;
                    _timer.Start();
                }
                else if (Images.ExtractBitmap(
                    _active.PicSources["trColumnsFront_section"][0]).GetColor((int)pos.X, (int)pos.Y).A == 255 &&
                    _civ.ThroneRoom.ColumnsFront < 4)
                {
                    _civ.ThroneRoom.ColumnsFront++;
                    _timeToUpgrade = false;
                    _timer.Start();
                }

                // Place decoration only if everything else is upgraded
                if (_civ.ThroneRoom.WallBack == 4 && _civ.ThroneRoom.Floor == 4 &&
                    _civ.ThroneRoom.Rug == 4 && _civ.ThroneRoom.WallFront == 4 &&
                    _civ.ThroneRoom.ThroneDecor == 4 && _civ.ThroneRoom.ColumnsBack == 4 &&
                    _civ.ThroneRoom.Throne == 4 && _civ.ThroneRoom.ColumnsFront == 4)
                {
                    if (Images.ExtractBitmap(
                        _active.PicSources["trDecorRugs_section"][0]).GetColor((int)pos.X, (int)pos.Y).A == 255
                        && !_civ.ThroneRoom.DecorRugs)
                    {
                        _civ.ThroneRoom.DecorRugs = true;
                        _timeToUpgrade = false;
                        _timer.Start();
                    }
                    else if (Images.ExtractBitmap(
                        _active.PicSources["trDecorPaintings_section"][0]).GetColor((int)pos.X, (int)pos.Y).A == 255
                        && !_civ.ThroneRoom.DecorPaintings)
                    {
                        _civ.ThroneRoom.DecorPaintings = true;
                        _timeToUpgrade = false;
                        _timer.Start();
                    }
                    else if (Images.ExtractBitmap(
                        _active.PicSources["trDecorBushes_section"][0]).GetColor((int)pos.X, (int)pos.Y).A == 255
                        && !_civ.ThroneRoom.DecorBushes)
                    {
                        _civ.ThroneRoom.DecorBushes = true;
                        _timeToUpgrade = false;
                        _timer.Start();
                    }
                    else if (Images.ExtractBitmap(
                        _active.PicSources["trDecorThroneBushes_section"][0]).GetColor((int)pos.X, (int)pos.Y).A == 255
                        && !_civ.ThroneRoom.DecorThroneBushes)
                    {
                        _civ.ThroneRoom.DecorThroneBushes = true;
                        _timeToUpgrade = false;
                        _timer.Start();
                    }
                    else if (Images.ExtractBitmap(
                        _active.PicSources["trDecorPots_section"][0]).GetColor((int)pos.X, (int)pos.Y).A == 255
                        && !_civ.ThroneRoom.DecorPots)
                    {
                        _civ.ThroneRoom.DecorPots = true;
                        _timeToUpgrade = false;
                        _timer.Start();
                    }
                    else if (Images.ExtractBitmap(
                        _active.PicSources["trDecorTreasures_section"][0]).GetColor((int)pos.X, (int)pos.Y).A == 255
                        && !_civ.ThroneRoom.DecorTreasures)
                    {
                        _civ.ThroneRoom.DecorTreasures = true;
                        _timeToUpgrade = false;
                        _timer.Start();
                    }
                    else if (Images.ExtractBitmap(
                        _active.PicSources["trDecorStatues_section"][0]).GetColor((int)pos.X, (int)pos.Y).A == 255
                        && !_civ.ThroneRoom.DecorStatues)
                    {
                        _civ.ThroneRoom.DecorStatues = true;
                        _timeToUpgrade = false;
                        _timer.Start();
                    }
                }
            }
        }
    }

    public override void OnKeyPress(KeyboardKey key)
    {
        base.Close();
    }

    public override void Draw(bool pulse)
    {
        Graphics.DrawRectangle(0, 0, Width, Height, Color.Black);

        if (!_timeToUpgrade || (_timeToUpgrade && _upgradeIntroScreen))
        {
            if (_oldRoom != null)
            {
                DrawThroneRoom(_oldRoom);
                DrawThroneRoom(_civ.ThroneRoom, _drawWidth);
            }
            else
            {
                DrawThroneRoom(_civ.ThroneRoom);
            }
        }
        else if (_timeToUpgrade && !_upgradeIntroScreen)
        {
            DrawThroneRoomSections();
        }

        _chooseSectionlabel.Visible = _timeToUpgrade && !_upgradeIntroScreen;
        foreach (var label in _spontaneouslyLabels)
            label.Visible = _timeToUpgrade && _upgradeIntroScreen;
        _clickMouseLabel.Visible = !_timer.Enabled;

        base.Draw(pulse);
    }

    private void DrawThroneRoom(ThroneRoom room, int drawWidth = PicWidth)
    {
        if (room.WallBack != 0)
            Graphics.DrawTexturePro(TextureCache.GetImage(_active.PicSources["trWallBack"][room.WallBack - 1]),
                new(0, 0, drawWidth, PicHeight), new(_offsetX, _offsetY, drawWidth, PicHeight), new(0, 0), 0f, Color.White);
        if (room.Floor != 0)
            Graphics.DrawTexturePro(TextureCache.GetImage(_active.PicSources["trFloor"][room.Floor - 1]),
                new(0, 0, drawWidth, PicHeight), new(_offsetX, _offsetY, drawWidth, PicHeight), new(0, 0), 0f, Color.White);
        if (room.Rug != 0)
            Graphics.DrawTexturePro(TextureCache.GetImage(_active.PicSources["trRug"][room.Rug - 1]),
                new(0, 0, drawWidth, PicHeight), new(_offsetX, _offsetY, drawWidth, PicHeight), new(0, 0), 0f, Color.White);
        if (room.WallFront != 0)
            Graphics.DrawTexturePro(TextureCache.GetImage(_active.PicSources["trWallFront"][room.WallFront - 1]),
                new(0, 0, drawWidth, PicHeight), new(_offsetX, _offsetY, drawWidth, PicHeight), new(0, 0), 0f, Color.White);
        if (room.DecorPaintings)
            Graphics.DrawTexturePro(TextureCache.GetImage(_active.PicSources["trDecorPaintings"][0]),
                new(0, 0, drawWidth, PicHeight), new(_offsetX, _offsetY, drawWidth, PicHeight), new(0, 0), 0f, Color.White);
        if (room.DecorBushes)
            Graphics.DrawTexturePro(TextureCache.GetImage(_active.PicSources["trDecorBushes"][0]),
                new(0, 0, drawWidth, PicHeight), new(_offsetX, _offsetY, drawWidth, PicHeight), new(0, 0), 0f, Color.White);
        if (room.ThroneDecor != 0)
            Graphics.DrawTexturePro(TextureCache.GetImage(_active.PicSources["trThroneDecor"][room.ThroneDecor - 1]),
                new(0, 0, drawWidth, PicHeight), new(_offsetX, _offsetY, drawWidth, PicHeight), new(0, 0), 0f, Color.White);
        if (room.DecorThroneBushes)
            Graphics.DrawTexturePro(TextureCache.GetImage(_active.PicSources["trDecorThroneBushes"][0]),
                new(0, 0, drawWidth, PicHeight), new(_offsetX, _offsetY, drawWidth, PicHeight), new(0, 0), 0f, Color.White);
        if (room.ColumnsBack != 0)
            Graphics.DrawTexturePro(TextureCache.GetImage(_active.PicSources["trColumnsBack"][room.ColumnsBack - 1]),
                new(0, 0, drawWidth, PicHeight), new(_offsetX, _offsetY, drawWidth, PicHeight), new(0, 0), 0f, Color.White);
        if (room.DecorRugs)
            Graphics.DrawTexturePro(TextureCache.GetImage(_active.PicSources["trDecorRugs"][0]),
                new(0, 0, drawWidth, PicHeight), new(_offsetX, _offsetY, drawWidth, PicHeight), new(0, 0), 0f, Color.White);
        if (room.DecorPots)
            Graphics.DrawTexturePro(TextureCache.GetImage(_active.PicSources["trDecorPots"][0]),
                new(0, 0, drawWidth, PicHeight), new(_offsetX, _offsetY, drawWidth, PicHeight), new(0, 0), 0f, Color.White);
        if (room.DecorTreasures)
            Graphics.DrawTexturePro(TextureCache.GetImage(_active.PicSources["trDecorTreasures"][0]),
                new(0, 0, drawWidth, PicHeight), new(_offsetX, _offsetY, drawWidth, PicHeight), new(0, 0), 0f, Color.White);
        if (room.DecorStatues)
            Graphics.DrawTexturePro(TextureCache.GetImage(_active.PicSources["trDecorStatues"][0]),
                new(0, 0, drawWidth, PicHeight), new(_offsetX, _offsetY, drawWidth, PicHeight), new(0, 0), 0f, Color.White);
        if (room.Throne != 0)
            Graphics.DrawTexturePro(TextureCache.GetImage(_active.PicSources["trThrone"][room.Throne - 1]),
                new(0, 0, drawWidth, PicHeight), new(_offsetX, _offsetY, drawWidth, PicHeight), new(0, 0), 0f, Color.White);
        if (room.ColumnsFront != 0)
            Graphics.DrawTexturePro(TextureCache.GetImage(_active.PicSources["trColumnsFront"][room.ColumnsFront - 1]),
                new(0, 0, drawWidth, PicHeight), new(_offsetX, _offsetY, drawWidth, PicHeight), new(0, 0), 0f, Color.White);
    }

    private void DrawThroneRoomSections()
    {
        Graphics.DrawTexture(TextureCache.GetImage(_active.PicSources["trWallBack_section"][0]),
                    _offsetX, _offsetY, Color.White);
        Graphics.DrawTexture(TextureCache.GetImage(_active.PicSources["trFloor_section"][0]),
                _offsetX, _offsetY, Color.White);
        Graphics.DrawTexture(TextureCache.GetImage(_active.PicSources["trRug_section"][0]),
                _offsetX, _offsetY, Color.White);
        Graphics.DrawTexture(TextureCache.GetImage(_active.PicSources["trWallFront_section"][0]),
                _offsetX, _offsetY, Color.White);
        Graphics.DrawTexture(TextureCache.GetImage(_active.PicSources["trThroneDecor_section"][0]),
                _offsetX, _offsetY, Color.White);
        Graphics.DrawTexture(TextureCache.GetImage(_active.PicSources["trColumnsBack_section"][0]),
                _offsetX, _offsetY, Color.White);
        Graphics.DrawTexture(TextureCache.GetImage(_active.PicSources["trThrone_section"][0]),
                _offsetX, _offsetY, Color.White);
        Graphics.DrawTexture(TextureCache.GetImage(_active.PicSources["trColumnsFront_section"][0]),
                _offsetX, _offsetY, Color.White);
        Graphics.DrawTexture(TextureCache.GetImage(_active.PicSources["trDecorRugs_section"][0]),
                _offsetX, _offsetY, Color.White);
        Graphics.DrawTexture(TextureCache.GetImage(_active.PicSources["trDecorPaintings_section"][0]),
                _offsetX, _offsetY, Color.White);
        Graphics.DrawTexture(TextureCache.GetImage(_active.PicSources["trDecorBushes_section"][0]),
                _offsetX, _offsetY, Color.White);
        Graphics.DrawTexture(TextureCache.GetImage(_active.PicSources["trDecorThroneBushes_section"][0]),
                _offsetX, _offsetY, Color.White);
        Graphics.DrawTexture(TextureCache.GetImage(_active.PicSources["trDecorPots_section"][0]),
                _offsetX, _offsetY, Color.White);
        Graphics.DrawTexture(TextureCache.GetImage(_active.PicSources["trDecorTreasures_section"][0]),
                _offsetX, _offsetY, Color.White);
        Graphics.DrawTexture(TextureCache.GetImage(_active.PicSources["trDecorStatues_section"][0]),
                _offsetX, _offsetY, Color.White);
    }
}
