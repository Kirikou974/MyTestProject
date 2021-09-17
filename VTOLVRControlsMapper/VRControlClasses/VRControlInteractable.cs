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
            VRControlHelper.Mod.Log("Interacting on " + Control);
            StartInteract();
            StopInteract();
        }
        public void StartInteract()
        {
            UnityControl.OnInteract.Invoke();
        }
        public void StopInteract()
        {
            UnityControl.OnStopInteract.Invoke();
        }
    }
}
