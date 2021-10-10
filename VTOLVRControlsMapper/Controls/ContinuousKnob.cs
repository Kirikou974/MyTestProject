using System;
using System.Collections;
using VTOLVRControlsMapper.Core;

namespace VTOLVRControlsMapper.Controls
{
    public class ContinuousKnob : ControlLeverBase<VRTwistKnob>
    {
        //TODO moar tests
        public ContinuousKnob(string twistKnobName) : base(twistKnobName) { }

        public override int UnityControlCurrentState => throw new NotImplementedException();

        public override int UnityControlStates => throw new NotImplementedException();

        public override Action<int> UnityControlSetState => throw new NotImplementedException();

        public override Action UnityControlSetPositionFromState => throw new NotImplementedException();

        public override void StartControlInteraction(VRHandController hand)
        {
        }
        public override void StopControlInteraction(VRHandController hand)
        {
        }
    }
}
