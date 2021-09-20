namespace VTOLVRControlsMapper
{
    public class Interactable : ControlBase<VRInteractable>
    {
        public Interactable(string interactableName) : base(ControlsHelper.GetGameControl<VRInteractable>(interactableName)) { }
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
