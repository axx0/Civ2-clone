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
        public IUnit Attacker, Defender;

        public UnitEventArgs(UnitEventType eventType)
        {
            EventType = eventType;
        }

        public UnitEventArgs(UnitEventType eventType, IUnit attacker, IUnit defender, List<bool> combatRoundsAttackerWins, List<int> attackerHitpoints, List<int> defenderHitpoints)
        {
            EventType = eventType;
            Attacker = attacker;
            Defender = defender;
            CombatRoundsAttackerWins = combatRoundsAttackerWins;
            AttackerHitpoints = attackerHitpoints;
            DefenderHitpoints = defenderHitpoints;
        }
    }
}
