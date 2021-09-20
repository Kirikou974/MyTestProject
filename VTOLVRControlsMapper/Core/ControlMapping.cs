using SharpDX.DirectInput;
using System;
using System.Collections.Generic;

namespace VTOLVRControlsMapper
{
    public enum ControllerActionType
    {
        Button,
        Axis,
        Touchpad
    }
    public class GameAction
    {
        public string ActionName { get; set; }
        public string ControllerActionName { get; set; }
        public ControllerActionType ControllerActionType { get; set; }
        public Guid ControllerInstanceGuid { get; set; }
        public GameAction(Guid controllerInstanceGuid, string actionName, string controllerActionName, ControllerActionType controllerActionType)
        {
            ControllerInstanceGuid = controllerInstanceGuid;
            ActionName = actionName;
            ControllerActionName = controllerActionName;
            ControllerActionType = controllerActionType;
        }
    }
    public class ControlMapping
    {
        public string ControlName { get; set; }
        public List<GameAction> KeyboardActions { get; set; }
        public List<GameAction> JoystickActions { get; set; }
        public List<Type> Types { get; set; }
        public ControlMapping(string controlName, List<Type> types)
        {
            ControlName = controlName;
            Types = types;
            //KeyboardActions = new List<GameAction>();
            //DirectInput di = new DirectInput();
            //Keyboard kb = new Keyboard(di);
            //KeyboardActions.Add(new GameAction(kb.Information.InstanceGuid, "Invoke", "a", ControllerActionType.Button));
        }
    }
}
