

ai.RegisterEvent(AiEvent.Turn_Start, function(ai,d)
    print(ai.civ.TribeName .. " Turn: " .. d.Turn)
end)

local function defender_score(unit)
    local defense = unit.type and unit.type.defense or 0
    local firepower = unit.type and unit.type.firepower or 1
    local hitpoints = unit.hitpoints or 1
    local score = defense * firepower * hitpoints
    if unit.veteran then
        score = score * 1.5
    end
    return score
end

ai.RegisterEvent(AiEvent.Unit_Orders_Needed, function(ai, data)
    local unit = data.Unit;

    print("Looking for orders for " .. unit.type.name)
    local currentTile = unit.location;

    if unit.type.role == AiRoleType.Defend then
        local city = currentTile.city
        if city then
            local defenders_count = 0
            local weakest_defender_score = nil
            local units_here = currentTile.units or {}
            for _, other in pairs(units_here) do
                if other ~= unit and other.type and other.type.role == AiRoleType.Defend then
                    defenders_count = defenders_count + 1
                    local score = defender_score(other)
                    if not weakest_defender_score or score < weakest_defender_score then
                        weakest_defender_score = score
                    end
                end
            end

            local required_defenders = 2 + math.floor((city.Size or 0) / 3)
            local unit_score = defender_score(unit)
            if defenders_count < required_defenders then
                return "F"
            end
            -- Weakest defender score will have a value when there are defenders if no defenders we exit above
            if unit_score > weakest_defender_score then
                return "F"
            end
        end
    end
    
    print(unit.type.role)

    if unit.type.role == AiRoleType.Settle then
        print("Check for city near")
        local isCityRadius = ai.GetNearestCity(currentTile, true)
    
        if isCityRadius then
            --TODO terrain improvements
        else
            return ai.CheckFertility(currentTile, unit)
        end
    end
    
    local moves = ai.GetPossibleMoves(unit)
    if not moves or #moves == 0 then
        return nil
    end

    local target = ai.NearestEnemy({ tile = currentTile, distance = 12, same_landmass = true })
    local current_dist
    if target then
        if ai.Distance then
            current_dist = ai.Distance(currentTile, target)
        else
            current_dist = math.abs(currentTile.x - target.x) + math.abs(currentTile.y - target.y)
        end
    end

    local best_move
    for _, move in ipairs(moves) do
        local action = move.actionType or move.type or move.ActionType
        local priority = ai.Random.Next(20)

        if action == "Attack" then
            priority = priority + 1000
        elseif action == "Move" then
            priority = priority + 200
        elseif action == "Unload" then
            priority = priority + 150
        elseif action == "Fortify" then
            if unit.type.role == AiRoleType.Defend then
                priority = priority + 300
            else
                priority = priority + 50
            end
        end

        if move.tile and target and current_dist then
            local move_dist
            if ai.Distance then
                move_dist = ai.Distance(move.tile, target)
            else
                move_dist = math.abs(move.tile.x - target.x) + math.abs(move.tile.y - target.y)
            end

            if move_dist < current_dist then
                priority = priority + 150
            elseif move_dist > current_dist then
                priority = priority - 50
            end
        end

        move.priority = priority
        if not best_move or move.priority > best_move.priority then
            best_move = move
        end
    end

    return best_move
end)

ai.RegisterEvent(AiEvent.Research_Complete, function(ai, data)
    print("Research_Complete called")
    local possibilities = data.researchPossibilities
    local bestTech = nil
    local bestScore = -1
    for _, tech in pairs(possibilities) do
        local name = tech.name or "unknown"
        local epoch = tech.epoch or 0
        local aiValue = tech.aiValue or 0
        -- Bias towards lower epochs by penalizing higher ones
        local score = aiValue - (epoch * 10)
        
        print("Considering tech: " .. name .. " (Epoch: " .. epoch .. ", Value: " .. aiValue .. ", Score: " .. score .. ")")
        if not bestTech or score > bestScore then
            bestTech = tech
            bestScore = score
        end
    end
    if bestTech then
        print("Selected tech: " .. bestTech.name)
    else
        print("No tech selected")
    end
    return bestTech
end)
