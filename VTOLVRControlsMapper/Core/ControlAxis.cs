using System.Collections;

namespace VTOLVRControlsMapper.Core
{
    public abstract class ControlAxis<T> : ControlBase<T> where T : UnityEngine.MonoBehaviour
    {
        public ControlAxis(string unityControlName) : base(unityControlName) { }
        [ControlMethod(SupportedBehavior = ControllerActionBehavior.Axis)]
        public abstract IEnumerator Update(float value);
    }
}
