using System;
using Civ2engine.Enums;
using Model.Core;

namespace Civ2engine;

public class Date : IGameDate
{
    public int StartingYear { get; }
    public int TurnYearIncrement { get; }
    private readonly bool _monthlyTurnIncrement;
    private readonly bool _defaultTurnIncrement;
    private readonly int _difficulty;

    public Date(int startingYear, int turnYearIncrement, int difficulty)
    {
        StartingYear = startingYear == 0 ? -4000 : startingYear;
        TurnYearIncrement = turnYearIncrement;
        _monthlyTurnIncrement = turnYearIncrement < 0;
        _defaultTurnIncrement = turnYearIncrement == 0;
        _difficulty = difficulty;
    }

    private int GameYear(int turnNo)
    {
        int gameYear;
        if (_monthlyTurnIncrement)
        {
            gameYear = StartingYear + turnNo - 1;
        }
        else if (_defaultTurnIncrement)
        {
            gameYear = _difficulty switch
            {
                (int)DifficultyType.Chieftain or (int)DifficultyType.Warlord => StartingYear + Math.Min(250, turnNo - 1) * 20 + Math.Min(50, Math.Max(0, turnNo - 1 - 250)) * 10 + Math.Min(50, Math.Max(0, turnNo - 1 - 300)) * 5 + Math.Min(50, Math.Max(0, turnNo - 1 - 350)) * 2 + Math.Max(0, turnNo - 1 - 400),
                (int)DifficultyType.Prince => StartingYear + Math.Min(60, turnNo - 1) * 50 + Math.Min(40, Math.Max(0, turnNo - 1 - 60)) * 25 + Math.Min(150, Math.Max(0, turnNo - 1 - 100)) * 10 + Math.Min(50, Math.Max(0, turnNo - 1 - 250)) * 5 + Math.Min(50, Math.Max(0, turnNo - 1 - 300)) * 2 + Math.Max(0, turnNo - 1 - 350),
                (int)DifficultyType.King => StartingYear + Math.Min(60, turnNo - 1) * 50 + Math.Min(40, Math.Max(0, turnNo - 1 - 60)) * 25 + Math.Min(50, Math.Max(0, turnNo - 1 - 100)) * 20 + Math.Min(50, Math.Max(0, turnNo - 1 - 150)) * 10 + Math.Min(50, Math.Max(0, turnNo - 1 - 200)) * 5 + Math.Min(50, Math.Max(0, turnNo - 1 - 250)) * 2 + Math.Max(0, turnNo - 1 - 300),
                _ => StartingYear + Math.Min(60, turnNo - 1) * 50 + Math.Min(40, Math.Max(0, turnNo - 1 - 60)) * 25 + Math.Min(75, Math.Max(0, turnNo - 1 - 100)) * 20 + Math.Min(25, Math.Max(0, turnNo - 1 - 175)) * 10 + Math.Min(50, Math.Max(0, turnNo - 1 - 200)) * 2 + Math.Max(0, turnNo - 1 - 250),
            };
        }
        else
        {
            gameYear = StartingYear + turnNo * TurnYearIncrement - 1;
        }
        
        return gameYear;
    }

    public string GameYearString(int turnNo, string separator = " ")
    {
        int gameYear = GameYear(turnNo);

        if (_monthlyTurnIncrement)
        {
            int Nmonth, Nyear = Math.DivRem(gameYear, 12, out Nmonth);
            if (Nmonth < 0)
            {
                Nmonth += 12;
            }
            string month = Nmonth switch
            {
                0 => Labels.For(LabelIndex.Jan),
                1 => Labels.For(LabelIndex.Feb),
                2 => Labels.For(LabelIndex.Mar),
                3 => Labels.For(LabelIndex.Apr),
                4 => Labels.For(LabelIndex.May),
                5 => Labels.For(LabelIndex.June),
                6 => Labels.For(LabelIndex.July),
                7 => Labels.For(LabelIndex.Aug),
                8 => Labels.For(LabelIndex.Sept),
                9 => Labels.For(LabelIndex.Oct),
                10 => Labels.For(LabelIndex.Nov),
                _ => Labels.For(LabelIndex.Dec),
            };
            return string.Join(separator, month, Math.Abs(Nyear));
        }
        else
        {
            return gameYear < 0 ?
                string.Join(separator, Math.Abs(gameYear).ToString(), Labels.For(LabelIndex.BC)) :
                string.Join(separator, Labels.For(LabelIndex.AD), gameYear.ToString());
        }
    }

}