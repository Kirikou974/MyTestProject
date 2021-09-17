using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTOLVRControlsMapper
{
    public class VRControlButton: VRControlBase<VRButton>
    {
        private readonly VRControlInteractable _vrInteractable;
        public VRControlButton(Control button) : base(VRControlHelper.GetVRControl<VRButton>(button))
        {
            _vrInteractable = new VRControlInteractable(button);
        }
        public void Click()
        {
            _vrInteractable.Invoke();
        } 
    }
}
