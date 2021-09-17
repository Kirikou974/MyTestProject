using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTOLVRControlsMapper
{
    public class VRControlLever : VRControlToggleBase<VRLever>
    {
        public VRControlLever(string controlName): base(VRControlHelper.GetVRControl<VRLever>(controlName)) { }

        public override void Toggle()
        {
            int newState = 0;
            if (UnityControl.currentState == newState) { newState = 1; }
            UnityControl.SetState(newState);
            UnityControl.ReturnSpring();
        }
    }
}
