using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using Civ2engine.Enums;
using Civ2engine.Improvements;
using Civ2engine.Units;
using Civ2engine.Terrains;

namespace Civ2engine
{
    public class City : BaseInstance
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool CanBuildCoastal { get; set; }
        public bool AutobuildMilitaryRule { get; set; }
        public bool StolenTech { get; set; }
        public bool ImprovementSold { get; set; }
        public bool WeLoveKingDay { get; set; }
        public bool CivilDisorder { get; set; }
        public bool CanBuildShips { get; set; }
        public bool Objectivex3 { get; set; }
        public bool Objectivex1 { get; set; }
        public Civilization Owner { get; set; }
        public int OwnerId => Game.GetCivs.IndexOf(Owner);
        public int Size { get; set; }
        public Civilization WhoBuiltIt { get; set; }
        public int FoodInStorage { get; set; }
        public int ShieldsProgress { get; set; }
        public int NetTrade { get; set; }
        public string Name { get; set; }
        public bool[] DistributionWorkers { get; set; }
        public int NoOfSpecialistsx4 { get; set; }
        public int ItemInProduction { get; set; }
        public int ActiveTradeRoutes { get; set; }
        public CommodityType[] CommoditySupplied { get; set; }
        public CommodityType[] CommodityDemanded { get; set; }
        public CommodityType[] CommodityInRoute { get; set; }
        public int[] TradeRoutePartnerCity { get; set; }
        public int NoOfTradeIcons { get; set; }
        public int FoodProduction { get; set; }
        public int ShieldProduction { get; set; }
        public int HappyCitizens { get; set; }
        public int UnhappyCitizens { get; set; }

        private int _population;
        public int Population
        {
            get
            {
                 _population = 0;
                for (int i = 1; i <= Size; i++)
                    _population += i * 10000;
                return _population;
            }
        }

        private readonly List<IImprovement> _improvements = new List<IImprovement>();
        public IImprovement[] Improvements => _improvements.OrderBy(i => i.Id).ToArray();
        public void AddImprovement(IImprovement improvement) => _improvements.Add(improvement);
        public bool ImprovementExists(ImprovementType improvement) => _improvements.Exists(i => i.Type == improvement);
        public List<IUnit> UnitsInCity => Game.GetUnits.Where(unit => unit.X == X && unit.Y == Y).ToList();
        public List<IUnit> SupportedUnits => Game.GetUnits.Where(unit => unit.HomeCity == this).ToList();
        public bool AnyUnitsPresent() => Game.GetUnits.Any(unit => unit.X == this.X && unit.Y == this.Y);

        // Determine which units, supported by this city, cost shields
        public bool[] SupportedUnitsWhichCostShields()
        {
            List<IUnit> supportedUnits = SupportedUnits;
            bool[] costShields = new bool[SupportedUnits.Count];
            //First determine how many units have 0 costs due to different goverernment types
            int noCost = 0;
            switch (Game.GetCivs[OwnerId].Government)
            {
                case GovernmentType.Anarchy:
                case GovernmentType.Despotism:
                    noCost = Math.Min(supportedUnits.Count, Size); //only units above city size cost 1 shield
                    break;
                case GovernmentType.Communism:
                case GovernmentType.Monarchy:
                    noCost = Math.Min(supportedUnits.Count, 3);    //first 3 units have no shield cost
                    break;
                case GovernmentType.Fundamentalism:
                    noCost = Math.Min(supportedUnits.Count, 10);   //first 10 units have no shield cost
                    break;
                case GovernmentType.Republic:
                case GovernmentType.Democracy:
                    noCost = 0;    //each unit costs 1 shield per turn
                    break;
            }
            //Now determine bools of units that require upkeep
            for (int i = 0; i < supportedUnits.Count; i++)
            {
                if (supportedUnits[i].Type == UnitType.Diplomat || supportedUnits[i].Type == UnitType.Caravan || supportedUnits[i].Type == UnitType.Fanatics ||
                    supportedUnits[i].Type == UnitType.Spy || supportedUnits[i].Type == UnitType.Freight)   //some units never require upkeep
                {
                    costShields[i] = false;
                }
                else if (noCost != 0)
                {
                    costShields[i] = false;
                    noCost--;   //reduce counter
                }
                else
                {
                    costShields[i] = true;
                }
            }

            return costShields;
        }

        private int[] _foodDistribution;
        public int[] FoodDistribution
        {
            get
            {
                _foodDistribution = new int[21];    //21 squares around city
                int[,] offsets = new int[21, 2] { { 0, 0 }, { -1, -3 }, { -3, -1 }, { -3, 1 }, { -1, 3 }, { 1, 3 }, { 3, 1 }, { 3, -1 }, { 1, -3 }, { -2, -2 }, { -2, 2 }, { 2, 2 },
                                                  { 2, -2 }, { 0, -2 }, { -1, -1 }, { -2, 0 }, { -1, 1 }, { 0, 2 }, { 1, 1 }, { 2, 0 }, { 1, -1 } };    //offset of squares from city square (0,0)
                for (int i = 0; i < 21; i++)
                {
                    if (DistributionWorkers[i])
                    {
                        int x = X + offsets[i, 0];
                        int y = Y + offsets[i, 1];
                        x = (x - (y % 2)) / 2;    //map format
                        _foodDistribution[i] = Map.Tile[x, y].Food;
                        if (Map.Tile[x, y].Irrigation) _foodDistribution[i] += Map.Tile[x, y].IrrigationBonus;
                    }
                    else _foodDistribution[i] = 0;
                }
                return _foodDistribution;
            }
        }

        private int[] _shieldDistribution;
        public int[] ShieldDistribution
        {
            get
            {
                _shieldDistribution = new int[21];    //21 squares around city
                int[,] offsets = new int[21, 2] { { 0, 0 }, { -1, -3 }, { -3, -1 }, { -3, 1 }, { -1, 3 }, { 1, 3 }, { 3, 1 }, { 3, -1 }, { 1, -3 }, { -2, -2 }, { -2, 2 }, { 2, 2 },
                                                  { 2, -2 }, { 0, -2 }, { -1, -1 }, { -2, 0 }, { -1, 1 }, { 0, 2 }, { 1, 1 }, { 2, 0 }, { 1, -1 } };    //offset of squares from city square (0,0)
                for (int i = 0; i < 21; i++)
                {
                    if (DistributionWorkers[i])
                    {
                        int x = X + offsets[i, 0];
                        int y = Y + offsets[i, 1];
                        x = (x - (y % 2)) / 2;    //map format
                        _shieldDistribution[i] = Map.Tile[x, y].Shields;
                        if (Map.Tile[x, y].Mining) _shieldDistribution[i] += Map.Tile[x, y].MiningBonus;
                    }
                    else _shieldDistribution[i] = 0;
                }
                return _shieldDistribution;
            }
        }

        private int[] _tradeDistribution;
        public int[] TradeDistribution
        {
            get
            {
                _tradeDistribution = new int[21];    //21 squares around city
                int[,] offsets = new int[21, 2] { { 0, 0 }, { -1, -3 }, { -3, -1 }, { -3, 1 }, { -1, 3 }, { 1, 3 }, { 3, 1 }, { 3, -1 }, { 1, -3 }, { -2, -2 }, { -2, 2 }, { 2, 2 },
                                                  { 2, -2 }, { 0, -2 }, { -1, -1 }, { -2, 0 }, { -1, 1 }, { 0, 2 }, { 1, 1 }, { 2, 0 }, { 1, -1 } };    //offset of squares from city square (0,0)
                for (int i = 0; i < 21; i++)
                {
                    if (DistributionWorkers[i])
                    {
                        int x = X + offsets[i, 0];
                        int y = Y + offsets[i, 1];
                        x = (x - (y % 2)) / 2;    //map format
                        ITerrain map = Map.Tile[x, y];
                        _tradeDistribution[i] = map.Trade;
                        if (map.Road && (map.Type == TerrainType.Desert || map.Type == TerrainType.Grassland || map.Type == TerrainType.Plains)) _tradeDistribution[i]++;
                    }
                    else _tradeDistribution[i] = 0;
                }
                return _tradeDistribution;
            }
        }

        private int _food;
        public int Food
        {
            get
            {
                int maxFood = 2 * Size;
                foreach (IUnit unit in Game.GetUnits.Where(u => (u.Type == UnitType.Settlers || u.Type == UnitType.Engineers) && u.HomeCity == this)) maxFood++;  //increase max food for settlers & enineers with this home city
                _food = Math.Min(FoodDistribution.Sum(), maxFood);
                return _food;
            }
        }

        private int _surplusHunger;
        public int SurplusHunger
        {
            get
            {
                int maxFood = 2 * Size;
                foreach (IUnit unit in Game.GetUnits.Where(u => (u.Type == UnitType.Settlers || u.Type == UnitType.Engineers) && u.HomeCity == this)) maxFood++;  //increase max food for settlers & enineers with this home city
                _surplusHunger = FoodDistribution.Sum() - maxFood;
                return _surplusHunger;
            }
        }

        private int _trade;
        public int Trade
        {
            get
            {
                _trade = TradeDistribution.Sum();
                return _trade;
            }
        }

        private int _corruption;
        public int Corruption
        {
            get
            {
                _corruption = 0;
                if (!Improvements.Any(impr => impr.Type == ImprovementType.Palace)) _corruption++;  //if not capital
                return _corruption;
            }
        }

        private int _tax;
        public int Tax
        {
            get
            {
                _tax = Trade * Game.GetCivs[OwnerId].TaxRate / 100;
                return _tax;
            }
        }

        private int _lux;
        public int Lux
        {
            get
            {
                _lux = Trade - Science - Tax;
                return _lux;
            }
        }

        private int _science;
        public int Science
        {
            get
            {
                _science = Trade * Game.GetCivs[OwnerId].ScienceRate / 100;
                return _science;
            }
        }

        private int _support;
        public int Support
        {
            get
            {
                bool[] costShields = SupportedUnitsWhichCostShields();
                _support = costShields.Count(c => c);   //count true occurrences
                return _support;
            }
        }

        private int _production;
        public int Production
        {
            get
            {
                _production = ShieldDistribution.Sum() - Support;
                return _production;
            }
        }

        private int _waste;
        public int Waste
        {
            get
            {
                _waste = 1;
                return _waste;
            }
        }

        private PeopleType[] _people;
        public PeopleType[] People
        {
            get
            {
                _people = new PeopleType[Size];
                //Unhappy
                int additUnhappy = Size - 6;    //without units & improvements present, 6 people are content
                additUnhappy -= Math.Min(Game.GetUnits.Count(unit => unit.X == X && unit.Y == Y), 3);  //each new unit in city -> 1 less unhappy (up to 3 max)
                if (Improvements.Any(impr => impr.Type == ImprovementType.Temple)) additUnhappy -= 2;
                if (Improvements.Any(impr => impr.Type == ImprovementType.Colosseum)) additUnhappy -= 3;
                if (Improvements.Any(impr => impr.Type == ImprovementType.Cathedral)) additUnhappy -= 3;
                //Aristocrats
                int additArist = 0;
                switch (Size + 1 - DistributionWorkers.Count(c => c))   //populating aristocrats based on workers removed
                {
                    case 1: additArist += 1; break;
                    case 2:
                    case 3: additArist += 3; break;
                    case 4:
                    case 5:
                    case 6: additArist += 4; break;
                    case 7: additArist += 5; break;
                    case 8:
                    case 9: additArist += 6; break;
                    case 10: additArist += 7; break;
                    case 11: additArist += 8; break;
                    default: break;
                }
                //Elvis
                int additElvis = Size + 1 - DistributionWorkers.Count(c => c);  //no of elvis = no of workers removed
                //Populate
                for (int i = 0; i < Size; i++) _people[i] = PeopleType.Worker;
                for (int i = 0; i < additUnhappy; i++) _people[Size - 1 - i] = PeopleType.Unhappy;
                for (int i = 0; i < additArist; i++) _people[i] = PeopleType.Aristocrat;
                for (int i = 0; i < additElvis; i++) _people[Size - 1 - i] = PeopleType.Elvis;
                return _people;
            }
            set
            {
                _people = value;
            }
        }

        //public Bitmap Graphic(bool citySizeWindow, int zoom) => Draw.City(this, citySizeWindow, zoom);

        //private Bitmap _textGraphic;
        //public Bitmap TextGraphic
        //{
        //    get
        //    {
        //        // Define text characteristics for zoom levels
        //        int shadowOffset, fontSize;
        //        switch (Game.Zoom)
        //        {
        //            case -7: shadowOffset = 0; fontSize = 1; break;
        //            case -6: shadowOffset = 0; fontSize = 3; break;
        //            case -5: shadowOffset = 0; fontSize = 5; break;
        //            case -4: shadowOffset = 1; fontSize = 7; break;
        //            case -3: shadowOffset = 1; fontSize = 10; break;
        //            case -2: shadowOffset = 1; fontSize = 11; break;
        //            case -1: shadowOffset = 1; fontSize = 13; break;
        //            case 0: shadowOffset = 2; fontSize = 14; break;
        //            case 1: shadowOffset = 2; fontSize = 16; break;
        //            case 2: shadowOffset = 2; fontSize = 17; break;
        //            case 3: shadowOffset = 2; fontSize = 19; break;
        //            case 4: shadowOffset = 2; fontSize = 21; break;
        //            case 5: shadowOffset = 2; fontSize = 24; break;
        //            case 6: shadowOffset = 2; fontSize = 25; break;
        //            case 7: shadowOffset = 2; fontSize = 26; break;
        //            case 8: shadowOffset = 2; fontSize = 28; break;
        //            default: shadowOffset = 2; fontSize = 14; break;
        //        }
        //        // Draw
        //        Graphics gr = Graphics.FromImage(new Bitmap(1, 1));
        //        SizeF stringSize = gr.MeasureString(Name, new Font("Times New Roman", fontSize));
        //        int stringWidth = (int)stringSize.Width;
        //        int stringHeight = (int)stringSize.Height;
        //        _textGraphic = new Bitmap(stringWidth + 2, stringHeight + 2);
        //        Graphics g = Graphics.FromImage(_textGraphic);
        //        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
        //        g.DrawString(Name, new Font("Times New Roman", fontSize), Brushes.Black, new PointF(shadowOffset, 0));
        //        g.DrawString(Name, new Font("Times New Roman", fontSize), Brushes.Black, new PointF(0, shadowOffset));
        //        g.DrawString(Name, new Font("Times New Roman", fontSize), new SolidBrush(CivColors.CityTextColor[OwnerId]), new PointF(0, 0));
        //        return _textGraphic;
        //    }
        //}
    }
}
