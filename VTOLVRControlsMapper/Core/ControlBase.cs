using System;

namespace VTOLVRControlsMapper.Core
{
    public interface IControl
    {
        string ControlName { get; }
    }
    public interface IControl<T1> : IControl
        where T1 : UnityEngine.Object
    {
        T1 UnityControl { get; }
    }
    public abstract class ControlBase<T> : IControl<T>
        where T : UnityEngine.Object
    {
        public T UnityControl
        {
            get;
            internal set;
        }
        public ControlBase(string unityControlName)
        {
            UnityControl = Main.GetGameControl<T>(unityControlName);
            if (UnityControl is null)
            {
                throw new NullReferenceException(string.Format("Unity control not found ({0})", typeof(T).Name));
            }
            _controlName = UnityControl.name;
        }

        private readonly string _controlName;
        public string ControlName
        {
            get
            {
                return _controlName;
            }
        }
    }
}
