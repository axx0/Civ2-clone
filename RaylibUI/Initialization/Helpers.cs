using System.Reflection;
using Civ2engine;
using Civ2engine.IO;
using Model;
using Model.Interface;
using Raylib_CSharp.Fonts;

namespace RaylibUI.Initialization;

public static class Helpers
{
    public static IList<IUserInterface> LoadInterfaces(IMain main)
    {
        var implementors = new List<IUserInterface>();
        var dir = new DirectoryInfo(Settings.BasePath); 
        var userInterfaceType = typeof(IUserInterface);
            
        foreach (var file in dir.GetFiles("*.dll")) //loop through all dll files in directory
        {
            Assembly currentAssembly;
            try
            {
                var name = AssemblyName.GetAssemblyName(file.FullName);
                currentAssembly = Assembly.Load(name);
            }
            catch (Exception)
            {
                continue;
            }

            implementors.AddRange(currentAssembly.GetTypes()
                .Where(t => t != userInterfaceType && userInterfaceType.IsAssignableFrom(t) && !t.IsAbstract)
                .Select(x => (IUserInterface)Activator.CreateInstance(x, main)));
        }
        return implementors.ToArray();
    }

    public static IUserInterface GetInterface(string path, IList<IUserInterface> interfaces, Ruleset[] ruleSets)
    {
        foreach (var ruleSet in ruleSets)
        {
            if (ruleSet.Paths.Contains(path))
            {
                return interfaces[ruleSet.InterfaceIndex];
            }
        }
        return ruleSets.Length > 0 ? interfaces[ruleSets[0].InterfaceIndex] : interfaces[0];
    }

    public static void LoadFonts()
    {
        var tnr = Utils.GetFilePath("times-new-roman.ttf");
        Fonts.SetTnr(Font.Load(tnr));
        var bold = Utils.GetFilePath("times-new-roman-bold.ttf");
        Fonts.SetBold(Font.LoadEx(bold, 104, null));
        var alternative = Utils.GetFilePath("ARIAL.ttf");
        Fonts.SetArial(Font.LoadEx(alternative, 112, null));
    }
}