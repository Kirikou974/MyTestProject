using System;
using System.Collections;
using System.Reflection;
using VTOLVRControlsMapper.Core;

namespace VTOLVRControlsMapper.Controls
{
    [ControlClass(UnityTypes = new Type[] { typeof(VRInteractable), typeof(VRThrottle) })]
    public class Throttle : ControlAxis<VRThrottle>
    {
        public Throttle(string unityControlName) : base(unityControlName)
        {
            //Remove controller vibration
            Type handType = UnityControl.GetType();
            FieldInfo hapticFactorField = handType.GetField("hapticFactor", BindingFlags.Instance | BindingFlags.NonPublic);
            hapticFactorField.SetValue(UnityControl, 0.0f);
        }
        public override IEnumerator Update(float value)
        {
            StartControlInteraction(LeftHand);
            UnityControl.OnSetThrottle.Invoke(value);
            UnityControl.UpdateThrottleAnim(value);
            yield return null;
        }
        public override void StartControlInteraction(VRHandController hand)
        {
            hand.gloveAnimation.SetPoseInteractable(GloveAnimation.Poses.JetThrottle);

            //Set active controller on the interactable
            Type interactableType = UnityControl.interactable.GetType();
            PropertyInfo activeControllerProperty = interactableType.GetProperty(nameof(UnityControl.interactable.activeController));
            activeControllerProperty.SetValue(UnityControl.interactable, hand);

            //Set active interactable on the controller
            hand.activeInteractable = UnityControl.interactable;

            //Start throttle interaction
            UnityControl.interactable.StartInteraction();

            //Set grabbed to false so that animation is only updated by the mod
            Type unityControlType = UnityControl.GetType();
            FieldInfo grabbedField = unityControlType.GetField("grabbed", BindingFlags.Instance | BindingFlags.NonPublic);
            grabbedField.SetValue(UnityControl, false);
        }
        public override void StopControlInteraction(VRHandController hand)
        {
        }
    }
}
