﻿using System.Collections;

namespace VTOLVRControlsMapper.Core
{
    public abstract class ControlJoystick<T> : ControlBase<T> where T : UnityEngine.MonoBehaviour
    {
        public ControlJoystick(string unityControlName) : base(unityControlName) { }
        [ControlMethod(SupportedBehavior = ControllerActionBehavior.Axis)]
        public abstract void UpdateAxis(float value);
        public override void StopControlInteraction(VRHandController hand) { }
    }
}