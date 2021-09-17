using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTOLVRControlsMapper
{
    public class VRControlCover : VRControlBase<VRSwitchCover>
    {
        private VRControlLever _lever;
        public VRControlCover(VRSwitchCover cover, VRControlLever lever) : base(cover)
        {
            _lever = lever;
        }

        public override void Invoke()
        {
            //UnityControl.OnSetState((UnityControl.covered ? 0 : 1));
            _lever.Toggle();
        }
    }
}
