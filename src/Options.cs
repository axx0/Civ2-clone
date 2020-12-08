namespace civ2
{
    public class Options : BaseInstance
    {
        // START GAME RULES
        public bool SimplifiedCombat { get; private set; }
        public bool FlatEarth { get; private set; }
        public bool Bloodlust { get; private set; }
        public bool DontRestartIfEliminated { get; private set; }

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

        public bool CheatPenaltyWarning { get; set; }
        public bool Grid { get; set; }

        public void Set(bool[] optionsArray)
        {
            Bloodlust = optionsArray[0];
            SimplifiedCombat = optionsArray[1];
            FlatEarth = optionsArray[2];
            DontRestartIfEliminated = optionsArray[3];
            SoundEffects = optionsArray[4];
            Music = optionsArray[5];
            CheatMenu = optionsArray[6];
            AlwaysWaitAtEndOfTurn = optionsArray[7];
            AutosaveEachTurn = optionsArray[8];
            ShowEnemyMoves = optionsArray[9];
            NoPauseAfterEnemyMoves = optionsArray[10];
            FastPieceSlide = optionsArray[11];
            InstantAdvice = optionsArray[12];
            TutorialHelp = optionsArray[13];
            EnterClosestCityScreen = optionsArray[14];
            MoveUnitsWithoutMouse = optionsArray[15];
            ThroneRoomGraphics = optionsArray[16];
            DiplomacyScreenGraphics = optionsArray[17];
            AnimatedHeralds = optionsArray[18];
            CivilopediaForAdvances = optionsArray[19];
            HighCouncil = optionsArray[20];
            WonderMovies = optionsArray[21];
            WarnWhenCityGrowthHalted = optionsArray[22];
            ShowCityImprovementsBuilt = optionsArray[23];
            ShowNonCombatUnitsBuilt = optionsArray[24];
            ShowInvalidBuildInstructions = optionsArray[25];
            AnnounceCitiesInDisorder = optionsArray[26];
            AnnounceOrderRestored = optionsArray[27];
            AnnounceWeLoveKingDay = optionsArray[28];
            WarnWhenFoodDangerouslyLow = optionsArray[29];
            WarnWhenPollutionOccurs = optionsArray[30];
            WarnChangProductWillCostShields = optionsArray[31];
            ZoomToCityNotDefaultAction = optionsArray[32];
            CheatPenaltyWarning = optionsArray[33];
            Grid = optionsArray[34];
        }
    }
}

