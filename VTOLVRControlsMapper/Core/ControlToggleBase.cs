namespace VTOLVRControlsMapper.Core
{
    public interface IControlToggle
    {
        void Toggle();
    }
    public abstract class ControlToggleBase<T> : ControlBase<T>, IControlToggle
        where T : UnityEngine.Object
    {
        protected ControlToggleBase(string unityControlName) : base(unityControlName) { }
        public abstract void Toggle();
    }
}
