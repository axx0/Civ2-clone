using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Civ2engine;
using Model;

namespace EtoFormsUI.Initialization
{
    public static class Helpers
    {
        public static IList<IUserInterface> LoadInterfaces()
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
                    .Select(x => (IUserInterface)Activator.CreateInstance(x)));
            }
            return implementors.ToArray();
        }

        public static IUserInterface GetInterface(string path, IList<IUserInterface> interfaces)
        {
            var gameTxt = Path.Combine(path, "Game.txt");
            if (!File.Exists(gameTxt)) return null;
        
            var title = File.ReadLines(gameTxt)
                .Where(l => l.StartsWith("@title"))
                .Select(l => l.Split("=", 2)[1])
                .FirstOrDefault();
            foreach (var userInterface in interfaces)
            {
                if (userInterface.CanDisplay(title))
                {
                    return userInterface;
                }
            }

            return null;
        }
    }
}