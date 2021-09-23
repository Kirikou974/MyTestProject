using VTOLVRControlsMapper.Core;

namespace VTOLVRControlsMapper.Controls
{
    public class Button : ControlBase<VRButton>
    {
        public VRInteractable InteractableControl => Main.GetGameControl<VRInteractable>(ControlName);
        public Button(string interactableName) : base(interactableName) { }
        [Control(SupportedBehavior = ControllerActionBehavior.Toggle)]
        public void Invoke()
        {
            StartInteract();
            StopInteract();
        }
        [Control(SupportedBehavior = ControllerActionBehavior.HoldOn)]
        public void StartInteract()
        {
            InteractableControl.OnInteract.Invoke();
        }
        [Control(SupportedBehavior = ControllerActionBehavior.HoldOff)]
        public void StopInteract()
        {
            InteractableControl.OnStopInteract.Invoke();
        }
    }
}
