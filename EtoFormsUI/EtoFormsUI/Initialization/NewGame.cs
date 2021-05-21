using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.Events;
using Eto.Forms;

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

        internal static void Start(Main mainForm, bool customizeWorld)
        {
            var config = new GameInitializationConfig
                {CustomizeWorld = customizeWorld, Random = new Random(), RuleSet = SelectGameToStart(mainForm)};
            if (config.RuleSet == null)
            {
                mainForm.MainMenu();
            }
            else
            {
                Labels.UpdateLabels(config.RuleSet);
                MapImageLoader.LoadCities(config.RuleSet);
                config.PopUps = PopupBoxReader.LoadPopupBoxes(config.RuleSet.Root).Aggregate( new Dictionary<string, PopupBox>(), (boxes, box) =>
                {
                    boxes[box.Name] = box;
                    return boxes;
                });
                GetWorldSize(mainForm, config);
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
            if (worldSizeDialog.SelectedButton == "Custom")
            {
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

            SelectDifficultly(mainForm, config);
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

                config.SimplifiedCombat = false;
                config.FlatWorld = false;
                config.SelectComputerOpponents = false;
                config.AcceleratedStartup = 0;
                config.Bloodlust = false;
                config.DontRestartEliminatedPlayers = false;
                if (customizeRules)
                {
                    var customRulesDialog = new Civ2dialogV2(mainForm, config.PopUps["ADVANCED"], checkboxOptionState: new[] {false, false, false, false, false, false});
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
            var playerCiv = MakeCivilization(config, tribe);
            config.Civilizations = new List<Civilization>
            {
                playerCiv
            };
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
                                InitialValue = playerCiv.LeaderName,
                                Name = "LeaderName"
                            },
                            new()
                            {
                                index = 1,
                                InitialValue = playerCiv.TribeName,
                                Name = "Tribe"
                            },
                            new()
                            {
                                index = 2,
                                InitialValue = playerCiv.Adjective,
                                Name = "Adjective"
                            }
                        });
                    customTribe.ShowModal(mainForm);
                    if (customTribe.SelectedIndex == int.MinValue)
                    {
                        SelectGender(mainForm, config);
                        return;
                    }

                    playerCiv.LeaderName = customTribe.TextValues["LeaderName"];
                    playerCiv.Adjective = customTribe.TextValues["Adjective"];
                    playerCiv.TribeName = customTribe.TextValues["Tribe"];
                    if (customTribe.SelectedButton == "Titles")
                    {
                        var titlesPop = config.PopUps["CUSTOMTRIBE2"];
                        titlesPop.Text = config.Rules.Governments.Select(g => g.Name + ": ").ToList();
                        
                        if (!titlesPop.Button.Contains(Labels.Cancel))
                        {
                            titlesPop.Button.Add(Labels.Cancel);
                        }
                        var customTitles = new Civ2dialogV2(mainForm, titlesPop, textBoxes: playerCiv.Titles.Select(
                            ((s, i) => new TextBoxDefinition
                            {
                                index = i,
                                Name = config.Rules.Governments[i].Name,
                                InitialValue = s
                            })).ToList());
                        customTitles.ShowModal(mainForm);
                        if (customTitles.SelectedButton == Labels.Ok)
                        {
                            playerCiv.Titles = config.Rules.Governments.Select(g => customTitles.TextValues[g.Name])
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
                        InitialValue = playerCiv.LeaderName,
                        Name = "LeaderName"
                    }
                });
                nameDialog.ShowModal(mainForm);
                if (nameDialog.SelectedButton == Labels.Cancel)
                {
                    SelectGender(mainForm, config);
                    return;
                }

                playerCiv.LeaderName = nameDialog.TextValues["LeaderName"];
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
                    MapImageLoader.Cities[7].Bitmap, MapImageLoader.Cities[15].Bitmap, 
                    MapImageLoader.Cities[23].Bitmap, MapImageLoader.Cities[31].Bitmap
                });
            citiesDialog.SelectedIndex = (int)config.Civilizations[0].CityStyle;
            citiesDialog.ShowModal(mainForm);
            if (citiesDialog.SelectedIndex == int.MinValue)
            {
                SelectGender(mainForm, config);
                return;
            }

            config.Civilizations[0].CityStyle = (CityStyleType) citiesDialog.SelectedIndex;
            
            //TODO: Start game...
        }

        private static Civilization MakeCivilization(GameInitializationConfig config, LeaderDefaults tribe)
        {
            var titles = config.Rules.Governments.Select((g, i) => GetLeaderTitle(config, tribe, g, i)).ToArray();
            return new Civilization
            {
                Adjective = tribe.Adjective,
                Alive = true,
                Government = GovernmentType.Despotism,
                Id = 0,
                Money = 0,
                Techs = new bool[config.Rules.Advances.Length],
                CityStyle = (CityStyleType) tribe.CityStyle,
                LeaderGender = config.Gender,
                LeaderName = config.Gender == 0 ? tribe.NameMale : tribe.NameFemale,
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