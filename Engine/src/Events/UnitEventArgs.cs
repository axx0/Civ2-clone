using System;
using System.Collections.Generic;
using Civ2engine.Enums;
using Civ2engine.Units;

namespace Civ2engine.Events
{
    public class UnitEventArgs : EventArgs
    {
        public UnitEventType EventType;
        public List<bool> CombatRoundsAttackerWins;
        public List<int> AttackerHitpoints;
        public List<int> DefenderHitpoints;
        public readonly Unit Unit;
        public IUnit Defender;

        public BlockedReason Reason { get; set; }

        public UnitEventArgs(UnitEventType eventType)
        {
            EventType = eventType;
        }
        
        public UnitEventArgs(UnitEventType eventType, Unit subjectUnit, BlockedReason reason)
        {
            EventType = eventType;
            Unit = subjectUnit;
            Reason = reason;
        }

        public UnitEventArgs(UnitEventType eventType, Unit unit, IUnit defender, List<bool> combatRoundsAttackerWins, List<int> attackerHitpoints, List<int> defenderHitpoints)
        {
            EventType = eventType;
            Unit = unit;
            Defender = defender;
            CombatRoundsAttackerWins = combatRoundsAttackerWins;
            AttackerHitpoints = attackerHitpoints;
            DefenderHitpoints = defenderHitpoints;
        }
    }
}
