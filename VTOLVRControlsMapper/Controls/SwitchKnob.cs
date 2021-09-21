using VTOLVRControlsMapper.Core;

namespace VTOLVRControlsMapper.Controls
{
    public class SwitchKnob : ControlKnobBase<VRTwistKnobInt>, IControlToggle
    {
        public bool IsOff => UnityControl.currentState == 0;
        public SwitchKnob(string twistKnobIntName) : base(twistKnobIntName) { }
        [Control(SupportedBehavior = ControllerActionBehavior.Increase)]
        public override void Increase()
        {
            int newState = UnityControl.currentState + 1;
            SetState(newState);
        }
        [Control(SupportedBehavior = ControllerActionBehavior.Decrese)]
        public override void Decrease()
        {
            int newState = UnityControl.currentState - 1;
            SetState(newState);
        }
        [Control(SupportedBehavior = ControllerActionBehavior.Toggle)]
        public void Toggle()
        {
            if (IsOff)
            {
                On();
            }
            else
            {
                Off();
            }
        }
        [Control(SupportedBehavior = ControllerActionBehavior.On)]
        public void On()
        {
            SetState(UnityControl.states - 1);
        }
        [Control(SupportedBehavior = ControllerActionBehavior.Off)]
        public void Off()
        {
            SetState(0);
        }
        public override void SetState(int state)
        {
            if (state != UnityControl.currentState && state >= 0 && state < UnityControl.states)
            {
                UnityControl.SetState(state);
                UnityControl.SetRotationFromState();
            }
        }
    }
}
