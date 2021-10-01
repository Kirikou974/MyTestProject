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
            StartControlInteraction();
            //UnityControl.OnHandlePull.Invoke();
            yield return WaitForDefaultTime();
            StopControlInteraction();
        }
        public override void StartControlInteraction()
        {
            //Interactable interactable = new Interactable(ControlName);
            ClosestHand.gloveAnimation.SetLeverTransform(UnityControl.transform);
            ClosestHand.gloveAnimation.SetPoseInteractable(GloveAnimation.Poses.Eject);
            //UnityControl.Interactable_OnStartInteraction(ClosestHand);
        }
        public override void StopControlInteraction()
        {
            //ClosestHand.gloveAnimation.ClearInteractPose();
            //UnityControl.Interactable_OnStopInteraction(ClosestHand);
        }
    }
}
