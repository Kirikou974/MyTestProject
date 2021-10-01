using System;
using VTOLVRControlsMapper.Core;

namespace VTOLVRControlsMapper.Controls
{
    public class ContinuousKnob : ControlBase<VRTwistKnob>
    {
        //TODO moar tests
        public ContinuousKnob(string twistKnobName) : base(twistKnobName) { }
        public override void StartControlInteraction()
        {
            throw new System.NotImplementedException();
        }
        public override void StopControlInteraction()
        {
            throw new System.NotImplementedException();
        }
    }
}
