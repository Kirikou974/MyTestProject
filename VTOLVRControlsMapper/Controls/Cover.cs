using VTOLVRControlsMapper.Core;

namespace VTOLVRControlsMapper.Controls
{
    public class Cover : ControlToggleBase<VRSwitchCover>
    {
        private Lever _lever;
        public Lever Lever
        {
            get
            {
                if(_lever is null)
                {
                    _lever = new Lever(ControlName);
                }
                return _lever;
            }
        }
        public Cover(string coverName) : base(coverName) { }
        public override void Toggle()
        {
            Lever.Toggle();
        }
    }
}
