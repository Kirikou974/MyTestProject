using VTOLVRControlsMapper.Core;

namespace VTOLVRControlsMapper.Controls
{
    public class SwitchKnob : ControlLeverBase<VRTwistKnobInt>
    {
        public override int UnityControlCurrentState => UnityControl.currentState;
        public override int UnityControlStates => UnityControl.states;
        public SwitchKnob(string twistKnobIntName) : base(twistKnobIntName) { }
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
