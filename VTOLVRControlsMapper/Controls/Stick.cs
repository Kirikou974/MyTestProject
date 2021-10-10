using System;
using System.Collections;
using VTOLVRControlsMapper.Core;

namespace VTOLVRControlsMapper.Controls
{
    [ControlClass(UnityTypes = new Type[] { typeof(VRInteractable), typeof(VRJoystick) })]
    public class Stick : ControlJoystick<VRJoystick>
    {
        public Stick(string unityControlName) : base(unityControlName) { }

        public override void ClickMenu()
        {
            throw new NotImplementedException();
        }

        public override void StartControlInteraction(VRHandController hand)
        {
            throw new NotImplementedException();
        }
        public override void UpdateAxis(float value)
        {
            throw new NotImplementedException();
        }
    }
}
