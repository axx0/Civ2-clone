using Civ2.Dialogs;
using Civ2engine;
using Model;
using Model.Images;
using Raylib_cs;

namespace Civ2;

public abstract class Civ2Interface : IUserInterface
{
    public bool CanDisplay(string? title)
    {
        return title == Title;
    }

    public InterfaceStyle Look { get; } = new()
    {
        Outer = new BitmapStorage("ICONS", new Rectangle(199, 322, 64, 32)),
        Inner = new BitmapStorage("ICONS", new Rectangle(298, 190, 32, 32))
    };

    public abstract string Title { get; }

    public virtual void Initialize()
    {
        Dialogs = PopupBoxReader.LoadPopupBoxes(Settings.Civ2Path);

        var handlerInterface = typeof(ICivDialogHandler);
        DialogHandlers = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t != handlerInterface && handlerInterface.IsAssignableFrom(t) && !t.IsAbstract)
            .Select(Activator.CreateInstance)
            .OfType<ICivDialogHandler>()
            .Select(h => h.UpdatePopupData(Dialogs))
            .ToDictionary(k => k.Name);
    }

    protected Dictionary<string, ICivDialogHandler> DialogHandlers { get; private set; }

    public IInterfaceAction ProcessDialog(string dialogName, DialogResult dialogResult)
    {
        if (!DialogHandlers.ContainsKey(dialogName))
        {
            throw new NotImplementedException(dialogName);
        }

        return DialogHandlers[dialogName].HandleDialogResult(dialogResult, DialogHandlers);
    }

    public IInterfaceAction ProcessFile(IEnumerable<string> filenames, bool ok)
    {
        throw new NotImplementedException();
    }

    public IInterfaceAction GetInitialAction()
    {
        return DialogHandlers["MAINMENU"].Show();
    }

    public virtual IImageSource? BackgroundImage => null;

    public Dictionary<string, PopupBox> Dialogs { get; set; }
}