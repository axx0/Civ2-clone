using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoskusCiv2
{
    class Units_
    {
        //Data from game
        public static string[] unitTypeNames = new string[63] { "Settler", "Engineers", "Warriors", "Phalanx", "Archers", "Legions", "Pikemen", "Musketeers", "Fanatics", "Partisans", "Alpine Troops", "Riflemen", "Marines", "Paratroopers", "Mech. Inf.", "Horsemen", "Chariot", "Elephant", "Crusaders", "Knights", "Dragoons", "Cavalry", "Armor", "Catapult", "Cannon", "Artillery", "Howitzer", "Fighter", "Bomber", "Helicopter", "Stlth Ftr.", "Stlth Bmbr", "Trireme", "Caravel", "Galleon", "Frigate", "Ironclad", "Destroyer", "Cruiser", "AEGIS Cruiser", "Battleship", "Submarine", "Carrier", "Transport", "Cruise Msl.", "Nuclear Msl.", "Diplomat", "Spy", "Caravan", "Freight", "Explorer", "Extra 1", "Extra 2", "Extra 3", "Extra 4", "Extra 5", "Extra 6", "Extra 7", "Extra 8", "Extra 9", "Extra 10", "Extra 11", "Extra 12" };
        public static int[] unitTurns = new int[63] { 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 3, 2, 2, 2, 2, 2, 2, 2, 3, 1, 1, 1, 2, 10, 8, 6, 14, 12, 3, 3, 4, 4, 4, 6, 5, 5, 4, 3, 5, 5, 12, 16, 2, 3, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
        public static int[] landseaairUnit = new int[63] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 3, 3, 3, 3, 3, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 3, 3, 1, 1, 1, 1, 1, 1, 2, 3, 1, 1, 1, 1, 1, 1, 1, 1, 1 };

        //Some starting units
        public static int unitNumber = 2;
        public static int[] unitType = new int[2] { 1, 5 };        //type of unit
        public static int[] unitTurnsLeft = new int[2] { unitTurns[unitType[0]], unitTurns[unitType[1]] };
        public static bool[] veteranStatus = new bool[2] { false, true };
        public static int[] locationX = new int[2] { 19, 13 };
        public static int[] locationY = new int[2] { 21, 35 };
        public static string[] unitCity = new string[2] { "London", "London" };
        public static int[] unitCiv = new int[2] { 6, 7 };

        //Adding new units to game
        public static void AddUnit(int type, bool veteran, int locX, int locY, string city, int civ)
        {
            unitNumber += 1;
            unitTurnsLeft = unitTurnsLeft.Concat(new int[] { unitTurns[type] }).ToArray();
            unitType = unitType.Concat(new int[] { type }).ToArray();
            veteranStatus = veteranStatus.Concat(new bool[] { veteran }).ToArray();
            locationX = locationX.Concat(new int[] { locX }).ToArray();
            locationY = locationY.Concat(new int[] { locY }).ToArray();
            unitCity = unitCity.Concat(new string[] { city }).ToArray();
            unitCiv = unitCiv.Concat(new int[] { civ }).ToArray();
        }
    }
}
