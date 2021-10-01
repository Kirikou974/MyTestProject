using System.Collections;
using UnityEngine;

namespace VTOLVRControlsMapper.Core
{
    public abstract class ControlButtonBase<T> : ControlBase<T> where T : MonoBehaviour
    {
        public ControlButtonBase(string interactableName) : base(interactableName) { }
        [ControlMethod(SupportedBehavior = ControllerActionBehavior.Toggle)]
        public IEnumerator InteractWithControl()
        {
            StartControlInteraction();
            yield return WaitForDefaultTime();
            StopControlInteraction();
        }
    }
}
