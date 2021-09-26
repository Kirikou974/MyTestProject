using VTOLVRControlsMapper.Core;

namespace VTOLVRControlsMapper.Core
{
    public abstract class ControlButtonBase<T> : ControlBase<T> where T : UnityEngine.Object
    {
        public ControlButtonBase(string interactableName) : base(interactableName) { }
        [Control(SupportedBehavior = ControllerActionBehavior.Toggle)]
        public void Invoke()
        {
            StartInteract();
            StopInteract();
        }
        [Control(SupportedBehavior = ControllerActionBehavior.HoldOn)]
        public abstract void StartInteract();
        [Control(SupportedBehavior = ControllerActionBehavior.HoldOff)]
        public abstract void StopInteract();
    }
}
