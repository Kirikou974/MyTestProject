using System;
using UnityEngine.Events;

namespace VTOLVRControlsMapper
{
    public class VRControlSwitchKnob : VRControlKnobBase<VRTwistKnobInt>, IVRControlToggle
    {
        private VRControlInteractable _temp;
        private bool _isOff
        {
            get
            {
                return UnityControl.currentState == 0;
            }
        }
        public VRControlSwitchKnob(Control control) : base(VRControlHelper.GetVRControl<VRTwistKnobInt>(control))
        {
            _temp = new VRControlInteractable(control);
        }
        public override void Increase()
        {
            int newState = UnityControl.currentState + 1;
            SetState(newState);
        }
        public override void Decrease()
        {
            int newState = UnityControl.currentState - 1;
            SetState(newState);
        }
        public void Toggle()
        {
            if (_isOff)
            {
                On();
            }
            else
            {
                Off();
            }
        }
        public void On()
        {
            SetState(UnityControl.states - 1);
        }
        public void Off()
        {
            SetState(0);
        }

        public override void SetState(int state)
        {
            if (state != UnityControl.currentState && state >= 0 && state < UnityControl.states)
            {
                UnityControl.SetState(state);
                UnityControl.SetRotationFromState();
            }
        }
    }
}
