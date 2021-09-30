using System;
using System.Collections;
using UnityEngine;
using VTOLVRControlsMapper.Core;

namespace VTOLVRControlsMapper.Controls
{
    public class Button : ControlButtonBase<VRButton>
    {
        public Interactable InteractableControl { get; protected set; }
        public Button(string interactableName) : base(interactableName)
        {
            InteractableControl = new Interactable(ControlName);
        }
        [Control(SupportedBehavior = ControllerActionBehavior.HoldOn)]
        public override void StartControlInteraction()
        {
            MonoBehaviour behaviour = UnityControl;
            ClosestHand.gloveAnimation.PressButton(behaviour.transform, true);
            InteractableControl.StartControlInteraction();
        }
        [Control(SupportedBehavior = ControllerActionBehavior.HoldOff)]
        public override void StopControlInteraction()
        {
            InteractableControl.StopControlInteraction();
            ClosestHand.gloveAnimation.UnPressButton();
        }
    }
}
