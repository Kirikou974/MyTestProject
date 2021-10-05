using System.Collections;
using UnityEngine;

namespace VTOLVRControlsMapper.Core
{
    public abstract class ControlButtonBase<T> : ControlBase<T> where T : MonoBehaviour
    {
        public ControlButtonBase(string interactableName) : base(interactableName) { }
        [ControlMethod(SupportedBehavior = ControllerActionBehavior.Toggle)]
        public IEnumerator InteractWithControl()
        {
            VRHandController hand = ClosestHand;
            hand.gloveAnimation.ClearInteractPose();
            StartControlInteraction(hand);
            yield return WaitForDefaultTime();
            StopControlInteraction(hand);
        }
    }
}
