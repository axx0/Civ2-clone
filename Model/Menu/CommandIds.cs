namespace Model.Menu;

public static class CommandIds
{
    public const string GameOptions = "GAME_OPTIONS";
    public const string GraphicOptions = "GRAPHIC_OPTIONS";
    public const string CityReportOptions = "CITY_REPORT_OPTIONS";
    public const string SaveGame = "SAVE_GAME";
    public const string LoadGame = "LOAD_GAME";
    public const string QuitGame = "GAME_QUIT";

    public const string ZoomIn = "ZOOM_IN";
    public const string ZoomOut = "ZOOM_OUT";
    public const string MaxZoomIn = "MAX_ZOOM_IN";
    public const string StandardZoom = "STANDARD_ZOOM";
    public const string MediumZoomOut = "MEDIUM_ZOOM_OUT";
    public const string MaxZoomOut = "MAX_ZOOM_OUT";
    public const string ShowMapGrid = "SHOW_MAP_GRID";
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
    
    public const string SaveScenario = "SAVE_SCENARIO";
    public const string LoadScenario = "LOAD_SCENARIO";
    public const string SaveMap = "SAVE_MAP";
    public const string LoadMap = "LOAD_MAP";
    
    public const string CheatRevealMapCommand = "CHEAT_REVEAL_MAP";
}