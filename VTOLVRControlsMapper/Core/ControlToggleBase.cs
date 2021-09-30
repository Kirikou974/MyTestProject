using System.Collections;
using UnityEngine;

namespace VTOLVRControlsMapper.Core
{
    public abstract class ControlToggleBase<T> : ControlBase<T> where T : MonoBehaviour
    {
        protected ControlToggleBase(string unityControlName) : base(unityControlName) { }
        [Control(SupportedBehavior = ControllerActionBehavior.Toggle)]
        public abstract IEnumerator Toggle();
    }
}
