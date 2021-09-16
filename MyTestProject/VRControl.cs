using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MyTestProject
{
    public enum ControlType
    {
        VRInteractable,
        VRSwitchCover,
        VRLever,
        VRButton,
        VRTwistKnob,
        VRTwistKnobInt,
        VRJoystick,
        VRThrottle,
        None
    }

    public enum Control
    {
        GearInteractable,

        helmVisorInteractable,
        helmNVGInteractable,
        
        altitudeAPButton,
        speedAPButton,
        headingAPButton,
        navAPButton,
        ClrWptButton,
        offAPButton,
        MasterCautionInteractable,
        AltitudeModeInteractable,
        mfdSwapButton,
        
        FuelPort,
        
        CATOTrimInteractable,
        GLimitInteractable,
        RollSASInteractable,
        YawSASInteractable,
        PitchSASInteractable,
        AssistMasterInteractable,
        
        FlareInteractable,
        ChaffInteractable,
        
        BrakeLockInteractable,
        WingSwitchInteractable,
        
        NavLightInteractable,
        StrobLightInteractable,
        LandingLightInteractable,
        InsturmentLightInteractable
    }

    public enum Plane
    {
        FA26B,
        F45A,
        AV42C
    }
    public interface IVRControl
    {
        void Invoke();
        Control Control { get; }
    }
    public static class VRControl
    {
        private static VRInteractable[] _vrInteractables;
        private static VRButton[] _vrButtons;
        private static VRTwistKnob[] _vrTwistKnobs;
        private static VRTwistKnobInt[] _vrTwistKnobsInt;
        private static VRSwitchCover[] _vrSwitchCovers;
        private static VRLever[] _vrLevers;
        private static VTOLMOD _mod;
        private static List<IVRControl> _vrControls;
        public static VTOLMOD Mod { get { return _mod; } }

        public static IVRControl Get(Control control)
        {
            if (!ControlsLoaded)
            {
                throw new Exception("Controls not loaded");
            }
            return _vrControls.Find(x => x.Control == control);
        }

        public static VRInteractable GetVRInteractable(Control control)
        {
            if (!ControlsLoaded)
            {
                throw new Exception("Controls not loaded");
            }
            return _vrInteractables.ToList().Find(x => x.name == control.ToString());
        }

        public static bool ControlsLoaded
        {
            get
            {
                return
                    _vrInteractables != null && _vrInteractables.Length > 0 &&
                    _vrButtons != null && _vrButtons.Length > 0 &&
                    _vrTwistKnobs != null && _vrTwistKnobs.Length > 0 &&
                    _vrTwistKnobsInt != null && _vrTwistKnobsInt.Length > 0 &&
                    _vrSwitchCovers != null && _vrSwitchCovers.Length > 0 &&
                    _vrLevers != null && _vrLevers.Length > 0;
            }
        }
        public static void LoadControls(VTOLMOD mod, VTOLVehicles vehicle)
        {
            _mod = mod;
            Mod.Log("Start LoadControls for " + vehicle);

            _vrInteractables = UnityEngine.Object.FindObjectsOfType<VRInteractable>();
            _vrButtons = UnityEngine.Object.FindObjectsOfType<VRButton>();
            _vrTwistKnobs = UnityEngine.Object.FindObjectsOfType<VRTwistKnob>();
            _vrTwistKnobsInt = UnityEngine.Object.FindObjectsOfType<VRTwistKnobInt>();
            _vrSwitchCovers = UnityEngine.Object.FindObjectsOfType<VRSwitchCover>();
            _vrLevers = UnityEngine.Object.FindObjectsOfType<VRLever>();

            if (ControlsLoaded)
            {
                Mod.Log(" - _vrInteractables count : " + _vrInteractables.Count());
                Mod.Log(" - _vrButtons count : " + _vrButtons.Count());
                Mod.Log(" - _vrTwistKnobs count : " + _vrTwistKnobs.Count());
                Mod.Log(" - _vrTwistKnobsInt count : " + _vrTwistKnobsInt.Count());
                Mod.Log(" - _vrSwitchCovers count : " + _vrSwitchCovers.Count());
                Mod.Log(" - _vrLevers count : " + _vrLevers.Count());
                _vrControls = new List<IVRControl>();

                switch (vehicle)
                {
                    case VTOLVehicles.FA26B:
                        _vrControls.Add(new VRControl<VRLever>(_vrLevers.ToList().Find(x => x.name == Control.GearInteractable.ToString())));
                        
                        _vrControls.Add(new VRControl<VRLever>(_vrLevers.ToList().Find(x => x.name == Control.FuelPort.ToString())));

                        _vrControls.Add(new VRControl<VRLever>(_vrLevers.ToList().Find(x => x.name == Control.CATOTrimInteractable.ToString())));
                        _vrControls.Add(new VRControl<VRLever>(_vrLevers.ToList().Find(x => x.name == Control.GLimitInteractable.ToString())));
                        _vrControls.Add(new VRControl<VRLever>(_vrLevers.ToList().Find(x => x.name == Control.RollSASInteractable.ToString())));
                        _vrControls.Add(new VRControl<VRLever>(_vrLevers.ToList().Find(x => x.name == Control.YawSASInteractable.ToString())));
                        _vrControls.Add(new VRControl<VRLever>(_vrLevers.ToList().Find(x => x.name == Control.PitchSASInteractable.ToString())));
                        _vrControls.Add(new VRControl<VRLever>(_vrLevers.ToList().Find(x => x.name == Control.AssistMasterInteractable.ToString())));
                        
                        _vrControls.Add(new VRControl<VRLever>(_vrLevers.ToList().Find(x => x.name == Control.FlareInteractable.ToString())));
                        _vrControls.Add(new VRControl<VRLever>(_vrLevers.ToList().Find(x => x.name == Control.ChaffInteractable.ToString())));

                        _vrControls.Add(new VRControl<VRLever>(_vrLevers.ToList().Find(x => x.name == Control.BrakeLockInteractable.ToString())));
                        _vrControls.Add(new VRControl<VRLever>(_vrLevers.ToList().Find(x => x.name == Control.WingSwitchInteractable.ToString())));

                        _vrControls.Add(new VRControl<VRLever>(_vrLevers.ToList().Find(x => x.name == Control.NavLightInteractable.ToString())));
                        _vrControls.Add(new VRControl<VRLever>(_vrLevers.ToList().Find(x => x.name == Control.StrobLightInteractable.ToString())));
                        _vrControls.Add(new VRControl<VRLever>(_vrLevers.ToList().Find(x => x.name == Control.LandingLightInteractable.ToString())));
                        _vrControls.Add(new VRControl<VRLever>(_vrLevers.ToList().Find(x => x.name == Control.InsturmentLightInteractable.ToString())));

                        _vrControls.Add(new VRControl<VRInteractable>(_vrInteractables.ToList().Find(x => x.name == Control.helmVisorInteractable.ToString())));
                        _vrControls.Add(new VRControl<VRInteractable>(_vrInteractables.ToList().Find(x => x.name == Control.helmNVGInteractable.ToString())));

                        _vrControls.Add(new VRControl<VRInteractable>(_vrInteractables.ToList().Find(x => x.name == Control.altitudeAPButton.ToString())));
                        _vrControls.Add(new VRControl<VRInteractable>(_vrInteractables.ToList().Find(x => x.name == Control.headingAPButton.ToString())));
                        _vrControls.Add(new VRControl<VRInteractable>(_vrInteractables.ToList().Find(x => x.name == Control.navAPButton.ToString())));
                        _vrControls.Add(new VRControl<VRInteractable>(_vrInteractables.ToList().Find(x => x.name == Control.speedAPButton.ToString())));
                        _vrControls.Add(new VRControl<VRInteractable>(_vrInteractables.ToList().Find(x => x.name == Control.offAPButton.ToString())));
                        _vrControls.Add(new VRControl<VRInteractable>(_vrInteractables.ToList().Find(x => x.name == Control.ClrWptButton.ToString())));
                        _vrControls.Add(new VRControl<VRInteractable>(_vrInteractables.ToList().Find(x => x.name == Control.MasterCautionInteractable.ToString())));
                        _vrControls.Add(new VRControl<VRInteractable>(_vrInteractables.ToList().Find(x => x.name == Control.AltitudeModeInteractable.ToString())));
                        _vrControls.Add(new VRControl<VRInteractable>(_vrInteractables.ToList().Find(x => x.name == Control.mfdSwapButton.ToString())));
                        break;
                    case VTOLVehicles.F45A:
                    case VTOLVehicles.AV42C:
                    case VTOLVehicles.None:
                    default:
                        //Not implemented yet
                        break;
                }
            }
        }
    }
    public class VRControl<T> : IVRControl where T : UnityEngine.Object
    {
        private T _unityControl;
        public T UnityControl
        {
            get
            {
                return _unityControl;
            }
        }
        public VRControl(T unityControl)
        {
            _unityControl = unityControl;
            if (!Enum.TryParse(unityControl.name, out _control))
            {
                throw new Exception("Error in VRControl build");
            }
        }
        private ControlType _controlType;
        public ControlType ControlType
        {
            get
            {
                _controlType = ControlType.None;
                bool parseResult = Enum.TryParse(typeof(T).Name, out _controlType);
                if (!parseResult)
                {
                    throw new Exception("Control type is not valid");
                }
                else
                {
                    return _controlType;
                }
            }
        }
        private Control _control;
        public Control Control
        {
            get
            {
                return _control;
            }
        }
        public void Invoke()
        {
            VRControl.Mod.Log(string.Format("[{0}] Interacting with {1}", ControlType, UnityControl.name));

            switch (ControlType)
            {
                case ControlType.VRInteractable:
                    InvokeVRInteractable();
                    break;
                case ControlType.VRButton:
                    InvokeVRInteractable(VRControl.GetVRInteractable(Control));
                    break;
                case ControlType.VRSwitchCover:
                    //VRSwitchCover cover = UnityControl;
                    //Mod.Log(string.Format("[VRSwitchCover] {0} covered : {1}", UnityControl.name, cover.covered));
                    //cover.OnSetState(0);
                    //cover.coveredSwitch.OnInteract.Invoke();
                    break;
                case ControlType.VRLever:
                    VRLever lever = UnityControl as VRLever;
                    int newState = 0;
                    if (lever.currentState == newState) { newState = 1; }
                    lever.SetState(newState);
                    lever.ReturnSpring();
                    break;
                case ControlType.VRTwistKnob:
                    //VRTwistKnob knob = _vrKnobs.ToList().Find(x => x.name == vrControl.Key);
                    //VRTwistKnobInt knobInt = _vrKnobsInt.ToList().Find(x => x.name == vrControl.Key);
                    //Mod.Log(string.Format("[VRTwistKnob] {0} current value is {1}", UnityControl.name, knob.currentValue));
                    //knob.SetKnobValue(1);
                    //Mod.Log(string.Format("[VRTwistKnob] {0} new value is {1}", UnityControl.name, knob.currentValue));
                    //knob.FireEvent();
                    //knobInt.onPushButton.Invoke();
                    //VRInteractable temp = _vrInteractables.ToList().Find(x => x.name == vrControl.Key);
                    //temp.OnInteract.Invoke();
                    break;
                case ControlType.VRTwistKnobInt:
                    break;
                case ControlType.VRJoystick:
                    break;
                case ControlType.VRThrottle:
                    break;
                case ControlType.None:
                    break;
                default:
                    break;
            }
        }

        private void InvokeVRInteractable()
        {
            VRInteractable vrInteractable = UnityControl as VRInteractable;
            InvokeVRInteractable(vrInteractable);
        }
        private void InvokeVRInteractable(VRInteractable vrInteractable)
        {
            vrInteractable.OnInteract.Invoke();
        }
    }
    //        new KeyValuePair<string, Type>("altitudeAPButton", typeof(VRButton)),
    //        new KeyValuePair<string, Type>("helmVisorInteractable", typeof(VRInteractable)),
    //        new KeyValuePair<string, Type>("MFD2PowerInteractable", typeof(VRInteractable)),
    //        new KeyValuePair<string, Type>("coverSwitchInteractable_masterArm", typeof(VRSwitchCover)),
    //        new KeyValuePair<string, Type>("GearInteractable", typeof(VRLever))
}
