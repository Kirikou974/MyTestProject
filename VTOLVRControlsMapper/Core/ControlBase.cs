using System;

namespace VTOLVRControlsMapper.Core
{
    public interface IControl { }
    public interface IControl<T> : IControl where T : UnityEngine.Object { }
    public abstract class ControlBase<T> : IControl<T> where T : UnityEngine.Object
    {
        public T UnityControl { get; protected set; }
        public string ControlName => UnityControl.name;
        public ControlBase(string unityControlName)
        {
            UnityControl = ControlsHelper.GetGameControl<T>(unityControlName);
            if (UnityControl is null)
            {
                throw new NullReferenceException(string.Format("Unity control {0} of type {1} not found", unityControlName, typeof(T).Name));
            }
            //IEnumerable<VRHandController> hands = ControlsHelper.GetGameControls<VRHandController>();
            //if (hands is null || hands.Count() > 0)
            //{
            //    throw new NullReferenceException("Hand controllers not found");
            //}
            //LeftHand = hands.ElementAt(1);
            //RightHand = hands.ElementAt(0);
        }
    }
}
