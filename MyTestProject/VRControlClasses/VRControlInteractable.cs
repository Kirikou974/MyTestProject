using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTOLVRControlsMapper
{    public class VRControlInteractable: VRControlBase<VRInteractable>
    {
        public VRControlInteractable(Control interactable) : base(VRControlHelper.GetVRControl<VRInteractable>(interactable)) { }
        public void Invoke()
        {
            VRControlHelper.Mod.Log(string.Format("Invoke control {0} of type {1}", Control, this.GetType().Name));
            UnityControl.OnInteract.Invoke();
        }
    }
}
