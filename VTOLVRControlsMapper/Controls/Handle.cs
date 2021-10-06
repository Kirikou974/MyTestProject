using System;
using System.Collections;
using VTOLVRControlsMapper.Core;

namespace VTOLVRControlsMapper.Controls
{
    [ControlClass(UnityTypes = new Type[] { typeof(VRInteractable), typeof(EjectHandle) })]
    public class Handle : ControlBase<EjectHandle>
    {
        public Handle(string ejectHandleName) : base(ejectHandleName) { }
        [ControlMethod(SupportedBehavior = ControllerActionBehavior.Toggle)]
        public IEnumerator Pull()
        {
            UnityControl.OnHandlePull.Invoke();
            yield return null;
        }
        public override void StartControlInteraction(VRHandController hand)
        {
            throw new NotImplementedException();
        }
        public override void StopControlInteraction(VRHandController hand)
        {
            throw new NotImplementedException();
        }
    }
}
