

ai.RegisterEvent(AiEvent.Turn_Start, function(ai,d)
    print(ai.civ.TribeName .. " Turn: " .. d.Turn)
end)

ai.RegisterEvent(AiEvent.Unit_Orders_Needed, function(ai, data)
    local unit = data.Unit;

    print("Looking for orders for " .. unit.Name)
    local currentTile = unit.CurrentLocation;

    if unit.AiRole == AiRoleType.Defend then
        if (currentTile.CityHere) then
    
            --if (currentTile.UnitsHere.Count(u => u != unit && u.AiRole == AiRoleType.Defend) <
        -- 2 + currentTile.CityHere.Size / 3)
            return "F"
        end
    end
    
    print(unit.AiRole)

    if unit.AiRole == AiRoleType.Settle then
        local isCityRadius = ai.GetNearestCity(currentTile, true)
    
        if isCityRadius then
            --TODO terrain improvements
        else
            if currentTile.Fertility == -2 then
                return "B"
            end
            local fertile = ai.CheckFertility(currentTile, unit)
            if fertile then
                return fertile
            end
        end
    end
    
    local moves = ai.GetPossibleMoves(unit)
    local best = moves[1]
    for move in moves do
        move.Priority = ai.Random.Next(50)
        
        -- Here evaluate the move and weigh it up or down
        if move.Priority > best.Priority then
            best = move
        end
    end
    
    return best
end)