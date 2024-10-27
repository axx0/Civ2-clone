namespace Model.Menu;

public static class CommandIds
{
    public const string GameOptions = "GAME_OPTIONS";
    public const string GraphicOptions = "GRAPHIC_OPTIONS";
    public const string CityReportOptions = "CITY_REPORT_OPTIONS";
    public const string QuitGame = "GAME_QUIT";

    public const string MapLayoutToggle = "MAP_LAYOUT";
    
    public const string WaitOrder = "UNIT_ORDER_WAIT";
    public const string UnloadOrder = "UNIT_ORDER_UNLOAD";
    public const string SleepOrder = "UNIT_ORDER_SLEEP";
    public const string SkipOrder = "UNIT_ORDER_SKIP";
    public const string PillageOrder = "UNIT_ORDER_PILLAGE";
    public const string BuildCityOrder = "UNIT_ORDER_BUILD_CITY";
    public const string GotoOrder = "UNIT_ORDER_GOTO";
    public const string FortifyOrder = "UNIT_ORDER_FORTIFY";
    public const string BuildImprovementOrderBase = "UNIT_ORDER_BUILD_IMPROVEMENT";
    public const string BuildImprovementOrderNormal = BuildImprovementOrderBase + "_NORMAL";
    public const string BuildImprovementOrderForeground = BuildImprovementOrderBase + "_FRONT";
    public const string RemoveNegativeImprovementOrder = BuildImprovementOrderBase + "_NEGATIVE";

    public const string BuildRoadOrder = BuildImprovementOrderNormal + "_ROAD";
    public const string BuildIrrigationOrder = BuildImprovementOrderNormal + "_IRRIGATION";
    public const string BuildMineOrder = BuildImprovementOrderNormal + "_MINE";
    
    public const string EndTurn = "END_PLAYER_TURN";
    
    public const string OpenLuaConsole = "OPEN_LUA_CONSOLE";
}