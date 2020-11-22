using System.Drawing;
using civ2.Enums;

namespace civ2.Terrains
{
    internal class Terrain : ITerrain
    {
        public TerrainType Type { get; set; }
        public SpecialType? SpecType { get; set; }

        //From RULES.TXT
        public string Name { get; set; }
        public string SpecName { get; set; }
        public int MoveCost { get; set; }
        public int Defense { get; set; }
        public int Food { get; set; }
        public int Shields { get; set; }
        public int Trade { get; set; }
        public bool CanIrrigate { get; set; }
        public TerrainType? IrrigationResult { get; set; }
        public int IrrigationBonus { get; set; }
        public int TurnsToIrrigate { get; set; }
        public int AIirrigation { get; set; }
        public bool CanMine { get; set; }
        public TerrainType? MiningResult { get; set; }
        public int MiningBonus { get; set; }
        public int TurnsToMine { get; set; }
        public int AImining { get; set; }
        public TerrainType? TransformResult { get; set; }

        public bool Resource { get; set; }
        public bool River { get; set; }
        public bool UnitPresent { get; set; }
        public bool CityPresent { get; set; }
        public bool Irrigation { get; set; }
        public bool Mining { get; set; }
        public bool Road { get; set; }
        public bool Railroad { get; set; }
        public bool Fortress { get; set; }
        public bool Pollution { get; set; }
        public bool Farmland { get; set; }
        public bool Airbase { get; set; }
        public int Island { get; set; }
        public bool[] Visibility { get; set; }
                        
        public string Hexvalue { get; set; }

        //Making a new terrain according to RULES.TXT
        public Terrain(TerrainType type, SpecialType? stype)
        {
            //Regular terrain
            Name = Rules.TechName[(int)(type)];
            MoveCost = Rules.TerrainMovecost[(int)(type)];
            Defense = Rules.TerrainDefense[(int)(type)];
            Food = Rules.TerrainFood[(int)(type)];
            Shields = Rules.TerrainShields[(int)(type)];
            Trade = Rules.TerrainTrade[(int)(type)];
            if (Rules.TerrainIrrigate[(int)(type)] == "yes")
            {
                CanIrrigate = true;
                IrrigationResult = null;
            }
            else if (Rules.TerrainIrrigate[(int)(type)] == "no")
            {
                CanIrrigate = false;
                IrrigationResult = null;
            }
            else
            {
                CanIrrigate = true;
                //IrrigationResult = Rules.TerrainIrrigate[(int)(type)]; TO-DO
            }
            IrrigationBonus = Rules.TerrainIrrigateBonus[(int)(type)];
            TurnsToIrrigate = Rules.TerrainIrrigateTurns[(int)(type)];
            AIirrigation = Rules.TerrainIrrigateAI[(int)(type)];
            if (Rules.TerrainMine[(int)(type)] == "yes")
            {
                CanMine = true;
                MiningResult = null;
            }
            else if (Rules.TerrainMine[(int)(type)] == "no")
            {
                CanMine = false;
                MiningResult = null;
            }
            else
            {
                CanMine = true;
                //MiningResult = Rules.TerrainMine[(int)(type)]; TO-DO
            }
            MiningBonus = Rules.TerrainMineBonus[(int)(type)];
            TurnsToMine = Rules.TerrainMineTurns[(int)(type)];
            AImining = Rules.TerrainMineAI[(int)(type)];
            if (Rules.TerrainTransform[(int)(type)] == "no")
            {
                TransformResult = null;
            }
            else
            {
                //TransformResult = Rules.TerrainTransform[(int)(type)]; TO-DO
            }

            //Special terrain
            if (stype == null)
            {
                SpecName = "";
            }
            else
            {
                SpecName = Rules.TerrainSpecName[(int)(stype)];
                //Overwrite
                MoveCost = Rules.TerrainSpecMovecost[(int)(stype)];
                Defense = Rules.TerrainSpecDefense[(int)(stype)];
                Food = Rules.TerrainSpecFood[(int)(stype)];
                Shields = Rules.TerrainSpecShields[(int)(stype)];
                Trade = Rules.TerrainSpecTrade[(int)(stype)];
            }
        }

        public Bitmap Graphic { get; set; }
    }
}
