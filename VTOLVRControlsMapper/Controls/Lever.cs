using VTOLVRControlsMapper.Core;

namespace VTOLVRControlsMapper.Controls
{
    public class Lever : ControlLeverBase<VRLever>
    {
        public Lever(string leverName) : base(leverName) { }
        public override int UnityControlCurrentState => UnityControl.currentState;
        public override int UnityControlStates => UnityControl.states;
        public override void SetState(int state)
        {
            if (state != UnityControl.currentState && state >= 0 && state < UnityControl.states)
            {
                UnityControl.SetState(state);
                UnityControl.ReturnSpring();
            }
        }
    }
}
