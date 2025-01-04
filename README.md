# About

Civilization II (original/Multiplayer Gold Edition & Test of Time remake) clone in .NET 8.0 using [C# bindings](https://github.com/MrScautHD/Raylib-CSharp) of [Raylib](https://www.raylib.com/) for Win/Linux/Mac.

Folders:
- Engine = the Civ2 logic (core) code
- Civ2 (Civ2Gold + Civ2TOT) = interfaces with game-specific logic for MGE and Test of Time
- RaylibUI = graphical implementation of the game
- EtoFormsUI = We're still transitioning from Eto to raylib UI. So Eto folder is there untill this process is over, then we'll remove it.

# Requirements

You need the original game with its assets (sounds, images, texts) to run this.

To achieve this, download the version of Civ II you desire, install it (if you are on Windows 10+, you will need to create an XP VM), and point to where the files have been installed. Normally is where the Civ2.exe, Game.txt, etc. are located)

[You can download the Civ II assets here](https://1drv.ms/u/s!Ary9ImGwIZdvsaFkzokff72aofn6QA?e=GM0fkZ) (It is already installed)

You can also download it online; there are several sites that provide the installer, the above version is already installed to save time. There are also some save files (.sav) that will be necessary for you to load the game.


# Status & goals

Currently you can:
- Load a .sav game or scenario
- Move around the map and minimap
- Move units
- Open city window

Goals:
- rewrite the game, closely matching the original's functionalities and features
- implement various improvements & QoL features (support more than 7 civs, graphical files with richer colours, etc.)
- enable Lua scripting

# Running the game

Download the .NET 8.0 SDK. The easiest way is to run the game with Visual Studio or VSCode.
 
You have to point to RaylibUI folder then build it.

Once you build and run the game, a dialog should automatically open so you can locate the folder with the original game files. Otherwise make a path to Civ2 folder in appsettings.json (located in the Engine folder or in the RaylibUI/bin folder once you build the game).

# Screenshots

![civ2](https://github.com/user-attachments/assets/48372674-e978-431c-a9ef-2927c0b5e203)
![tot](https://github.com/user-attachments/assets/0e776c19-be01-4bdc-868b-13e31424bd7f)



