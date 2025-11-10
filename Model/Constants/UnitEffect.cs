namespace Civ2engine.Units;

public enum UnitEffect
{
    /// <summary>
    /// Unit behaves as a partisan with attack bonus against non-combat units.
    /// </summary>
    Partisan = 1,
    
    /// <summary>
    /// Vulnerable to SDI defence
    /// </summary>
    SDIVulnerable = 2,
    
    /// <summary>
    /// Unit behaves as fanatics
    /// </summary>
    Fanatics = 4,
    
    /// <summary>
    /// Unit is free for specified government (since 
    /// </summary>
    FreeSupport = 8
}