using VTOLVRControlsMapper.Core;

namespace VTOLVRControlsMapper.Controls
{
    public class Handle : ControlBase<EjectHandle>
    {
        public Handle(string ejectHandleName) : base(ejectHandleName) { }
        [Control(SupportedBehavior = ControllerActionBehavior.Toggle)]
        public void Pull()
        {
            UnityControl.OnHandlePull.Invoke();
        }
    }
}
