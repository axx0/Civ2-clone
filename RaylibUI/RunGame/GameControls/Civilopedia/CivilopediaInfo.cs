using Civ2engine;
using Civ2engine.Terrains;
using Model.Controls;
using Model.Core.Advances;
using Model.Core.Units;
using Model.Images;
using RaylibUI.BasicTypes.Controls;
using System.Data;

namespace RaylibUI.RunGame.GameControls.CityControls;

public class CivilopediaInfo : BaseControl
{
    public CivilopediaInfo(CivilopediaWindow window, GameScreen gameScreen, List<Advance> advances,
        List<Improvement> improvements, List<Improvement> wonders, List<UnitDefinition> units,
        List<ITerrain> terrains, Civilopedia pedia) : base(window)
    {
        var active = gameScreen.MainWindow.ActiveInterface;
        var rules = gameScreen.Game.Rules;

        Width = window.Width - window.LayoutPadding.Left - window.LayoutPadding.Right;
        Height = window.Height - window.LayoutPadding.Top - window.LayoutPadding.Bottom;
        Location = new(window.LayoutPadding.Left, window.LayoutPadding.Top);

        switch (pedia.InfoType)
        {
            case CivilopediaInfoType.Advances:
                var advance = advances[pedia.Id];
                Advance? preq1 = advance.Prereq1 != -1 ? rules.Advances[advance.Prereq1] : null;
                Advance? preq2 = advance.Prereq2 != -1 ? rules.Advances[advance.Prereq2] : null;
                var advanceId = Array.FindIndex(rules.Advances, row => row == advance);
                var allowed = rules.Advances.Where(a => a.Prereq1 == advanceId || a.Prereq2 == advanceId).ToArray();
                Advance?[] allowedWith = new Advance[allowed.Length];
                for (int i = 0; i < allowed.Length; i++)
                {
                    if (allowed[i].Prereq1 == advanceId && allowed[i].Prereq2 != -1)
                    {
                        allowedWith[i] = rules.Advances[allowed[i].Prereq2];
                    }
                    else if (allowed[i].Prereq2 == advanceId && allowed[i].Prereq1 != -1)
                    {
                        allowedWith[i] = rules.Advances[allowed[i].Prereq1];
                    }
                }
                var improvs = rules.Improvements.Where(i => i.Prerequisite == advanceId).ToArray();
                var unts = rules.UnitTypes.Where(u => u.Prereq == advanceId).ToArray();
                var endsWonder = rules.Improvements.Where(i => i.ExpiresAt == advanceId).ToArray();

                var icon =
                    new ImageBox(window, new(active.PicSources["advanceCategories"][5 * advance.Epoch + advance.KnowledgeCategory]), true)
                    {
                        Location = new(4, 7)
                    };
                Controls.Add(icon);

                var prereqLabel = new PediaLabel(window, Labels.For(LabelIndex.Prerequisites) + ":",
                    (int)icon.Location.X + icon.Width + 11, (int)icon.Location.Y);
                Controls.Add(prereqLabel);

                var offsetX = prereqLabel.Location.X + prereqLabel.Width + 10;

                if (preq1 == null && preq2 == null)
                {
                    Controls.Add(new PediaLabel(window, "NONE", (int)offsetX, (int)icon.Location.Y));
                }

                if (preq1 != null)
                {
                    var prereq1Label = new PediaLinkLabel(window, preq1.Name, (int)offsetX, (int)icon.Location.Y);
                    prereq1Label.Click += (_, _) =>
                    {
                        pedia.Id = advances.IndexOf(preq1);
                        window.UpdateControls();
                    };
                    Controls.Add(prereq1Label);
                    offsetX += prereq1Label.Width;
                }

                if (preq1 != null && preq2 != null)
                {
                    offsetX += 3;
                    Controls.Add(new PediaLabel(window, ",", (int)offsetX, (int)icon.Location.Y));
                    offsetX += 13;
                }

                if (preq2 != null)
                {
                    var prereq2Label = new PediaLinkLabel(window, preq2.Name, (int)offsetX, (int)icon.Location.Y);
                    prereq2Label.Click += (_, _) =>
                    {
                        pedia.Id = advances.IndexOf(preq2);
                        window.UpdateControls();
                    };
                    Controls.Add(prereq2Label);
                }

                var offsetY = prereqLabel.Location.Y + prereqLabel.Height + 1;

                var allowsTxt = new PediaLabel(window, Labels.For(LabelIndex.Allows) + ":", 4, (int)offsetY);
                Controls.Add(allowsTxt);
                offsetX = allowsTxt.Location.X + allowsTxt.Width + 7;

                for (int i = 0; i < allowed.Length; i++)
                {
                    var alw = allowed[i];

                    var allowedLabel = new PediaLinkLabel(window, alw.Name, (int)offsetX, (int)offsetY);
                    Controls.Add(allowedLabel);
                    allowedLabel.Click += (_, _) =>
                    {
                        pedia.Id = advances.IndexOf(alw);
                        window.UpdateControls();
                    };

                    if (allowedWith[i] != null)
                    {
                        var alww = allowedWith[i];

                        var withLabel = new PediaLabel(window, $"  ({Labels.For(LabelIndex.with)} ",
                            (int)allowedLabel.Location.X + allowedLabel.Width, (int)offsetY);
                        Controls.Add(withLabel);

                        var allowedWithLabel = new PediaLinkLabel(window, alww!.Name,
                            (int)withLabel.Location.X + withLabel.Width, (int)offsetY);
                        Controls.Add(allowedWithLabel);
                        allowedWithLabel.Click += (_, _) =>
                        {
                            pedia.Id = advances.IndexOf(alww);
                            window.UpdateControls();
                        };

                        Controls.Add(new PediaLabel(window, ")",
                            (int)allowedWithLabel.Location.X + allowedWithLabel.Width, (int)offsetY));
                    }

                    offsetY += allowedLabel.Height + 1;
                }

                for (var i = 0; i < improvs.Length; i++)
                {
                    var impr = improvs[i];

                    var nextXoffset = (i % 2) * 225;
                    var imprImg = new ImageBox(window,
                        new(active.GetImprovementImage(impr, rules.FirstWonderIndex)), true)
                    {
                        Location = new(offsetX + nextXoffset, offsetY + 1)
                    };
                    Controls.Add(imprImg);

                    var improvLabel = new PediaLinkLabel(window, impr.Name,
                        (int)imprImg.Location.X + imprImg.Width, (int)offsetY);
                    Controls.Add(improvLabel);
                    improvLabel.Click += (_, _) =>
                    {
                        pedia.InfoType = CivilopediaInfoType.Improvements;
                        pedia.Id = improvements.IndexOf(impr);
                        if (pedia.Id == -1)
                        {
                            pedia.InfoType = CivilopediaInfoType.Wonders;
                            pedia.Id = wonders.IndexOf(impr);
                        }
                        window.UpdateControls();
                    };

                    if (i % 2 == 1 || i == improvs.Length - 1)
                    {
                        offsetY += improvLabel.Height + 1;
                    }
                }

                for (var i = 0; i < unts.Length; i++)
                {
                    var unt = unts[i];

                    var nextXoffset = (i % 2) * 225;
                    var untImg = new ImageBox(window, new(active.PicSources["unit"][unt.Type]), true)
                    {
                        Location = new(offsetX + nextXoffset, offsetY + 1)
                    };
                    Controls.Add(untImg);

                    var unitLabel = new PediaLinkLabel(window, unt.Name,
                        (int)untImg.Location.X + untImg.Width, (int)offsetY);
                    unitLabel.Location = new(unitLabel.Location.X, unitLabel.Location.Y + (untImg.Height - unitLabel.Height) / 2);
                    Controls.Add(unitLabel);
                    unitLabel.Click += (_, _) =>
                    {
                        pedia.InfoType = CivilopediaInfoType.Units;
                        pedia.Id = units.IndexOf(unt);
                        window.UpdateControls();
                    };

                    if (i % 2 == 1 || i == unts.Length - 1)
                    {
                        offsetY += untImg.Height + 1;
                    }
                }

                for (int i = 0; i < endsWonder.Length; i++)
                {
                    var impr = endsWonder[i];

                    var label = new PediaLabel(window, "Cancels the effect of ", (int)offsetX, (int)offsetY);
                    Controls.Add(label);

                    var imprLabel = new PediaLinkLabel(window, impr.Name, (int)label.Location.X + label.Width, (int)offsetY);
                    Controls.Add(imprLabel);
                    imprLabel.Click += (_, _) =>
                    {
                        pedia.InfoType = CivilopediaInfoType.Wonders;
                        pedia.Id = wonders.IndexOf(impr);
                        window.UpdateControls();
                    };

                    offsetY += imprLabel.Height + 1;
                }

                break;

            case CivilopediaInfoType.Improvements:

                var improv = improvements[pedia.Id];
                Advance? preq = improv.Prerequisite != -1 ? rules.Advances[improv.Prerequisite] : null;

                icon = new ImageBox(window, new(active.GetImprovementImage(improv, rules.FirstWonderIndex), 2f), true)
                {
                    Location = new(12, 10)
                };
                Controls.Add(icon);

                offsetY = icon.Location.Y + icon.Height + 1;
                prereqLabel = new PediaLabel(window, Labels.For(LabelIndex.Prerequisites) + ":  ",
                    (int)icon.Location.X, (int)offsetY);
                Controls.Add(prereqLabel);

                if (preq == null)
                {
                    Controls.Add(new PediaLabel(window, Labels.For(LabelIndex.NONE),
                        (int)prereqLabel.Location.X + prereqLabel.Width, (int)offsetY));
                }
                else
                {
                    var preqLabel = new PediaLinkLabel(window, preq.Name, (int)prereqLabel.Location.X + prereqLabel.Width, (int)offsetY);
                    Controls.Add(preqLabel);
                    preqLabel.Click += (_, _) =>
                    {
                        pedia.InfoType = CivilopediaInfoType.Advances;
                        pedia.Id = advances.IndexOf(preq);
                        window.UpdateControls();
                    };
                }
                offsetY += prereqLabel.Height + 1;

                Controls.Add(new PediaLabel(window, Labels.For(LabelIndex.Cost), (int)icon.Location.X, (int)offsetY));
                var costLabel = new PediaLabel(window, $"{improv.Cost * 10} ",
                    (int)prereqLabel.Location.X + prereqLabel.Width, (int)offsetY);
                Controls.Add(costLabel);

                var shieldImg = new ImageBox(window, new(active.ResourceImages.First(i => i.Name == "Shields").LargeImage), true);
                shieldImg.Location = new((int)costLabel.Location.X + costLabel.Width, (int)offsetY + (costLabel.Height - shieldImg.Height) / 2);
                Controls.Add(shieldImg);

                offsetY += prereqLabel.Height + 1;

                Controls.Add(new PediaLabel(window, Labels.For(LabelIndex.Maintenance), (int)icon.Location.X, (int)offsetY));
                var maintLabel = new PediaLabel(window, $"{improv.Upkeep} ",
                    (int)prereqLabel.Location.X + prereqLabel.Width, (int)offsetY);
                Controls.Add(maintLabel);

                var goldImg = new ImageBox(window, new(active.PicSources["gold,large"][0]), true);
                goldImg.Location = new((int)maintLabel.Location.X + maintLabel.Width, (int)offsetY + (maintLabel.Height - goldImg.Height) / 2);
                Controls.Add(goldImg);

                offsetY += 2 * (prereqLabel.Height + 1);

                var text = CivilopediaLoader.GetPediaImprovementText(Array.FindIndex(rules.Improvements, row => row == improv));
                var wrappedTexts = DialogUtils.GetWrappedTexts(text, Width - (int)prereqLabel.Location.X,
                    active.Look.LabelFont, 22);
                foreach (var txt in wrappedTexts)
                {
                    Controls.Add(new PediaLabel(window, txt, (int)prereqLabel.Location.X, (int)offsetY));
                    offsetY += prereqLabel.Height + 1;
                }

                break;

            case CivilopediaInfoType.Wonders:
                var wonder = wonders[pedia.Id];
                preq = wonder.Prerequisite != -1 ? rules.Advances[wonder.Prerequisite] : null;
                Advance? expires = wonder.ExpiresAt != -1 ? rules.Advances[wonder.ExpiresAt] : null;

                icon = new ImageBox(window, new(active.GetImprovementImage(wonder, rules.FirstWonderIndex), 2f), true)
                {
                    Location = new(12, 10)
                };
                Controls.Add(icon);

                offsetY = icon.Location.Y + icon.Height + 1;
                prereqLabel = new PediaLabel(window, Labels.For(LabelIndex.Prerequisites) + ":  ",
                    (int)icon.Location.X, (int)offsetY);
                Controls.Add(prereqLabel);

                if (preq == null)
                {
                    Controls.Add(new PediaLabel(window, Labels.For(LabelIndex.NONE),
                        (int)prereqLabel.Location.X + prereqLabel.Width, (int)offsetY));
                }
                else
                {
                    var preqLabel = new PediaLinkLabel(window, preq.Name, (int)prereqLabel.Location.X + prereqLabel.Width, (int)offsetY);
                    Controls.Add(preqLabel);
                    preqLabel.Click += (_, _) =>
                    {
                        pedia.InfoType = CivilopediaInfoType.Advances;
                        pedia.Id = advances.IndexOf(preq);
                        window.UpdateControls();
                    };
                }
                offsetY += prereqLabel.Height + 1;

                Controls.Add(new PediaLabel(window, Labels.For(LabelIndex.Cost), (int)icon.Location.X, (int)offsetY));
                costLabel = new PediaLabel(window, $"{wonder.Cost * 10} ",
                    (int)prereqLabel.Location.X + prereqLabel.Width, (int)offsetY);
                Controls.Add(costLabel);

                shieldImg = new ImageBox(window, new(active.ResourceImages.First(i => i.Name == "Shields").LargeImage), true);
                shieldImg.Location = new((int)costLabel.Location.X + costLabel.Width, (int)offsetY + (costLabel.Height - shieldImg.Height) / 2);
                Controls.Add(shieldImg);

                offsetY += prereqLabel.Height + 1;

                var expireLabel = new PediaLabel(window, Labels.For(LabelIndex.Expires) + ":  ",
                    (int)icon.Location.X, (int)offsetY);
                Controls.Add(expireLabel);

                if (expires == null)
                {
                    Controls.Add(new PediaLabel(window, Labels.For(LabelIndex.NONE),
                        (int)prereqLabel.Location.X + prereqLabel.Width, (int)offsetY));
                }
                else
                {
                    var expLabel = new PediaLinkLabel(window, expires.Name, (int)prereqLabel.Location.X + prereqLabel.Width, (int)offsetY);
                    Controls.Add(expLabel);
                    expLabel.Click += (_, _) =>
                    {
                        pedia.InfoType = CivilopediaInfoType.Advances;
                        pedia.Id = advances.IndexOf(expires);
                        window.UpdateControls();
                    };
                }

                offsetY += 2 * (prereqLabel.Height + 1);

                text = CivilopediaLoader.GetPediaImprovementText(Array.FindIndex(rules.Improvements, row => row == wonder));
                wrappedTexts = DialogUtils.GetWrappedTexts(text, Width - (int)prereqLabel.Location.X,
                    active.Look.LabelFont, 22);
                foreach (var txt in wrappedTexts)
                {
                    Controls.Add(new PediaLabel(window, txt, (int)prereqLabel.Location.X, (int)offsetY));
                    offsetY += prereqLabel.Height + 1;
                }

                break;

            case CivilopediaInfoType.Units:
                var unit = units[pedia.Id];
                preq = unit.Prereq != -1 ? rules.Advances[unit.Prereq] : null;

                icon = new ImageBox(window, new(active.PicSources["unit"][unit.Type], 2f), true)
                {
                    Location = new(12, 7)
                };
                Controls.Add(icon);

                prereqLabel = new PediaLabel(window, Labels.For(LabelIndex.Prerequisites) + ":  ",
                    (int)icon.Location.X + icon.Width + 54, 0);
                prereqLabel.Location = new(prereqLabel.Location.X, icon.Location.Y + (icon.Height - prereqLabel.Height) / 2);
                Controls.Add(prereqLabel);

                if (preq == null)
                {
                    Controls.Add(new PediaLabel(window, Labels.For(LabelIndex.NONE),
                        (int)prereqLabel.Location.X + prereqLabel.Width, (int)prereqLabel.Location.Y));
                }
                else
                {
                    var preqLabel = new PediaLinkLabel(window, preq.Name, (int)prereqLabel.Location.X + prereqLabel.Width,
                        (int)prereqLabel.Location.Y);
                    Controls.Add(preqLabel);
                    preqLabel.Click += (_, _) =>
                    {
                        pedia.InfoType = CivilopediaInfoType.Advances;
                        pedia.Id = advances.IndexOf(preq);
                        window.UpdateControls();
                    };
                }

                offsetY = icon.Location.Y + icon.Height + 16;

                costLabel = new PediaLabel(window, Labels.For(LabelIndex.Cost) + ":",
                    (int)icon.Location.X, (int)offsetY);
                Controls.Add(costLabel);
                var costvLabel = new PediaLabel(window, $"{10 * unit.Cost} ",
                    (int)icon.Location.X + 210, (int)offsetY);
                Controls.Add(costvLabel);
                shieldImg = new ImageBox(window, new(active.ResourceImages.First(i => i.Name == "Shields").LargeImage), true);
                shieldImg.Location = new((int)costvLabel.Location.X + costvLabel.Width, (int)offsetY + (costvLabel.Height - shieldImg.Height) / 2);
                Controls.Add(shieldImg);
                offsetY += costLabel.Height + 1;

                var attLabel = new PediaLabel(window, Labels.For(LabelIndex.AttackStrength) + ":",
                    (int)icon.Location.X, (int)offsetY);
                Controls.Add(attLabel);
                var attvLabel = new PediaLabel(window, $"{unit.Attack}",
                    (int)icon.Location.X + 210, (int)offsetY);
                Controls.Add(attvLabel);
                offsetY += attLabel.Height + 1;

                var defLabel = new PediaLabel(window, Labels.For(LabelIndex.DefenseStrength) + ":",
                    (int)icon.Location.X, (int)offsetY);
                Controls.Add(defLabel);
                var defvLabel = new PediaLabel(window, $"{unit.Defense}",
                    (int)icon.Location.X + 210, (int)offsetY);
                Controls.Add(defvLabel);
                offsetY += defLabel.Height + 1;

                var hitpLabel = new PediaLabel(window, Labels.For(LabelIndex.HitPoints) + ":",
                    (int)icon.Location.X, (int)offsetY);
                Controls.Add(hitpLabel);
                var hitpvLabel = new PediaLabel(window, $"{unit.Hitp / 10}",
                    (int)icon.Location.X + 210, (int)offsetY);
                Controls.Add(hitpvLabel);
                offsetY += hitpLabel.Height + 1;

                var frpLabel = new PediaLabel(window, Labels.For(LabelIndex.Firepower) + ":",
                   (int)icon.Location.X, (int)offsetY);
                Controls.Add(frpLabel);
                var frpvLabel = new PediaLabel(window, $"{unit.Firepwr}",
                    (int)icon.Location.X + 210, (int)offsetY);
                Controls.Add(frpvLabel);
                offsetY += frpLabel.Height + 1;

                var movLabel = new PediaLabel(window, Labels.For(LabelIndex.MovementRate) + ":",
                   (int)icon.Location.X, (int)offsetY);
                Controls.Add(movLabel);
                var movvLabel = new PediaLabel(window, $"{unit.Move / 3}",
                    (int)icon.Location.X + 210, (int)offsetY);
                Controls.Add(movvLabel);

                offsetY = (int)costLabel.Location.Y;
                text = CivilopediaLoader.GetPediaUnitText(unit.Flags);
                wrappedTexts = DialogUtils.GetWrappedTexts(text, 310, active.Look.LabelFont, 22);
                foreach (var txt in wrappedTexts)
                {
                    Controls.Add(new PediaLabel(window, txt, Width / 2, (int)offsetY));
                    offsetY += prereqLabel.Height + 1;
                }

                break;

            case CivilopediaInfoType.Governments:
                break;

            case CivilopediaInfoType.Terrains:
                var terrain = terrains[pedia.Id];
                var names = terrains.Select(t => t.Name).ToArray();

                if (terrain is Terrain t)
                {
                    icon = new ImageBox(window, new(active.PicSources["base1"][(int)t.Type], 2f), true)
                    {
                        Location = new(12, 7)
                    };
                    Controls.Add(icon);

                    offsetY = 80;
                    var moveLabel = new PediaLabel(window, $"{Labels.For(LabelIndex.MoveCost)}:", 11, (int)offsetY);
                    Controls.Add(moveLabel);
                    Controls.Add(new PediaLabel(window, $"{t.MoveCost}", 200, (int)offsetY));

                    offsetY += moveLabel.Height + 1;
                    var defBnsLabel = new PediaLabel(window, $"{Labels.For(LabelIndex.UnitDefenseBonus)}:",
                        11, (int)offsetY);
                    Controls.Add(defBnsLabel);
                    var defBonus = 50 * t.Defense;
                    var defTxt = $"{Labels.For(LabelIndex.Normal)}";
                    if (defBonus > 100)
                    {
                        defTxt = "+" + (defBonus - 100) + "%";
                    }
                    else if (defBonus < 100)
                    {
                        defTxt = (defBonus - 100) + "%";
                    }
                    Controls.Add(new PediaLabel(window, defTxt, 200, (int)offsetY));

                    offsetY += defBnsLabel.Height + 1;
                    var foodLabel = new PediaLabel(window, $"{Labels.For(LabelIndex.Food)}:", 11, (int)offsetY);
                    Controls.Add(foodLabel);
                    var food = new PediaLabel(window, $"{t.Food} ", 200, (int)offsetY);
                    Controls.Add(food);
                    var foodIcon = new ImageBox(window, new(active.ResourceImages.First(i => i.Name == "Food").LargeImage), true);
                    foodIcon.Location = new((int)food.Location.X + food.Width,
                        (int)offsetY + (food.Height - foodIcon.Height) / 2);
                    Controls.Add(foodIcon);

                    offsetY += foodLabel.Height + 1;
                    var shieldsLabel = new PediaLabel(window, $"{Labels.For(LabelIndex.Shields)}:", 11, (int)offsetY);
                    Controls.Add(shieldsLabel);
                    var shields = new PediaLabel(window, $"{t.Shields} ", 200, (int)offsetY);
                    Controls.Add(shields);
                    var shldIcon = new ImageBox(window, new(active.ResourceImages.First(i => i.Name == "Shields").LargeImage), true);
                    shldIcon.Location = new((int)shields.Location.X + shields.Width,
                        (int)offsetY + (shields.Height - shldIcon.Height) / 2);
                    Controls.Add(shldIcon);

                    offsetY += shieldsLabel.Height + 1;
                    var tradeLabel = new PediaLabel(window, $"{Labels.For(LabelIndex.Trade)}:", 11, (int)offsetY);
                    Controls.Add(tradeLabel);
                    var trade = new PediaLabel(window, $"{t.Trade} ", 200, (int)offsetY);
                    Controls.Add(trade);
                    var trdIcon = new ImageBox(window, new(active.ResourceImages.First(i => i.Name == "Trade").LargeImage), true);
                    trdIcon.Location = new((int)trade.Location.X + trade.Width,
                        (int)offsetY + (trade.Height - trdIcon.Height) / 2);
                    Controls.Add(trdIcon);

                    offsetY += tradeLabel.Height + 1;
                    var irrigLabel = new PediaLabel(window, $"{Labels.For(LabelIndex.EffectsofIrrigation)}:",
                        11, (int)offsetY);
                    Controls.Add(irrigLabel);
                    var irrig = new PediaLabel(window, "TO-DO", 200, (int)offsetY);
                    Controls.Add(irrig);

                    offsetY += irrigLabel.Height + 1;
                    var irrigtLabel = new PediaLabel(window, $"{Labels.For(LabelIndex.TurnstoIrrigate)}:",
                        11, (int)offsetY);
                    Controls.Add(irrigtLabel);
                    var irrigt = new PediaLabel(window, "TO-DO", 200, (int)offsetY);
                    Controls.Add(irrigt);

                    offsetY += irrigtLabel.Height + 1;
                    var mineLabel = new PediaLabel(window, $"{Labels.For(LabelIndex.EffectsofMining)}:",
                        11, (int)offsetY);
                    Controls.Add(mineLabel);
                    var mine = new PediaLabel(window, "TO-DO", 200, (int)offsetY);
                    Controls.Add(mine);

                    offsetY += mineLabel.Height + 1;
                    var minetLabel = new PediaLabel(window, $"{Labels.For(LabelIndex.TurnstoMine)}:",
                        11, (int)offsetY);
                    Controls.Add(minetLabel);
                    var minet = new PediaLabel(window, "TO-DO", 200, (int)offsetY);
                    Controls.Add(minet);

                    // Transformation effects
                    offsetX = Width / 2;
                    offsetY = 80;
                    var effectsLabel = new PediaLabel(window, $"{Labels.For(LabelIndex.EffectofEngineerTransformation)}:",
                        (int)offsetX, (int)offsetY);
                    Controls.Add(effectsLabel);

                    offsetY += effectsLabel.Height + 1;
                    if (t.Transform >= 0)
                    {
                        var transfTo = rules.Terrains[0][t.Transform];
                        var transfLabel = new PediaLinkLabel(window, transfTo.Name, (int)offsetX + 20, (int)offsetY);
                        transfLabel.Click += (_, _) =>
                        {
                            pedia.InfoType = CivilopediaInfoType.Terrains;
                            pedia.Id = terrains.IndexOf(transfTo);
                            window.UpdateControls();
                        };
                        Controls.Add(transfLabel);
                    }
                    else
                    {
                        Controls.Add(new PediaLabel(window, $"{Labels.For(LabelIndex.NA)}",
                            (int)offsetX + 20, (int)offsetY));
                    }

                    // Effect of roads
                    if (t.CanBeTransformed) // do this to skip ocean
                    {
                        offsetY += effectsLabel.Height + 1;
                        var roadLabel = new PediaLabel(window, $"{Labels.For(LabelIndex.EffectsofRoads)}:",
                            (int)offsetX, (int)offsetY);
                        Controls.Add(roadLabel);
                        offsetY += effectsLabel.Height + 1;
                        var roadLabel2 = new PediaLabel(window, $"{Labels.For(LabelIndex.MoveCost)}: 1/3 of a point",
                            (int)offsetX + 20, (int)offsetY);
                        Controls.Add(roadLabel2);
                    }

                    // Possible resources
                    offsetY += effectsLabel.Height + 1;
                    var possibLabel = new PediaLabel(window, $"{Labels.For(LabelIndex.PossibleResources)}:",
                        (int)offsetX, (int)offsetY);
                    Controls.Add(possibLabel);
                    for (var i = 0; i < t.Specials.Length; i++)
                    {
                        offsetY += effectsLabel.Height + 1;
                        var spec = t.Specials[i];
                        var name = spec.Name;
                        if (t.Specials[0].Name == t.Specials[1].Name)
                            name += $" ({Labels.For(LabelIndex.Shield)}s)";
                        var specLabel = new PediaLinkLabel(window, name, (int)offsetX + 20, (int)offsetY);
                        specLabel.Click += (_, _) =>
                        {
                            pedia.InfoType = CivilopediaInfoType.Terrains;
                            pedia.Id = terrains.IndexOf(spec);
                            window.UpdateControls();
                        };
                        Controls.Add(specLabel);

                        if (t.Specials[0].Name == t.Specials[1].Name)
                            break;
                    }
                }
                else
                {
                    var icons = new IImageSource[2];
                    var s = (Special)terrain;
                    var baseTerrain = rules.Terrains[0].FirstOrDefault(t => t.Specials[0] == s);
                    if (baseTerrain != null)
                    {
                        icons[0] = active.PicSources["base1"][(int)baseTerrain.Type];
                        icons[1] = active.PicSources["special1"][(int)baseTerrain.Type];
                    }
                    else
                    {
                        baseTerrain = rules.Terrains[0].FirstOrDefault(t => t.Specials[1] == s);
                        icons[0] = active.PicSources["base1"][(int)baseTerrain.Type];
                        icons[1] = active.PicSources["special2"][(int)baseTerrain.Type];
                    }

                    // Give grassland's special a custom name and icon
                    if (pedia.Id > 0 && names[pedia.Id] == names[pedia.Id - 1])
                    {
                        //_headerText += $" ({Labels.For(LabelIndex.Shield)})";
                        icons[0] = active.PicSources["base1"][0];
                        icons[1] = active.PicSources["shield"][0];
                    }

                    icon = new ImageBox(window, new(icons, 2f), true)
                    {
                        Location = new(12, 7)
                    };
                    Controls.Add(icon);

                    prereqLabel = new PediaLabel(window, Labels.For(LabelIndex.TerrainType) + ":  ",
                        (int)icon.Location.X + icon.Width + 54, 0);
                    prereqLabel.Location = new(prereqLabel.Location.X, icon.Location.Y + (icon.Height - prereqLabel.Height) / 2);
                    Controls.Add(prereqLabel);

                    var preqLabel = new PediaLinkLabel(window, baseTerrain.Name, (int)prereqLabel.Location.X + prereqLabel.Width,
                        (int)prereqLabel.Location.Y);
                    Controls.Add(preqLabel);

                    // Food, shield, trade
                    offsetY = 105;
                    var foodLabel = new PediaLabel(window, $"{Labels.For(LabelIndex.Food)}:", 11, (int)offsetY);
                    Controls.Add(foodLabel);
                    var food = new PediaLabel(window, $"{s.Food} ", 200, (int)offsetY);
                    Controls.Add(food);
                    var foodIcon = new ImageBox(window, new(active.ResourceImages.First(i => i.Name == "Food").LargeImage), true);
                    foodIcon.Location = new((int)food.Location.X + food.Width,
                        (int)offsetY + (food.Height - foodIcon.Height) / 2);
                    Controls.Add(foodIcon);

                    offsetY += foodLabel.Height + 1;
                    var shieldsLabel = new PediaLabel(window, $"{Labels.For(LabelIndex.Shields)}:", 11, (int)offsetY);
                    Controls.Add(shieldsLabel);
                    var shields = new PediaLabel(window, $"{s.Shields} ", 200, (int)offsetY);
                    Controls.Add(shields);
                    var shldIcon = new ImageBox(window, new(active.ResourceImages.First(i => i.Name == "Shields").LargeImage), true);
                    shldIcon.Location = new((int)shields.Location.X + shields.Width,
                        (int)offsetY + (shields.Height - shldIcon.Height) / 2);
                    Controls.Add(shldIcon);

                    offsetY += shieldsLabel.Height + 1;
                    var tradeLabel = new PediaLabel(window, $"{Labels.For(LabelIndex.Trade)}:", 11, (int)offsetY);
                    Controls.Add(tradeLabel);
                    var trade = new PediaLabel(window, $"{s.Trade} ", 200, (int)offsetY);
                    Controls.Add(trade);
                    var trdIcon = new ImageBox(window, new(active.ResourceImages.First(i => i.Name == "Trade").LargeImage), true);
                    trdIcon.Location = new((int)trade.Location.X + trade.Width,
                        (int)offsetY + (trade.Height - trdIcon.Height) / 2);
                    Controls.Add(trdIcon);

                    // Bottom texts
                    offsetY = 200;
                    if (s.Food < baseTerrain.Food)
                    {
                        Controls.Add(new PediaLabel(window, $"Decrease the amount of {Labels.For(LabelIndex.Food)} produced in " +
                            $"{baseTerrain.Name} Terrain from {baseTerrain.Food} to {s.Food}.", 11, (int)offsetY));
                    }
                    else if (s.Food > baseTerrain.Food)
                    {
                        Controls.Add(new PediaLabel(window, $"Increase the amount of {Labels.For(LabelIndex.Food)} produced in " +
                            $"{baseTerrain.Name} Terrain from {baseTerrain.Food} to {s.Food}.", 11, (int)offsetY));
                    }
                    else
                    {
                        Controls.Add(new PediaLabel(window, $"There is no increase in {Labels.For(LabelIndex.Food)} production.", 11, (int)offsetY));
                    }

                    offsetY += tradeLabel.Height + 1;
                    if (s.Shields < baseTerrain.Shields)
                    {
                        Controls.Add(new PediaLabel(window, $"Decrease {Labels.For(LabelIndex.Shield)}s from " +
                            $"{baseTerrain.Shields} to {s.Shields}, and", 11, (int)offsetY));
                    }
                    else if (s.Shields > baseTerrain.Shields)
                    {
                        Controls.Add(new PediaLabel(window, $"Increase {Labels.For(LabelIndex.Shield)}s from " +
                            $"{baseTerrain.Shields} to {s.Shields}, and", 11, (int)offsetY));
                    }
                    else
                    {
                        Controls.Add(new PediaLabel(window, $"There is no increase in {Labels.For(LabelIndex.Shield)}s, and",
                            11, (int)offsetY));
                    }

                    offsetY += tradeLabel.Height + 1;
                    if (s.Trade < baseTerrain.Trade)
                    {
                        Controls.Add(new PediaLabel(window, $"decreases {Labels.For(LabelIndex.Trade)} from " +
                            $"{baseTerrain.Trade} to {s.Trade}.", 11, (int)offsetY));
                    }
                    else if (s.Trade > baseTerrain.Trade)
                    {
                        Controls.Add(new PediaLabel(window, $"increases {Labels.For(LabelIndex.Trade)} from " +
                            $"{baseTerrain.Trade} to {s.Trade}.", 11, (int)offsetY));
                    }
                    else
                    {
                        Controls.Add(new PediaLabel(window, $"no increase in {Labels.For(LabelIndex.Trade)}.",
                            11, (int)offsetY));
                    }

                    preqLabel.Click += (_, _) =>
                    {
                        pedia.InfoType = CivilopediaInfoType.Terrains;
                        pedia.Id = terrains.IndexOf(baseTerrain);
                        window.UpdateControls();
                    };
                }

                break;

            case CivilopediaInfoType.Concepts:
                break;
            default: throw new NotImplementedException();
        }
    }
}