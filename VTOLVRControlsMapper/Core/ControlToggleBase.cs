namespace VTOLVRControlsMapper.Core
{
    public interface IControlToggle
    {
        void Toggle();
    }
    public abstract class ControlToggleBase<T> : ControlBase<T>, IControlToggle
        where T : UnityEngine.Object
    {
        protected ControlToggleBase(T unityControl) : base(unityControl) { }
        public abstract void Toggle();
    }
}
