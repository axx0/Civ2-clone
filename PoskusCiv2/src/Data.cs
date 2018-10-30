using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoskusCiv2
{
    public class Data
    {
        public int TurnNumber { get; set; }
        public int TurnNumberForGameYear { get; set; }
        public int UnitSelectedAtGameStart { get; set; }
        public int WhichHumanPlayerIsUsed { get; set; }
        public int PlayersMapUsed { get; set; }
        public int PlayersCivilizationNumberUsed { get; set; }
        public bool MapRevealed { get; set; }
        public int DifficultyLevel { get; set; }
        public int BarbarianActivity { get; set; }
        public int PollutionAmount { get; set; }
        public int GlobalTempRiseOccured { get; set; }
        public int NoOfTurnsOfPeace { get; set; }
        public int NumberOfUnits { get; set; }
        public int NumberOfCities { get; set; }
        public int MapXdim { get; set; }
        public int MapYdim { get; set; }
        public int MapArea { get; set; }
        public int MapSeed { get; set; }
        public int MapLocatorXdim { get; set; }
        public int MapLocatorYdim { get; set; }
    }
}
