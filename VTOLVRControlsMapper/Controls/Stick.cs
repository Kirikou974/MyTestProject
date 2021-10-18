using System;
using UnityEngine.Events;
using VTOLVRControlsMapper.Core;

namespace VTOLVRControlsMapper.Controls
{
    [ControlClass(UnityTypes = new Type[] { typeof(VRInteractable), typeof(VRJoystick) })]
    public class Stick : ControlJoystick<VRJoystick>
    {
        private VRInteractable _vrInteractable;
        public Stick(string unityControlName) : base(unityControlName)
        {
            _vrInteractable = ControlsHelper.GetGameControl<VRInteractable>(unityControlName);
        }
        public override UnityEvent OnMenuButtonDown => UnityControl.OnMenuButtonDown;
        public override UnityEvent OnMenuButtonUp => UnityControl.OnMenuButtonUp;
        public override VRHandController MainHand => RightHand;
        public override Vector3Event OnSetThumbstick => UnityControl.OnSetThumbstick;
        public override VRInteractable VRInteractable => _vrInteractable;
        public override void UpdateControl()
        {
            UnityControl.OnSetStick.Invoke(VectorUpdate);
            //UnityControl.SetStickAnimation();
        }
    }
}
