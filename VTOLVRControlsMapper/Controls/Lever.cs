using VTOLVRControlsMapper.Core;

namespace VTOLVRControlsMapper.Controls
{
    public class Lever : ControlToggleBase<VRLever>
    {
        public Lever(string controlName) : base(ControlsHelper.GetGameControl<VRLever>(controlName)) { }
        [Control(SupportedBehavior = ControllerActionBehavior.Toggle)]
        public override void Toggle()
        {
            int newState = 0;
            if (UnityControl.currentState == newState) { newState = 1; }
            UnityControl.SetState(newState);
            UnityControl.ReturnSpring();
        }
    }
}
