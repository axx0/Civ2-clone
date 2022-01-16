using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.Events;
using Civ2engine.IO;
using Civ2engine.NewGame;
using Civ2engine.Units;
using Eto.Drawing;
using Eto.Forms;
using EtoFormsUI.ImageLoader;

namespace EtoFormsUI.Initialization
{
    public static class NewGame
    {
        static PicturePanel stPeterburgPanel, mingGeneralPanel, anicientPersonsPanel, barbariansPanel, galleyPanel, peoplePanel1, 
            peoplePanel2, templePanel, islandPanel, desertPanel, snowPanel, canyonPanel;
        static byte[] bytes;

        private static Ruleset SelectGameToStart(Main main)
        {
            var rulesFiles = Helpers.LocateRules(Settings.SearchPaths);
            switch (rulesFiles.Count)
            {
                case 0:
                    var warningDialog = new Civ2dialog(main, new PopupBox()
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
                    var popupBox = new Civ2dialog(main,
                        new PopupBox
                        {
                            Title = "Select game version", Options = rulesFiles.Select(f => f.Name).ToList(),
                            Button = new List<string> {"OK",Labels.Cancel}
                        });
                    popupBox.ShowModal(main);

                    return popupBox.SelectedIndex == int.MinValue ? null : rulesFiles[popupBox.SelectedIndex];
            }
        }

        internal static bool StartPreMade(Main mainForm, Ruleset ruleset, string mapFileName)
        {
            Labels.UpdateLabels(ruleset);
            CityLoader.LoadCities(ruleset);
            var config = new GameInitializationConfig {RuleSet = ruleset};
            config.PopUps = PopupBoxReader.LoadPopupBoxes(config.RuleSet.Root);
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
                    var resourceSeedDialog = new Civ2dialog(mainForm, CorrectedPopup("USESEED"));
                    resourceSeedDialog.ShowModal(mainForm);

                    if (resourceSeedDialog.SelectedIndex == 1)
                    {
                        config.ResourceSeed = mapData.ResourceSeed % 64;
                    }
                }
                
                if (mapData.StartPositions.Any(p=>p.First != -1 && p.Second != -1))
                {
                    
                    var startPositions = new Civ2dialog(mainForm, CorrectedPopup("USESTARTLOC"));
                    startPositions.ShowModal(mainForm);

                    if (startPositions.SelectedIndex == 1)
                    {
                        config.StartPositions = mapData.StartPositions.Select(p=> new int[] {p.First, p.Second}).ToArray();
                    }
                }

                config.FlatWorld = mapData.FlatWorld;
                config.WorldSize = new[] {mapData.Width /2, mapData.Height};
                config.TerrainData = mapData.TerrainData;
            }
            catch
            {
                var failedToLoad = new Civ2dialog(mainForm, config.PopUps["FAILEDTOLOAD"]);
                failedToLoad.ShowModal(mainForm);
                return false;
            }
            
            SelectDifficultly(mainForm, config);
            
            return config.Started;
        }

        private static int CustomWorldDialog(Main mainForm, PopupBox configPopUp, GameInitializationConfig config)
        {
            var dialog = new Civ2dialog(mainForm, configPopUp);
            dialog.Location = new Point((int)(Screen.PrimaryScreen.Bounds.Width * 0.5 - dialog.Width / 2),
                                        (int)(Screen.PrimaryScreen.Bounds.Height - dialog.Height - 75));

            dialog.ShowModal(mainForm);

            return dialog.SelectedButton == configPopUp.Button[0] ? config.Random.Next(configPopUp.Options.Count) : dialog.SelectedIndex;
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

                config.PopUps = PopupBoxReader.LoadPopupBoxes(config.RuleSet.Root);

                
                GetWorldSize(mainForm, config);
                if (customizeWorld)
                {
                    var configPopUp = config.PopUps["CUSTOMLAND"];

                    config.PropLand = CustomWorldDialog(mainForm, configPopUp, config);
                    if (config.PropLand == int.MinValue)
                    {
                        stPeterburgPanel.Dispose();
                        mainForm.MainMenu();
                        return;
                    }

                    stPeterburgPanel.Dispose();
                    islandPanel = new PicturePanel(Images.ExtractBitmap(bytes, "islandPic"));
                    mainForm.layout.Add(islandPanel, new Point((int)(Screen.PrimaryScreen.Bounds.Width / 2 - islandPanel.Width / 2), 76));
                    configPopUp = config.PopUps["CUSTOMFORM"];

                    config.Landform = CustomWorldDialog(mainForm, configPopUp, config);
                    if (config.Landform == int.MinValue)
                    {
                        islandPanel.Dispose();
                        mainForm.MainMenu();
                        return;
                    }

                    islandPanel.Dispose();
                    desertPanel = new PicturePanel(Images.ExtractBitmap(bytes, "desertPic"));
                    mainForm.layout.Add(desertPanel, new Point((int)(Screen.PrimaryScreen.Bounds.Width / 2 - desertPanel.Width / 2), 76));
                    configPopUp = config.PopUps["CUSTOMCLIMATE"];
                    config.Climate = CustomWorldDialog(mainForm, configPopUp, config);
                    if (config.Climate == int.MinValue)
                    {
                        desertPanel.Dispose();
                        mainForm.MainMenu();
                        return;
                    }

                    desertPanel.Dispose();
                    snowPanel = new PicturePanel(Images.ExtractBitmap(bytes, "snowPic"));
                    mainForm.layout.Add(snowPanel, new Point((int)(Screen.PrimaryScreen.Bounds.Width / 2 - snowPanel.Width / 2), 76));
                    configPopUp = config.PopUps["CUSTOMTEMP"];

                    config.Temperature = CustomWorldDialog(mainForm, configPopUp, config);
                    if (config.Temperature == int.MinValue)
                    {
                        snowPanel.Dispose();
                        mainForm.MainMenu();
                        return;
                    }

                    snowPanel.Dispose();
                    canyonPanel = new PicturePanel(Images.ExtractBitmap(bytes, "canyonPic"));
                    mainForm.layout.Add(canyonPanel, new Point((int)(Screen.PrimaryScreen.Bounds.Width / 2 - canyonPanel.Width / 2), 76));
                    configPopUp = config.PopUps["CUSTOMAGE"];

                    config.Age = CustomWorldDialog(mainForm, configPopUp, config);
                    if (config.Age == int.MinValue)
                    {
                        canyonPanel.Dispose();
                        mainForm.MainMenu();
                        return;
                    }
                    canyonPanel.Dispose();
                }

                if (!stPeterburgPanel.IsDisposed) stPeterburgPanel.Dispose();
                SelectDifficultly(mainForm, config);
            }
        }

        private static void GetWorldSize(Main mainForm, GameInitializationConfig config)
        {
            bytes = File.ReadAllBytes(Settings.Civ2Path + "Intro.dll");

            stPeterburgPanel = new PicturePanel(Images.ExtractBitmap(bytes, "stPeterburgPic"));
            mainForm.layout.Add(stPeterburgPanel, new Point((int)(Screen.PrimaryScreen.Bounds.Width / 2 - stPeterburgPanel.Width / 2), 120));

            while (true)
            {
                var worldSizeDialog = new Civ2dialog(mainForm, config.PopUps["SIZEOFMAP"]);
                worldSizeDialog.Location = new Point((int)(Screen.PrimaryScreen.Bounds.Width * 0.5 - worldSizeDialog.Width / 2),
                                                     (int)(Screen.PrimaryScreen.Bounds.Height - worldSizeDialog.Height - 75));

                worldSizeDialog.ShowModal(mainForm);
                if (worldSizeDialog.SelectedIndex == int.MinValue)
                {
                    stPeterburgPanel.Dispose();
                    mainForm.MainMenu();
                    return;
                }

                config.WorldSize = worldSizeDialog.SelectedIndex switch
                {
                    1 => new[] { 50, 80 },
                    2 => new[] { 75, 120 },
                    _ => new[] { 40, 50 }
                };

                if (worldSizeDialog.SelectedButton != "Custom") return;

                var customsizePopup = config.PopUps["CUSTOMSIZE"];
                if (!customsizePopup.Button.Contains(Labels.Cancel)) customsizePopup.Button.Add(Labels.Cancel);
                var customSizeDialog = new Civ2dialog(mainForm, customsizePopup,
                    textBoxes: new List<TextBoxDefinition>
                    {
                    new()
                    {
                        index = 3, Name = "Width", MinValue = 20, InitialValue = config.WorldSize[0].ToString(), Width = 75
                    },
                    new()
                    {
                        index = 4, Name = "Height", MinValue = 20, InitialValue = config.WorldSize[1].ToString(), Width = 75
                    }
                    });
                customSizeDialog.Location = new Point((int)(Screen.PrimaryScreen.Bounds.Width * 0.5 - customSizeDialog.Width / 2),
                                                      (int)(Screen.PrimaryScreen.Bounds.Height - customSizeDialog.Height - 75));
                customSizeDialog.ShowModal(mainForm);
                
                if (int.TryParse(customSizeDialog.TextValues["Width"], out var width))
                {
                    config.WorldSize[0] = width;
                }

                if (int.TryParse(customSizeDialog.TextValues["Height"], out var height))
                {
                    config.WorldSize[1] = height;
                }

                if (customSizeDialog.SelectedButton != "Cancel") break;
            }
        }

        private static void SelectDifficultly(Main mainForm, GameInitializationConfig config)
        {
            config.Rules = RulesParser.ParseRules(config.RuleSet);

            mingGeneralPanel = new PicturePanel(Images.ExtractBitmap(bytes, "mingGeneralPic"));
            mainForm.layout.Add(mingGeneralPanel, new Point((int)Screen.PrimaryScreen.Bounds.Width - 156 - mingGeneralPanel.Width, 76));

            var difficultyDialog = new Civ2dialog(mainForm, config.PopUps["DIFFICULTY"]);
            difficultyDialog.Location = new Point(160, (int)(Screen.PrimaryScreen.Bounds.Height - difficultyDialog.Height - 72));
            difficultyDialog.ShowModal(mainForm);
            if (difficultyDialog.SelectedIndex == int.MinValue)
            {
                mingGeneralPanel.Dispose();
                mainForm.MainMenu();
                return;
            }

            config.DifficultlyLevel = difficultyDialog.SelectedIndex;
            mingGeneralPanel.Dispose();

            SelectNumberOfCivs(mainForm, config);
        }

        private static void SelectNumberOfCivs(Main mainForm, GameInitializationConfig config)
        {
            anicientPersonsPanel = new PicturePanel(Images.ExtractBitmap(bytes, "ancientPersonsPic"));
            mainForm.layout.Add(anicientPersonsPanel, new Point(160, 76));

            var enemiesPopup = config.PopUps["ENEMIES"];
            var possibleCivs = MapImages.PlayerColours.Length - 1;

            if (enemiesPopup.Options.Count + 2 != possibleCivs)
            {
                var suffix = enemiesPopup.Options[0].Split(" ", 2,
                    StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)[1];
                enemiesPopup.Options = Enumerable.Range(0, possibleCivs - 2)
                    .Select(v => $"{(possibleCivs - v)} {suffix}").ToArray();
            }
            var numberOfCivsDialog = new Civ2dialog(mainForm, enemiesPopup);

            numberOfCivsDialog.Location = new Point((int)(Screen.PrimaryScreen.Bounds.Width - numberOfCivsDialog.Width - 156),
                                                    (int)(Screen.PrimaryScreen.Bounds.Height - numberOfCivsDialog.Height - 75));
            numberOfCivsDialog.ShowModal(mainForm);

            if (numberOfCivsDialog.SelectedIndex == int.MinValue)
            {
                anicientPersonsPanel.Dispose();
                SelectDifficultly(mainForm, config);
                return;
            }

            config.NumberOfCivs = possibleCivs - (numberOfCivsDialog.SelectedButton == "Random"
                ? config.Random.Next(possibleCivs -2)
                : numberOfCivsDialog.SelectedIndex);

            anicientPersonsPanel.Dispose();
            SelectBarbarity(mainForm, config);
        }

        private static void SelectBarbarity(Main mainForm, GameInitializationConfig config)
        {
            barbariansPanel = new PicturePanel(Images.ExtractBitmap(bytes, "barbariansPic"));
            mainForm.layout.Add(barbariansPanel, new Point((int)(Screen.PrimaryScreen.Bounds.Width - barbariansPanel.Width - 81), 76));

            var barbarityDialog = new Civ2dialog(mainForm, config.PopUps["BARBARITY"]);
            barbarityDialog.Location = new Point(161, (int)(Screen.PrimaryScreen.Bounds.Height - barbarityDialog.Height - 75));
            barbarityDialog.ShowModal(mainForm);

            if (barbarityDialog.SelectedIndex == int.MinValue)
            {
                barbariansPanel.Dispose();
                SelectDifficultly(mainForm, config);
                return;
            }

            config.BarbarianActivity = (barbarityDialog.SelectedButton == "Random"
                ? config.Random.Next(4)
                : barbarityDialog.SelectedIndex);

            barbariansPanel.Dispose();
            SelectCustomizeRules(mainForm, config);
        }

        private static void SelectCustomizeRules(Main mainForm, GameInitializationConfig config)
        {
            while (true)
            {
                galleyPanel = new PicturePanel(Images.ExtractBitmap(bytes, "galleyWreckPic"));
                mainForm.layout.Add(galleyPanel, new Point(161, 76));

                var rulesDialog = new Civ2dialog(mainForm, config.PopUps["RULES"]);
                rulesDialog.Location = new Point((int)(Screen.PrimaryScreen.Bounds.Width - rulesDialog.Width - 156), 
                                                 (int)(Screen.PrimaryScreen.Bounds.Height - rulesDialog.Height - 72));
                rulesDialog.ShowModal(mainForm);

                if (rulesDialog.SelectedIndex == int.MinValue)
                {
                    galleyPanel.Dispose();
                    mainForm.MainMenu();
                    return;
                }

                var customizeRules = rulesDialog.SelectedIndex == 1;

                if (customizeRules)
                {
                    mainForm.layout.Move(galleyPanel, new Point((int)(Screen.PrimaryScreen.Bounds.Width - galleyPanel.Width - 155), 76));

                    var customRulesDialog = new Civ2dialog(mainForm, config.PopUps["ADVANCED"], checkboxOptionState: new[] { config.SimplifiedCombat, config.FlatWorld, config.SelectComputerOpponents,config.AcceleratedStartup > 0, config.Bloodlust, config.DontRestartEliminatedPlayers});
                    customRulesDialog.Location = new Point(161, (int)(Screen.PrimaryScreen.Bounds.Height - customRulesDialog.Height - 72));
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
                            var startYearDialog = new Civ2dialog(mainForm, config.PopUps["ACCELERATED"], new List<string> {"4000", "3000", "2000"});
                            startYearDialog.ShowModal(mainForm);

                            if (startYearDialog.SelectedIndex == int.MinValue)
                            {
                                continue;
                            }

                            config.AcceleratedStartup = startYearDialog.SelectedIndex;
                        }
                    }
                }

                config.MapTask = MapGenerator.GenerateMap(config);

                galleyPanel.Dispose();
                SelectGender(mainForm, config);
                break;
            }
        }

        private static void SelectGender(Main mainForm, GameInitializationConfig config)
        {
            peoplePanel1 = new PicturePanel(Images.ExtractBitmap(bytes, "peoplePic1"));
            mainForm.layout.Add(peoplePanel1, new Point((int)(Screen.PrimaryScreen.Bounds.Width / 2 - peoplePanel1.Width / 2), 76));

            var genderDialog = new Civ2dialog(mainForm, config.PopUps["GENDER"]);
            genderDialog.Location = new Point((int)(Screen.PrimaryScreen.Bounds.Width / 2 - genderDialog.Width / 2), 
                                              (int)(Screen.PrimaryScreen.Bounds.Height - genderDialog.Height - 72));
            genderDialog.ShowModal(mainForm);
            if (genderDialog.SelectedIndex == int.MinValue)
            {
                peoplePanel1.Dispose();
                SelectDifficultly(mainForm, config);
                return;
            }
            config.Gender = genderDialog.SelectedIndex;

            peoplePanel1.Dispose();
            SelectTribe(mainForm, config);
        }

        private static void SelectTribe(Main mainForm, GameInitializationConfig config)
        {
            peoplePanel2 = new PicturePanel(Images.ExtractBitmap(bytes, "peoplePic2"));
            mainForm.layout.Add(peoplePanel2, new Point((int)(Screen.PrimaryScreen.Bounds.Width / 2 - peoplePanel2.Width / 2), 76));

            var popup = config.PopUps["TRIBE"];
            if (!popup.Button.Contains(Labels.Cancel))
            {
                popup.Button.Add(Labels.Cancel);
            }
            popup.Options = config.Rules.Leaders.Select(l => l.Plural).ToList();
            var tribeDialog = new Civ2dialog(mainForm, popup, optionsCols: 3);
            tribeDialog.Location = new Point((int)(Screen.PrimaryScreen.Bounds.Width / 2 - tribeDialog.Width / 2),
                                             (int)(Screen.PrimaryScreen.Bounds.Height - tribeDialog.Height - 72));
            tribeDialog.ShowModal(mainForm);

            if (tribeDialog.SelectedIndex == int.MinValue)
            {
                peoplePanel2.Dispose();
                SelectGender(mainForm, config);
                return;
            }

            var tribe = config.Rules.Leaders[tribeDialog.SelectedIndex];
            config.PlayerCiv = MakeCivilization(config, tribe, true, tribe.Color);
            
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
                    var customTribe = new Civ2dialog(mainForm, tribePopup,
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
                    customTribe.Location = new Point((int)(Screen.PrimaryScreen.Bounds.Width / 2 - customTribe.Width / 2),
                                                     (int)(Screen.PrimaryScreen.Bounds.Height - customTribe.Height - 72));
                    customTribe.ShowModal(mainForm);
                    if (customTribe.SelectedIndex == int.MinValue)
                    {
                        peoplePanel2.Dispose();
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
                        var customTitles = new Civ2dialog(mainForm, titlesPop, textBoxes: config.PlayerCiv.Titles.Select(
                            (s, i) => new TextBoxDefinition
                            {
                                index = i,
                                Name = config.Rules.Governments[i].Name,
                                InitialValue = s,
                            }).ToList());
                        customTitles.Location = new Point((int)(Screen.PrimaryScreen.Bounds.Width / 2 - customTitles.Width / 2),
                                                          (int)(Screen.PrimaryScreen.Bounds.Height - customTitles.Height - 72));
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

                var nameDialog = new Civ2dialog(mainForm, namePopup, textBoxes: new List<TextBoxDefinition>
                {
                    new()
                    {
                        index = 0,
                        InitialValue = config.PlayerCiv.LeaderName,
                        Name = "LeaderName"
                    }
                });
                nameDialog.Location = new Point((int)(Screen.PrimaryScreen.Bounds.Width / 2 - nameDialog.Width / 2),
                                                (int)(Screen.PrimaryScreen.Bounds.Height - nameDialog.Height - 72));
                nameDialog.ShowModal(mainForm);
                if (nameDialog.SelectedButton == Labels.Cancel)
                {
                    peoplePanel2.Dispose();
                    SelectGender(mainForm, config);
                    return;
                }

                config.PlayerCiv.LeaderName = nameDialog.TextValues["LeaderName"];
            }

            peoplePanel2.Dispose();
            SelectCityStyle(mainForm, config);
        }

        private static void SelectCityStyle(Main mainForm, GameInitializationConfig config)
        {
            templePanel = new PicturePanel(Images.ExtractBitmap(bytes, "templePic"));
            mainForm.layout.Add(templePanel, new Point(160, 76));

            var citiesPopup = config.PopUps["CUSTOMCITY"];
            citiesPopup.Options ??= Labels.Items[247..251];

            if (citiesPopup.Button.IndexOf(Labels.Cancel) == -1)
            {
                citiesPopup.Button.Add(Labels.Cancel);
            }

            var citiesDialog = new Civ2dialog(mainForm, citiesPopup,
                icons: new[]
                {
                    MapImages.Cities[7].Bitmap, MapImages.Cities[15].Bitmap, MapImages.Cities[23].Bitmap,
                    MapImages.Cities[31].Bitmap
                }) {SelectedIndex = (int) config.PlayerCiv.CityStyle};
            citiesDialog.Location = new Point((int)(Screen.PrimaryScreen.Bounds.Width - citiesDialog.Width - 156),
                                              (int)(Screen.PrimaryScreen.Bounds.Height - citiesDialog.Height - 72));
            citiesDialog.ShowModal(mainForm);
            if (citiesDialog.SelectedIndex == int.MinValue)
            {
                templePanel.Dispose();
                SelectGender(mainForm, config);
                return;
            }
            templePanel.Dispose();

            config.PlayerCiv.CityStyle = (CityStyleType) citiesDialog.SelectedIndex;
            var groupedTribes = config.Rules.Leaders
                .ToLookup(g => g.Color);

   
            var civilizations = new List<Civilization>
            {
                new () {Adjective = Labels.Items[17], LeaderName = Labels.Items[18], Alive = true, Id = 0, PlayerType = PlayerType.Barbarians, Advances =new bool[config.Rules.Advances.Length]},
                config.PlayerCiv
            };
            if (config.SelectComputerOpponents)
            {
                galleyPanel = new PicturePanel(Images.ExtractBitmap(bytes, "galleyPic"));
                mainForm.layout.Add(galleyPanel, new Point((int)(Screen.PrimaryScreen.Bounds.Width / 2 - galleyPanel.Width / 2), 75));

                var opponentPop = config.PopUps["OPPONENT"];
                var opponentNumber = 1;
                for (var i = 1; civilizations.Count <= config.NumberOfCivs; i++, opponentNumber++)
                {
                    if (i == config.PlayerCiv.Id)
                    {
                        opponentNumber--;
                        continue;
                    }

                    var tribes = groupedTribes.Contains(i)
                        ? groupedTribes[i].ToList()
                        : config.Rules.Leaders
                            .Where(leader => civilizations.All(civ => civ.Adjective != leader.Adjective)).ToList();

                    opponentPop.Options =
                        new[] { opponentPop.Options[0] }.Concat(tribes.Select(leader =>
                            $"{leader.Plural} ({(leader.Female ? leader.NameFemale : leader.NameMale)})")).ToList();
                    var oppDia = new Civ2dialog(mainForm, opponentPop,
                        replaceNumbers: new List<int>() { opponentNumber }, optionsCols: tribes.Count / 5);
                    oppDia.Location = new Point((int)(Screen.PrimaryScreen.Bounds.Width / 2 - oppDia.Width / 2),
                                                (int)(Screen.PrimaryScreen.Bounds.Height - oppDia.Height - 72));
                    oppDia.ShowModal(mainForm);

                    if (oppDia.SelectedIndex == int.MinValue)
                    {
                        SelectCityStyle(mainForm, config);
                        return;
                    }

                    civilizations.Add(MakeCivilization(config,
                        tribes[
                            oppDia.SelectedIndex == 0
                                ? config.Random.Next(tribes.Count)
                                : oppDia.SelectedIndex - 1], false, i));
                }
                galleyPanel.Dispose();
            }
            else
            {
                for (var i = 1; civilizations.Count <= config.NumberOfCivs; i++)
                {
                    if (i == config.PlayerCiv.Id) continue;

                    var tribes = groupedTribes.Contains(i)
                        ? groupedTribes[i].ToList()
                        : config.Rules.Leaders
                            .Where(leader => civilizations.All(civ => civ.Adjective != leader.Adjective)).ToList();

                    civilizations.Add(MakeCivilization(config, config.Random.ChooseFrom(tribes), false,
                        i));
                }
            }

            if (config.PlayerCiv.Id >= civilizations.Count)
            {
                var correctColour = MapImages.PlayerColours[config.PlayerCiv.Id];
                var colours = new List<PlayerColour>(MapImages.PlayerColours[..(civilizations.Count - 1)]) {correctColour};
                MapImages.PlayerColours = colours.ToArray();
                config.PlayerCiv.Id = MapImages.PlayerColours.Length - 1;
            }

            var maps = config.MapTask.Result;
            
            NewGameInitialisation.StartNewGame(config, maps, civilizations.OrderBy(c=>c.Id).ToList(), new LocalPlayer(mainForm));
            
            Images.LoadGraphicsAssetsFromFiles(config.RuleSet, config.Rules);
            mainForm.popupBoxList = config.PopUps;
            
            mainForm.Sounds.LoadSounds(config.RuleSet.Paths);
            mainForm.Playgame();
            config.Started = true;
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
                Advances = new bool[config.Rules.Advances.Length],
                CityStyle = (CityStyleType) tribe.CityStyle,
                LeaderGender =gender ,
                LeaderName = gender == 0 ? tribe.NameMale : tribe.NameFemale,
                LeaderTitle = titles[(int)GovernmentType.Despotism],
                LuxRate = 0,
                ScienceRate = 60,
                TaxRate = 40,
                TribeName = tribe.Plural,
                Titles = titles,
                PlayerType = human ? PlayerType.Local : PlayerType.AI 
            };
        }

        private static string GetLeaderTitle(GameInitializationConfig config, LeaderDefaults tribe, Government gov, int governmentType)
        {
            var govt = tribe.Titles.FirstOrDefault(t=>t.Gov == governmentType) ?? (IGovernmentTitles)gov;
            return config.Gender == 0 ? govt.TitleMale : govt.TitleFemale;
        }
    }
}