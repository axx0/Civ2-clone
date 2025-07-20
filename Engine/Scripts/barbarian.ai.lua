
-- First setup structures 

hordes = {}

-- Current the initial level, offset how are behind or ahead barbarians are
local function make_unit_list(current, offset, comp)
    return {
        current,
        comp,
        offset,
        max = 0
    }
end

function standard_comp(first, second)
    return first.CombatValue  > second.CombatValue
end

infantry = make_unit_list(-1, 1)

cav = make_unit_list(0,1)

artillery = make_unit_list(-1,1)

partisans = make_unit_list(-1,0)

fanatics = make_unit_list(-1,0)

transports = make_unit_list(-1,1,  function(first, second)
    return first.hold > second.hold
end)

diplomat = make_unit_list(0,0)


local current_unit_index = 0

local function add_unit(listing, unit) 
    local current = listing.max
    listing.max = current +1
    local comp = listing.comp or standard_comp
    while current > 0 and comp(listing[current -1], unit) do
        listing[current] = listing[current -1]
        current = current -1
    end
    listing[current] = unit
end

local unprocessed_unit = civ.getUnitType(current_unit_index)
while unprocessed_unit do
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
            if unprocessed_unit.move > 1 and unprocessed_unit.move >= unprocessed_unit.hitpoints then
                --cav
                add_unit(cav, unprocessed_unit)
            elseif (unprocessed_unit.flags & 32703) == 0 then
                --artillery
                add_unit(artillery, unprocessed_unit)
            end
        elseif unprocessed_unit.role == 6 then
            add_unit(diplomat, unprocessed_unit)
        end
    elseif unprocessed_unit.domain == 1 then --SEA
        if unprocessed_unit.hold > 0 then
            -- we have a transport
            add_unit(transports, unprocessed_unit)
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
    if next == listing.max then
        return
    end

end

function check_units()
    check_unit(infantry)
end

ai.RegisterEvent(AiEvent.Turn_Start, function(a,d)
    print("Processing Barbarians turn: " .. d.Turn)
    
    check_units()
    
    --if its turn 16 or a multiple select a tile
    --if d.Turn % 16 == 0 then
        candidate = nil
        target = nil
    
        tries = 3
        while not candidate and tries > 0 do
            candidate = a.RandomTile({ global = true})
            
            if(candidate.impassable or (transports.current == -1 and candidate.terrain.isOcean)) then
                candidate = nil
            else
            --don't spawn on top of another player
                near_enemy = a.NearestEnemy({ tile= candidate, distance = 3, same_landmass = true})
    
                if not near_enemy then
                    --don't spawn where there is nothing
                    target = a.NearestEnemy({ tile= candidate, distance = 50, same_landmass = true})
                end
                if not target then
                    candidate = nil
                    tries = tries -1
                end
            end
        end
        -- if tile is free
        
        --spawn a new barbarian horde
    --end
    
end)


ai.RegisterEvent(AiEvent.Unit_Orders_Needed, function(ai, data)
    local unit = data.Unit;
    -- is unit in horde ? move with horde
    
    --collect new horde 
end)