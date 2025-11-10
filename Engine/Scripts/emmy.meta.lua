---@meta

---@class UnitEffects
---@field Add fun(effect: number|string, value: number)
---@field ContainsKey fun(effect: number|string): boolean

---@class UnitType
---@field Effects UnitEffects
---@field name string
---@field advancedFlags number
---@field attack number
---@field attacksPerTurn number
---@field buildTransport number
---@field cost number
---@field defense number
---@field domain number
---@field expires Tech|nil
---@field firepower number
---@field flags number
---@field hitpoints number
---@field hold number
---@field id number
---@field minimumBribe number
---@field move number
---@field nativeTransport number
---@field notAllowedOnMap number
---@field prereq Tech|nil
---@field prereqId number
---@field range number
---@field role number
---@field tribeMayBuild number
---@field useTransport number
---@field CombatValue number
---@field canEnter fun(tile: Tile|TileApi): boolean

---@class CivCore
---@field UnitEffects { Fanatics: number, FreeSupport: number, Partisan: number, SdiVulnerable: number }
---@field Effects table
---@field Resources table
---@field Improvements table
---@field UnitDomain { Land: number, Air: number, Sea: number }

---@class Tech
---@field id number
---@field name string
---@field researched boolean

---@class Unit
---@field attackSpent number
---@field carriedBy Unit|nil
---@field damage number
---@field domainSpec number
---@field gotoTile TileApi|nil
---@field hitpoints number
---@field homeCity CityApi|nil
---@field id number
---@field location TileApi
---@field moveSpent number
---@field order number
---@field owner Tribe
---@field type UnitType
---@field veteran boolean
---@field activate fun()
---@field teleport fun(tile: TileApi)
---@field GetNum fun(field: string): number
---@field SetNum fun(field: string, value: number)
---@field GetString fun(field: string): string|nil
---@field SetString fun(field: string, value: string|nil)

---@class CityApi
---@field BaseCity City
---@field Name string
---@field Size number
---@field OwnerId number
---@field UnitsHere Unit[]
---@field CityHere boolean

---@class Tribe
---@field Id number
---@field Civ Civilization
---@field TribeName string

---@class Civilization
---@field Id number
---@field TribeName string
---@field Units Unit[]
---@field Cities City[]

---@class City
---@field X number
---@field Y number
---@field MapIndex number
---@field Size number
---@field Owner Civilization
---@field OwnerId number
---@field Name string
---@field Location Tile
---@field UnitsInCity Unit[]
---@field SupportedUnits Unit[]

---@class Civ
---@field core CivCore
---@field getUnitType fun(id: number): UnitType
---@field TribeName string
---@field cosmic CosmicScripts
---@field ui any
---@field scen any
---@field sleep fun(ms: number)
---@field getImprovement fun(id: number): any
---@field getTech fun(id: number): any
---@field getTile fun(x: number, y: number, z: number): Tile
---@field canEnter fun(unit: any, tile: any): boolean
---@field createUnit fun(unitType: any, owner: any, tile: any): Unit

-- Declare existing global provided at runtime without changing it:
---@type Civ
_G.civ = _G.civ

---@class AiRoleType
---@field Attack number
---@field Defend number
---@field NavalSuperiority number
---@field AirSuperiority number
---@field SeaTransport number
---@field Settle number
---@field Diplomacy number
---@field Trade number

---@type AiRoleType
_G.AiRoleType = _G.AiRoleType

---@class AiEvent
---@field Turn_Start string
---@field Turn_End string
---@field Unit_Orders_Needed string
---@field Research_Complete string
---@field Units_Lost string


---@type AiEvent
_G.AiEvent = _G.AiEvent

---@class CosmicScripts
---@field roadMultiplier number

-- Tile and Terrain types exposed to Lua
---@class TerrainApi
---@field isOcean boolean

---@class TileApi
---@field baseTerrain BaseTerrain
---@field city City|nil
---@field defender Tribe|nil
---@field fertility number
---@field grasslandShield boolean
---@field hasGoodieHut boolean
---@field improvements number
---@field landmass number
---@field owner Tribe|nil
---@field river boolean
---@field terrain TerrainApi
---@field terrainType number
---@field units Unit[]
---@field visibility number
---@field visibleImprovements number[]
---@field x number
---@field y number
---@field z number
---@field BaseTile Tile
---@field CityHere City|nil
---@field UnitsHere Unit[]

---@class BaseTerrain
---@field Terrain Terrain

---@class Terrain
---@field isOcean boolean

---@class Tile
---@field city City|nil
---@field defender any
---@field fertility number
---@field grasslandShield boolean
---@field hasGoodieHut boolean
---@field improvements number
---@field landmass number
---@field owner any
---@field river boolean
---@field terrain Terrain
---@field terrainType number
---@field units fun(): Unit[]
---@field visibility number
---@field visibleImprovements number[]
---@field x number
---@field y number
---@field z number
---@field impassable boolean
---@field CityHere City|nil
---@field UnitsHere Unit[]

---@class FastRandom
---@field Next fun(max: number): number
---@field NextBool fun(): boolean

---@class AiTurnData
---@field Turn number

---@class AiUnitOrderData
---@field Unit Unit

---@class AiUnitsLostData
---@field Units Unit[]
---@field By Unit|nil

---@class AiUnitAction
---@field type string
---@field priority number
---@field tile Tile|nil

---@class Ai
---@field Random FastRandom
---@field civ Civ
---@field difficulty number
---@field RegisterEvent fun(eventName: string, callback: fun(ai: Ai, data: AiTurnData|AiUnitOrderData|AiUnitsLostData|table): any)
---@field HasEvent fun(eventName: string): boolean
---@field Call fun(eventName: string, args: table): any
---@field GetNearestCity fun(tile: TileApi|Tile, inRadiusOnly?: boolean): City|nil
---@field CheckFertility fun(currentTile: TileApi|Tile, unit: Unit): any
---@field RandomTile fun(args: table): TileApi
---@field NearestEnemy fun(args: table): TileApi|nil
---@field NearestFriend fun(args: table): TileApi|nil
---@field GetPossibleMoves fun(unit: Unit): AiUnitAction[]

---@type Ai
_G.ai = _G.ai