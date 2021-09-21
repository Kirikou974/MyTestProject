namespace VTOLVRControlsMapper.Core
{
    public abstract class ControlToggleBase<T> : ControlBase<T>
        where T : UnityEngine.Object
    {
        protected ControlToggleBase(string unityControlName) : base(unityControlName) { }
        [Control(SupportedBehavior = ControllerActionBehavior.Toggle)]
        public abstract void Toggle();
    }
}
