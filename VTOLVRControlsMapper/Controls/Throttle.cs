﻿using System;
using UnityEngine.Events;
using VTOLVRControlsMapper.Core;

namespace VTOLVRControlsMapper.Controls
{
    [ControlClass(UnityTypes = new Type[] { typeof(VRInteractable), typeof(VRThrottle) })]
    public class Throttle : ControlJoystick<VRThrottle>
    {
        public override UnityEvent OnMenuButtonDown => UnityControl.OnMenuButtonDown;
        public override UnityEvent OnMenuButtonUp => UnityControl.OnMenuButtonUp;
        public override VRHandController MainHand => LeftHand;
        public override Vector3Event OnSetThumbstick => UnityControl.OnSetThumbstick;
        public override VRInteractable VRInteractable => UnityControl.interactable;
        public override FloatEvent OnTriggerAxis => UnityControl.OnTriggerAxis;
        public override UnityEvent OnTriggerButtonDown => UnityControl.OnTriggerDown;
        public override UnityEvent OnTriggerButtonUp => UnityControl.OnTriggerUp;
        public Throttle(string unityControlName) : base(unityControlName) { }
        public override void UpdateUnityControl()
        {
            UnityControl.OnSetThrottle.Invoke(VectorUpdate.x);
            //UnityControl.UpdateThrottleAnim(VectorUpdate.x);
        }
    }
}
