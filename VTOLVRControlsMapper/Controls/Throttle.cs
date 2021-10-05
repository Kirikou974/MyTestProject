using System;
using System.Collections;
using VTOLVRControlsMapper.Core;

namespace VTOLVRControlsMapper.Controls
{
    [ControlClass(UnityTypes = new Type[] { typeof(VRInteractable), typeof(VRThrottle) })]
    public class Throttle : ControlAxis<VRThrottle>
    {
        public Throttle(string unityControlName) : base(unityControlName) { }
        public override IEnumerator Update(float value)
        {
            StartControlInteraction(ClosestHand);
            //UnityControl.OnSetThrottle.Invoke(value);
            UnityControl.UpdateThrottle(value);
            UnityControl.UpdateThrottleAnim(value);
            StopControlInteraction(ClosestHand);
            yield return null;
        }
        public override void StartControlInteraction(VRHandController hand)
        {
            hand.gloveAnimation.SetLeverTransform(UnityControl.transform);
            hand.gloveAnimation.SetPoseInteractable(GloveAnimation.Poses.Throttle);
        }
        public override void StopControlInteraction(VRHandController hand)
        {
            //UnityControl.I_OnStopInteraction(hand);
        }
    }
}
