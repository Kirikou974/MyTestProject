using System;

namespace VTOLVRControlsMapper.Core
{
    public abstract class ControlKnobBase<T> : ControlLeverBase<T>
        where T : UnityEngine.Object
    {
        protected ControlKnobBase(string unityControlName) : base(unityControlName) { }
    }
}
