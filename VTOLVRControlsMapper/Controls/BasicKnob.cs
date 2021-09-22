using System;
using VTOLVRControlsMapper.Core;

namespace VTOLVRControlsMapper.Controls
{
    public class BasicKnob : ControlKnobBase<VRTwistKnobInt>
    {
        public override int UnityControlCurrentState => UnityControl.currentState;
        public override int UnityControlStates => UnityControl.states;
        public override Action<int> UnityControlSetState => UnityControl.SetState;
        public override Action UnityControlSetStateAfterSetState => UnityControl.SetRotationFromState;
        public BasicKnob(string twistKnobIntName) : base(twistKnobIntName) { }
    }
}
