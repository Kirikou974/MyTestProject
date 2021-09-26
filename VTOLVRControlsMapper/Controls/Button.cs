using VTOLVRControlsMapper.Core;

namespace VTOLVRControlsMapper.Controls
{
    public class Button : ControlButtonBase<VRButton>
    {
        public Interactable InteractableControl { get; protected set; }
        public Button(string interactableName) : base(interactableName)
        {
            InteractableControl = new Interactable(ControlName);
        }
        public override void StartInteract()
        {
            InteractableControl.StartInteract();
        }
        public override void StopInteract()
        {
            InteractableControl.StopInteract();
        }
    }
}
