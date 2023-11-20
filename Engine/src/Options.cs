namespace Civ2engine
{
    public class Options
    {
        public Options(GameInitializationConfig config)
        {
            SimplifiedCombat = config.SimplifiedCombat;
            Bloodlust = config.Bloodlust;
            DontRestartIfEliminated = config.DontRestartEliminatedPlayers;
            FlatEarth = config.FlatWorld;
        }

        // START GAME RULES
        public bool SimplifiedCombat { get; private set; }
        public bool Bloodlust { get; private set; }
        public bool DontRestartIfEliminated { get; private set; }
        public bool FlatEarth { get; private set; }

        // GAME OPTIONS
        public bool SoundEffects { get; set; }
        public bool Music { get; set; }
        public bool CheatMenu { get; set; }
        public bool AlwaysWaitAtEndOfTurn { get; set; }
        public bool AutosaveEachTurn { get; set; }
        public bool ShowEnemyMoves { get; set; }
        public bool NoPauseAfterEnemyMoves { get; set; }
        public bool FastPieceSlide { get; set; }
        public bool InstantAdvice { get; set; }
        public bool TutorialHelp { get; set; }
        public bool EnterClosestCityScreen { get; set; }
        public bool MoveUnitsWithoutMouse { get; set; }

        // GRAPHICS OPTIONS
        public bool ThroneRoomGraphics { get; set; }
        public bool DiplomacyScreenGraphics { get; set; }
        public bool AnimatedHeralds { get; set; }
        public bool CivilopediaForAdvances { get; set; }
        public bool HighCouncil { get; set; }
        public bool WonderMovies { get; set; }

        // CITY REPORT OPTIONS
        public bool WarnWhenCityGrowthHalted { get; set; }
        public bool ShowCityImprovementsBuilt { get; set; }
        public bool ShowNonCombatUnitsBuilt { get; set; }
        public bool ShowInvalidBuildInstructions { get; set; }
        public bool AnnounceCitiesInDisorder { get; set; }
        public bool AnnounceOrderRestored { get; set; }
        public bool AnnounceWeLoveKingDay { get; set; }
        public bool WarnWhenFoodDangerouslyLow { get; set; }
        public bool WarnWhenPollutionOccurs { get; set; }
        public bool WarnChangProductWillCostShields { get; set; }
        public bool ZoomToCityNotDefaultAction { get; set; }

        public bool CheatPenaltyWarningDisabled { get; set; }
        public bool Grid { get; set; }

        public Options(bool[] optionsArray)
        {
            SimplifiedCombat = optionsArray[0];
            Bloodlust = optionsArray[1];
            DontRestartIfEliminated = optionsArray[2];
            FlatEarth = optionsArray[3];
            Music = optionsArray[4];
            SoundEffects = optionsArray[5];
            Grid = optionsArray[6];
            EnterClosestCityScreen = optionsArray[7];
            MoveUnitsWithoutMouse = optionsArray[8];
            TutorialHelp = optionsArray[9];
            InstantAdvice = optionsArray[10];
            FastPieceSlide = optionsArray[11];
            NoPauseAfterEnemyMoves = optionsArray[12];
            ShowEnemyMoves = optionsArray[13];
            AutosaveEachTurn = optionsArray[14];
            AlwaysWaitAtEndOfTurn = optionsArray[15];
            CheatMenu = optionsArray[16];
            WonderMovies = optionsArray[17];
            ThroneRoomGraphics = optionsArray[18];
            DiplomacyScreenGraphics = optionsArray[19];
            CivilopediaForAdvances = optionsArray[20];
            HighCouncil = optionsArray[21];
            AnimatedHeralds = optionsArray[22];
            CheatPenaltyWarningDisabled = optionsArray[23];
            WarnWhenCityGrowthHalted = optionsArray[24];
            ShowCityImprovementsBuilt = optionsArray[25];
            ShowNonCombatUnitsBuilt = optionsArray[26];
            ShowInvalidBuildInstructions = optionsArray[27];
            AnnounceCitiesInDisorder = optionsArray[28];
            AnnounceOrderRestored = optionsArray[29];
            AnnounceWeLoveKingDay = optionsArray[30];
            WarnWhenFoodDangerouslyLow = optionsArray[31];
            WarnWhenPollutionOccurs = optionsArray[32];
            WarnChangProductWillCostShields = optionsArray[33];
            ZoomToCityNotDefaultAction = optionsArray[34];
        }
    }
}

