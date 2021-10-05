using System;
using System.Collections;
using VTOLVRControlsMapper.Core;

namespace VTOLVRControlsMapper.Controls
{
    [ControlClass(UnityTypes = new Type[] { typeof(VRInteractable) })]
    public class Interactable : ControlButtonBase<VRInteractable>
    {
        public Interactable(string interactableName) : base(interactableName) { }
        public override void StartControlInteraction(VRHandController hand)
        {
            UnityControl.OnInteract.Invoke();
        }
        public override void StopControlInteraction(VRHandController hand)
        {
            UnityControl.OnStopInteract.Invoke();
        }
    }
}
