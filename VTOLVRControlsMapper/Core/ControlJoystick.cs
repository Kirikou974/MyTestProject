using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using static GloveAnimation;

namespace VTOLVRControlsMapper.Core
{
    public abstract class ControlJoystick<T> : ControlBase<T> where T : MonoBehaviour
    {
        private readonly FieldInfo _grabbedField;
        private readonly PropertyInfo _activeControllerProperty;
        public bool Grabbed
        {
            get => (bool)_grabbedField.GetValue(UnityControl);
            set => _grabbedField.SetValue(UnityControl, value);
        }
        public abstract VRInteractable VRInteractable { get; }
        public abstract UnityEvent OnMenuButtonDown { get; }
        public abstract UnityEvent OnMenuButtonUp { get; }
        public abstract Vector3Event OnSetThumbstick { get; }
        public abstract VRHandController MainHand { get; }
        public Vector3 VectorUpdate { get; protected set; }
        public ControlJoystick(string unityControlName) : base(unityControlName)
        {
            //Remove controller vibration
            Type handType = UnityControl.GetType();
            FieldInfo hapticFactorField = handType.GetField("hapticFactor", BindingFlags.Instance | BindingFlags.NonPublic);
            hapticFactorField.SetValue(UnityControl, 0.0f);

            Type interactableType = typeof(VRInteractable);
            _activeControllerProperty = interactableType.GetProperty("activeController");

            Type unityControlType = UnityControl.GetType();
            _grabbedField = unityControlType.GetField("grabbed", BindingFlags.Instance | BindingFlags.NonPublic);
        }
        public void UpdateThumbstickAxis(Vector3 vector)
        {
            StartControlInteraction(MainHand);
            OnSetThumbstick.Invoke(vector);
        }
        public abstract void UpdateUnityControl();
        public virtual void UpdateMainAxis(Vector3 vector)
        {
            StartControlInteraction(MainHand);
            VectorUpdate = vector;
            UpdateUnityControl();
        }
        public void ClickMenu()
        {
            StartControlInteraction(MainHand);
            OnMenuButtonDown.Invoke();
            OnMenuButtonUp.Invoke();
        }
        public void UpdateTriggerAxis()
        {
            StartControlInteraction(MainHand);
            OnMenuButtonDown.Invoke();
            OnMenuButtonUp.Invoke();
        }
        public override void StartControlInteraction(VRHandController hand)
        {
            //Set active controller on the interactable
            _activeControllerProperty.SetValue(VRInteractable, hand);
            //Set active interactable on the controller
            hand.activeInteractable = VRInteractable;
            //Start throttle interaction
            VRInteractable.StartInteraction();
            //Set grabbed to false so that animation is only updated by the mod
            Grabbed = false;
        }
    }
}
