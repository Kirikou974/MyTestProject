using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using VTOLVRControlsMapper.Core;

namespace VTOLVRControlsMapperUI
{
    public class ActionItem
    {
        public ControllerActionBehavior Behavior { get; set; }
        public string ControlName { get; set; }
        public Guid DeviceID { get; set; }
        public ActionItem(ControllerActionBehavior behavior)
        {
            Behavior = behavior;
        }
    }
}
