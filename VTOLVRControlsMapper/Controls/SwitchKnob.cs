namespace VTOLVRControlsMapper
{
    public class SwitchKnob : ControlKnobBase<VRTwistKnobInt>, IControlToggle
    {
        public bool IsOff
        {
            get
            {
                return UnityControl.currentState == 0;
            }
        }
        public SwitchKnob(string controlName) : base(ControlsHelper.GetGameControl<VRTwistKnobInt>(controlName)) { }
        public override void Increase()
        {
            int newState = UnityControl.currentState + 1;
            SetState(newState);
        }
        public override void Decrease()
        {
            int newState = UnityControl.currentState - 1;
            SetState(newState);
        }
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
        public void On()
        {
            SetState(UnityControl.states - 1);
        }
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
