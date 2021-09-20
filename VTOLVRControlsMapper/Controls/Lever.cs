namespace VTOLVRControlsMapper
{
    public class Lever : ControlToggleBase<VRLever>
    {
        public Lever(string controlName) : base(ControlsHelper.GetGameControl<VRLever>(controlName)) { }

        public override void Toggle()
        {
            int newState = 0;
            if (UnityControl.currentState == newState) { newState = 1; }
            UnityControl.SetState(newState);
            UnityControl.ReturnSpring();
        }
    }
}
