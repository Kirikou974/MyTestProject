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
    public class GameAction
    {
        public Guid ControllerInstanceGuid { get; set; }
    }
    public class GenericGameAction : GameAction
    {
        public string ControllerButtonName { get; set; }
        public ControllerActionBehavior ControllerActionBehavior { get; set; }
    }
    public class Axis
    {
        public string Name { get; set; }
        public bool Invert { get; set; }
        public MappingRange MappingRange { get; set; }
    }
    public class ThrottleAction: JoystickAction
    {
        public Axis Power { get; set; }
    }
    public class Thumbstick
    {
        public Axis X { get; set; }
        public Axis Y { get; set; }
    }
    public class StickAction: JoystickAction
    {
        public Axis Pitch { get; set; }
        public Axis Yaw { get; set; }
        public Axis Roll { get; set; }
    }
    public class JoystickAction : GameAction
    {
        public Thumbstick Thumbstick { get; set; }
        public Axis Trigger { get; set; }
        public string Menu { get; set; }
    }
    public class ControlMapping
    {
        public string GameControlName { get; set; }
        public List<GameAction> GameActions { get; set; }
        public List<Type> Types { get; set; }
        [JsonConstructor]
        public ControlMapping(string gameControlName, List<Type> types)
        {
            GameControlName = gameControlName;
            Types = types;
        }
    }
}
