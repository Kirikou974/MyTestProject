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
        [Control(SupportedBehavior = ControllerActionBehavior.Increase)]
        public IEnumerator Increase()
        {
            yield return SetState(UnityControlCurrentState + 1);
        }
        [Control(SupportedBehavior = ControllerActionBehavior.Decrease)]
        public IEnumerator Decrease()
        {
            yield return SetState(UnityControlCurrentState - 1);
        }
        [Control(SupportedBehavior = ControllerActionBehavior.On)]
        public IEnumerator On()
        {
            yield return SetState(UnityControlStates - 1);
        }
        [Control(SupportedBehavior = ControllerActionBehavior.Off)]
        public IEnumerator Off()
        {
            yield return SetState(0);
        }
        public virtual IEnumerator SetState(int state)
        {
            if (state != UnityControlCurrentState && state >= 0 && state < UnityControlStates)
            {
                StartControlInteraction();
                UnityControlSetState.Invoke(state);
                UnityControlSetPositionFromState();
                yield return WaitForDefaultTime();
                StopControlInteraction();
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
