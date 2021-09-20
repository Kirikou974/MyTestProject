namespace VTOLVRControlsMapper.Core
{
    public interface IControlKnob
    {
        void Increase();
        void Decrease();
        void SetState(int state);
    }
    public abstract class ControlKnobBase<T> : ControlBase<T>, IControlKnob
        where T : UnityEngine.Object
    {
        protected ControlKnobBase(T unityControl) : base(unityControl) { }
        public abstract void Increase();
        public abstract void Decrease();
        public abstract void SetState(int state);
    }
}
