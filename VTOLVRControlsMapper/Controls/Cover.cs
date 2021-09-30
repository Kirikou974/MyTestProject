using System.Collections;
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
        public override IEnumerator Toggle()
        {
            yield return Lever.Toggle();
        }
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
