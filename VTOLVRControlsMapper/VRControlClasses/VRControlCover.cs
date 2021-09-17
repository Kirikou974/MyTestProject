using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTOLVRControlsMapper
{
    public class VRControlCover : VRControlToggleBase<VRSwitchCover>
    {
        private readonly VRControlLever _lever;
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
            //Not sure why this is necessary except for coverSwitchInteractable_jettisonButton
            if (cover != Control.coverSwitchInteractable_jettisonButton)
            {
                Toggle();
            }
        }
        public override void Toggle()
        {
            _lever.Toggle();
        }
    }
}
