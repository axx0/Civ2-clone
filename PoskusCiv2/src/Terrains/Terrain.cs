using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Terrains
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
                        
        public string Hexvalue { get; set; }

        //Making a new terrain according to RULES.TXT
        public Terrain(TerrainType type, SpecialType? stype)
        {
            //Regular terrain
            Name = ReadFiles.TechName[(int)(type)];
            MoveCost = ReadFiles.TerrainMovecost[(int)(type)];
            Defense = ReadFiles.TerrainDefense[(int)(type)];
            Food = ReadFiles.TerrainFood[(int)(type)];
            Shields = ReadFiles.TerrainShields[(int)(type)];
            Trade = ReadFiles.TerrainTrade[(int)(type)];
            if (ReadFiles.TerrainIrrigate[(int)(type)] == "yes")
            {
                CanIrrigate = true;
                IrrigationResult = null;
            }
            else if (ReadFiles.TerrainIrrigate[(int)(type)] == "no")
            {
                CanIrrigate = false;
                IrrigationResult = null;
            }
            else
            {
                CanIrrigate = true;
                //IrrigationResult = ReadFiles.TerrainIrrigate[(int)(type)]; TO-DO
            }
            IrrigationBonus = ReadFiles.TerrainIrrigateBonus[(int)(type)];
            TurnsToIrrigate = ReadFiles.TerrainIrrigateTurns[(int)(type)];
            AIirrigation = ReadFiles.TerrainIrrigateAI[(int)(type)];
            if (ReadFiles.TerrainMine[(int)(type)] == "yes")
            {
                CanMine = true;
                MiningResult = null;
            }
            else if (ReadFiles.TerrainMine[(int)(type)] == "no")
            {
                CanMine = false;
                MiningResult = null;
            }
            else
            {
                CanMine = true;
                //MiningResult = ReadFiles.TerrainMine[(int)(type)]; TO-DO
            }
            MiningBonus = ReadFiles.TerrainMineBonus[(int)(type)];
            TurnsToMine = ReadFiles.TerrainMineTurns[(int)(type)];
            AImining = ReadFiles.TerrainMineAI[(int)(type)];
            if (ReadFiles.TerrainTransform[(int)(type)] == "no")
            {
                TransformResult = null;
            }
            else
            {
                //TransformResult = ReadFiles.TerrainTransform[(int)(type)]; TO-DO
            }

            //Special terrain
            if (stype == null)
            {
                SpecName = "";
            }
            else
            {
                SpecName = ReadFiles.TerrainSpecName[(int)(stype)];
                //Overwrite
                MoveCost = ReadFiles.TerrainSpecMovecost[(int)(stype)];
                Defense = ReadFiles.TerrainSpecDefense[(int)(stype)];
                Food = ReadFiles.TerrainSpecFood[(int)(stype)];
                Shields = ReadFiles.TerrainSpecShields[(int)(stype)];
                Trade = ReadFiles.TerrainSpecTrade[(int)(stype)];
            }

        }
    }
}
