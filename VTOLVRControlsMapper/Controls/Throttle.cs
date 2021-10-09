using System;
using System.Collections;
using System.Reflection;
using VTOLVRControlsMapper.Core;

namespace VTOLVRControlsMapper.Controls
{
    [ControlClass(UnityTypes = new Type[] { typeof(VRInteractable), typeof(VRThrottle) })]
    public class Throttle : ControlJoystick<VRThrottle>
    {
        private PropertyInfo _activeControllerProperty;
        private FieldInfo _grabbedField;
        public Throttle(string unityControlName) : base(unityControlName)
        {
            //Remove controller vibration
            Type handType = UnityControl.GetType();
            FieldInfo hapticFactorField = handType.GetField("hapticFactor", BindingFlags.Instance | BindingFlags.NonPublic);
            hapticFactorField.SetValue(UnityControl, 0.0f);

            Type interactableType = UnityControl.interactable.GetType();
            Type unityControlType = UnityControl.GetType();
            _activeControllerProperty = interactableType.GetProperty(nameof(UnityControl.interactable.activeController));
            _grabbedField = unityControlType.GetField("grabbed", BindingFlags.Instance | BindingFlags.NonPublic);
        }
        public override void UpdateAxis(float value)
        {
            StartControlInteraction(LeftHand);
            UnityControl.OnSetThrottle.Invoke(value);
            UnityControl.UpdateThrottleAnim(value);
        }
        public override void StartControlInteraction(VRHandController hand)
        {
            hand.gloveAnimation.SetPoseInteractable(GloveAnimation.Poses.JetThrottle);

            //Set active controller on the interactable
            _activeControllerProperty.SetValue(UnityControl.interactable, hand);

            //Set active interactable on the controller
            hand.activeInteractable = UnityControl.interactable;

            //Start throttle interaction
            UnityControl.interactable.StartInteraction();

            //Set grabbed to false so that animation is only updated by the mod
            _grabbedField.SetValue(UnityControl, false);
        }
    }
}
