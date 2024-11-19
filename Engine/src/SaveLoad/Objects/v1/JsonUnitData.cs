using System;
using System.Collections.Generic;
using Civ2engine.Units;

namespace Civ2engine.SaveLoad;

public class JsonUnitData
{
    public JsonUnitData()
    {
        
    }
    public JsonUnitData(Unit unit, List<City> cities)
    {
        X = unit.X;
        Y = unit.Y;
        Z = unit.MapIndex;
        CivId = unit.Owner.Id;
        TypeId = unit.Type;
        MovePointsLost = unit.MovePointsLost;
        MadeFirstMove = unit.MadeFirstMove;
        HitPointsLost = unit.HitPointsLost;
        Veteran = unit.Veteran;
        if (unit.PrevXy?.Length == 2)
        {
            PrevX = unit.PrevXy[0];
            PrevY = unit.PrevXy[1];
        }
        Order = unit.Order;
        if (unit.HomeCity != null)
        {
            HomeCity = cities.IndexOf(unit.HomeCity) +1;
        }
        GoToY = unit.GoToY;
        GoToX = unit.GoToX;
        Commodity = unit.CaravanCommodity;
        Counter = unit.Counter;
    }

    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }
    public int CivId { get; set; }
    public int TypeId { get; set; }
    public int MovePointsLost { get; set; }
    public int HitPointsLost { get; set; }
    public bool MadeFirstMove { get; set; }
    public bool Veteran { get; set; }
    public int PrevX { get; set; }
    public int PrevY { get; set; }
    public int Order { get; set; }
    public int HomeCity { get; set; }
    public int GoToX { get; set; }
    public int GoToY { get; set; }
    public int Commodity { get; set; }
    public int Counter { get; set; }
}