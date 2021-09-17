﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTOLVRControlsMapper
{
    public class VRControlCover : VRControlToggle<VRSwitchCover>
    {
        private VRControlLever _lever;
        public VRControlLever Level
        {
            get
            {
                return _lever;
            }
        }
        public VRControlCover(Control cover) : base(VRControlHelper.GetVRControl<VRSwitchCover>(cover))
        {
            _lever = new VRControlLever(cover);
            //Do a toggle when class is instanciated, either way the switch cover is initialized to false and the covered lever can be activated without visually lifting the cover
            Toggle();
        }
        public override void Toggle()
        {
            VRControlHelper.Mod.Log(string.Format("Trying to toggle {0} of type {1}", Control, this.GetType().Name));
            _lever.Toggle();
        }
    }
}