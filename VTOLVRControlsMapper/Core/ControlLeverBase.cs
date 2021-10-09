using System;
using System.Collections;
using UnityEngine;

namespace VTOLVRControlsMapper.Core
{
    public abstract class ControlLeverBase<T> : ControlToggleBase<T> where T : MonoBehaviour
    {
        public abstract int UnityControlCurrentState { get; }
        public abstract int UnityControlStates { get; }
        public bool IsOff { get => UnityControlCurrentState == 0; }
        public abstract Action<int> UnityControlSetState { get; }
        public abstract Action UnityControlSetPositionFromState { get; }
        protected ControlLeverBase(string unityControlName) : base(unityControlName) { }
        [ControlMethod(SupportedBehavior = ControllerActionBehavior.Increase)]
        public IEnumerator Increase()
        {
            yield return SetState(UnityControlCurrentState + 1);
        }
        [ControlMethod(SupportedBehavior = ControllerActionBehavior.Decrease)]
        public IEnumerator Decrease()
        {
            yield return SetState(UnityControlCurrentState - 1);
        }
        [ControlMethod(SupportedBehavior = ControllerActionBehavior.On)]
        public IEnumerator On()
        {
            yield return SetState(UnityControlStates - 1);
        }
        [ControlMethod(SupportedBehavior = ControllerActionBehavior.Off)]
        public IEnumerator Off()
        {
            yield return SetState(0);
        }
        public virtual IEnumerator StartSecondaryControlInteraction()
        {
            yield return null;
        }
        public virtual IEnumerator SetState(int state)
        {
            //Check if selected control state is within boundaries of supported states
            if (state != UnityControlCurrentState && state >= 0 && state < UnityControlStates)
            {
                VRHandController hand = ClosestHand;
                hand.gloveAnimation.ClearInteractPose();
                StartControlInteraction(hand);
                yield return StartSecondaryControlInteraction();
                UnityControlSetState.Invoke(state);
                UnityControlSetPositionFromState();
                yield return WaitForDefaultTime();
                StopControlInteraction(hand);
            }
        }

        public override IEnumerator Toggle()
        {
            if (IsOff)
            {
                yield return On();
            }
            else
            {
                yield return Off();
            }
        }
    }
}
