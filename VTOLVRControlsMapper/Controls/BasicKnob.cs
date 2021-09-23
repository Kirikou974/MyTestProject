using System;
using VTOLVRControlsMapper.Core;

namespace VTOLVRControlsMapper.Controls
{
    public class BasicKnob : ControlLeverBase<VRTwistKnobInt>
    {
        public override int UnityControlCurrentState { get => UnityControl.currentState; }
        public override int UnityControlStates { get => UnityControl.states; }
        public override Action<int> UnityControlSetState { get => UnityControl.SetState; }
        public override Action UnityControlSetStateAfterSetState { get => UnityControl.SetRotationFromState; }
        public BasicKnob(string twistKnobIntName) : base(twistKnobIntName) { }
    }
}
