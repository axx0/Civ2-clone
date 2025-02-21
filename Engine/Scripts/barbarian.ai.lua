ai.RegisterEvent(AiEvent.Turn_Start, function(a,d)
    print("Processing Barbarians turn: " .. d.Turn)
end)