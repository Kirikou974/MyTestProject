using System;
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
        public override void StartControlInteraction()
        {
            ClosestHand.gloveAnimation.PressButton(UnityControl.transform, true);
            InteractableControl.StartControlInteraction();
        }
        [ControlMethod(SupportedBehavior = ControllerActionBehavior.HoldOff)]
        public override void StopControlInteraction()
        {
            InteractableControl.StopControlInteraction();
            ClosestHand.gloveAnimation.UnPressButton();
        }
    }
}
