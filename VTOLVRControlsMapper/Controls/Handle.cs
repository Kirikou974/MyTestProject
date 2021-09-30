using System.Collections;
using VTOLVRControlsMapper.Core;

namespace VTOLVRControlsMapper.Controls
{
    public class Handle : ControlBase<EjectHandle>
    {
        public Handle(string ejectHandleName) : base(ejectHandleName) { }
        [Control(SupportedBehavior = ControllerActionBehavior.Toggle)]
        public IEnumerator Pull()
        {
            ClosestHand.gloveAnimation.SetLockTransform(UnityControl.handleTransform);
            ClosestHand.gloveAnimation.SetPoseInteractable(GloveAnimation.Poses.Eject);
            //UnityControl.OnHandlePull.Invoke();
            yield return null;
        }
        public override void StartControlInteraction()
        {
            throw new System.NotImplementedException();
        }
        public override void StopControlInteraction()
        {
            throw new System.NotImplementedException();
        }
    }
}
