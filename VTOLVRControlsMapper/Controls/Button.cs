using System;
using System.Collections;
using VTOLVRControlsMapper.Core;

namespace VTOLVRControlsMapper.Controls
{
    [ControlClass(UnityTypes = new Type[] { typeof(VRInteractable), typeof(VRButton) })]
    public class Button : ControlButtonBase<VRButton>
    {
        public Interactable InteractableControl { get; protected set; }
        public Button(string interactableName) : base(interactableName)
        {
            InteractableControl = new Interactable(ControlName);
        }
        public override void StartControlInteraction(VRHandController hand)
        {
            hand.gloveAnimation.PressButton(UnityControl.transform, true);
            InteractableControl.UnityControl.OnInteract.Invoke();
        }
        public override void StopControlInteraction(VRHandController hand)
        {
            Main.instance.Log("StopControlInteraction");
            InteractableControl.UnityControl.OnStopInteract.Invoke();
            ClosestHand.gloveAnimation.UnPressButton();
        }
        [ControlMethod(SupportedBehavior = ControllerActionBehavior.HoldOff)]
        public IEnumerator HoldOff()
        {
            StopControlInteraction(ClosestHand);
            yield return null;
        }
        [ControlMethod(SupportedBehavior = ControllerActionBehavior.HoldOn)]
        public IEnumerator HoldOn()
        {
            StartControlInteraction(ClosestHand);
            yield return null;
        }
    }
}
