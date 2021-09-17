using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTOLVRControlsMapper
{
    public class VRControlInteractable : VRControlBase<VRInteractable>
    {
        public VRControlInteractable(VRInteractable vrInteractable): base(vrInteractable) { }
        public override void Invoke()
        {
            UnityControl.OnInteract.Invoke();
        }
    }
}
