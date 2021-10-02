using System;
using System.Collections;
using VTOLVRControlsMapper.Core;

namespace VTOLVRControlsMapper.Controls
{
    [ControlClass(UnityTypes = new Type[] { typeof(VRInteractable) })]
    public class Interactable : ControlButtonBase<VRInteractable>
    {
        public Interactable(string interactableName) : base(interactableName) { }
        public override IEnumerator StartControlInteraction(VRHandController hand)
        {
            UnityControl.OnInteract.Invoke();
            yield return null;
        }
        public override void StopControlInteraction(VRHandController hand)
        {
            UnityControl.OnStopInteract.Invoke();
        }
    }
}
