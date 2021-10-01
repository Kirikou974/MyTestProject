using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using VTOLVRControlsMapper.Core;
using UnityEngine;

namespace VTOLVRControlsMapper.Controls
{
    [ControlClass(UnityTypes = new Type[] { typeof(VRInteractable), typeof(VRLever) })]
    public class Lever : ControlLeverBase<VRLever>
    {
        public Lever(string leverName) : base(leverName)
        {
            IEnumerable<VRSwitchCover> covers = ControlsHelper.GetGameControls<VRSwitchCover>();
            Cover = covers.SingleOrDefault(c => c.coveredSwitch.name == ControlName);
        }
        public override int UnityControlCurrentState { get => UnityControl.currentState; }
        public override int UnityControlStates { get => UnityControl.states; }
        public bool HasCover { get => Cover != null; }
        public VRSwitchCover Cover
        {
            get;
            protected set;
        }
        public override Action<int> UnityControlSetState { get => UnityControl.SetState; }
        public override Action UnityControlSetPositionFromState { get => UnityControl.ReturnSpring; }
        public override IEnumerator SetState(int state)
        {
            if (!HasCover || (HasCover && !Cover.covered))
            {
                yield return base.SetState(state);
            }
            else
            {
                yield return null;
            }
        }
        public override void StartControlInteraction()
        {
            ClosestHand.gloveAnimation.SetLeverTransform(UnityControl.transform);
            ClosestHand.gloveAnimation.SetPoseInteractable(GloveAnimation.Poses.Pinch);
            UnityControl.GrabbedRoutine();
        }
        public override void StopControlInteraction()
        {
            ClosestHand.gloveAnimation.ClearInteractPose();
            UnityControl.Vrint_OnStopInteraction(ClosestHand);
        }
    }
}
