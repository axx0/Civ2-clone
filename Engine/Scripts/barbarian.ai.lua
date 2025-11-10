-- First setup structures 
-- Current the initial level, offset how are behind or ahead barbarians are
local function make_unit_list(name, current, offset, comp)
    return {
        name = name,
        current = current,
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

diplomats = make_unit_list("dip",0,0)


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
                add_unit(diplomats, unprocessed_unit)
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

print("Units processed")

function check_unit(listing)
    print("Checking " .. listing.name)
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

print("Unit parse successful")

diplomat = -1

function check_units()
    print("Clearing" .. diplomat)
    diplomat = -1
    check_unit(infantry)
    check_unit(cav)
    check_unit(artillery)
    check_unit(fanatics)
    check_unit(partisans)
    check_unit(transports)
end

hordes = {}

function create_at(listing, owner, location, horde)
    local u = civ.createUnit(listing.items[listing.current],owner,location)
    u.SetNum("horde", horde)
    hordes[horde].count = hordes[horde].count + 1 
    return u
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

function get_free_horde()
    print("Finding horde")
    for i = 1, #hordes do
        if hordes[i].count == 0 then
            print("Reusing exhausted horde" .. i)
            return i
        end
    end
    table.insert(hordes, {})
    return #hordes
end

unit_type_counter = {}
    
function move_hordes(ai)
    for i = 1, #hordes do        
        if hordes[i].count > 0 then
            print("Moving horde " .. i)
            if hordes[i].target == nil or hordes[i].target == hordes[i].location then
                hordes[i].target = ai.NearestEnemy({ tile = hordes[i].location, distance = 50, same_landmass = true })
            end            
            if hordes[i].target then
                print("From " .. hordes[i].location.x .. "-" .. hordes[i].location.y)
                hordes[i].location = ai.LocationTowards(hordes[i])
                print("To " .. hordes[i].location.x .. "-" .. hordes[i].location.y)
                print("Target " .. hordes[i].target.x .. "-" .. hordes[i].target.y)
            end
        end
    end
end

ai.RegisterEvent(AiEvent.Turn_Start, function(a,d)
    print("Processing Barbarians turn: " .. d.Turn)

    check_units()
    
    --if its turn 16 or a multiple select a tile
    if d.Turn % 16 == 0 then
        local candidate
        local target
        local tries = 3
    
        while tries > 0 do
            print("Finding spawn" .. tries)
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
            local horde = get_free_horde()   
            
            hordes[horde] = {
                target = target,
                location = candidate,
                count = 0,
                speed = 1
            }
            
            if candidate.terrain.isOcean then                
                local ship = create_at(transports, a, candidate, horde)
                local cargo = ship.type.hold
                local cargo_type = get_unit_type(a)
                while cargo > 1 do
                    create_at(cargo_type, a, candidate, horde)
                    cargo = cargo -1
                end
                -- if we have a leader type put them last on the boat otherwise just fill it up
                if diplomats.current ~= -1 then
                    create_at(diplomats, a, candidate, horde)
                else
                    create_at(cargo_type, a, candidate, horde)
                end
                hordes[horde].speed = ship.type.move
            else
                local unit_type = get_unit_type(a)
                print("Attempting to create " .. unit_type.name .. " item " .. unit_type.current)
                local first_unit = create_at(unit_type, a, candidate, horde)
                hordes[horde].speed = first_unit.type.move
                if diplomats.current ~= -1 then
                    create_at(diplomats, a, candidate, horde)
                end
            end
        end
    end

    move_hordes(a)
end)

ai.RegisterEvent(AiEvent.Units_Lost, function(ai, data)
    for _, unit in ipairs(data.Units) do
        local horde = unit:GetNum("horde")
        if horde >= 0 then
            print("Unit from horde " .. horde .. " was lost")
            -- Update horde tracking, send reinforcements, etc.
            hordes[horde].count = hordes[horde].count - 1
        end
    end

    -- If we know what killed them
    if data.By then
        print("Killed by: " .. data.By.type.name)
    end
end)

ai.RegisterEvent(AiEvent.Unit_Orders_Needed, function(ai, data)
    local unit = data.Unit;
    print("Finding orders for " .. unit.type.name)
    -- Move leaders last and to nearest friendly unit
    if unit.type.role == AiRoleType.Diplomacy then
        if diplomat == unit.id then
            diplomat = -2
        end
        if diplomat == -2 then
            friend = ai.NearestFriend({ tile = unit.location, distance = unit.type.move, unit_type= unit.type.id })
            if friend then
                return friend
            end
            -- TODO: Run away from enemies if no friends around
            return nil
        end
    
        if diplomat == -1 then
            diplomat = unit.id
        end
        return "w"
    end
    -- is unit not in a horde, find one
    local horde = unit:GetNum("horde")
    if not horde then
        friend = ai.NearestFriend({ tile = unit.location, distance = 5, in_horde= true})
        if friend then
            -- Loop over units at the friend tile to find one with a horde set
            for _, friend_unit in pairs(friend.units) do
                local horde_id = friend_unit:GetNum("horde")
                if horde_id > 0 then
                    print("Found unit from horde " .. horde_id .. " at friend location")
                    horde = horde_id
                    break
                end
            end
        else
            --collect new horde 
            horde = get_free_horde()
            hordes[horde] = {
                location = unit.location,
                target = a.NearestEnemy({ tile = unit.location, distance = 50, same_landmass = true }),
                speed = unit.type.move,
                count = 0
            }
        end
        unit:SetNum("horde", horde)
        hordes[horde].count = hordes[horde].count + 1
    end
    
    local moves = ai.GetPossibleMoves(unit)

    -- Weight moves based on type
    local best_move
    
    local hLoc = hordes[horde].location
    local current_dist = ai.Distance(unit.location, hLoc)
    
    print("Current dist " .. current_dist .. " from " .. hLoc.x .. "-" .. hLoc.y .. " " .. hLoc.landmass)

    for _, move in ipairs(moves) do

        -- Weight by move type
        if move.actionType == "Attack" then
            move.priority = 1000  -- Prioritize attacks
        elseif move.actionType == "Unload" then
            if move.tile.landmass == hordes[horde].target.landmass then
                move.priority = 2000
            end
        elseif move.actionType == "Move" then
            move.priority = 500   -- Regular movement
        elseif move.actionType == "Fortify" then
            move.priority = 100   -- Low priority for fortifying
        end

        -- Additional weighting factors can be added here
        -- For example, prefer moves toward target
        if move.tile and hordes[horde] and hordes[horde].location then
            local move_dist = ai.Distance(move.tile, hordes[horde].location)
            if move_dist < current_dist then
                move.priority = move.priority + 200
            elseif move_dist > current_dist then
                move.priority = move.priority - 100
            end
        end

        print("Final priority for " .. move.actionType .. ": " .. move.priority .. " " .. move.tile.x .. "-" .. move.tile.y .. " " .. move.tile.landmass)
        if not best_move or move.priority > best_move.priority then
            print("New best")
            best_move = move
        end
    end

    if best_move and best_move.tile then
        print("Selected " .. best_move.actionType .. " to tile " .. best_move.tile.x .. "-" .. best_move.tile.y)
    end
    return best_move
end)