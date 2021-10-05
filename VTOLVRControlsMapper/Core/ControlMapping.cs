using System;
using System.Collections.Generic;
using Valve.Newtonsoft.Json;

namespace VTOLVRControlsMapper.Core
{
    public enum MappingRange
    {
        Low = 0,
        High = 1,
        Full = 2
    }
    public abstract class GameAction
    {
        public ControllerActionBehavior ControllerActionBehavior { get; set; }
        public string ControllerButtonName { get; set; }
        public Guid ControllerInstanceGuid { get; set; }
    }
    public class JoystickAxis
    {
        public string Name { get; set; }
        public bool Invert { get; set; }
        public MappingRange MappingRange { get; set; }
    }
    public class KeyboardAction : GameAction { }
    public class JoystickAction : GameAction
    {
        public List<JoystickAxis> ControllerAxis { get; set; }
    }
    public class ControlMapping
    {
        public string GameControlName { get; set; }
        public List<KeyboardAction> KeyboardActions { get; set; }
        public List<JoystickAction> JoystickActions { get; set; }
        public List<Type> Types { get; set; }
        [JsonConstructor]
        public ControlMapping(string gameControlName, List<Type> types)
        {
            GameControlName = gameControlName;
            Types = types;
            //List<JoystickAction> actions = new List<JoystickAction>();
            //SharpDX.DirectInput.DirectInput di = new SharpDX.DirectInput.DirectInput();
            //SharpDX.DirectInput.Joystick kb = new SharpDX.DirectInput.Joystick(di, new Guid("8e0fdc40-f559-11ea-8002-444553540000"));
            //actions.Add(new JoystickAction()
            //{
            //    Axis = new List<JoystickAxis>()
            //    {
            //        new JoystickAxis()
            //        {
            //            ControllerAxisName="Y",
            //            Invert=true,
            //            MappingRange=MappingRange.Full
            //        }
            //    },
            //    ControllerActionBehavior = ControllerActionBehavior.Axis,
            //    //ControllerButtonName = "",
            //    ControllerInstanceGuid = new Guid("8e0fdc40-f559-11ea-8002-444553540000")
            //});
            //JoystickActions = actions;
            //}
        }
    }
}
