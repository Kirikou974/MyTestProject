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
        public override FloatEvent OnTriggerAxis => UnityControl.OnTriggerAxis;
        public override UnityEvent OnTriggerButtonDown => UnityControl.OnTriggerDown;
        public override UnityEvent OnTriggerButtonUp => UnityControl.OnTriggerUp;
        public override void UpdateUnityControl()
        {
            //TODO test stick animation
            UnityControl.OnSetStick.Invoke(VectorUpdate);
            //UnityControl.SetStickAnimation();
        }
        public override void PressTriggerButton()
        {
            // use single fire if an AMRAMM
            if (VTOLAPI.GetPlayersVehicleGameObject().GetComponent<WeaponManager>().currentEquip is HPEquipRadarML)
            {
                StartControlInteraction(MainHand);
                VTOLAPI.GetPlayersVehicleGameObject().GetComponent<WeaponManager>().SingleFire();
            }
            else
            {
                base.PressTriggerButton();
            }
        }
    }
}
