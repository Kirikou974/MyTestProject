using System;
using System.Collections.Generic;
using System.Linq;
using VTOLVRControlsMapper.Core;

namespace VTOLVRControlsMapper.Controls
{
    public class Lever : ControlLeverBase<VRLever>
    {
        public Lever(string leverName) : base(leverName)
        {
            IEnumerable<VRSwitchCover> covers = ControlsHelper.GetGameControls<VRSwitchCover>();
            Cover = covers.SingleOrDefault(c => c.name == ControlName);
        }
        public override int UnityControlCurrentState { get => UnityControl.currentState; }
        public override int UnityControlStates { get => UnityControl.states; }
        public bool HasCover { get => Cover != null; }
        public VRSwitchCover Cover
        {
            get;
            protected set;
        }
        public override Action<int> UnityControlSetState { get => UnityControl.SetState; }
        public override Action UnityControlSetStateAfterSetState { get => UnityControl.ReturnSpring; }
        public override void SetState(int state)
        {
            if (!HasCover || (HasCover && !Cover.covered))
            {
                base.SetState(state);
            }
        }
    }
}
