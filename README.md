# About

Civilization II (original/Multiplayer Gold Edition & Test of Time remake) clone in .NET 6.0 using [C# bindings](https://github.com/ChrisDill/Raylib-cs) of [Raylib](https://www.raylib.com/) for Win/Linux/Mac.

Folders:
- Engine = the Civ2 logic (core) code
- Civ2 (Civ2Gold + Civ2TOT) = interfaces with game-specific logic for MGE and Test of Time
- RaylibUI = graphical implementation of the game

# Requirements

You need the original game with its assets (sounds, images, texts) to run this.

# Status & goals

Currently you can:
- Load a .sav game or scenario (TOT is not supported yet)
- Move around the map and minimap
- Move units
- Open city window

Goals:
- rewrite the game, closely matching the original's functionalities and features
- implement various improvements & QoL features (support more than 7 civs, graphical files with richer colours, etc.)
- enable Lua scripting

# Running the game

Download the .NET 6.0 SDK. The easiest way is to run the game with Visual Studio or VSCode.
Once you build and run the game, a dialog should automatically open so you can locate the folder with the original game files. Otherwise make a path to Civ2 folder in appsettings.json (located in the Engine folder or in the RaylibUI/bin folder once you build the game).

# Screenshots

![Clipboard02](https://github.com/axx0/Civ2-clone/assets/21365802/864ab9b4-ce4c-4715-9447-9e913298a971)

