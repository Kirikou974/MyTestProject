using System;
using System.Collections.Generic;
using Valve.Newtonsoft.Json;

namespace VTOLVRControlsMapper.Core
{
    public enum ControllerActionType
    {
        Button,
        Axis
    }
    public class GameAction
    {
        public ControllerActionBehavior ControllerActionBehavior { get; set; }
        public string ControllerActionName { get; set; }
        public bool LeftHand { get; set; }
        public Guid ControllerInstanceGuid { get; set; }
        public GameAction(Guid controllerInstanceGuid, string controllerActionName, ControllerActionBehavior controllerActionBehavior, bool isLeftHand)
        {
            ControllerInstanceGuid = controllerInstanceGuid;
            ControllerActionBehavior = controllerActionBehavior;
            ControllerActionName = controllerActionName;
            LeftHand = isLeftHand;
        }
    }
    public class ControlMapping
    {
        public string GameControlName { get; set; }
        public List<GameAction> KeyboardActions { get; set; }
        public List<GameAction> JoystickActions { get; set; }
        public List<Type> Types { get; set; }
        [JsonConstructor]
        public ControlMapping(string gameControlName, List<Type> types)
        {
            GameControlName = gameControlName;
            Types = types;
            //KeyboardActions = new List<GameAction>();
            //SharpDX.DirectInput.DirectInput di = new SharpDX.DirectInput.DirectInput();
            //SharpDX.DirectInput.Keyboard kb = new SharpDX.DirectInput.Keyboard(di);
            //KeyboardActions.Add(new GameAction(kb.Information.InstanceGuid, "a", ControllerActionBehavior.Toggle, true));
        }
    }
}
