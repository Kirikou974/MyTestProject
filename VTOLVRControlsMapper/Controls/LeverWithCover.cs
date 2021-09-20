namespace VTOLVRControlsMapper.Controls
{
    public class LeverWithCover : Lever
    {
        private readonly Cover _cover;
        public LeverWithCover(string leverName, string coverName) : base(leverName)
        {
            _cover = new Cover(coverName);
        }
        public override void Toggle()
        {
            if (!_cover.UnityControl.covered)
            {
                base.Toggle();
            }
        }
    }
}
