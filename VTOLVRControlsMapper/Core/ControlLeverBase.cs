namespace VTOLVRControlsMapper.Core
{
    public abstract class ControlLeverBase<T> : ControlToggleBase<T>
        where T : UnityEngine.Object
    {
        public abstract int UnityControlCurrentState { get; }
        public abstract int UnityControlStates { get; }
        public bool IsOff { get => UnityControlCurrentState == 0; }
        protected ControlLeverBase(string unityControlName) : base(unityControlName) { }
        [Control(SupportedBehavior = ControllerActionBehavior.Increase)]
        public void Increase()
        {
            int newState = UnityControlCurrentState + 1;
            SetState(newState);
        }
        [Control(SupportedBehavior = ControllerActionBehavior.Decrease)]
        public void Decrease()
        {
            int newState = UnityControlCurrentState - 1;
            SetState(newState);
        }
        [Control(SupportedBehavior = ControllerActionBehavior.On)]
        public void On()
        {
            int newState = UnityControlStates - 1;
            SetState(newState);
        }
        [Control(SupportedBehavior = ControllerActionBehavior.Off)]
        public void Off()
        {
            SetState(0);
        }
        public abstract void SetState(int state);
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
