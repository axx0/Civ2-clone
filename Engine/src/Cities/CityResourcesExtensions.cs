using System;
using System.Linq;
using System.Security.AccessControl;
using Model.Constants;

namespace Civ2engine;

public static class CityResourcesExtensions
{
    private static decimal GetMultiplier(this City city, Effects effect)
    {
        return (100 + city.Improvements
            .Where(i => i.Effects.ContainsKey(effect))
            .Select(b => b.Effects[effect]).Sum()) / 100m;
    }

    private static int GetBaseScience(this City city)
    {
        return city.Trade * city.Owner.ScienceRate / 100;
    }

    public static int GetScience(this City city)
    {
        return (int)(city.GetBaseScience() * city.GetMultiplier(Effects.ScienceMultiplier));
    }

    private static int GetBaseLuxury(this City city)
    {
        return city.Trade * city.Owner.LuxRate / 100;
    }

    public static int GetLuxury(this City city)
    {
        return (int)(city.GetBaseLuxury() * city.GetMultiplier(Effects.LuxMultiplier));
    }

    /// <summary>
    /// Formula should always round excess into tax
    /// </summary>
    public static int GetTax(this City city)
    {
        return (int)((city.Trade - GetBaseLuxury(city) - GetBaseScience(city)) *
                     GetMultiplier(city, Effects.TaxMultiplier));
    }

    public static int GetResourceValues(this City city, string name)
    {
        return name switch
        {
            "Science" => city.GetScience(),
            "Lux" => city.GetLuxury(),
            "Tax" => city.GetTax(),
            "Shields" => city.Production,
            "Food" => city.SurplusHunger,
            _ => throw new NotSupportedException()
        };
    }
    
    public static ResourceValues GetConsumableResourceValues(this City city, string resourceName)
    {
        switch (resourceName)
        {
            case "Food":
                return city.SurplusHunger > 0
                    ? new ResourceValues(consumption: city.Food, surplus: city.SurplusHunger, loss: 0)
                    : new ResourceValues(consumption: city.Food, surplus: 0, loss: -city.SurplusHunger);
            case "Shields":
                return new ResourceValues(consumption: city.Support, surplus: city.Production, loss: city.Waste);
            case "Trade":
                return new ResourceValues(consumption: city.Trade, surplus: 0, loss: city.Corruption);
            default:
                throw new NotImplementedException();
        }
    }
}