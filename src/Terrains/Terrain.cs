using System.Drawing;
using civ2.Enums;

namespace civ2.Terrains
{
    internal class Terrain : BaseInstance, ITerrain
    {
        public TerrainType Type { get; set; }
        public SpecialType? SpecType { get; set; }

        // From RULES.TXT
        public string Name { get { return Game.Rules.TerrainName[(int)Type]; } }
        public int MoveCost { get { return Game.Rules.TerrainMovecost[(int)Type]; } }
        public int Defense { get { return Game.Rules.TerrainDefense[(int)Type]; } }
        public int Food { get { return Game.Rules.TerrainFood[(int)Type]; } }
        public int Shields { get { return Game.Rules.TerrainShields[(int)Type]; } }
        public int Trade { get { return Game.Rules.TerrainTrade[(int)Type]; } }
        public bool CanBeIrrigated { get { return Game.Rules.TerrainCanIrrigate[(int)Type]; } }
        public TerrainType IrrigationResult { get; set; }       // TODO: read irrigation result from short name (relevant if CanBeIrrigated = true)
        public int IrrigationBonus { get { return Game.Rules.TerrainIrrigateBonus[(int)Type]; } }
        public int TurnsToIrrigate { get { return Game.Rules.TerrainIrrigateTurns[(int)Type]; } }
        public GovernmentType MinGovrnLevelAItoPerformIrrigation { get; set; }     // TODO: be careful, 0=never!
        public bool CanBeMined { get; set; }    // TODO: canbemined
        public int MiningBonus { get { return Game.Rules.TerrainMineBonus[(int)Type]; } }
        public int TurnsToMine { get { return Game.Rules.TerrainMineTurns[(int)Type]; } }
        public GovernmentType MinGovrnLevelAItoPerformMining { get; set; }      // TODO: be careful, 0=never!

        public TerrainType MiningResult { get; set; }       // TODO: read mining result from short name (relevant if CanBeMined = true)

        public string SpecName { get { return Game.Rules.TerrainSpecName[(int)SpecType]; } }
        
        
 
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
