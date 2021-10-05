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
            UnityControl.UpdateThrottle(value);
            UnityControl.UpdateThrottleAnim(value);
            yield return null;
        }
        public override void StartControlInteraction(VRHandController hand)
        {
        }
        public override void StopControlInteraction(VRHandController hand)
        {
        }
    }
}
