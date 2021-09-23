using System;

namespace VTOLVRControlsMapper.Core
{
    public abstract class ControlLeverBase<T> : ControlToggleBase<T>
        where T : UnityEngine.Object
    {
        public abstract int UnityControlCurrentState { get; }
        public abstract int UnityControlStates { get; }
        public bool IsOff { get => UnityControlCurrentState == 0; }
        public abstract Action<int> UnityControlSetState { get; }
        public abstract Action UnityControlSetStateAfterSetState { get; }
        protected ControlLeverBase(string unityControlName) : base(unityControlName) { }
        [Control(SupportedBehavior = ControllerActionBehavior.Increase)]
        public void Increase()
        {
            SetState(UnityControlCurrentState + 1);
        }
        [Control(SupportedBehavior = ControllerActionBehavior.Decrease)]
        public void Decrease()
        {
            SetState(UnityControlCurrentState - 1);
        }
        [Control(SupportedBehavior = ControllerActionBehavior.On)]
        public void On()
        {
            SetState(UnityControlStates - 1);
        }
        [Control(SupportedBehavior = ControllerActionBehavior.Off)]
        public void Off()
        {
            SetState(0);
        }
        public virtual void SetState(int state)
        {
            if (state != UnityControlCurrentState && state >= 0 && state < UnityControlStates)
            {
                UnityControlSetState(state);
                UnityControlSetStateAfterSetState();
            }
        }
        public override void Toggle()
        {
            if (IsOff)
            {
                On();
            }
            else
            {
                Off();
            }
        }
    }
}
