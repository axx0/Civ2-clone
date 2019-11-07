using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTciv2
{
    public static class Options
    {
        public static int Version { get; set; }
        public static bool Bloodlust { get; set; }
        public static bool SimplifiedCombat { get; set; }
        public static bool FlatEarth { get; set; }
        public static bool DontRestartIfEliminated { get; set; }
        public static bool MoveUnitsWithoutMouse { get; set; }
        public static bool EnterClosestCityScreen { get; set; }
        public static bool Grid { get; set; }
        public static bool SoundEffects { get; set; }
        public static bool Music { get; set; }
        public static bool CheatMenu { get; set; }
        public static bool AlwaysWaitAtEndOfTurn { get; set; }
        public static bool AutosaveEachTurn { get; set; }
        public static bool ShowEnemyMoves { get; set; }
        public static bool NoPauseAfterEnemyMoves { get; set; }
        public static bool FastPieceSlide { get; set; }
        public static bool InstantAdvice { get; set; }
        public static bool TutorialHelp { get; set; }
        public static bool AnimatedHeralds { get; set; }
        public static bool HighCouncil { get; set; }
        public static bool CivilopediaForAdvances { get; set; }
        public static bool ThroneRoomGraphics { get; set; }
        public static bool DiplomacyScreenGraphics { get; set; }
        public static bool WonderMovies { get; set; }
        public static bool CheatPenaltyWarning { get; set; }
        public static bool AnnounceWeLoveKingDay { get; set; }
        public static bool WarnWhenFoodDangerouslyLow { get; set; }
        public static bool AnnounceCitiesInDisorder { get; set; }
        public static bool AnnounceOrderRestored { get; set; }
        public static bool ShowNonCombatUnitsBuilt { get; set; }
        public static bool ShowInvalidBuildInstructions { get; set; }
        public static bool WarnWhenCityGrowthHalted { get; set; }
        public static bool ShowCityImprovementsBuilt { get; set; }
        public static bool ZoomToCityNotDefaultAction { get; set; }
        public static bool WarnWhenPollutionOccurs { get; set; }
        public static bool WarnWhenChangingProductionWillCostShields { get; set; }
    }
}

