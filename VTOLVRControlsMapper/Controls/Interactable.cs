using VTOLVRControlsMapper.Core;

namespace VTOLVRControlsMapper.Controls
{
    public class Interactable : ControlButtonBase<VRInteractable>
    {
        public Interactable(string interactableName) : base(interactableName) { }
        public override void StartInteract()
        {
            UnityControl.OnInteract.Invoke();
        }
        public override void StopInteract()
        {
            UnityControl.OnStopInteract.Invoke();
        }
    }
}
