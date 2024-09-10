using System.Collections.Generic;
using System.Linq;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Model.Constants;

namespace Civ2engine.Units;

public static class UnitExtensions
{
    public static double AttackFactor(this Unit attackUnit, Unit defendingUnit)
    {
        // Base attack factor from RULES
        double af = attackUnit.AttackBase;

        // Bonus for veteran units
        if (attackUnit.Veteran) af *= 1.5;

        // Partisan bonus agains non-combat units
        if (attackUnit.TypeDefinition.Effects.TryGetValue(UnitEffect.Partisan, out var effect) && defendingUnit.AttackBase == 0)
        {
            af *= effect;
        }

        return af;
    }


    public static int DefenseFactor(this Unit defendingUnit, Unit attackingUnit, Tile tile, int groundDefMultiplier)
    {
        //Carried units cannot be the defender
        if (defendingUnit.InShip != null) return 0;

        // Base defense factor from RULES
        decimal df = defendingUnit.DefenseBase;

        // Bonus for veteran units
        if (defendingUnit.Veteran) df *= 1.5m;

        // City walls bonus (applies only to land units)
        if (defendingUnit.Domain == UnitGas.Ground)
        {
            var bestGroundFactor = 0m;
            // Fortress bonus (Applies only to land units. Unit doesn't have to be fortified. Doesn't count if air unit is attacking.)
            if (groundDefMultiplier != 0 && attackingUnit.Domain != UnitGas.Air)
            {
                bestGroundFactor = df * groundDefMultiplier / 100;
            }

            // Fortified bonus
            if (defendingUnit.Order == (int)OrderType.Fortified)
            {
                var fortifiedFactor = df / 2m;
                if (fortifiedFactor > bestGroundFactor)
                {
                    bestGroundFactor = fortifiedFactor;
                }
            }

            //City walls (Note these are summed)
            if (tile.CityHere != null &&
                defendingUnit.Domain == UnitGas.Ground && !attackingUnit.NegatesCityWalls)
            {
                var totalWallDefence =
                    tile.CityHere.Improvements.Sum(i => i.Effects.GetValueOrDefault(Effects.Walled, 0)) / 100m;
                if (totalWallDefence > bestGroundFactor)
                {
                    bestGroundFactor = totalWallDefence;
                }
            }

            df += bestGroundFactor;
        }

        // Helicopters are vulnerable to anti air
        else if (defendingUnit is { Domain: UnitGas.Air, FuelRange: 0 } && attackingUnit.CanAttackAirUnits)
        {
            df /= 2;
        }

        if (tile.CityHere != null)
        {
            if (attackingUnit.Domain == UnitGas.Air)
            {
                if (defendingUnit is { Domain: UnitGas.Air, FuelRange: 1 })
                {
                    // TODO: Message box about fighters scrambling for defence
                    if (attackingUnit.FuelRange != 1)
                    {
                        df *= 4;
                    }
                    else
                    {
                        df *= 2;
                    }
                }
                else
                {
                    int samBonus = 0;
                    int sdiBonus = 0;
                    foreach (var improvement in tile.CityHere.Improvements)
                    {
                        if (improvement.Effects.TryGetValue(Effects.AirDefence, out var sam))
                        {
                            samBonus += sam;
                        }

                        if (improvement.Effects.TryGetValue(Effects.MissileDefence, out var missile))
                        {
                            sdiBonus += missile;
                        }
                    }

                    // Effect of SAM batteries (only when attacked from air)
                    if (samBonus > 0)
                    {
                        //TODO: SAM message?
                        df += df * samBonus / 100m;
                    }

                    if (sdiBonus > 0 &&
                        attackingUnit.TypeDefinition.Effects.TryGetValue(UnitEffect.SDIVulnerable,
                            out var sdimulti) && sdimulti > 0)
                    {
                        df += df * sdiBonus * sdimulti / 100m;
                    }
                }
            }
            else if (attackingUnit.Domain == UnitGas.Sea)
            {
                var seaDefence =
                    tile.CityHere.Improvements.Sum(i => i.Effects.GetValueOrDefault(Effects.SeaDefence, 0));
                if (seaDefence > 0)
                {
                    //TODO: Coastal fortress message
                    df += df * seaDefence / 100m;
                }
            }
        }

        // Effect of terrain
        df *= tile.Defense;

        return (int)df;
    }
}