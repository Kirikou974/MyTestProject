using System;
using VTOLVRControlsMapper.Core;

namespace VTOLVRControlsMapper.Controls
{
    [ControlClass(UnityTypes = new Type[] { typeof(VRInteractable), typeof(VRTwistKnobInt) })]
    public class BasicKnob : ControlLeverBase<VRTwistKnobInt>
    {
        public override int UnityControlCurrentState { get => UnityControl.currentState; }
        public override int UnityControlStates { get => UnityControl.states; }
        public override Action<int> UnityControlSetState { get => UnityControl.SetState; }
        public override Action UnityControlSetPositionFromState { get => UnityControl.SetRotationFromState; }
        public BasicKnob(string twistKnobIntName) : base(twistKnobIntName) { }
        public override void StartControlInteraction()
        {
            throw new NotImplementedException();
        }
        public override void StopControlInteraction()
        {
            throw new NotImplementedException();
        }
    }
}
