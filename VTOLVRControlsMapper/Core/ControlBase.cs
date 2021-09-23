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
        public VRHandController LeftHand => Main.GetGameControls<VRHandController>()[1];
        public VRHandController RightHand => Main.GetGameControls<VRHandController>()[0];
        public T UnityControl
        {
            get;
            internal set;
        }
        public string ControlName => UnityControl.name;
        public ControlBase(string unityControlName)
        {
            UnityControl = Main.GetGameControl<T>(unityControlName);
            if (UnityControl is null)
            {
                throw new NullReferenceException(string.Format("Unity control {0} of type {1} not found", unityControlName, typeof(T).Name));
            }
        }
    }
}
