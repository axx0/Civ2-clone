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
        public int HumanPlayerUsed { get; set; }
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

        private int _gameYear;
        public int GameYear
        {
            get
            {
                if (TurnNumber < 250)
                {
                    _gameYear = -4000 + TurnNumber * 20;
                }
                else if (TurnNumber >= 250 && TurnNumber < 300)
                {
                    _gameYear = 1000 + (TurnNumber - 250) * 10;
                }
                else if (TurnNumber >= 300 && TurnNumber < 350)
                {
                    _gameYear = 1500 + (TurnNumber - 300) * 5;
                }
                else if (TurnNumber >= 350 && TurnNumber < 400)
                {
                    _gameYear = 1750 + (TurnNumber - 350) * 2;
                }
                else
                {
                    _gameYear = 1850 + (TurnNumber - 400);
                }
                return _gameYear;
            }
        }
    }
}
