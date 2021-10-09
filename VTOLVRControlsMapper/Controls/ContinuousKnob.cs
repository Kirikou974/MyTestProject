using System.Collections;
using VTOLVRControlsMapper.Core;

namespace VTOLVRControlsMapper.Controls
{
    public class ContinuousKnob : ControlBase<VRTwistKnob>
    {
        //TODO moar tests
        public ContinuousKnob(string twistKnobName) : base(twistKnobName) { }
        public override void StartControlInteraction(VRHandController hand)
        {
            throw new System.NotImplementedException();
        }
        public override void StopControlInteraction(VRHandController hand)
        {
            throw new System.NotImplementedException();
        }
    }
}
