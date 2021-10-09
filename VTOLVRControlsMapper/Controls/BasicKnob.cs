using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using VTOLVRControlsMapper.Core;

namespace VTOLVRControlsMapper.Controls
{
    [ControlClass(UnityTypes = new Type[] { typeof(VRInteractable), typeof(VRTwistKnobInt) })]
    public class BasicKnob : ControlLeverBase<VRTwistKnobInt>
    {
        public override int UnityControlCurrentState { get => UnityControl.currentState; }
        public override int UnityControlStates { get => UnityControl.states; }
        public override Action<int> UnityControlSetState { get => UnityControl.SetState; }
        public override Action UnityControlSetPositionFromState { get => UnityControl.SetRotationFromState; }
        public BasicKnob(string twistKnobIntName) : base(twistKnobIntName) { }
        public override void StartControlInteraction(VRHandController hand)
        {
            //Getting protected property value
            FieldInfo fieldInfo = UnityControl.GetType().GetField("lockTransform", BindingFlags.NonPublic | BindingFlags.Instance);
            Transform lockTransform = fieldInfo.GetValue(UnityControl) as Transform;

            hand.gloveAnimation.SetKnobTransform(UnityControl.knobTransform, lockTransform, UnityControl.smallKnob);
            hand.gloveAnimation.SetPoseInteractable(GloveAnimation.Poses.Knob);

        }
        public override IEnumerator StartSecondaryControlInteraction()
        {
            yield return WaitFor(0.15f);
            yield return UnityControl.GrabbedRoutine();
        } 
        public override void StopControlInteraction(VRHandController hand)
        {
            UnityControl.Vrint_OnStopInteraction(hand);
        }
    }
}
