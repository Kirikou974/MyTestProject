using System.Linq;
using VTOLVRControlsMapper.Core;

namespace VTOLVRControlsMapper.Controls
{
    public class Hands : IControl<VRHandController>
    {
        public VRHandController LeftHand { get; set; }
        public VRHandController RightHand { get; set; }
        public Hands()
        {
            RightHand = ControlsHelper.GetGameControls<VRHandController>().ElementAt(0);
            LeftHand = ControlsHelper.GetGameControls<VRHandController>().ElementAt(1);
        }
    }
}
