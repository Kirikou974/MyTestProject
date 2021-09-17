using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTOLVRControlsMapper
{
    public class VRControlLever : VRControlToggleBase<VRLever>
    {
        public VRControlLever(Control control): base(VRControlHelper.GetVRControl<VRLever>(control)) { }

        public override void Toggle()
        {
            VRControlHelper.Mod.Log(string.Format("Trying to toggle {0} of type {1}", Control, this.GetType().Name));
            int newState = 0;
            if (UnityControl.currentState == newState) { newState = 1; }
            UnityControl.SetState(newState);
            UnityControl.ReturnSpring();
        }
    }
}
