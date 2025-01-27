using Civ2engine.Enums;
using Model.Constants;

namespace Civ2engine.Units
{
    public class UnitDefinition
    {
        public string Name { get; set; }
        public int Until { get; set; }
        public UnitGas Domain { get; set; }
        public int Move { get; set; }
        public int Range { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int Hitp { get; set; }
        public int Firepwr { get; set; }
        public int Cost { get; set; }
        public int Hold { get; set; }
        public AiRoleType AIrole { get; set; }
        public int Prereq { get; set; }
        public string Flags { get; set; }
        public int Type { get; set; }
        public string AttackSound { get; set; }
        
        public bool IsSettler { get; set; }
        
        public int WorkRate { get; set; }
        public bool IsEngineer { get; set; }
        public bool[] CivCanBuild { get; set; }
        public bool[] CanBeOnMap { get; set; }
        public int MinBribe { get; set; }
        public bool Invisible { get; set; }
        public bool NonDispandable { get; set; }
        public bool UnbribaleBarb { get; set; }
        public bool NothingImpassable { get; set; }
        public bool NonExpireForBarbarian { get; set; }

        public Dictionary<UnitEffect, int> Effects { get; } = new();
    }
}