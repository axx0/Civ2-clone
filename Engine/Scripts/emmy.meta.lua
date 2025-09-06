---@meta

---@class UnitEffects
---@field Add fun(effect: integer|string, value: integer)

---@class UnitType
---@field Effects UnitEffects

---@class CivCore
---@field UnitEffects { Fanatics: integer, FreeSupport: integer, Partisan: integer, SdiVulnerable: integer }

---@class Civ
---@field core CivCore
---@field getUnitType fun(id: integer): UnitType
---@field TribeName string
---@field cosmic CosmicScripts
---@field ui any
---@field scen any
---@field sleep fun(ms: integer)
---@field getImprovement fun(id: integer): any
---@field getTech fun(id: integer): any
---@field getTile fun(x: integer, y: integer, z: integer): Tile
---@field canEnter fun(unit: any, tile: any): boolean
---@field createUnit fun(unitType: any, owner: any, tile: any): any

-- Declare existing global provided at runtime without changing it:
---@type Civ
_G.civ = _G.civ

---@class AiRoleType
---@field Attack integer
---@field Defend integer
---@field NavalSuperiority integer
---@field AirSuperiority integer
---@field SeaTransport integer
---@field Settle integer
---@field Diplomacy integer
---@field Trade integer

---@type AiRoleType
_G.AiRoleType = _G.AiRoleType

---@class AiEvent
---@field Turn_Start string
---@field Turn_End string
---@field Unit_Orders_Needed string
---@field Research_Complete string

---@type AiEvent
_G.AiEvent = _G.AiEvent

---@class CosmicScripts
---@field roadMultiplier number

-- Tile and Terrain types exposed to Lua
---@class Terrain
---@field isOcean boolean

---@class Tile
---@field city any
---@field defender any
---@field fertility number
---@field grasslandShield boolean
---@field hasGoodieHut boolean
---@field improvements integer
---@field landmass integer
---@field owner any
---@field river boolean
---@field terrain Terrain
---@field terrainType integer
---@field units any
---@field visibility integer
---@field visibleImprovements integer[]
---@field x integer
---@field y integer
---@field z integer
---@field impassable boolean

---@class FastRandom
---@field Next fun(max: integer): integer
---@field NextBool fun(): boolean

---@class Ai
---@field Random FastRandom
---@field civ Civ
---@field difficulty integer
---@field RegisterEvent fun(eventName: string, callback: fun(ai: Ai, data: table): any)
---@field HasEvent fun(eventName: string): boolean
---@field Call fun(eventName: string, args: table): any
---@field GetNearestCity fun(tile: any, inRadiusOnly?: boolean): any
---@field CheckFertility fun(currentTile: any, unit: any): any
---@field RandomTile fun(args: table): Tile
---@field NearestEnemy fun(args: table): Tile
---@field GetPossibleMoves fun(unit: any): any

---@type Ai
_G.ai = _G.ai