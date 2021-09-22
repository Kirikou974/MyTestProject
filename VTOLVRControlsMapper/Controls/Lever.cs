﻿using System;
using VTOLVRControlsMapper.Core;

namespace VTOLVRControlsMapper.Controls
{
    public class Lever : ControlLeverBase<VRLever>
    {
        public Lever(string leverName) : base(leverName) { }
        public override int UnityControlCurrentState => UnityControl.currentState;
        public override int UnityControlStates => UnityControl.states;
        public override Action<int> UnityControlSetState => UnityControl.SetState;
        public override Action UnityControlSetStateAfterSetState => UnityControl.ReturnSpring;
    }
}
