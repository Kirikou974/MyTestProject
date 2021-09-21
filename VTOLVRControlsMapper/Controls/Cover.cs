using VTOLVRControlsMapper.Core;

namespace VTOLVRControlsMapper.Controls
{
    public class Cover : ControlToggleBase<VRSwitchCover>
    {
        public Lever Lever
        {
            get;
            internal set;
        }
        public Cover(string coverName) : base(coverName)
        {
            Lever = new Lever(coverName);
        }
        public override void Toggle()
        {
            Lever.Toggle();
        }
    }
}
