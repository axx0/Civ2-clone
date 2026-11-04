using System.Numerics;
using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.IO;
using Model;
using Model.Controls;
using Model.Core;
using Model.Core.Mapping;
using Model.Core.Units;
using Model.Input;
using Raylib_CSharp.Interact;
using RaylibUI.BasicTypes;
using RaylibUI.BasicTypes.Controls;
using RaylibUI.Controls;

namespace RaylibUI.RunGame.Commands.Cheat;

public class CreateUnit(GameScreen gameScreen)
    : AlwaysOnCommand(gameScreen, CommandIds.CheatCreateUnit, [new Shortcut(Key.F1, true)])
{
    private DynamicSizingDialog? _selectUnitDialog;
    private IUserInterface? _activeUserInterface;
    // TODO: Support for filtering units based on _selectedCivilization's tech advances
    private UnitDefinition[] _displayedUnits = [];
    private Tile? _selectedTile;
    private List<Civilization> _selectableCivilizations;

    // TODO: Needs a toggle for veteran status
    private readonly bool _veteranFlag = false;

    public override void Action()
    {
        _activeUserInterface = GameScreen.Main.ActiveInterface;
        _selectedTile = ((Game)GameScreen.Game).ActiveTile;
        _displayedUnits = GameScreen.Game.Rules.UnitTypes;
        _selectableCivilizations = GameScreen.Game.AllCivilizations.FindAll(c => c.Alive);
        _selectUnitDialog = new CreateUnitDialog(this, GameScreen.Main, _activeUserInterface);

        GameScreen.ShowDialog(_selectUnitDialog);
    }

    private void DoCreateUnit(string buttonText, int selectedUnitIndex, int selectedCivIndex)
    {
        if (buttonText.Equals(Labels.Cancel) || selectedUnitIndex < 0 || selectedCivIndex < 0 ||
            _selectedTile is null || _displayedUnits.Length < selectedUnitIndex)
        {
            GameScreen.CloseDialog(_selectUnitDialog);
            return;
        }

        var selectedCivilization = _selectableCivilizations[selectedCivIndex];
        var unitDef = _displayedUnits[selectedUnitIndex];
        var newUnit = new Unit
        {
            Counter = 0,
            Dead = false,
            Id = selectedCivilization.TribeId,
            Order = (int)OrderType.NoOrders,
            Owner = selectedCivilization,
            Veteran = _veteranFlag,
            X = _selectedTile.X,
            Y = _selectedTile.Y,
            CurrentLocation = _selectedTile,
            TypeDefinition = unitDef
        };
        selectedCivilization.Units.Add(newUnit);
        GameScreen.CloseDialog(_selectUnitDialog);
        GameScreen.TileCache.Clear();
        GameScreen.MapControl.ForceRedraw = true;
    }

    private class CreateUnitDialog : DynamicSizingDialog
    {
        private const string DialogTitle = "Select Unit To Create";
        private readonly CreateUnit _createUnitController;
        private readonly Listbox? _chooseUnitListbox;
        private readonly OptionsPanel? _chooseCivPanel;
        private int _selectedUnitIndex;

        public CreateUnitDialog(CreateUnit createUnitController, Main host, IUserInterface ui) :
            base(host, DialogTitle, host.ActiveInterface.DefaultDialogWidth, new Point(0, 0))
        {
            _createUnitController = createUnitController;
            var unitNames = createUnitController._displayedUnits.Select(u => u.Name).ToList();
            var innerLayout = new TableLayout();

            _chooseUnitListbox = MakeUnitsListBox(ui, unitNames);
            innerLayout.Add(_chooseUnitListbox, 1, 1, new Padding(2, 2, 2, 2), stretch: true);
            _chooseUnitListbox.ItemSelected += ChooseUnitListboxOnItemSelected;

            _chooseCivPanel = new OptionsPanel(this)
            {
                Texts = createUnitController._selectableCivilizations.Select(civ => civ.Adjective).ToList(),
                Type = OptionsType.Default
            };
            innerLayout.Add(_chooseCivPanel, 2, 1, new Padding(2, 2, 2, 2), stretch: true);

            var innerPanel = new TableLayoutPanel(this)
            {
                TableLayout = innerLayout
            };
            Controls.Add(innerPanel);

            var menuBar = new ControlGroup(this);
            foreach (var button in (string[])[Labels.Ok, Labels.Cancel])
            {
                var actionButton = new Button(this, button);

                actionButton.Click += OnActionButtonOnClick;
                menuBar.AddChild(actionButton);
            }

            Controls.Add(menuBar);
            SetButtons(menuBar);

            // Determine which control is focused at game start
            Focused = _chooseUnitListbox;
        }

        private Listbox MakeUnitsListBox(IUserInterface ui, List<string> unitNames)
        {
            var unitListboxGroups = unitNames.Select<string, ListboxGroup>(unitName => new ListboxGroup
            {
                Elements =
                [
                    new ListboxGroupElement
                    {
                        Text = unitName
                    }
                ]
            }).ToList();

            var listBox = new Listbox(this)
            {
                Looks = ui.GetListboxLooks(ListboxType.Default),
                Groups = unitListboxGroups
            };
            return listBox;
        }

        public override void OnKeyPress(KeyboardKey key)
        {
            switch (key)
            {
                case KeyboardKey.Enter when ButtonExists(Labels.Ok):
                    CloseDialog(Labels.Ok);
                    return;
                case KeyboardKey.Escape when ButtonExists(Labels.Cancel):
                    CloseDialog(Labels.Cancel);
                    return;
            }

            base.OnKeyPress(key);
        }

        private void OnActionButtonOnClick(object? sender, MouseEventArgs mouseEventArgs)
        {
            if (sender is not Button button) return;

            CloseDialog(button.Text);
        }

        private void ChooseUnitListboxOnItemSelected(object? sender, ListboxSelectionEventArgs e)
        {
            _selectedUnitIndex = e.Index;
        }

        private void CloseDialog(string buttonText)
        {
            _createUnitController.DoCreateUnit(buttonText, _selectedUnitIndex, _chooseCivPanel!.SelectedId);
        }
    }
}