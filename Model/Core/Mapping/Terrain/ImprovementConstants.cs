namespace Civ2engine.Terrains
{
    public static class ImprovementConstants
    {
        public const int Movement = 1;
        
        public const int Food = 3;
        public const int Shields = 4;
        public const int Trade = 5;
        public const int Transform = 6;
        public const int Airbase = 7;
        
        public const int NoStackElimination = 8;
        
        /// <summary>
        /// Any square with this improvement will not be auto selected for pollution placement
        ///  It can still be polluted with nukes, building this will not remove pollution
        /// </summary>
        public const int PreventPollutionInSquare = 9;
        
        public const int GroundDefence = 20;
    }
}   