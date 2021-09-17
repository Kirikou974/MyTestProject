using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTOLVRControlsMapper
{    public class VRControlInteractable: VRControlBase<VRInteractable>
    {
        public VRControlInteractable(string interactableName) : base(VRControlHelper.GetVRControl<VRInteractable>(interactableName)) { }
        public void Invoke()
        {
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
