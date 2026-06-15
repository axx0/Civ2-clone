using Model.Core;
using Model.Core.Cities;
using Model.Core.Units;

namespace Core.Tests.Units;

// "UnitTests" in two senses: unit tests for the "Unit" class :)
public class UnitTests
{
    [Fact]
    public void UnitLongDescription_YesVeteran_YesHomeCity()
    {
        Civilization civ = new Civilization();
        civ.Adjective = "American";

        City homeCity = new City();
        homeCity.Name = "New York";
        
        UnitDefinition unitDef = new UnitDefinition();
        unitDef.Name = "Catapult";

        Unit unitInstance = new Unit
        {
            TypeDefinition = unitDef,
            Veteran = true,
            Owner = civ,
            HomeCity = homeCity,
        };

        var expected = "American Veteran Catapult (New York)";
        var actual = unitInstance.LongDescription();
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void UnitLongDescription_NoVeteran_YesHomeCity()
    {
        Civilization civ = new Civilization();
        civ.Adjective = "American";

        City homeCity = new City();
        homeCity.Name = "New York";
        
        UnitDefinition unitDef = new UnitDefinition();
        unitDef.Name = "Catapult";

        Unit unitInstance = new Unit
        {
            TypeDefinition = unitDef,
            Veteran = false,
            Owner = civ,
            HomeCity = homeCity,
        };

        var expected = "American Catapult (New York)";
        var actual = unitInstance.LongDescription();
        Assert.Equal(expected, actual);
    }
        
    [Fact]
    public void UnitLongDescription_NoVeteran_NoHomeCity()
    {
        Civilization civ = new Civilization();
        civ.Adjective = "American";
        
        UnitDefinition unitDef = new UnitDefinition();
        unitDef.Name = "Catapult";

        Unit unitInstance = new Unit
        {
            TypeDefinition = unitDef,
            Veteran = false,
            Owner = civ
        };

        var expected = "American Catapult (NONE)";
        var actual = unitInstance.LongDescription();
        Assert.Equal(expected, actual);
    }
}