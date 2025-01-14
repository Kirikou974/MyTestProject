﻿using System;
namespace VTOLVRControlsMapper.Core
{
    public enum ControllerActionBehavior
    {
        Hold = 0,
        HoldOff = 1,
        Toggle = 2,
        Increase = 3,
        Decrease = 4,
        On = 5,
        Off = 6
    }
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class ControlMethodAttribute : Attribute
    {
        public ControllerActionBehavior SupportedBehavior { get; set; }
    }
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class ControlClassAttribute : Attribute
    {
        public Type[] UnityTypes { get; set; }
    }
}
