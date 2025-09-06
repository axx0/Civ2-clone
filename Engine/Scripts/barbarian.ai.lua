-- First setup structures 
-- Current the initial level, offset how are behind or ahead barbarians are
local function make_unit_list(name, current, offset, comp)
    return {
        name = name,
        current = current,  -- explicitly name the field
        comp = comp,
        offset = offset,
        max = 0,
        items = {}
    }
end

function standard_comp(first, second)
    return first.CombatValue  > second.CombatValue
end

infantry = make_unit_list("inf",-1, 1)

cav = make_unit_list("cav",0,1)

artillery = make_unit_list("art",-1,1)

partisans = make_unit_list("part",-1,0)

fanatics = make_unit_list("fan",-1,0)

transports = make_unit_list("ship",0,1,  function(first, second)
    return first.hold > second.hold
end)

diplomat = make_unit_list("dip",0,0)


local current_unit_index = 0

local function add_unit(listing, unit) 
    local current = listing.max
    listing.max = current +1
    local comp = listing.comp or standard_comp
    while current > 0 and comp(listing.items[current -1], unit) do
        listing.items[current] = listing.items[current -1]
        current = current -1
    end
    listing.items[current] = unit
end

local unprocessed_unit = civ.getUnitType(current_unit_index)
local moveMultiplier = civ.cosmic.roadMultiplier
while unprocessed_unit do
    if unprocessed_unit.prereqId ~= -2 then -- barbarians don't consider unbuildable units
        if unprocessed_unit.domain == 0 then --land
            if unprocessed_unit.role == 1 then
                if unprocessed_unit.Effects.ContainsKey(civ.core.UnitEffects.Fanatics) then
                    add_unit(fanatics, unprocessed_unit)
                elseif unprocessed_unit.Effects.ContainsKey(civ.core.UnitEffects.Partisan) then
                    add_unit(partisans, unprocessed_unit)
                elseif unprocessed_unit.flags == 0 then --simple land unit
                    add_unit(infantry, unprocessed_unit)
                end
            elseif unprocessed_unit.role == 0 then
                local move = unprocessed_unit.move / moveMultiplier
                if move > 1 and move >= unprocessed_unit.hitpoints / 10 then
                    --cav
                    add_unit(cav, unprocessed_unit)
                elseif (unprocessed_unit.flags & 32703) == 0 then
                    --artillery
                    add_unit(artillery, unprocessed_unit)
                end
            elseif unprocessed_unit.role == 6 then
                add_unit(diplomat, unprocessed_unit)
            end
        elseif unprocessed_unit.domain == 2 then --SEA
            if unprocessed_unit.role == 4 then
                -- we have a transport
                add_unit(transports, unprocessed_unit)
            end
        end
    end
    
    current_unit_index = current_unit_index +1
    unprocessed_unit = civ.getUnitType(current_unit_index)
end

function check_unit(listing)    
    local next = listing.current + 1 + listing.offset;
    if next < 0 then
        next = 0
    end
    if listing.items[next] == nil then
        print("No more items")
        return
    end
    
    local unit = listing.items[next]
    local tech = unit.prereq
    if not tech or tech.researched then
        listing.current = next
    end    
end

function check_units()
    check_unit(infantry)
    check_unit(cav)
    check_unit(artillery)
    check_unit(fanatics)
    check_unit(partisans)
    check_unit(transports)
end

function create_at(listing, owner, location)
    ---@diagnostic disable-next-line: undefined-global
    return civ.createUnit(listing.items[listing.current],owner,location)
end

function get_unit_type(ai)
    local cav_first = ai.Random.NextBool()
    if cav_first then
        if cav.current ~= -1 then
            return cav
        end
    end
    if infantry.current ~= -1 then
        return infantry
    end
    return cav
end 
    
---@diagnostic disable-next-line: undefined-global
ai.RegisterEvent(AiEvent.Turn_Start, function(a,d)
    print("Processing Barbarians turn: " .. d.Turn)
    
    check_units()
    
    --if its turn 16 or a multiple select a tile
    --if d.Turn % 16 == 0 then
        local candidate = nil
        local target = nil
        local tries = 3
    
        while tries > 0 do
            candidate = a.RandomTile({ global = true })
            target = nil

            if candidate and not candidate.impassable and not (transports.current == -1 and candidate.terrain.isOcean) then
                local near_enemy = a.NearestEnemy({ tile = candidate, distance = 3, same_landmass = true })
                if not near_enemy then
                    target = a.NearestEnemy({ tile = candidate, distance = 50, same_landmass = true })
                end
            end
        
            if target then
                break
            end
        
            candidate = nil
            tries = tries - 1
        end
        
        if candidate then
            -- target is guaranteed non-nil here by the loop invariant
            -- ...
            if candidate.terrain.isOcean then                
                local ship = create_at(transports, a, candidate)
                local cargo = ship.type.hold
                local cargo_type = get_unit_type(a)
                while cargo > 0 do
                    create_at(cargo_type, a, candidate)
                    cargo = cargo -1
                end
            else
                local unit_type = get_unit_type(a)
                print("Attempting to create " .. unit_type.name .. " item " .. unit_type.current)
                create_at(unit_type, a, candidate)
                
                
            end
        end
    --end
    
end)


---@diagnostic disable-next-line: undefined-global
ai.RegisterEvent(AiEvent.Unit_Orders_Needed, function(ai, data)
    local unit = data.Unit;
    -- is unit in horde ? move with horde
    
    --collect new horde 
end)