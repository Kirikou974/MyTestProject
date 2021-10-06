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

                Vector3 controlPosition = UnityControl.transform.position;
                float distanceFromLeftHand = Vector3.Distance(controlPosition, LeftHand.transform.position);
                float distanceFromRightHand = Vector3.Distance(controlPosition, RightHand.transform.position);
                if (distanceFromLeftHand < distanceFromRightHand)
                {
                    return LeftHand;
                }
                else
                {
                    return RightHand;
                }
            }
        }
        public VRHandController LeftHand
        {
            get;
            protected set;
        }
        public VRHandController RightHand
        {
            get;
            protected set;
        }
        public T UnityControl { get; protected set; }
        public string ControlName { get => UnityControl.name; }
        public abstract void StartControlInteraction(VRHandController hand);
        public abstract void StopControlInteraction(VRHandController hand);
        public ControlBase(string unityControlName)
        {
            UnityControl = ControlsHelper.GetGameControl<T>(unityControlName);
            if (UnityControl is null)
            {
                throw new NullReferenceException(string.Format("Unity control {0} of type {1} not found", unityControlName, typeof(T).Name));
            }
            LeftHand = VRHandController.controllers.Find(h => h.isLeft);
            RightHand = VRHandController.controllers.Find(h => !h.isLeft);
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
