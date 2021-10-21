using System;
using System.Collections.Generic;
using System.Text;

namespace VTOLVRControlsMapperUI.GridItem
{
    public class JoystickItem : BaseItem
    {
        public override string ControlName { get; set; }
        public virtual bool IsAxis => true;
        public JoystickItem(string name) : base(name) { }
    }
}
