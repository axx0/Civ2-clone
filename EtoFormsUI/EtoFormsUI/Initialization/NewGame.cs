using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.Events;
using Civ2engine.IO;
using Eto.Drawing;
using Eto.Forms;
using EtoFormsUI.ImageLoader;

namespace EtoFormsUI.Initialization
{
    public static class NewGame
    {
        private static Ruleset SelectGameToStart(Main main)
        {
            var rulesFiles = Helpers.LocateRules(Settings.SearchPaths);
            switch (rulesFiles.Count)
            {
                case 0:
                    var warningDialog = new Civ2dialogV2(main, new PopupBox()
                    {
                        Title = "No game to start",
                        Text = new List<string>
                            {"Please check you search paths", "Could not locate a version of civ2 to start"},
                        Button = new List<string> {"OK"}
                    });
                    warningDialog.ShowModal(main);
                    return null;
                case 1:
                    return rulesFiles[0];
                default:
                    var popupBox = new Civ2dialogV2(main,
                        new PopupBox
                        {
                            Title = "Select game version", Options = rulesFiles.Select(f => f.Name).ToList(),
                            Button = new List<string> {"OK",Labels.Cancel}
                        });
                    popupBox.ShowModal(main);

                    return popupBox.SelectedIndex == int.MinValue ? null : rulesFiles[popupBox.SelectedIndex];
            }
        }

        internal static bool StartPremade(Main mainForm, Ruleset ruleset, string mapFileName)
        {
            Labels.UpdateLabels(ruleset);
            CityLoader.LoadCities(ruleset);
            var config = new GameInitializationConfig {RuleSet = ruleset};
            config.PopUps = PopupBoxReader.LoadPopupBoxes(config.RuleSet.Root).Aggregate( new Dictionary<string, PopupBox>(), (boxes, box) =>
            {
                boxes[box.Name] = box;
                return boxes;
            });
            try
            {
                PopupBox CorrectedPopup(string popupId)
                {
                    var popUp = config.PopUps[popupId];
                    if (popUp.Options != null && popUp.Options.Count != 0) return popUp;
                    popUp.Options = new[] {popUp.Text[^2], popUp.Text[^1]};
                    popUp.Text = popUp.Text.Where(t => !popUp.Options.Contains(t)).ToArray();

                    return popUp;
                }

                var mapData = MapReader.Read(ruleset, mapFileName);

                if (mapData.ResourceSeed > 1)
                {
                    var resourceSeedDialog = new Civ2dialogV2(mainForm, CorrectedPopup("USESEED"));
                    resourceSeedDialog.ShowModal(mainForm);

                    if (resourceSeedDialog.SelectedIndex == 1)
                    {
                        config.ResourceSeed = mapData.ResourceSeed;
                    }
                }
                
                if (mapData.StartPositions.Any(p=>p.First != -1 && p.Second != -1))
                {
                    
                    var startPositions = new Civ2dialogV2(mainForm, CorrectedPopup("USESTARTLOC"));
                    startPositions.ShowModal(mainForm);

                    if (startPositions.SelectedIndex == 1)
                    {
                        config.StartPositions = mapData.StartPositions.Select(p=> new Point(p.First, p.Second)).ToArray();
                    }
                }

                config.FlatWorld = mapData.FlatWorld;
                config.WorldSize = new[] {mapData.Width, mapData.Height};
                config.TerrainData = mapData.TerrainData;
                config.MapArea = mapData.Area;
                SelectDifficultly(mainForm, config);
            }
            catch
            {
                var failedToLoad = new Civ2dialogV2(mainForm, config.PopUps["FAILEDTOLOAD"]);
                failedToLoad.ShowModal(mainForm);
                return false;
            }
            
            return false;
        }

        internal static void Start(Main mainForm, bool customizeWorld)
        {
            var config = new GameInitializationConfig
                { RuleSet = SelectGameToStart(mainForm)};
            if (config.RuleSet == null)
            {
                mainForm.MainMenu();
            }
            else
            {
                Labels.UpdateLabels(config.RuleSet);
                CityLoader.LoadCities(config.RuleSet);
                
                config.PopUps = PopupBoxReader.LoadPopupBoxes(config.RuleSet.Root).Aggregate( new Dictionary<string, PopupBox>(), (boxes, box) =>
                {
                    boxes[box.Name] = box;
                    return boxes;
                });

                
                GetWorldSize(mainForm, config);
                if (customizeWorld)
                {
                    
                }
                
                
                SelectDifficultly(mainForm, config);
            }
        }

        private static void GetWorldSize(Main mainForm, GameInitializationConfig config)
        {
            var worldSizeDialog = new Civ2dialogV2(mainForm, config.PopUps["SIZEOFMAP"]);

            worldSizeDialog.ShowModal(mainForm);
            if (worldSizeDialog.SelectedIndex == int.MinValue)
            {
                mainForm.MainMenu();
                return;
            }

            config.WorldSize = worldSizeDialog.SelectedIndex switch
            {
                1 => new[] {50, 80},
                2 => new[] {75, 120},
                _ => new[] {40, 50}
            };
            
            if (worldSizeDialog.SelectedButton != "Custom") return;
            
            var customSize = new Civ2dialogV2(mainForm, config.PopUps["CUSTOMSIZE"],
                textBoxes: new List<TextBoxDefinition>
                {
                    new()
                    {
                        index = 3, Name = "Width", MinValue = 20, InitialValue = config.WorldSize[0].ToString()
                    },
                    new()
                    {
                        index = 4, Name = "Height", MinValue = 20, InitialValue = config.WorldSize[1].ToString()
                    }
                });

            customSize.ShowModal(mainForm);
            if (int.TryParse(customSize.TextValues["Width"], out var width))
            {
                config.WorldSize[0] = width;
            }

            if (int.TryParse(customSize.TextValues["Height"], out var height))
            {
                config.WorldSize[1] = height;
            }
        }

        private static void SelectDifficultly(Main mainForm, GameInitializationConfig config)
        {
            var difficultyDialog = new Civ2dialogV2(mainForm, config.PopUps["DIFFICULTY"]);
            difficultyDialog.ShowModal(mainForm);

            if (difficultyDialog.SelectedIndex == int.MinValue)
            {
                mainForm.MainMenu();
                return;
            }

            config.DifficultlyLevel = difficultyDialog.SelectedIndex;
            SelectNumberOfCivs(mainForm, config);
        }

        private static void SelectNumberOfCivs(Main mainForm, GameInitializationConfig config)
        {
            var numberOfCivsDialog = new Civ2dialogV2(mainForm, config.PopUps["ENEMIES"]);
            numberOfCivsDialog.ShowModal(mainForm);

            if (numberOfCivsDialog.SelectedIndex == int.MinValue)
            {
                SelectDifficultly(mainForm, config);
                return;
            }


            config.NumberOfCivs = 7 - (numberOfCivsDialog.SelectedButton == "Random"
                ? config.Random.Next(0, 5)
                : numberOfCivsDialog.SelectedIndex);

            SelectBarbarity(mainForm, config);
        }

        private static void SelectBarbarity(Main mainForm, GameInitializationConfig config)
        {
            var barbarityDialog = new Civ2dialogV2(mainForm, config.PopUps["BARBARITY"]);
            barbarityDialog.ShowModal(mainForm);

            if (barbarityDialog.SelectedIndex == int.MinValue)
            {
                SelectDifficultly(mainForm, config);
                return;
            }

            config.BarbarianActivity = (barbarityDialog.SelectedButton == "Random"
                ? config.Random.Next(0, 4)
                : barbarityDialog.SelectedIndex);

            SelectCustomizeRules(mainForm, config);
        }

        private static void SelectCustomizeRules(Main mainForm, GameInitializationConfig config)
        {
            while (true)
            {
                var rulesDialog = new Civ2dialogV2(mainForm, config.PopUps["RULES"]);
                rulesDialog.ShowModal(mainForm);

                if (rulesDialog.SelectedIndex == int.MinValue)
                {
                    mainForm.MainMenu();
                    return;
                }

                var customizeRules = rulesDialog.SelectedIndex == 1;

                if (customizeRules)
                {
                    var customRulesDialog = new Civ2dialogV2(mainForm, config.PopUps["ADVANCED"], checkboxOptionState: new[] { config.SimplifiedCombat, config.FlatWorld, config.SelectComputerOpponents,config.AcceleratedStartup > 0, config.Bloodlust, config.DontRestartEliminatedPlayers});
                    customRulesDialog.ShowModal(mainForm);

                    if (customRulesDialog.SelectedIndex != int.MinValue)
                    {
                        config.SimplifiedCombat = customRulesDialog.CheckboxReturnStates[0];
                        config.FlatWorld = customRulesDialog.CheckboxReturnStates[1];
                        config.SelectComputerOpponents = customRulesDialog.CheckboxReturnStates[2];
                        config.Bloodlust = customRulesDialog.CheckboxReturnStates[4];
                        config.DontRestartEliminatedPlayers = customRulesDialog.CheckboxReturnStates[5];
                        if (customRulesDialog.CheckboxReturnStates[3])
                        {
                            var startYearDialog = new Civ2dialogV2(mainForm, config.PopUps["ACCELERATED"], new List<string> {"4000", "3000", "2000"});
                            startYearDialog.ShowModal(mainForm);

                            if (startYearDialog.SelectedIndex == int.MinValue)
                            {
                                continue;
                            }

                            config.AcceleratedStartup = startYearDialog.SelectedIndex;
                        }
                    }
                }


                SelectGender(mainForm, config);
                break;
            }
        }

        private static void SelectGender(Main mainForm, GameInitializationConfig config)
        {

            var genderDialog = new Civ2dialogV2(mainForm, config.PopUps["GENDER"]);
            genderDialog.ShowModal(mainForm);
            if (genderDialog.SelectedIndex == int.MinValue)
            {
                SelectDifficultly(mainForm, config);
                return;
            }
            config.Gender = genderDialog.SelectedIndex;

            SelectTribe(mainForm, config);
        }

        private static void SelectTribe(Main mainForm, GameInitializationConfig config)
        {
            config.Rules = RulesParser.ParseRules(config.RuleSet);
            var popup = config.PopUps["TRIBE"];
            if (!popup.Button.Contains(Labels.Cancel))
            {
                popup.Button.Add(Labels.Cancel);
            }
            popup.Options = config.Rules.Leaders.Select(l => l.Plural).ToList();
            var tribeDialog = new Civ2dialogV2(mainForm, popup, optionsCols: 3);
            tribeDialog.ShowModal(mainForm);

            if (tribeDialog.SelectedIndex == int.MinValue)
            {
                SelectGender(mainForm, config);
                return;
            }

            var tribe = config.Rules.Leaders[tribeDialog.SelectedIndex];
            config.PlayerCiv = MakeCivilization(config, tribe, true, tribe.PreferredColour);
            
            if (tribeDialog.SelectedButton == Labels.Custom)
            {
                var tribePopup = config.PopUps["CUSTOMTRIBE"];
                if (tribePopup.Text == null)
                {
                    tribePopup.Text = tribePopup.Options;
                    tribePopup.Options = null;
                }

                while (true)
                {
                    var customTribe = new Civ2dialogV2(mainForm, tribePopup,
                        textBoxes: new List<TextBoxDefinition>
                        {
                            new()
                            {
                                index = 0,
                                InitialValue = config.PlayerCiv.LeaderName,
                                Name = "LeaderName"
                            },
                            new()
                            {
                                index = 1,
                                InitialValue = config.PlayerCiv.TribeName,
                                Name = "Tribe"
                            },
                            new()
                            {
                                index = 2,
                                InitialValue = config.PlayerCiv.Adjective,
                                Name = "Adjective"
                            }
                        });
                    customTribe.ShowModal(mainForm);
                    if (customTribe.SelectedIndex == int.MinValue)
                    {
                        SelectGender(mainForm, config);
                        return;
                    }

                    config.PlayerCiv.LeaderName = customTribe.TextValues["LeaderName"];
                    config.PlayerCiv.Adjective = customTribe.TextValues["Adjective"];
                    config.PlayerCiv.TribeName = customTribe.TextValues["Tribe"];
                    if (customTribe.SelectedButton == "Titles")
                    {
                        var titlesPop = config.PopUps["CUSTOMTRIBE2"];
                        titlesPop.Text = config.Rules.Governments.Select(g => g.Name + ": ").ToList();
                        
                        if (!titlesPop.Button.Contains(Labels.Cancel))
                        {
                            titlesPop.Button.Add(Labels.Cancel);
                        }
                        var customTitles = new Civ2dialogV2(mainForm, titlesPop, textBoxes: config.PlayerCiv.Titles.Select(
                            ((s, i) => new TextBoxDefinition
                            {
                                index = i,
                                Name = config.Rules.Governments[i].Name,
                                InitialValue = s
                            })).ToList());
                        customTitles.ShowModal(mainForm);
                        if (customTitles.SelectedButton == Labels.Ok)
                        {
                            config.PlayerCiv.Titles = config.Rules.Governments.Select(g => customTitles.TextValues[g.Name])
                                .ToArray();
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                var namePopup = config.PopUps["NAME"];
                if (namePopup.Text == null)
                {
                    namePopup.Text = namePopup.Options;
                    namePopup.Options = null;
                }

                var nameDialog = new Civ2dialogV2(mainForm, namePopup, textBoxes: new List<TextBoxDefinition>
                {
                    new()
                    {
                        index = 0,
                        InitialValue = config.PlayerCiv.LeaderName,
                        Name = "LeaderName"
                    }
                });
                nameDialog.ShowModal(mainForm);
                if (nameDialog.SelectedButton == Labels.Cancel)
                {
                    SelectGender(mainForm, config);
                    return;
                }

                config.PlayerCiv.LeaderName = nameDialog.TextValues["LeaderName"];
            }
            
            SelectCityStyle(mainForm, config);
        }

        private static void SelectCityStyle(Main mainForm, GameInitializationConfig config)
        {

            var citiesPopup = config.PopUps["CUSTOMCITY"];
            citiesPopup.Options ??= Labels.Items[247..251];

            if (citiesPopup.Button.IndexOf(Labels.Cancel) == -1)
            {
                citiesPopup.Button.Add(Labels.Cancel);
            }

            var citiesDialog = new Civ2dialogV2(mainForm, citiesPopup,
                icons: new[]
                {
                    MapImages.Cities[7].Bitmap, MapImages.Cities[15].Bitmap, MapImages.Cities[23].Bitmap,
                    MapImages.Cities[31].Bitmap
                }) {SelectedIndex = (int) config.PlayerCiv.CityStyle};
            citiesDialog.ShowModal(mainForm);
            if (citiesDialog.SelectedIndex == int.MinValue)
            {
                SelectGender(mainForm, config);
                return;
            }

            config.PlayerCiv.CityStyle = (CityStyleType) citiesDialog.SelectedIndex;
            var groupedTribes = config.Rules.Leaders
                .ToLookup(g => g.PreferredColour);

   
            var civilizations = new List<Civilization>
            {
                new () {Adjective = Labels.Items[17], LeaderName = Labels.Items[18], Alive = true, Id = 0},
                config.PlayerCiv
            };
            if (config.SelectComputerOpponents)
            {  
                var opponentPop = config.PopUps["OPPONENT"];
                for (var i = 1; i < groupedTribes.Count && civilizations.Count +1  <= config.NumberOfCivs ; i++)
                {
                    if (i == config.PlayerCiv.Id) continue;
                    
                    if (i == config.PlayerCiv.Id) continue;

                    var group = groupedTribes.Contains(i)
                        ? groupedTribes[i].ToList()
                        : config.Rules.Leaders
                            .Where(leader => civilizations.All(civ => civ.Adjective != leader.Adjective)).ToList();
                    
                    opponentPop.Options =
                        new[] {opponentPop.Options[0]}.Concat(@group.Select(leader => $"{leader.Plural} ({(leader.Female ? leader.NameFemale : leader.NameMale)})")).ToList();
                    var oppDia = new Civ2dialogV2(mainForm, opponentPop, new List<string>() {(i + 1).ToString()},optionsCols: group.Count / 6);
                    oppDia.ShowModal(mainForm);

                    if (oppDia.SelectedIndex == int.MinValue)
                    {
                        SelectCityStyle(mainForm, config);
                        return;
                    }
                    civilizations.Add(MakeCivilization(config,
                            @group[
                                oppDia.SelectedIndex == 0
                                    ? config.Random.Next(0, @group.Count)
                                    : oppDia.SelectedIndex - 1], false, i +1));
                }
            }
            else
            {
                for (var i = 1; civilizations.Count + 1 <= config.NumberOfCivs; i++)
                {
                    if (i == config.PlayerCiv.Id) continue;

                    var group = groupedTribes.Count < i && groupedTribes[i].Any()
                        ? groupedTribes[i].ToList()
                        : config.Rules.Leaders
                            .Where(leader => civilizations.All(civ => civ.Adjective != leader.Adjective)).ToList();

                    civilizations.Add(MakeCivilization(config, @group[config.Random.Next(0, @group.Count)], false,
                        i + 1));
                }
            }

            //TODO: Start game...
            Game.Create(config.Rules, new GameData
            {
                MapArea = config.MapArea,
                MapXdim = config.WorldSize[0],
                MapYdim = config.WorldSize[1],
                Options = new Options
                {
                    
                },
                Civilizations = civilizations.OrderBy(C => C.Id).ToList()
            });
        }

        private static Civilization MakeCivilization(GameInitializationConfig config, LeaderDefaults tribe, bool human, int id)
        {
            var titles = config.Rules.Governments.Select((g, i) => GetLeaderTitle(config, tribe, g, i)).ToArray();
            var gender = human ? config.Gender : tribe.Female ? 1 : 0;
            return new Civilization
            {
                Adjective = tribe.Adjective,
                Alive = true,
                Government = GovernmentType.Despotism,
                Id = id,
                Money = 0,
                Techs = new bool[config.Rules.Advances.Length],
                CityStyle = (CityStyleType) tribe.CityStyle,
                LeaderGender =gender ,
                LeaderName = gender == 0 ? tribe.NameMale : tribe.NameFemale,
                LeaderTitle = titles[(int)GovernmentType.Despotism],
                LuxRate = 0,
                ScienceRate = 60,
                TaxRate = 40,
                TribeName = tribe.Plural,
                Titles = titles
            };
        }

        private static string GetLeaderTitle(GameInitializationConfig config, LeaderDefaults tribe, Government gov, int governmentType)
        {
            var govt = tribe.Titles.FirstOrDefault(t=>t.Gov == governmentType) ?? (IGovernmentTitles)gov;
            return config.Gender == 0 ? govt.TitleMale : govt.TitleFemale;
        }
    }
}