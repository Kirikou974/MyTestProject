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
        [ControlMethod(SupportedBehavior = ControllerActionBehavior.HoldOn)]
        public override void StartControlInteraction(VRHandController hand)
        {
            hand.gloveAnimation.PressButton(UnityControl.transform, true);
            InteractableControl.StartControlInteraction(hand);
        }
        [ControlMethod(SupportedBehavior = ControllerActionBehavior.HoldOff)]
        public override void StopControlInteraction(VRHandController hand)
        {
            InteractableControl.StopControlInteraction(hand);
            ClosestHand.gloveAnimation.UnPressButton();
        }
    }
}
