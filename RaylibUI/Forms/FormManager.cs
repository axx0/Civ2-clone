using Raylib_cs;

namespace RaylibUI.Forms;

public static class FormManager
{
    private static List<IForm> Forms = new();
    public static MenuBar MenuBar;

    public static void Initialize()
    {
        MenuBar = new MenuBar();
    }

    public static void Add(IForm form)
    {
        Forms.Add(form);
    }

    public static void Clear()
    {
        Forms.Clear();
    }

    public static void DrawForms()
    {
        // If any dialog present --> disable all panel interactions
        if (Forms.OfType<Dialog>().Any())
        {
            Forms.OfType<Panel>().ToList().ForEach(f => f.Disable());
            MenuBar.Disable();

            // Bring pressed dialog to front and focus on it
            var pressedDialogs = from d in Forms
                     where d.GetType() == typeof(Dialog)
                     where d.Pressed
                     select d;

            if (pressedDialogs.Any())
            {
                var lastPressed = pressedDialogs.ToList().Last();
                Forms.Remove(lastPressed);
                Forms.Add(lastPressed);
            }

            // Last dialog in the list is focused and drawn in front
            Forms.OfType<Dialog>().ToList().ForEach(f => f.UnFocus());
            Forms.OfType<Dialog>().Last().Focus();
        }
        else
        {
            MenuBar.Enable();

            // Bring pressed panels to front and focus on it
            var pressedPanels = from d in Forms
                                where d.Pressed
                                select d;

            if (pressedPanels.Any())
            {
                var lastPressed = pressedPanels.ToList().Last();
                Forms.Remove(lastPressed);
                Forms.Add(lastPressed);
            }

            // Last panel in the list is focused and drawn in front
            Forms.OfType<Panel>().ToList().ForEach(f => f.UnFocus());
            Forms.OfType<Panel>().Last().Focus();

        }

        Forms.ToList().ForEach(f => f.Draw());
        MenuBar.Draw();
    }
}
