using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Civ2engine.Advances;
using Civ2engine.IO;
using Civ2engine.MapObjects;
using Civ2engine.Production;
using Civ2engine.SaveLoad.SerializationUtils;
using Civ2engine.Units;
using Model.Core;
using Model.Core.Cities;

namespace Civ2engine.SaveLoad;

public class GameSerializer
{
    
    public static readonly CityInfo? DummyCityHere = new();
    
    public void Write(FileStream saveFile, IGame game, Ruleset ruleset, Dictionary<string, string> viewData)
    {
        //For debug purposes we write out formatted json, once we're satisfied it works we can minify and gzip it
        // using var compressor = new GZipStream(saveFile, CompressionMode.Compress);
        using var writer = new Utf8JsonWriter(saveFile, new JsonWriterOptions { Indented = true });

        writer.WriteStartObject();
        writer.WriteNumber("version", 0.1);
        writer.WriteStartObject("extendedMetadata");
        foreach (var pair in ruleset.Metadata)
        {
            writer.WriteString(pair.Key, pair.Value);
        }
        writer.WriteEndObject();
        writer.WriteStartObject("viewData");
        foreach (var pair in viewData)
        {
            writer.WriteString(pair.Key, pair.Value);
        }
        writer.WriteEndObject();
        writer.WriteStartObject("game");
        writer.WriteNonDefaultFields("opts", game.Options);
        writer.WriteNonDefaultFields("data", new JsonGameData(game));
        var encoder = new ImprovementEncoder(game.TerrainImprovements);
        writer.WriteStartObject("encoder");
        foreach (var pair in encoder.EncoderData)
        {
            writer.WriteString(pair.Key, pair.Value);
        }
        writer.WriteEndObject();
        writer.WritePropertyName("maps");
        MapSerializer.Write(writer, game.Maps, encoder);
        writer.WriteStartArray("civs");
        foreach (var civilization in game.AllCivilizations.Skip(1)) // Don't include barbarians at all we know about them
        {
            writer.WriteNonDefaultFields(new JsonCivData(civilization, game.Rules));
        }
        writer.WriteEndArray();
        var productionOrders = ProductionOrder.GetAll(game.Rules);
        writer.WriteStartArray("cities");
        foreach (var city in game.AllCities)
        {
            writer.WriteNonDefaultFields(new JsonCityData(city, game.Rules, productionOrders));
        }
        writer.WriteEndArray();
        writer.WriteStartArray("units");
        var activeUnit = game.ActivePlayer.ActiveUnit;
        int activeUnitIndex = -1;
        int index = 0;
        foreach (var unit in game.AllCivilizations.SelectMany(civilization =>
                     civilization.Units.Where(unit => !unit.Dead)))
        {
            if (unit == activeUnit)
            {
                activeUnitIndex = index;
            }

            index++;
            writer.WriteNonDefaultFields(new JsonUnitData(unit, game.AllCities));
        }

        writer.WriteEndArray();
        if (activeUnitIndex != -1)
        {
            writer.WriteNumber("activeUnit", activeUnitIndex);
        }

        writer.WriteEndObject();
        writer.WriteEndObject();
        
        writer.Flush();
    }


    public static IGame Read(JsonElement gameElement, Ruleset activeRuleSet, Rules rules)
    {
        var options = JsonSerializer.Deserialize<Options>(gameElement.GetProperty("opts").GetRawText());
        var gameData = JsonSerializer.Deserialize<JsonGameData>(gameElement.GetProperty("data").GetRawText());
        ImprovementEncoder? improvementEncoder = null;
        if (gameElement.TryGetProperty("encoder", out var encoderElement))
        {
            improvementEncoder = new ImprovementEncoder(encoderElement);
        }
     
        var gameObjects = new JsonSaveObjects
        {
            Maps = MapSerializer.Read(gameElement.GetProperty("maps"), rules, improvementEncoder),
            Civilizations = new [] {Barbarians.Civilization}
                .Concat(gameElement.GetProperty("civs").Deserialize<JsonCivData[]>()
                    .Select((cd,index) => HydrateCiv(cd,index +1, rules))).ToList()
        };
        
        // Hydrate Cities
        var productionOrders = ProductionOrder.GetAll(rules);
        gameObjects.Cities = gameElement.GetProperty("cities").Deserialize<JsonCityData[]>().Select((data, index) => HydrateCity(data, index, rules, gameObjects.Maps, gameObjects.Civilizations, productionOrders)).ToList();
        
        //Active unit
        var activeUnitIndex = -1;
        if (gameElement.TryGetProperty("activeUnit", out JsonElement activeUnitElement))
        {
            activeUnitIndex = activeUnitElement.GetInt32();
        }
        
        //Hydrate units
        var unitData = gameElement.GetProperty("units").Deserialize<JsonUnitData[]>();
        for (var index = 0; index < unitData.Length; index++)
        {
            var unit = HydrateUnit(unitData[index], rules, gameObjects.Maps, gameObjects.Civilizations,
                gameObjects.Cities);
            if (index == activeUnitIndex)
            {
                gameObjects.ActiveUnit = unit;
            }
        }

        return Game.Create(rules, gameData, gameObjects, activeRuleSet, options);
    }

    private static Unit HydrateUnit(JsonUnitData unitData, Rules rules, List<Map> maps, List<Civilization> civilizations, List<City> cities)
    {
        var x = unitData.X;
        var y = unitData.Y;
        var mapNo = unitData.Z;
        var validTile = maps[mapNo].IsValidTileC2(x, y);

        var civilization = civilizations[unitData.CivId];
        var unit = new Unit
        {
            Id = civilization.Units.Count,
            TypeDefinition = rules.UnitTypes[unitData.TypeId],
            Dead = x < 0 || !validTile,
            CurrentLocation = validTile ? maps[mapNo].TileC2(x, y) : null,
            X = x,
            Y = y,
            MapIndex = mapNo,
            MovePointsLost = unitData.MovePointsLost,
            HitPointsLost = unitData.HitPointsLost,
            MadeFirstMove = unitData.MadeFirstMove,
            Veteran = unitData.Veteran,
            Owner = civilization,
            PrevXy = [unitData.PrevX, unitData.PrevY],
            Order = unitData.Order,
            HomeCity = unitData.HomeCity == 0 ? null : cities[unitData.HomeCity - 1],
            GoToX = unitData.GoToX,
            GoToY = unitData.GoToY,
            CaravanCommodity = unitData.Commodity,
            Counter = unitData.Counter
        };
        
        civilization.Units.Add(unit);
        return unit;
    }

    private static City HydrateCity(JsonCityData cityData, int index, Rules rules, List<Map> maps,
        List<Civilization> civilizations, IProductionOrder[] productionOrders)
    {
        var x = cityData.X;
        var y = cityData.Y;
        var mapNo = cityData.Z;
        var tile = maps[mapNo].TileC2(x, y);
        var owner = civilizations[cityData.OwnerId];
        var city = new City
        {
            X = x,
            Y = y,
            MapIndex = mapNo,
            StolenTech = cityData.StolenTech,
            ImprovementSold = cityData.ImprovementSold,
            WeLoveKingDay = cityData.WeLoveKingDay,
            CivilDisorder = cityData.Disorder,
            Objective = cityData.Objective,
            Owner = owner,
            Size = cityData.Size,
            WhoBuiltIt = civilizations[cityData.Builder],
            FoodInStorage = cityData.FoodStorage,
            ShieldsProgress = cityData.SheildsProgress,
            Name = cityData.Name,
            ItemInProduction = productionOrders[cityData.ProductionOrder],
            CommoditySupplied = cityData.CommoditySupplied?.Where(c => c < rules.CaravanCommoditie.Length)
                .Select(c => rules.CaravanCommoditie[c]).ToArray(),
            CommodityDemanded = cityData.CommodityDemanded?.Where(c => c < rules.CaravanCommoditie.Length)
                .Select(c => rules.CaravanCommoditie[c]).ToArray(),
            //CommodityInRoute = commodityInRoute.Select(c => (CommodityType)c).ToArray(),
            TradeRoutePartnerCity = cityData.TradeRoutePartnerCity,
            //Science = science,    //what does this mean???
            //Tax = tax,
            //NoOfTradeIcons = noOfTradeIcons,
            // HappyCitizens = happyCitizens,
            // UnhappyCitizens = unhappyCitizens,
            Location = tile
        };
        
        owner.Cities.Add(city);

        if (tile.PlayerKnowledge != null)
        {
            foreach (var playerTile in tile.PlayerKnowledge)
            {
                if (playerTile != null && playerTile.CityHere == DummyCityHere)
                {
                    playerTile.CityHere = new CityInfo { Name = city.Name, Size = city.Size, OwnerId = city.OwnerId };
                }
            }
        }

        if (cityData.CommodityInRoute != null && cityData.TradeRoutePartnerCity != null)
        {
            city.TradeRoutes = cityData.CommodityInRoute.Zip(cityData.TradeRoutePartnerCity).Select(((tuple) =>
                new TradeRoute
                {
                    Commodity = rules.CaravanCommoditie[tuple.First % rules.CaravanCommoditie.Length],
                    Destination = tuple.Second
                })).ToArray();
        }

        foreach (var (first, second) in maps[mapNo].CityRadius(tile,true).Zip(cityData.Workers))
        {
            if (first != null && second)
            {
                first.WorkedBy = city;
            }
        }
        
        tile.CityHere = city;

        for (var improvementNo = 0; improvementNo < cityData.Improvements.Length && improvementNo < rules.Improvements.Length -1; improvementNo++)
            if (cityData.Improvements[improvementNo]) city.AddImprovement(rules.Improvements[improvementNo+1]);
        return city;
    }

    private static Civilization HydrateCiv(JsonCivData jsonCivData, int id, Rules rules)
    {
        var style = jsonCivData.CityStyle - 1;
        
        var tribe = rules.Leaders[jsonCivData.TribeId];
        var gender = jsonCivData.Gender;
        
        var gov = rules.Governments[jsonCivData.GovernmentId];
        var leaderTitle = tribe.Titles.FirstOrDefault(t=>t.Gov == jsonCivData.GovernmentId) as IGovernmentTitles ?? gov;
        return new Civilization
        {
            TribeId = jsonCivData.TribeId,
            Id = id,
            Alive = jsonCivData.Alive,
            CityStyle = style == -1 ? tribe.CityStyle : style ,
            LeaderName = string.IsNullOrWhiteSpace(jsonCivData.LeaderName) ? (gender == 1 ? tribe.NameFemale : tribe.NameMale) : jsonCivData.LeaderName,
            LeaderGender = gender,
            LeaderTitle = (gender == 0) ? leaderTitle.TitleMale : leaderTitle.TitleFemale,
            TribeName = string.IsNullOrWhiteSpace(jsonCivData.TribeName) ? tribe.Plural : jsonCivData.TribeName,
            Adjective = string.IsNullOrWhiteSpace(jsonCivData.Adjective) ? tribe.Adjective : jsonCivData.Adjective,
            Money = jsonCivData.Money,
            ReseachingAdvance = jsonCivData.ResearchingAdvance,
            Advances = jsonCivData.Advances ?? [],
            ScienceRate = jsonCivData.SciRate,
            PlayerType = jsonCivData.PlayerType,
            TaxRate = jsonCivData.TaxRate,
            Government = jsonCivData.GovernmentId,
            AllowedAdvanceGroups = tribe.AdvanceGroups ?? [AdvanceGroupAccess.CanResearch]
        };
    }
}