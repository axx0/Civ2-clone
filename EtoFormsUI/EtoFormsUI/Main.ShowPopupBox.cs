using System;
using System.IO;
using Eto.Forms;
using Eto.Drawing;
using Civ2engine.Events;
using Civ2engine;
using System.Collections.Generic;

namespace EtoFormsUI
{
    public partial class Main : Form
    {
        private string savDirectory, savName;

        private void LocateStartingFiles(string title, FileFilter filter, Action<string, string> initializer,
            Action start)
        {
            using var ofd = new OpenFileDialog
            {
                Directory = new Uri(Settings.Civ2Path),
                Title = title,
                Filters = { filter }
            };

            var result = ofd.ShowDialog(this.ParentWindow);
            sinaiPanel.Dispose();
            sinaiPanel = null;
            if (result == DialogResult.Ok)
            {
                // Get SAV name & directory name from result
                savDirectory = Path.GetDirectoryName(ofd.FileName);
                savName = Path.GetFileName(ofd.FileName);
                initializer(savDirectory, savName);
                Sounds.Stop();
                Sounds.PlaySound("MENUOK.WAV");

                start();
            }
            else
            {
                OnPopupboxEvent?.Invoke(null, new PopupboxEventArgs("MAINMENU"));
            }
        }

        private void PopupboxEvent(object sender, PopupboxEventArgs e)
        {
            switch (e.BoxName)
            {
                case "MAINMENU":
                    {
                        var popupBox = new Civ2dialog_v2(this, popupBoxList.Find(p => p.Name == e.BoxName))
                        {
                            Location = new Point((int) (Screen.PrimaryScreen.Bounds.Width * 0.745),
                                (int) (Screen.PrimaryScreen.Bounds.Height * 0.570))
                        };
                        popupBox.ShowModal(Parent);
                        switch (popupBox.SelectedIndex)
                        {
                            //New Game
                            case 0:
                            {
                                NewGame(false);
                                break;
                            }
                            // Start premade
                            case 1:
                            {
                                LocateStartingFiles("Select Map To Load",
                                    new FileFilter("Save Files (*.mp)", ".mp"), LoadScenarioInit,
                                    () => { });
                                break;
                            }

                            //Customise World
                            case 2:
                            {
                                NewGame(true);
                                break;
                            }
                            // Load scenario
                            case 3:
                            {
                                LocateStartingFiles("Select Scenario To Load",
                                    new FileFilter("Save Files (*.scn)", ".scn"), LoadScenarioInit,
                                    () =>
                                    {

                                    });
                            

                                break;
                            }
                            // Load game
                            case 4:
                            {
                                LocateStartingFiles("Select Game To Load", new FileFilter("Save Files (*.sav)", ".SAV"),
                                    LoadGameInitialization, () =>
                                        OnPopupboxEvent?.Invoke(null,
                                            new PopupboxEventArgs("LOADOK",
                                                new List<string>
                                                {
                                                    Game.GetActiveCiv.LeaderTitle, Game.GetActiveCiv.LeaderName,
                                                    Game.GetActiveCiv.TribeName, Game.GetGameYearString,
                                                    Game.DifficultyLevel.ToString()
                                                }))
                                );

                                break;
                            }
                        }
                        break;
                    }
                case "GAMEOPTIONS":
                    {
                        var checkboxOptions = new List<bool> { Game.Options.SoundEffects, Game.Options.Music, Game.Options.AlwaysWaitAtEndOfTurn, Game.Options.AutosaveEachTurn,
                            Game.Options.ShowEnemyMoves, Game.Options.NoPauseAfterEnemyMoves, Game.Options.FastPieceSlide, Game.Options.InstantAdvice, Game.Options.TutorialHelp,
                            Game.Options.MoveUnitsWithoutMouse, Game.Options.EnterClosestCityScreen };
                        var popupbox = new Civ2dialog_v2(this, popupBoxList.Find(p => p.Name == e.BoxName), e.ReplaceStrings, checkboxOptions);
                        popupbox.ShowModal(Parent);
                        Game.Options.SoundEffects = popupbox.CheckboxReturnStates[0];
                        Game.Options.Music = popupbox.CheckboxReturnStates[1];
                        Game.Options.AlwaysWaitAtEndOfTurn = popupbox.CheckboxReturnStates[2];
                        Game.Options.AutosaveEachTurn = popupbox.CheckboxReturnStates[3];
                        Game.Options.ShowEnemyMoves = popupbox.CheckboxReturnStates[4];
                        Game.Options.NoPauseAfterEnemyMoves = popupbox.CheckboxReturnStates[5];
                        Game.Options.FastPieceSlide = popupbox.CheckboxReturnStates[6];
                        Game.Options.InstantAdvice = popupbox.CheckboxReturnStates[7];
                        Game.Options.TutorialHelp = popupbox.CheckboxReturnStates[8];
                        Game.Options.MoveUnitsWithoutMouse = popupbox.CheckboxReturnStates[9];
                        Game.Options.EnterClosestCityScreen = popupbox.CheckboxReturnStates[10];
                        break;
                    }
                case "GRAPHICOPTIONS":
                    {
                        var checkboxOptions = new List<bool> { Game.Options.ThroneRoomGraphics, Game.Options.DiplomacyScreenGraphics, Game.Options.AnimatedHeralds, 
                            Game.Options.CivilopediaForAdvances, Game.Options.HighCouncil, Game.Options.WonderMovies };
                        var popupbox = new Civ2dialog_v2(this, popupBoxList.Find(p => p.Name == e.BoxName), e.ReplaceStrings, checkboxOptions);
                        popupbox.ShowModal(Parent);
                        Game.Options.ThroneRoomGraphics = popupbox.CheckboxReturnStates[0];
                        Game.Options.DiplomacyScreenGraphics = popupbox.CheckboxReturnStates[1];
                        Game.Options.AnimatedHeralds = popupbox.CheckboxReturnStates[2];
                        Game.Options.CivilopediaForAdvances = popupbox.CheckboxReturnStates[3];
                        Game.Options.HighCouncil = popupbox.CheckboxReturnStates[4];
                        Game.Options.WonderMovies = popupbox.CheckboxReturnStates[5];
                        break;
                    }
                case "MESSAGEOPTIONS":
                    {
                        var checkboxOptions = new List<bool> { Game.Options.WarnWhenCityGrowthHalted, Game.Options.ShowCityImprovementsBuilt, Game.Options.ShowNonCombatUnitsBuilt,
                            Game.Options.ShowInvalidBuildInstructions, Game.Options.AnnounceCitiesInDisorder, Game.Options.AnnounceOrderRestored,
                            Game.Options.AnnounceWeLoveKingDay, Game.Options.WarnWhenFoodDangerouslyLow, Game.Options.WarnWhenPollutionOccurs,
                            Game.Options.WarnChangProductWillCostShields, Game.Options.ZoomToCityNotDefaultAction };
                        var popupbox = new Civ2dialog_v2(this, popupBoxList.Find(p => p.Name == e.BoxName), e.ReplaceStrings, checkboxOptions);
                        popupbox.ShowModal(Parent);
                        Game.Options.WarnWhenCityGrowthHalted = popupbox.CheckboxReturnStates[0];
                        Game.Options.ShowCityImprovementsBuilt = popupbox.CheckboxReturnStates[1];
                        Game.Options.ShowNonCombatUnitsBuilt = popupbox.CheckboxReturnStates[2];
                        Game.Options.ShowInvalidBuildInstructions = popupbox.CheckboxReturnStates[3];
                        Game.Options.AnnounceCitiesInDisorder = popupbox.CheckboxReturnStates[4];
                        Game.Options.AnnounceOrderRestored = popupbox.CheckboxReturnStates[5];
                        Game.Options.AnnounceWeLoveKingDay = popupbox.CheckboxReturnStates[6];
                        Game.Options.WarnWhenFoodDangerouslyLow = popupbox.CheckboxReturnStates[7];
                        Game.Options.WarnWhenPollutionOccurs = popupbox.CheckboxReturnStates[8];
                        Game.Options.WarnChangProductWillCostShields = popupbox.CheckboxReturnStates[9];
                        Game.Options.ZoomToCityNotDefaultAction = popupbox.CheckboxReturnStates[10];
                        break;
                    }
                case "LOADOK":
                    {
                        var popupbox = new Civ2dialog_v2(this, popupBoxList.Find(p => p.Name == e.BoxName), e.ReplaceStrings);
                        popupbox.ShowModal(Parent);
                        StartGame();
                        Sounds.PlaySound("MENUOK.WAV");
                        break;
                    }
                default: break;
            }
        }
    }
}
