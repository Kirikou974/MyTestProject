using VTOLVRControlsMapper.Core;

namespace VTOLVRControlsMapper.Controls
{
    public class LeverWithCover : Lever
    {
        public Cover Cover
        {
            get;
            internal set;
        }
        public LeverWithCover(string leverName, string coverName) : base(leverName)
        {
            Cover = new Cover(coverName);
        }
        public override void Toggle()
        {
            if (!Cover.UnityControl.covered)
            {
                base.Toggle();
            }
        }
    }
}
