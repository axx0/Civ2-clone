using Civ2engine;
using Civ2engine.Enums;
using Model;
using Model.Controls;
using Model.Images;
using Raylib_CSharp.Collision;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Transformations;
using RaylibUtils;
using System.Numerics;

namespace RaylibUI.RunGame.GameControls.CityControls;

public class CityView : FullscreenView
{
    private readonly IUserInterface _active;
    private readonly int _backId, _picWidth, _picHeight, _offsetX, _offsetY;
    private readonly string _backBase;
    private readonly City _city;
    private readonly List<(IImageSource Source, Vector2 Pos)> _drawTiles = [];

    public CityView(GameScreen gameScreen, City city) : base(gameScreen)
    {
        _city = city;
        _active = gameScreen.MainWindow.ActiveInterface;
        var tiles = _active.GetCityViewTiles();
        var altTiles = _active.GetCityViewAltTiles();

        for (var id = 0; id < 56; id++)
        {
            if (_city.IsNextToOcean() && id > 49) continue;
            if (_city.IsNextToRiver() && id > 49 && id < 54) continue;

            if (_city.ImprovementExists(tiles[id].RulesId))
            {
                _drawTiles.Add(new(tiles[id].Source, tiles[id].Position));
            }
            else
            {
                _drawTiles.Add(new(altTiles[tiles[id].AlternativeTileId], tiles[id].Position));
            }
        }

        // City walls & Offshore platrofrm
        foreach (var id in new int[] { 8, 31 })
        {
            if (_city.ImprovementExists(id))
            {
                var tileId = tiles.FirstOrDefault(t => t.RulesId == id).Id;
                _drawTiles.Add(new(tiles[tileId].Source, tiles[tileId].Position));
            }
        }

        // Harbor/port fac.
        if (_city.ImprovementExists(34))    // port fac.
        {
            var tileId = tiles.FirstOrDefault(t => t.RulesId == 34).Id;
            _drawTiles.Add(new(tiles[tileId].Source, tiles[tileId].Position));
        }
        else if (_city.ImprovementExists(30))    // harbor
        {
            var tileId = tiles.FirstOrDefault(t => t.RulesId == 30).Id;
            _drawTiles.Add(new(tiles[tileId].Source, tiles[tileId].Position));
        }

        // Colossus
        if (_city.ImprovementExists(41))
        {
            if (_city.IsNextToRiver())
            {
                _drawTiles.Add(new(tiles[61].Source, tiles[61].Position));
            }
            else
            {
                _drawTiles.Add(new(tiles[60].Source, tiles[60].Position));
            }
        }

        // Lighthouse
        if (_city.ImprovementExists(42))
        {
            if (_city.IsNextToRiver())
            {
                _drawTiles.Add(new(tiles[63].Source, tiles[63].Position));
            }
            else
            {
                _drawTiles.Add(new(tiles[62].Source, tiles[62].Position));
            }
        }

        // Statue liberty
        // TODO: find out where to draw statue of liberty in city view

        // Great wall
        var _id = 45;
        if (_city.ImprovementExists(_id))
        {
            var tileId = tiles.FirstOrDefault(t => t.RulesId == _id).Id;
            _drawTiles.Add(new(tiles[tileId].Source, tiles[tileId].Position));
        }

        _backId = city.Owner.Epoch switch
        {
            0 or 1 => 0,
            2 => 1,
            3 => 2,
            _ => throw new ArgumentOutOfRangeException($"Out of range epoch={city.Owner.Epoch}.")
        };

        if (city.Improvements.Any(i => i.Type == (int)ImprovementType.Superhighways))
        {
            _backId = 3;
        }

        if (city.IsNextToOcean())
        {
            _backBase = "cvOcean";
        }
        else if (city.IsNextToRiver())
        {
            _backBase = "cvRiver";
        }
        else
        {
            _backBase = "cvContinent";
        }

        _picWidth = Images.GetImageWidth(_active.PicSources[_backBase][0], _active);
        _picHeight = Images.GetImageHeight(_active.PicSources[_backBase][0], _active);
        _offsetX = (Width - _picWidth) / 2;
        _offsetY = (Height - _picHeight) / 2;
    }

    public override void OnKeyPress(KeyboardKey key)
    {
        base.Close();
    }

    public override void MouseOutsideControls(Vector2 mousePos)
    {
        if (Input.IsMouseButtonReleased(MouseButton.Left) &&
            ShapeHelper.CheckCollisionPointRec(mousePos, new(_offsetX, _offsetY, _picWidth, _picHeight)))
        {
            base.Close();
        }
    }

    public override void Draw(bool pulse)
    {
        Graphics.DrawRectangle(0, 0, Width, Height, Color.Black);

        Graphics.DrawTexture(TextureCache.GetImage(_active.PicSources[_backBase][_backId]),
            _offsetX, _offsetY, Color.White);

        foreach (var (Source, Pos) in _drawTiles)
        {
            Graphics.DrawTextureEx(TextureCache.GetImage(Source), 
                Pos + new Vector2(_offsetX, _offsetY), 0f, 1f, Color.White);
        }

        base.Draw(pulse);
    }
}
