using System;
using System.Collections.Generic;
using System.Text;

namespace VTOLVRControlsMapperUI.GridItem
{
    public class JoystickItem : BaseItem
    {
        public string ControlName { get; set; }
        public JoystickItem(string name) : base(name) { }

    }
}
