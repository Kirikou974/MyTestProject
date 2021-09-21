using System;
using System.Collections.Generic;
namespace VTOLVRControlsMapper.Core
{
    public enum ControllerActionBehavior
    {
        HoldOn = 0,
        HoldOff = 1,
        Toggle = 2,
        Increase = 3,
        Decrease = 4,
        On = 5,
        Off = 6,
        Continuous = 7
    }
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class ControlAttribute : Attribute
    {
        public ControllerActionBehavior SupportedBehavior { get; set; }
    }
}
