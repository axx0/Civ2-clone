using System.Numerics;
using System.IO;
using System.Text.RegularExpressions;
using Civ2engine;
using Civ2engine.IO;
using Civ2engine.Terrains;
using Model;
using Model.Controls;
using Model.Controls.Civilopedia;
using Model.Core.Advances;
using Model.Core.Cities;
using Model.Core.GameRules;
using Model.Core.Mapping;
using Model.Core.Units;
using Model.Images;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Transformations;
using Raylib_CSharp.Textures;
using RaylibUI.BasicTypes;
using RaylibUI.BasicTypes.Controls;
using RaylibUtils;

namespace RaylibUI.RunGame.GameControls.Civilopedia;

public sealed class CivilopediaAdvanceInfo : BaseControl
{
    private const int Margin = 16;
    private const int ArtWidth = 340;
    private const int ArtHeight = 250;
    private const int DescriptionWidth = 500;
    private const int DescriptionHeight = 250;
    private const int SectionFontSize = 20;
    private const int BodyFontSize = 19;
    private const int LinkFontSize = 19;
    private const int LineHeight = 27;
    private const int HeadingWidth = 145;
    private const int LinkGap = 14;

    private readonly CivilopediaWindow _window;
    private readonly GameScreen _gameScreen;
    private readonly List<Advance> _advances;
    private readonly List<Improvement> _improvements;
    private readonly List<Improvement> _wonders;
    private readonly List<UnitDefinition> _units;
    private readonly CivilopediaEntry _pedia;
    private readonly IUserInterface _active;
    private readonly string? _advanceImagePath;
    private readonly IImageSource? _fallbackAdvanceImage;
    private readonly Advance _advance;
    private static readonly Dictionary<string, Texture2D> FossArtTextures = new(StringComparer.OrdinalIgnoreCase);

    public CivilopediaAdvanceInfo(CivilopediaWindow window, GameScreen gameScreen, List<Advance> advances,
        List<Improvement> improvements, List<Improvement> wonders, List<UnitDefinition> units,
        List<ITerrain> terrains, CivilopediaEntry pedia) : base(window)
    {
        _window = window;
        _gameScreen = gameScreen;
        _advances = advances;
        _improvements = improvements;
        _wonders = wonders;
        _units = units;
        _pedia = pedia;
        _active = gameScreen.MainWindow.ActiveInterface;
        _advance = _advances[_pedia.Id];
        _advanceImagePath = FindFossArtAdvanceImagePath(_advance);
        _fallbackAdvanceImage = _active.GetAdvanceImage(_advance)
                                ?? _active.PicSources["advanceCategories"][5 * _advance.Epoch + _advance.KnowledgeCategory];

        Width = window.Width - window.LayoutPadding.Left - window.LayoutPadding.Right;
        Height = window.Height - window.LayoutPadding.Top - window.LayoutPadding.Bottom;
        Location = new Vector2(window.LayoutPadding.Left, window.LayoutPadding.Top);

        BuildControls();
    }

    private void BuildControls()
    {
        var rules = _gameScreen.Game.Rules;
        var advanceId = Array.FindIndex(rules.Advances, row => row == _advance);

        var rightX = Margin + ArtWidth + 22;
        AddSectionLabel("Civilopedia", rightX, Margin - 4, DescriptionWidth);
        AddDescriptionBox(_advance, advanceId, rightX, Margin + 26);

        var y = Margin + ArtHeight + 38;
        y = AddPrerequisites(_advance, rules, y);
        y = AddAllows(advanceId, rules, y);
        y = AddEnabledByAdvance(advanceId, rules, y);
        AddObsoletedByAdvance(advanceId, rules, y);
    }

    private void AddDescriptionBox(Advance advance, int advanceId, int x, int y)
    {
        var description = NormalizeDescription(CivilopediaLoader.GetDescription(_pedia, advanceId));
        if (string.IsNullOrWhiteSpace(description))
        {
            description = advance.Name;
        }

        var textWidth = DescriptionWidth - 18;
        var wrappedTexts = DialogUtils.GetWrappedTexts(description, textWidth,
            _active.Look.LabelFont, BodyFontSize);

        var lineY = y + 8;
        var maxLines = Math.Max(1, (DescriptionHeight - 16) / LineHeight);
        foreach (var text in wrappedTexts.Take(maxLines))
        {
            AddBodyLabel(string.IsNullOrWhiteSpace(text) ? " " : text, x + 8, lineY, textWidth, LineHeight);
            lineY += LineHeight;
        }
    }

    private int AddPrerequisites(Advance advance, Rules rules, int y)
    {
        var prereqs = new List<Advance>();
        if (advance.Prereq1 != AdvancesConstants.Nil)
        {
            prereqs.Add(rules.Advances[advance.Prereq1]);
        }
        if (advance.Prereq2 != AdvancesConstants.Nil)
        {
            prereqs.Add(rules.Advances[advance.Prereq2]);
        }

        return prereqs.Count == 0
            ? AddInfoLine(Labels.For(LabelIndex.Prerequisites), Labels.For(LabelIndex.NONE), Margin, y)
            : AddAdvanceLinks(Labels.For(LabelIndex.Prerequisites), prereqs, Margin, y, Width - Margin);
    }

    private int AddAllows(int advanceId, Rules rules, int y)
    {
        var allows = rules.Advances
            .Where(a => a.Prereq1 == advanceId || a.Prereq2 == advanceId)
            .Where(a => _advances.Contains(a))
            .Take(18)
            .ToList();

        return allows.Count == 0 ? y : AddAdvanceLinks(Labels.For(LabelIndex.Allows), allows, Margin, y, Width - Margin);
    }

    private int AddEnabledByAdvance(int advanceId, Rules rules, int y)
    {
        var labels = new List<(string Text, Action Click)>();

        foreach (var improvement in rules.Improvements.Where(i => i.Prerequisite == advanceId))
        {
            labels.Add((improvement.Name, () => OpenImprovement(improvement)));
        }

        foreach (var unit in rules.UnitTypes.Where(u => u.Prereq == advanceId))
        {
            labels.Add((unit.Name, () => OpenUnit(unit)));
        }

        return labels.Count == 0 ? y : AddFlowLinks("Enables", labels, Margin, y, Width - Margin);
    }

    private int AddObsoletedByAdvance(int advanceId, Rules rules, int y)
    {
        var cancelled = rules.Improvements.Where(i => i.ExpiresAt == advanceId).ToList();
        if (cancelled.Count == 0)
        {
            return y;
        }

        var labels = cancelled.Select(improvement => (improvement.Name, (Action)(() => OpenImprovement(improvement)))).ToList();
        return AddFlowLinks("Cancels", labels, Margin, y, Width - Margin);
    }

    private int AddAdvanceLinks(string heading, IReadOnlyList<Advance> advances, int x, int y, int maxX)
    {
        var labels = advances
            .Select(advance => (advance.Name, (Action)(() => OpenAdvance(advance))))
            .ToList();

        return AddFlowLinks(heading, labels, x, y, maxX);
    }

    private int AddFlowLinks(string heading, IReadOnlyList<(string Text, Action Click)> links, int x, int y, int maxX)
    {
        AddSectionLabel(heading + ":", x, y, HeadingWidth);
        var cursorX = x + HeadingWidth + 8;
        var cursorY = y;
        var startX = cursorX;

        foreach (var (text, click) in links)
        {
            var label = new PediaLinkLabel(_window, text, cursorX, cursorY)
            {
                FontSize = LinkFontSize,
                Height = LineHeight,
                ColorShadow = Color.Blank,
                ShadowOffset = Vector2.Zero,
                VerticalAlignment = VerticalAlignment.Center
            };

            if (cursorX + label.Width > maxX && cursorX > startX)
            {
                cursorX = startX;
                cursorY += LineHeight;
                label.Location = new(cursorX, cursorY);
            }

            label.Click += (_, _) => click();
            Controls.Add(label);
            cursorX += label.Width + LinkGap;
        }

        return cursorY + LineHeight + 3;
    }

    private int AddInfoLine(string heading, string value, int x, int y)
    {
        AddSectionLabel(heading + ":", x, y, HeadingWidth);
        AddBodyLabel(value, x + HeadingWidth + 8, y, Width - x - HeadingWidth - 16, LineHeight,
            verticalAlignment: VerticalAlignment.Center);
        return y + LineHeight + 3;
    }

    private void AddSectionLabel(string text, int x, int y, int width)
    {
        var label = new LabelControl(_window, text, true,
            font: _active.Look.HeaderLabelFont,
            fontSize: SectionFontSize,
            colorFront: new Color(70, 45, 12, 255),
            colorShadow: new Color(255, 255, 255, 170),
            shadowOffset: Vector2.One)
        {
            Location = new(x, y),
            Width = width,
            Height = LineHeight,
            VerticalAlignment = VerticalAlignment.Center
        };
        Controls.Add(label);
    }

    private void AddBodyLabel(string text, int x, int y, int width, int height,
        VerticalAlignment verticalAlignment = VerticalAlignment.Top)
    {
        var label = new LabelControl(_window, text, true,
            font: _active.Look.LabelFont,
            fontSize: BodyFontSize,
            colorFront: new Color(22, 22, 18, 255),
            colorShadow: Color.Blank,
            shadowOffset: Vector2.Zero)
        {
            Location = new(x, y),
            Width = width,
            Height = height,
            VerticalAlignment = verticalAlignment
        };
        Controls.Add(label);
    }

    private static string NormalizeDescription(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        return Regex.Replace(text.Replace('_', ' '), @"\s+", " ").Trim();
    }

    private static string? FindFossArtAdvanceImagePath(Advance advance)
    {
        foreach (var candidateName in GetAdvanceArtNames(advance.Name))
        {
            var target = NormalizeFossArtName(candidateName);
            foreach (var directory in GetFossArtAdvanceDirectories())
            {
                if (!Directory.Exists(directory))
                {
                    continue;
                }

                var matchingFile = Directory.EnumerateFiles(directory)
                    .Where(file => IsSupportedArtFile(file))
                    .FirstOrDefault(file => string.Equals(
                        NormalizeFossArtName(Path.GetFileNameWithoutExtension(file)),
                        target,
                        StringComparison.OrdinalIgnoreCase));

                if (!string.IsNullOrWhiteSpace(matchingFile))
                {
                    return matchingFile;
                }
            }
        }

        return null;
    }

    private static bool IsSupportedArtFile(string file)
    {
        var extension = Path.GetExtension(file);
        return extension.Equals(".png", StringComparison.OrdinalIgnoreCase)
               || extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase)
               || extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase)
               || extension.Equals(".gif", StringComparison.OrdinalIgnoreCase)
               || extension.Equals(".bmp", StringComparison.OrdinalIgnoreCase);
    }

    private static IEnumerable<string> GetAdvanceArtNames(string advanceName)
    {
        yield return advanceName;

        if (advanceName.StartsWith("The ", StringComparison.OrdinalIgnoreCase))
        {
            yield return advanceName[4..];
        }

        if (advanceName.Equals("Advanced Flight", StringComparison.OrdinalIgnoreCase))
        {
            yield return "advancedflight";
        }
    }

    private static IEnumerable<string> GetFossArtAdvanceDirectories()
    {
        var roots = new[]
        {
            Environment.CurrentDirectory,
            AppContext.BaseDirectory,
            Path.Combine(Environment.CurrentDirectory, "RaylibUI"),
            Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."))
        };

        foreach (var root in roots.Where(root => !string.IsNullOrWhiteSpace(root)).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            yield return Path.Combine(root, "FOSSart", "Advances");
            yield return Path.Combine(root, "FOSSart", "FOSSart", "Advances");
            yield return Path.Combine(root, "RaylibUI", "FOSSart", "Advances");
            yield return Path.Combine(root, "RaylibUI", "FOSSart", "FOSSart", "Advances");
        }
    }

    private static string NormalizeFossArtName(string value)
    {
        return Regex.Replace(value, "[^A-Za-z0-9]", string.Empty).ToLowerInvariant();
    }

    private void OpenAdvance(Advance advance)
    {
        var id = _advances.IndexOf(advance);
        if (id < 0)
        {
            return;
        }

        _pedia.InfoType = CivilopediaInfoType.Advances;
        _pedia.WindowType = CivilopediaWindowType.Info;
        _pedia.Id = id;
        _window.UpdateControls();
    }

    private void OpenImprovement(Improvement improvement)
    {
        var id = _improvements.IndexOf(improvement);
        if (id >= 0)
        {
            _pedia.InfoType = CivilopediaInfoType.Improvements;
            _pedia.WindowType = CivilopediaWindowType.Info;
            _pedia.Id = id;
            _window.UpdateControls();
            return;
        }

        id = _wonders.IndexOf(improvement);
        if (id >= 0)
        {
            _pedia.InfoType = CivilopediaInfoType.Wonders;
            _pedia.WindowType = CivilopediaWindowType.Info;
            _pedia.Id = id;
            _window.UpdateControls();
        }
    }

    private void OpenUnit(UnitDefinition unit)
    {
        var id = _units.IndexOf(unit);
        if (id < 0)
        {
            return;
        }

        _pedia.InfoType = CivilopediaInfoType.Units;
        _pedia.WindowType = CivilopediaWindowType.Info;
        _pedia.Id = id;
        _window.UpdateControls();
    }

    public override void Draw(bool pulse)
    {
        var leftX = (int)(Bounds.X + Margin - 8);
        var topY = (int)(Bounds.Y + Margin - 8);
        var artPanelW = ArtWidth + 16;
        var artPanelH = ArtHeight + 40;
        Graphics.DrawRectangle(leftX, topY, artPanelW, artPanelH, new Color(250, 246, 224, 235));
        Graphics.DrawRectangleLines(leftX, topY, artPanelW, artPanelH, new Color(92, 70, 35, 255));

        DrawAdvanceArtwork(leftX + 8, topY + 8, ArtWidth, artPanelH - 16);

        var rightX = leftX + artPanelW + 14;
        var rightW = DescriptionWidth + 16;
        Graphics.DrawRectangle(rightX, topY, rightW, artPanelH, new Color(243, 238, 214, 235));
        Graphics.DrawRectangleLines(rightX, topY, rightW, artPanelH, new Color(92, 70, 35, 255));

        var lowerY = topY + artPanelH + 10;
        var lowerH = Math.Max(1, (int)(Bounds.Y + Height - lowerY - 10));
        Graphics.DrawRectangle(leftX, lowerY, Width - 2 * Margin + 16, lowerH, new Color(250, 246, 224, 220));
        Graphics.DrawRectangleLines(leftX, lowerY, Width - 2 * Margin + 16, lowerH, new Color(92, 70, 35, 255));

        base.Draw(pulse);
    }

    private void DrawAdvanceArtwork(int x, int y, int width, int height)
    {
        if (_advanceImagePath != null && TryGetFossArtTexture(_advanceImagePath, out var fossTexture))
        {
            DrawTextureFit(fossTexture, x, y, width, height);
            return;
        }

        if (_fallbackAdvanceImage == null)
        {
            return;
        }

        var fallbackTexture = TextureCache.GetImage(_fallbackAdvanceImage);
        if (fallbackTexture.Width > 0 && fallbackTexture.Height > 0)
        {
            DrawTextureFit(fallbackTexture, x, y, width, height);
        }
    }

    private static bool TryGetFossArtTexture(string path, out Texture2D texture)
    {
        if (FossArtTextures.TryGetValue(path, out texture))
        {
            return texture.Width > 0 && texture.Height > 0;
        }

        texture = default;
        try
        {
            var image = Images.LoadImageFromFile(path).Image;
            if (image.Width <= 0 || image.Height <= 0)
            {
                return false;
            }

            texture = Texture2D.LoadFromImage(image);
            texture.SetFilter((TextureFilter)Settings.TextureFilter);
            FossArtTextures[path] = texture;
            return texture.Width > 0 && texture.Height > 0;
        }
        catch
        {
            return false;
        }
    }

    private static void DrawTextureFit(Texture2D texture, int x, int y, int width, int height)
    {
        if (texture.Width <= 0 || texture.Height <= 0)
        {
            return;
        }

        var scale = Math.Min(width / (float)texture.Width, height / (float)texture.Height);
        scale = Math.Max(0.01f, scale);
        var drawWidth = texture.Width * scale;
        var drawHeight = texture.Height * scale;
        var drawX = x + (width - drawWidth) / 2f;
        var drawY = y + (height - drawHeight) / 2f;

        Graphics.DrawTexturePro(texture,
            new Rectangle(0, 0, texture.Width, texture.Height),
            new Rectangle(drawX, drawY, drawWidth, drawHeight),
            Vector2.Zero,
            0f,
            Color.White);
    }

}
