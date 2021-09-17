using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTOLVRControlsMapper
{
    public class VRControlLeverWithCover: VRControlLever
    {
        private VRControlCover _cover;
        public VRControlLeverWithCover(Control lever, Control cover) : base(lever)
        {
            _cover = new VRControlCover(cover);
        }

        public override void Toggle()
        {
            VRControlHelper.Mod.Log(string.Format("Trying to toggle {0} of type {1}", Control, this.GetType().Name));
            VRControlHelper.Mod.Log("- covered : " + _cover.UnityControl.covered);

            if (!_cover.UnityControl.covered)
            {
                base.Toggle();
            }
        }
    }
}
