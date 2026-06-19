using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.IO;
using Civ2engine.UnitActions;
using Model.Core.Units;
using Model.Core.Cities;

namespace RaylibUI.RunGame.GameControls.CityControls;

internal static class CityUnitMenu
{
    private const int ActivateUnitIndex = 0;
    private const int NoOrdersIndex = 1;
    private const int SleepIndex = 2;
    private const int FortifyIndex = 3;
    private const int DisbandIndex = 4;
    private const int ZoomIndex = 5;

    public static void Show(CityWindow cityWindow, Unit unit)
    {
        var screen = cityWindow.CurrentGameScreen;
        screen.ShowPopup("UNITOPTIONS",
            handleButtonClick: (button, selectedIndex, _, _) =>
            {
                if (button == Labels.Ok)
                {
                    HandleSelection(cityWindow, unit, selectedIndex);
                }
            },
            replaceStrings: [$"{unit.Owner.Adjective} {unit.Name}", "", $"{unit.HomeCity?.Name ?? Labels.For(LabelIndex.NONE)}"],
            options:
            [
                "Activate Unit",
                Labels.For(LabelIndex.NoOrders),
                Labels.For(LabelIndex.SleepBoardnextship),
                Labels.For(LabelIndex.Fortify),
                Labels.For(LabelIndex.Disband),
                Labels.For(LabelIndex.ZoomToCity)
            ],
            dialogImage: new(unit, screen.MainWindow.ActiveInterface));
    }

    private static void HandleSelection(CityWindow cityWindow, Unit unit, int selectedIndex)
    {
        if (unit.Dead)
        {
            return;
        }

        switch (selectedIndex)
        {
            case ActivateUnitIndex:
                ActivateUnit(cityWindow, unit);
                break;
            case NoOrdersIndex:
                ClearOrders(cityWindow, unit);
                break;
            case SleepIndex:
                unit.Sleep();
                cityWindow.CurrentGameScreen.ForceRedraw();
                break;
            case FortifyIndex:
                FortifyUnit(cityWindow, unit);
                break;
            case DisbandIndex:
                DisbandUnit(cityWindow, unit);
                break;
            case ZoomIndex:
                ZoomToUnit(cityWindow, unit);
                break;
        }
    }

    private static void ActivateUnit(CityWindow cityWindow, Unit unit)
    {
        ClearOrders(cityWindow, unit);
        cityWindow.CurrentGameScreen.Player.SetUnitActive(unit, false);
        cityWindow.CurrentGameScreen.CloseDialog(cityWindow);
        cityWindow.CurrentGameScreen.ForceRedraw();
    }

    private static void ClearOrders(CityWindow cityWindow, Unit unit)
    {
        unit.Order = (int)OrderType.NoOrders;
        unit.WaitOrder = false;
        cityWindow.CurrentGameScreen.ForceRedraw();
    }

    private static void FortifyUnit(CityWindow cityWindow, Unit unit)
    {
        if (!UnitFunctions.CanFortifyHere(unit, unit.CurrentLocation))
        {
            return;
        }

        unit.Order = (int)OrderType.Fortify;
        unit.MovePointsLost = unit.MaxMovePoints;
        cityWindow.CurrentGameScreen.ForceRedraw();
    }

    private static void DisbandUnit(CityWindow cityWindow, Unit unit)
    {
        var screen = cityWindow.CurrentGameScreen;
        if (screen.Player.ActiveUnit == unit)
        {
            screen.Player.SetUnitActive(null, false);
        }

        ApplyDisbandProductionCredit(cityWindow.City, unit, screen.Game.Rules.Cosmic.RowsShieldBox);

        unit.Dead = true;
        unit.Owner.Units.Remove(unit);
        cityWindow.UpdateProduction();
        screen.ForceRedraw();
    }

    private static void ApplyDisbandProductionCredit(City city, Unit unit, int shieldRows)
    {
        if (unit.HomeCity != city && unit.CurrentLocation != city.Location)
        {
            return;
        }

        var totalCost = Math.Max(1, city.ItemInProduction.Cost * Math.Max(1, shieldRows));
        var shieldCredit = Math.Max(1, unit.TypeDefinition.Cost * Math.Max(1, shieldRows) / 2);
        city.ShieldsProgress = Math.Min(totalCost, city.ShieldsProgress + shieldCredit);

        var queuedItem = city.ConstructionQueue.Current;
        if (queuedItem != null)
        {
            queuedItem.RemainingCost = Math.Max(0, queuedItem.RemainingCost - shieldCredit);
        }
    }

    private static void ZoomToUnit(CityWindow cityWindow, Unit unit)
    {
        var screen = cityWindow.CurrentGameScreen;
        screen.Game.ActivePlayer.ActiveTile = unit.CurrentLocation;
        screen.Player.ActiveTile = unit.CurrentLocation;
        screen.CloseDialog(cityWindow);
        screen.ForceRedraw();
    }
}
