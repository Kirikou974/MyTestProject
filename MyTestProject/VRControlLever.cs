using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTOLVRControlsMapper
{
    public class VRControlLever : VRControlBase<VRLever>
    {
        public VRControlLever(VRLever lever) : base(lever) { }

        public override void Invoke()
        {
            int newState = 0;
            if (UnityControl.currentState == newState) { newState = 1; }
            UnityControl.SetState(newState);
            UnityControl.ReturnSpring();
        }
    }
}
