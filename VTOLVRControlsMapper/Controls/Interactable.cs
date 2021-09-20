using VTOLVRControlsMapper.Core;

namespace VTOLVRControlsMapper.Controls
{
    public class Interactable : ControlBase<VRInteractable>
    {
        public Interactable(string interactableName) : base(ControlsHelper.GetGameControl<VRInteractable>(interactableName)) { }
        [Control(SupportedBehavior = ControllerActionBehavior.Toggle)]
        public void Invoke()
        {
            StartInteract();
            StopInteract();
        }
        [Control(SupportedBehavior = ControllerActionBehavior.HoldOn)]
        public void StartInteract()
        {
            UnityControl.OnInteract.Invoke();
        }
        [Control(SupportedBehavior = ControllerActionBehavior.HoldOff)]
        public void StopInteract()
        {
            UnityControl.OnStopInteract.Invoke();
        }
    }
}
