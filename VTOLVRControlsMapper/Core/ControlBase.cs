using System;
using System.Collections.Generic;
using System.Linq;

namespace VTOLVRControlsMapper.Core
{
    public interface IControl
    {
        string ControlName { get; }
        VRHandController LeftHand { get; set; }
        VRHandController RightHand { get; set; }
    }
    public interface IControl<T> : IControl where T : UnityEngine.Object { }
    public abstract class ControlBase<T> : IControl<T>
        where T : UnityEngine.Object
    {
        public VRHandController LeftHand { get; set; }
        public VRHandController RightHand { get; set; }
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
