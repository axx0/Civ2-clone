using System;
using System.Collections.Generic;
using System.Linq;
using Civ2engine.MapObjects;
using Civ2engine.SaveLoad.SerializationUtils;

namespace Civ2engine.SaveLoad;

public class JsonCityData
{
    public JsonCityData()
    {
        
    }

    public JsonCityData(City city, Rules gameRules, IProductionOrder[] productionOrders)
    {
        X = city.X;
        Y = city.Y;
        Z = city.MapIndex;
        OwnerId = city.OwnerId;
        StolenTech = city.StolenTech;
        ImprovementSold = city.ImprovementSold;
        WeLoveKingDay = city.WeLoveKingDay;
        Disorder = city.CivilDisorder;
        Objective = city.Objective;
        Size = city.Size;
        Builder = city.WhoBuiltIt.Id;
        Name = city.Name;
        ProductionOrder = Array.IndexOf(productionOrders, city.ItemInProduction);
        FoodStorage = city.FoodInStorage;
        SheildsProgress = city.ShieldsProgress;
        CommoditySupplied = city.CommoditySupplied?.Select(c=>c.Id).ToArray();
        CommodityDemanded = city.CommodityDemanded?.Select(c=>c.Id).ToArray();
        TradeRoutePartnerCity = city.TradeRoutePartnerCity;
        CommodityInRoute = city.CommodityInRoute != null ? city.CommodityInRoute.Select(c=>c.Id).ToArray() : null;
        Workers = city.Location.Map.CityRadius(city.Location, true).Select(c=> c != null && c.WorkedBy == city).ToArray().Clamp();
        Improvements = gameRules.Improvements.Select(possible=>city.Improvements.Any(actual=> actual == possible)).ToArray().Clamp();
    }
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }
    public int OwnerId { get; set; }
    public bool StolenTech { get; set; }
    public bool ImprovementSold { get; set; }
    public bool WeLoveKingDay { get; set; }
    public bool Disorder { get; set; }
    public int Objective { get; set; }
    public int Size { get; set; }
    public int Builder { get; set; }
    public string Name { get; set; }
    public int ProductionOrder { get; set; }
    public int FoodStorage { get; set; }
    public int SheildsProgress { get; set; }
    
    public int[]? CommoditySupplied { get; set; }
    public int[]? CommodityDemanded { get; set; }
    public int[]? TradeRoutePartnerCity { get; set; }
    public int[]? CommodityInRoute { get; set; }
    public bool[] Workers { get; set; }
    public bool[] Improvements { get; set; }
}