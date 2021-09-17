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

        public VRControlLeverWithCover(VRLever lever, VRControlCover cover) : base(lever)
        {
            _cover = cover;
        }

        public override void Invoke()
        {
            VRControlHelper.Mod.Log("covered : " + _cover.UnityControl.covered);
            VRControlHelper.Mod.Log("setOffOnClosed : " + _cover.UnityControl.setOffOnClosed);

            if (!_cover.UnityControl.covered)
            {
                base.Invoke();
            }
        }
    }
}
