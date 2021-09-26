using VTOLVRControlsMapper.Core;

namespace VTOLVRControlsMapper.Controls
{
    public class Cover : ControlToggleBase<VRSwitchCover>
    {
        public Lever Lever
        {
            get;
            protected set;
        }
        public Cover(string coverName) : base(coverName) {
            Lever = new Lever(ControlName);
        }
        public override void Toggle()
        {
            Lever.Toggle();
        }
    }
}
