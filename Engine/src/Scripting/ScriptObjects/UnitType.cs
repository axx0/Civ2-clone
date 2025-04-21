using System.Collections.Generic;
using Civ2engine.Advances;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Civ2engine.UnitActions;
using Civ2engine.Units;
using Model.Constants;
using Model.Core.Units;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace Civ2engine.Scripting.ScriptObjects;

public class UnitType(UnitDefinition unitDefinition, Game game)
{
    internal UnitDefinition BaseDefinition => unitDefinition;
    
    /// <summary>
    /// Returns the name of the unit type.
    /// </summary>
    public string name
    {
        get => unitDefinition.Name;
        set => unitDefinition.Name = value;
    }

    /// <summary>
    /// Returns the 'advanced flags' settings of the unit type (bitmask).
    /// </summary>
    public int advancedFlags
    {
        get =>
            Utils.ToBitmask([
                unitDefinition.Invisible,
                unitDefinition.NonDispandable,
                unitDefinition.UnbribaleBarb,
                unitDefinition.NothingImpassable,
                unitDefinition.IsEngineer,
                unitDefinition.NonExpireForBarbarian
            ]);
        set
        {
            var extraFlags = Utils.FromBitmask(value);
            unitDefinition.Invisible = extraFlags[0];
            unitDefinition.NonDispandable = extraFlags[1];
            unitDefinition.UnbribaleBarb = extraFlags[3];
            unitDefinition.NothingImpassable = extraFlags[4];
            unitDefinition.IsEngineer = extraFlags[5];
            unitDefinition.NonExpireForBarbarian = extraFlags[6];
        }
    }
    
    /// <summary>
    /// Returns the attack factor of the unit type.
    /// </summary>
    public int attack
    {
        get => unitDefinition.Attack;
        set => unitDefinition.Attack = value;
    }
    
    /// <summary>
    /// Returns the number of attacks available per turn of the unit type (from the 'Attacks per turn' patch).
    /// </summary>
    /// <param name="???"></param>
    public int attacksPerTurn
    {
        get => unitDefinition.AttackPerTurn;
        set => unitDefinition.AttackPerTurn = value;
    }
    
    /// <summary>
    /// Returns the 'build transport site' settings of the unit type (bitmask).
    /// </summary>
    /// <param name="???"></param>
    /// <returns></returns>
    public int buildTransport
    {
        get => Utils.ToBitmask(unitDefinition.CanBuildMapLink);
        set => unitDefinition.CanBuildMapLink = Utils.FromBitmask(value);
    }
    
    /// <summary>
    /// Returns the cost of the unit type.
    /// </summary>
    /// <returns></returns>
    public int cost
    {
        get => unitDefinition.Cost;
        set => unitDefinition.Cost = value;
    }
    
    /// <summary>
    /// Returns the defense factor of the unit type.
    /// </summary>
    public int defense
    {
        get => unitDefinition.Defense;
        set => unitDefinition.Defense = value;
    }

    /// <summary>
    /// Returns the domain of the unit type (0 - Ground, 1 - Air, 2 - Sea).
    /// </summary>
    public int domain
    {
        get => (int)unitDefinition.Domain;
        set => unitDefinition.Domain = (UnitGas)value;
    }

    /// <summary>
    /// Returns the tech that renders the unit obsolete, or `nil` if there isn't any.
    /// </summary>
    public Tech expires
    {
        get => new(game.Rules.Advances, unitDefinition.Until);
        set => unitDefinition.Until = value.id;
    }

    /// <summary>
    /// Returns the firepower of the unit type.
    /// </summary>
    public int firepower
    {
        get => unitDefinition.Firepwr;
        set => unitDefinition.Firepwr = value;
    }

    /// <summary>
    /// Returns the flags of the unit type (bitmask).
    ///     TODO: Update effects based on flags set
    /// </summary>
    public int flags
    {
        get => Utils.ToBitmask(unitDefinition.Flags);
        set => unitDefinition.Flags = Utils.FromBitmask(value);
    }
    
    /// <summary>
    /// Returns the number of hit points of the unit type.
    /// </summary>
    public int hitpoints
    {
        get => unitDefinition.Hitp;
        set => unitDefinition.Hitp = value;
    }
    
    /// <summary>
    /// Returns the number of holds of the unit type.
    /// </summary>
    public int hold
    {
        get => unitDefinition.Hold;
        set => unitDefinition.Hold = value;
    }

    /// <summary>
    /// Returns the id of the unit type.
    /// </summary>
    public int id => unitDefinition.Type;
    
    /// <summary>
    /// Returns the minimum amount to bribe the unit type.
    /// </summary>
    public int minimumBribe
    {
        get => unitDefinition.MinBribe;
        set => unitDefinition.MinBribe = value;
    }

    /// <summary>
    /// Returns the movement rate of the unit type.
    /// </summary>
    public int move
    {
        get => unitDefinition.Move;
        set => unitDefinition.Move = value;
    }

    /// <summary>
    /// Returns the 'native transport' settings of the unit type (bitmask).
    /// </summary>
    public int nativeTransport
    {
        get => Utils.ToBitmask(unitDefinition.CanMoveWithoutLink);
        set => unitDefinition.CanMoveWithoutLink = Utils.FromBitmask(value);
    }

    /// <summary>
    /// Returns the 'not allowed on map' settings of the unit type (bitmask).
    /// </summary>
    public int notAllowedOnMap
    {
        get => Utils.ToBitmask(unitDefinition.CanBeOnMap);
        set => unitDefinition.CanBeOnMap = Utils.FromBitmask(value);
    }

    /// <summary>
    /// Returns the prerequisite technology of the unit type, or `nil` if it doesn't have one.
    /// </summary>
    /// <returns></returns>
    public Tech? prereq
    {
        get => unitDefinition.Prereq is AdvancesConstants.Nil or AdvancesConstants.No
            ? null
            : new Tech(game.Rules.Advances, unitDefinition.Prereq);
        set => unitDefinition.Prereq = value?.id ?? AdvancesConstants.Nil;
    }
    
    /// <summary>
    /// Returns the range of the unit type.
    /// </summary>
    public int range
    {
        get => unitDefinition.Range;
        set => unitDefinition.Range = value;
    }

    /// <summary>
    /// Returns the role of the unit type.
    /// </summary>
    public int role
    {
        get => (int)unitDefinition.AIrole;
        set => unitDefinition.AIrole = (AiRoleType)value;
    }
    
    /// <summary>
    /// Returns the 'tribe may build' settings of the unit type (bitmask).
    /// </summary>
    /// <returns></returns>
    public int tribeMayBuild
    {
        get => Utils.ToBitmask(unitDefinition.CivCanBuild);
        set => unitDefinition.CivCanBuild = Utils.FromBitmask(value);
    }

    /// <summary>
    /// Returns the 'use transport site' settings of the unit type (bitmask).
    /// </summary>
    public int useTransport
    {
        get => Utils.ToBitmask(unitDefinition.CanUseMapLink);
        set => unitDefinition.CanUseMapLink = Utils.FromBitmask(value);
    }

    /// <summary>
    /// Alias for `civ.canEnter(unittype, tile)
    /// </summary>
    /// <param name="tile"></param>
    /// <returns></returns>
    public bool canEnter(object tile)
    {
        return tile switch
        {
            TileApi apiTile => UnitFunctions.CanEnter(unitDefinition.Domain, apiTile.BaseTile),
            Tile coreTile => UnitFunctions.CanEnter(unitDefinition.Domain, coreTile),
            _ => false
        };
    }

    public Dictionary<UnitEffect, int> Effects => unitDefinition.Effects;
}