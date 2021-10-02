using System;
using System.Collections;
using UnityEngine;

namespace VTOLVRControlsMapper.Core
{
    public interface IControl { }
    public interface IControl<T> : IControl where T : MonoBehaviour
    {
        T UnityControl { get; }
        string ControlName { get; }
        VRHandController ClosestHand { get; }
    }
    public abstract class ControlBase<T> : IControl<T> where T : MonoBehaviour
    {
        protected float WaitTime { get => 0.25f; }
        public VRHandController ClosestHand
        {
            get
            {
                if (VRHandController.controllers.Count != 2)
                {
                    throw new Exception("Hands not found. Pas de bras, pas de chocolat.");
                }
                VRHandController leftHand = VRHandController.controllers[1];
                VRHandController rightHand = VRHandController.controllers[0];

                Vector3 controlPosition = UnityControl.transform.position;
                float distanceFromLeftHand = Vector3.Distance(controlPosition, leftHand.transform.position);
                float distanceFromRightHand = Vector3.Distance(controlPosition, rightHand.transform.position);
                if (distanceFromLeftHand < distanceFromRightHand)
                {
                    return leftHand;
                }
                else
                {
                    return rightHand;
                }
            }
        }
        public T UnityControl { get; protected set; }
        public string ControlName { get => UnityControl.name; }
        public abstract IEnumerator StartControlInteraction(VRHandController hand);
        public abstract void StopControlInteraction(VRHandController hand);
        public ControlBase(string unityControlName)
        {
            UnityControl = ControlsHelper.GetGameControl<T>(unityControlName);
            if (UnityControl is null)
            {
                throw new NullReferenceException(string.Format("Unity control {0} of type {1} not found", unityControlName, typeof(T).Name));
            }
        }
        public IEnumerator WaitForDefaultTime()
        {
            yield return WaitFor(WaitTime);
        }
        public IEnumerator WaitFor(float seconds)
        {
            yield return new WaitForSeconds(seconds);
        }
    }
}
