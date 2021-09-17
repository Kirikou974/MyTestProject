using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTOLVRControlsMapper
{
    public class VRControlLeverWithCover: VRControlLever
    {
        private readonly VRControlCover _cover;
        public VRControlLeverWithCover(string leverName, string coverName) : base(leverName)
        {
            _cover = new VRControlCover(coverName);
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
