using System;
using Civ2engine.Events;
using Civ2engine.Scripting.ScriptObjects;
using Civ2engine.Units;
using Model.Core.Units;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable InconsistentNaming

namespace Civ2engine.Scripting
{
    public class ScenarioHooks(Game game)
    {
        #region onActivateUnit
        public void onActivateUnit(Func<UnitApi, bool, bool, object> activateHook)
        {
            game.OnUnitEvent += (_, args) =>
            {
                if (args is ActivationEventArgs activationArgs)
                {
                    activateHook(new UnitApi(activationArgs.Unit, game), activationArgs.UserInitiated, activationArgs.Reactivation);
                }
            };
        }
        
        public void onActivateUnit(Func<UnitApi, bool, object> activateHook)
        {
            game.OnUnitEvent += (_, args) =>
            {
                if (args is ActivationEventArgs activationArgs)
                {
                    activateHook(new UnitApi(activationArgs.Unit, game), activationArgs.UserInitiated);
                }
            };
        }
        
        public void onActivateUnit(Func<UnitApi, object> activateHook)
        {
            game.OnUnitEvent += (_, args) =>
            {
                if (args is ActivationEventArgs activationArgs)
                {
                    activateHook(new UnitApi(activationArgs.Unit, game));
                }
            };
        }
        public void onActivateUnit(Func<object> activateHook)
        {
            game.OnUnitEvent += (_, args) =>
            {
                if (args is ActivationEventArgs)
                {
                    activateHook();
                }
            };
        }
        #endregion
    }
}