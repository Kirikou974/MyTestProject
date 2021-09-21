﻿using System;
using System.Collections.Generic;
namespace VTOLVRControlsMapper.Core
{
    public enum ControllerActionBehavior
    {
        HoldOn,
        HoldOff,
        Toggle,
        Increase,
        Decrese,
        On,
        Off,
        Continuous
    }
    public class ControlAttribute : Attribute
    {
        public ControllerActionBehavior SupportedBehavior { get; set; }
    }
}