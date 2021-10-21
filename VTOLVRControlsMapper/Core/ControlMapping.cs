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
        public virtual bool IsValid()
        {
            return ControllerInstanceGuid != null && ControllerInstanceGuid != Guid.NewGuid();
        }
    }
    public class Axis
    {
        public string Name { get; set; }
        public bool Invert { get; set; }
        public MappingRange MappingRange { get; set; }
        public Axis() { }
        public Axis(string name, bool invert, MappingRange range)
        {
            Name = name;
            Invert = invert;
            MappingRange = range;
        }
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Name);
        }
    }
    public class JoystickAction : GameAction
    {
        public Thumbstick Thumbstick { get; set; }
        public Axis Trigger { get; set; }
        public string Menu { get; set; }
        public override bool IsValid()
        {
            return base.IsValid() && (!string.IsNullOrEmpty(Menu) ||
                (Trigger != null && Trigger.IsValid()) ||
                (Thumbstick != null && Thumbstick.IsValid()));
        }
    }
    public class GenericGameAction : GameAction
    {
        public string ControllerButtonName { get; set; }
        public ControllerActionBehavior ControllerActionBehavior { get; set; }
        public override bool IsValid()
        {
            return base.IsValid() && !string.IsNullOrEmpty(ControllerButtonName);
        }
    }
    public class ThrottleAction : JoystickAction
    {
        public Axis Power { get; set; }
        public override bool IsValid()
        {
            return base.IsValid() || (Power != null && Power.IsValid());
        }
    }
    public class Thumbstick
    {
        public Axis X { get; set; }
        public Axis Y { get; set; }
        public bool IsValid()
        {
            return (X != null && X.IsValid()) || (Y != null && Y.IsValid());
        }
    }
    public class StickAction : JoystickAction
    {
        public Axis Pitch { get; set; }
        public Axis Yaw { get; set; }
        public Axis Roll { get; set; }
        public override bool IsValid()
        {
            return base.IsValid() || 
                (Pitch != null && Pitch.IsValid()) || 
                (Yaw != null && Yaw.IsValid()) || 
                (Roll != null && Roll.IsValid());
        }
    }
    public class ControlMapping
    {
        public string GameControlName { get; set; }
        public string Description { get; set; }
        public List<GameAction> GameActions { get; set; }
        public List<Type> Types { get; set; }
        [JsonConstructor]
        public ControlMapping(string gameControlName, List<Type> types)
        {
            GameControlName = gameControlName;
            Types = types;
        }
        public override string ToString()
        {
            return GameControlName;
        }
    }
}
