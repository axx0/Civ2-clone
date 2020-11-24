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

        public void SetReadonlyOptions(bool simplifiedCombat, bool flatEarth, bool bloodlust, bool dontRestartIfEliminated)
        {
            SimplifiedCombat = simplifiedCombat;
            FlatEarth = flatEarth;
            Bloodlust = bloodlust;
            DontRestartIfEliminated = dontRestartIfEliminated;
        }
    }
}

