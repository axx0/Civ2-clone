using System;
using Civ2engine.Events;
using Civ2engine.Units;
using Model.Core.Units;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable InconsistentNaming

namespace Civ2engine.Scripting
{
    public class ScenarioHooks
    {
        private readonly Game _game;

        public ScenarioHooks(Game game)
        {
            _game = game;
        }

        #region onActivateUnit
        public void onActivateUnit(Func<Unit, bool, bool, object> activateHook)
        {
            _game.OnUnitEvent += (_, args) =>
            {
                if (args is ActivationEventArgs activationArgs)
                {
                    activateHook(activationArgs.Unit, activationArgs.UserInitiated, activationArgs.Reactivation);
                }
            };
        }
        
        public void onActivateUnit(Func<Unit, bool, object> activateHook)
        {
            _game.OnUnitEvent += (_, args) =>
            {
                if (args is ActivationEventArgs activationArgs)
                {
                    activateHook(activationArgs.Unit, activationArgs.UserInitiated);
                }
            };
        }
        
        public void onActivateUnit(Func<Unit, object> activateHook)
        {
            _game.OnUnitEvent += (_, args) =>
            {
                if (args is ActivationEventArgs activationArgs)
                {
                    activateHook(activationArgs.Unit);
                }
            };
        }
        public void onActivateUnit(Func<object> activateHook)
        {
            _game.OnUnitEvent += (_, args) =>
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