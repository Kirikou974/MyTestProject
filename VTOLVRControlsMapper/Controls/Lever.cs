using System;
using System.Collections.Generic;
using System.Linq;
using VTOLVRControlsMapper.Core;

namespace VTOLVRControlsMapper.Controls
{
    public class Lever : ControlLeverBase<VRLever>
    {
        public Lever(string leverName) : base(leverName) { }
        public override int UnityControlCurrentState { get => UnityControl.currentState; }
        public override int UnityControlStates { get => UnityControl.states; }
        public bool HasCover { get => Cover != null; }
        private Cover _cover;
        public Cover Cover
        {
            get
            {
                if(_cover is null)
                {
                    List<VRSwitchCover> covers = Main.GetGameControls<VRSwitchCover>().ToList();
                    VRSwitchCover cover = covers.Find(c => c.name == ControlName);
                    _cover = new Cover(cover.name);
                }
                return _cover;
            }
        }
        public override Action<int> UnityControlSetState { get => UnityControl.SetState; }
        public override Action UnityControlSetStateAfterSetState { get => UnityControl.ReturnSpring; }
        public override void SetState(int state)
        {
            if (!HasCover || (HasCover && !Cover.UnityControl.covered))
            {
                base.SetState(state);
            }
        }
    }
}
