using System.Text;
using Civ2engine;
using Civ2engine.Enums;
using Model.Input;
using Civ2engine.IO;
using Model.Core;
using Model.Controls;
using Model.Core.Mapping;
using Model.Core.Units;
using RaylibUI.RunGame.Commands.Orders;

namespace RaylibUI.RunGame.Commands.Cheat;

public class EditUnit(GameScreen gameScreen) : AlwaysOnCommand(gameScreen, CommandIds.CheatEditUnit, [new Shortcut(Key.U, ctrl: true, shift: true)])
{
    private Tile? _cursorTile;
    private List<Unit>? _unitsAtCursor;
    private Unit? _editedUnit;
    private CivDialog? _unitSelectionDialog;
    private CivDialog? _unitEditorDialog;

    public override void Action()
    {
        _cursorTile = ((Game)GameScreen.Game).ActiveTile;
        _unitsAtCursor = _cursorTile.UnitsHere;
        if (_unitsAtCursor.Count <= 0)
        {
            // No unit selected, nothing to do
            return;
        }
        if (_unitsAtCursor.Count == 1)
        {
            _unitEditorDialog = BuildEditUnitDialog(_unitsAtCursor[0]);
            GameScreen.ShowDialog(_unitEditorDialog);
        }
        else
        {
            _unitSelectionDialog = BuildUnitSelectionDialog(_unitsAtCursor);
            GameScreen.ShowDialog(_unitSelectionDialog);
        }
    }
    
    private CivDialog BuildUnitSelectionDialog(List<Unit> units)
    {
        // TODO: The original game's dialog includes graphics next to the units in this list
        List<String> unitDescriptions = units.Select(u => u.LongDescription()).ToList();
        var unitSelection = new PopupBox
        {
            Title = "Select Unit",
            Options = unitDescriptions,
            Button = [Labels.Ok, Labels.Cancel]
        };

        return new CivDialog(
            GameScreen.Main, new DialogElements(unitSelection), HandleSelectUnit);
    }
    
    private void HandleSelectUnit(string button, int selectedIndex, IList<bool>? check,
        IDictionary<string, string>? textBoxes)
    {
        if (button == Labels.Ok)
        {
            _unitEditorDialog = BuildEditUnitDialog(_unitsAtCursor[selectedIndex]);
            GameScreen.ShowDialog(_unitEditorDialog);
        }
        GameScreen.CloseDialog(_unitSelectionDialog);
    }

    private CivDialog BuildEditUnitDialog(Unit unit)
    {
        _editedUnit = unit;
        String[] options =
        [
            "No Changes", "Toggle Veteran Status", "Clear Movement Allowance", "Set Hit Points", "Set Home City",
            "Fortify/Unfortify", "Change Caravan Commodity"
        ];
        
        var unitSelection = new PopupBox
        {
            Title = $"Editing {_editedUnit.LongDescription()}",
            Options = options,
            Button = [Labels.Ok, Labels.Cancel]
        };
        
        return new CivDialog(
            GameScreen.Main, new DialogElements(unitSelection), HandleEditUnit);
    }
    
    private void HandleEditUnit(string button, int selectedIndex, IList<bool>? check,
        IDictionary<string, string>? textBoxes)
    {
        if (button == Labels.Ok)
        {
            switch (selectedIndex)
            {
                case 0: // No changes - NOOP
                    break;
                case 1: // Toggle veteran status
                    _editedUnit.Veteran = !_editedUnit.Veteran;
                    break;
                case 2: // Clear Movement Allowance
                    _editedUnit.MovePointsLost = 0;
                    break;
                case 3: // Set Hit Points
                    // TODO - Hitpoint subdialog
                    break;
                case 4: // Set Home City
                    // TODO - Homecity subdialog
                    break;
                case 5: // Fortify/Unfortify
                    if (_editedUnit.Order == (int)OrderType.Fortified)
                    {
                        _editedUnit.Order = (int)OrderType.NoOrders;
                    }
                    else
                    {
                        _editedUnit.MovePointsLost = _editedUnit.MaxMovePoints;
                        _editedUnit.Order = (int)OrderType.Fortified;
                    }

                    break;
                case 6: // Set Caravan Commodity
                    // TODO - Caravan commodity subdialog
                    break;
            }
            
            GameScreen.StatusPanel.Update();
            GameScreen.TileCache.Clear();
            GameScreen.MapControl.ForceRedraw = true;
        }

        GameScreen.CloseDialog(_unitEditorDialog);
    }
}