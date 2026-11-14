using System.Collections.ObjectModel;

namespace RaylibUI;

/// <summary>
/// List of child controls that keeps each child's <see cref="BaseControl.Parent"/> in sync with its owner
/// (similar to WinForms' Control.ControlCollection).
/// </summary>
public class ControlCollection : Collection<IControl>
{
    private readonly IComponent _owner;

    public ControlCollection(IComponent owner)
    {
        _owner = owner;
    }

    /// <summary>
    /// Replace all children with the given controls.
    /// </summary>
    public void ReplaceWith(IEnumerable<IControl>? controls)
    {
        // Materialize first in case controls is a query over this collection
        var newControls = controls?.ToList() ?? [];
        Clear();
        foreach (var control in newControls)
        {
            Add(control);
        }
    }

    protected override void InsertItem(int index, IControl item)
    {
        base.InsertItem(index, item);
        Attach(item);
    }

    protected override void SetItem(int index, IControl item)
    {
        Detach(this[index]);
        base.SetItem(index, item);
        Attach(item);
    }

    protected override void RemoveItem(int index)
    {
        Detach(this[index]);
        base.RemoveItem(index);
    }

    protected override void ClearItems()
    {
        foreach (var item in this)
        {
            Detach(item);
        }
        base.ClearItems();
    }

    private void Attach(IControl item)
    {
        if (item is BaseControl control)
        {
            control.Parent = _owner;
        }
    }

    private void Detach(IControl item)
    {
        // Only clear the parent if the control wasn't meanwhile added to another container
        if (item is BaseControl control && control.Parent == _owner)
        {
            control.Parent = null;
        }
    }
}
