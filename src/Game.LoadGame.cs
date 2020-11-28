using System.Collections.Generic;
using System.IO;
using civ2.Bitmaps;
using civ2.Units;
using civ2.Enums;

namespace civ2
{
    public partial class Game : BaseInstance
    {
        public static Game LoadGame(string savDirectoryPath, string SAVname)
        {
            // Import graphical assets from SAV directory. If they don't exist import from root civ2 directory.
            Images.LoadGraphicsAssetsFromFiles(savDirectoryPath);

            // Read SAV file & RULES.txt
            ReadGameData gameData = new ReadGameData(savDirectoryPath, SAVname);

            // Make an instance of a new game
            _instance = new Game(gameData);

            // Import SAV
            //_instance.ImportSAV(savDirectoryPath + "\\" + SAVname);



            //_instance.ActiveUnit = Data.SelectedUnitIndex == -1 ? null : Units.Find(unit => unit.Id == Data.SelectedUnitIndex);    //null means all units have ended turn
            //_instance._activeCiv = _instance._civs[Data.HumanPlayer];
            //_instance.ActiveUnit = Game.Units[0];   //temp!!!
            //_instance.ActiveCiv = Game.Civs[0];   //temp!!!
            
            return _instance;   // Return instance so it can be read by forms
        }

        private Game(GameData SAVgameData)
        {
            _units = new List<IUnit>();
            _casualties = new List<IUnit>();
            _cities = new List<City>();
            _civs = new List<Civilization>();
            _rules = new Rules();

            _gameVersion = SAVgameData.GameVersion;

            _options.Set(SAVgameData.Options);

            TurnNumber = SAVgameData.TurnNumber;
            TurnNumberForGameYear = SAVgameData.TurnNumberForGameYear;
            SelectedUnitIndex = SAVgameData.SelectedUnitIndex;
            HumanPlayer = SAVgameData.HumanPlayer;
            PlayersMapUsed = SAVgameData.PlayersMapUsed;
            PlayersCivilizationNumberUsed = SAVgameData.PlayersCivilizationNumberUsed;
            _difficultyLevel = SAVgameData.DifficultyLevel;
            _barbarianActivity = SAVgameData.BarbarianActivity;
            CivsInPlay = SAVgameData.CivsInPlay;
            PollutionAmount = SAVgameData.PollutionAmount;
            GlobalTempRiseOccured = SAVgameData.GlobalTempRiseOccured;
            NoOfTurnsOfPeace = SAVgameData.NoOfTurnsOfPeace;
            NumberOfUnits = SAVgameData.NumberOfUnits;
            NumberOfCities = SAVgameData.NumberOfCities;

            // Create all 8 civs (tribes)
            for (int i = 0; i < 8; i++)
            {
                CreateCiv(i, SAVgameData.HumanPlayer, SAVgameData.CivCityStyle[i], SAVgameData.CivLeaderName[i], SAVgameData.CivTribeName[i],
                    SAVgameData.CivAdjective[i], SAVgameData.RulerGender[i], SAVgameData.CivMoney[i], SAVgameData.CivNumber[i],
                    SAVgameData.CivResearchProgress[i], SAVgameData.CivResearchingTech[i], SAVgameData.CivSciRate[i], SAVgameData.CivTaxRate[i],
                    SAVgameData.CivGovernment[i], SAVgameData.CivReputation[i], SAVgameData.CivTechs);
            }
        }



    }
}
