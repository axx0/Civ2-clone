using Model.Core.Units;

namespace Model.Core.GoodyHuts.Outcomes
{
    internal class TribeOutcome : GoodyHutOutcome
    {
        public override GoodyHutOutcomeResult ApplyOutcome(Unit unit)
        {
            throw new NotImplementedException();
            //if (unit.CurrentLocation != null)
            //{
            //    // Terrain is the determining factor in which is selected..
            //    // Advanced Tribes occur only on Grasslands or Plains. All other terrain types produce nomads.
            //    if (unit.CurrentLocation.Terrain.Type == TerrainType.Grassland || unit.CurrentLocation.Terrain.Type == TerrainType.Plains)
            //    {
            //        // Before 1000AD, all cities founded from Advanced Tribes have a size of 1.
            //        // After that date, the size of the city can be 1 to 4. It will always have a Temple.
            //        // In addition, it has a 50% chance of having a Marketplace, 33% chance of a Granary, and a 25% chance of a Library.
            //        new AdvancedTribeOutcome().ApplyOutcome(unit);
            //    }
            //    else
            //    {
            //        // TODO: Nomads can also occur on Plains or Grasslands when the surrounding terrain makes the site poorly suited for a city.
            //        // TODO: The Nomads result is subject to the Nomads Rule and the Explosives Rule (see below).
            //        // After any Civ acquires Explosives, the Nomad result is suppressed and its chance is added to Gold. Thus, on Nomad-suited terrain, the outcome ratio becomes 0:2:1:1:1
            //        new NomadsOutcome().ApplyOutcome(unit);
            //    }
            //}
        }
    }

    //internal class AdvancedTribeOutcome : TribeOutcome
    //{
    //    public string Name => "Advanced Tribe";
    //    public string Description => "You have discovered an advanced tribe.";

    //    public override void ApplyOutcome(Unit unit)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    //internal class NomadsOutcome : TribeOutcome
    //{
    //    public string Name => "Nomads";
    //    public string Description => "You discover a band of wandering nomads.\r\nThey agree to join your tribe.";
    //    public override void ApplyOutcome(Unit unit)//, Rules rules)
    //    {
    //        // Add Settlers unit to the players units at this unit's location.
    //        var nomad = new Unit
    //        {
    //            Counter = 0,
    //            Dead = false,
    //            Id = unit.Owner.Id,
    //            Order = (int)OrderType.NoOrders,
    //            Owner = unit.Owner,
    //            Veteran = false,
    //            X = unit.CurrentLocation.X,
    //            Y = unit.CurrentLocation.Y,
    //            CurrentLocation = unit.CurrentLocation,
    //            //TypeDefinition = rules.UnitTypes[(int)UnitType.Settlers];
    //        };

    //        unit.Owner.Units.Add(unit);

    //        throw new NotImplementedException();
    //    }
    //}
}
