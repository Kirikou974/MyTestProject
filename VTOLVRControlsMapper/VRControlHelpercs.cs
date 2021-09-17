using System;
using System.Collections.Generic;
using System.Linq;

namespace VTOLVRControlsMapper
{
    public static class VRControlHelper
    {
        //private static List<VRButton> _vrButtons;
        private static List<VRInteractable> _vrInteractables;
        private static List<VRTwistKnob> _vrTwistKnobs;
        private static List<VRTwistKnobInt> _vrTwistKnobsInt;
        private static List<VRSwitchCover> _vrSwitchCovers;
        private static List<VRLever> _vrLevers;
        private static VRJoystick _vrJoystick;
        private static VRThrottle _vrThrottle;

        private static VTOLMOD _mod;
        private static List<IVRControl> _vrControlCache;
        public static VTOLMOD Mod
        {
            get
            {
                if (_mod == null)
                {
                    throw new NullReferenceException("_mod variable not loaded");
                }
                return _mod;
            }
        }
        public static void ToggleControl(string controlName)
        {
            IVRControlToggle controlToggle = GetVRControl<IVRControlToggle>(controlName);
            if (controlToggle == null)
            {
                throw new NullReferenceException("controlToggle is null");
            }
            else
            {
                controlToggle.Toggle();
            }
        }
        public static T GetVRControl<T>(string controlName)
            where T : class
        {
            T returnValue = null;

            switch (typeof(T).Name)
            {
                case "VRLever":
                    returnValue = _vrLevers.Find(x => x.name == controlName) as T;
                    break;
                case "VRInteractable":
                    returnValue = _vrInteractables.Find(x => x.name == controlName) as T;
                    break;
                case "VRSwitchCover":
                    returnValue = _vrSwitchCovers.Find(x => x.name == controlName) as T;
                    break;
                case "VRTwistKnob":
                    returnValue = _vrTwistKnobs.Find(x => x.name == controlName) as T;
                    break;
                case "VRTwistKnobInt":
                    returnValue = _vrTwistKnobsInt.Find(x => x.name == controlName) as T;
                    break;
                default:
                    returnValue = _vrControlCache.Find(x => x.ControlName == controlName && x is T) as T;
                    break;
            }
            if (returnValue is null)
            {
                throw new NullReferenceException("GetVRControl failed to get a returnValue for " + controlName);
            }
            return returnValue;
        }

        public static bool ControlsLoaded_FA26B
        {
            get
            {
                return
                    _vrJoystick != null && _vrThrottle != null &&
                    _vrInteractables != null && _vrInteractables.Count() > 0 &&
                    _vrTwistKnobs != null && _vrTwistKnobs.Count() > 0 &&
                    _vrTwistKnobsInt != null && _vrTwistKnobsInt.Count() > 0 &&
                    _vrSwitchCovers != null && _vrSwitchCovers.Count() > 0 &&
                    _vrLevers != null && _vrLevers.Count() > 0;
            }
        }
        public static void LoadControls_FA26B(VTOLMOD mod)
        {
            _mod = mod;

            _vrJoystick = UnityEngine.Object.FindObjectOfType<VRJoystick>();
            _vrThrottle = UnityEngine.Object.FindObjectOfType<VRThrottle>();
            _vrInteractables = UnityEngine.Object.FindObjectsOfType<VRInteractable>().ToList();
            _vrTwistKnobs = UnityEngine.Object.FindObjectsOfType<VRTwistKnob>().ToList();
            _vrTwistKnobsInt = UnityEngine.Object.FindObjectsOfType<VRTwistKnobInt>().ToList();
            _vrSwitchCovers = UnityEngine.Object.FindObjectsOfType<VRSwitchCover>().ToList();
            _vrLevers = UnityEngine.Object.FindObjectsOfType<VRLever>().ToList();

            if (ControlsLoaded_FA26B)
            {
                Mod.Log(" - _vrInteractables count : " + _vrInteractables.Count());
                Mod.Log(" - _vrTwistKnobs count : " + _vrTwistKnobs.Count());
                Mod.Log(" - _vrTwistKnobsInt count : " + _vrTwistKnobsInt.Count());
                Mod.Log(" - _vrSwitchCovers count : " + _vrSwitchCovers.Count());
                Mod.Log(" - _vrLevers count : " + _vrLevers.Count());

                _vrControlCache = new List<IVRControl>();

                _vrControlCache.Add(new VRControlLever(VRControlNames.Lever_LandingGear));
                _vrControlCache.Add(new VRControlCover(VRControlNames.Cover_MasterArm));
                _vrControlCache.Add(new VRControlLeverWithCover(VRControlNames.Switch_MasterArm, VRControlNames.Cover_MasterArm));
                _vrControlCache.Add(new VRControlLever(VRControlNames.Switch_Fuel_Port));
                _vrControlCache.Add(new VRControlLever(VRControlNames.Switch_Hook));
                _vrControlCache.Add(new VRControlLever(VRControlNames.Switch_Hook_Cat));

                _vrControlCache.Add(new VRControlCover(VRControlNames.Cover_Engine_Right));
                _vrControlCache.Add(new VRControlLeverWithCover(VRControlNames.Switch_Engine_Right, VRControlNames.Cover_Engine_Right));

                _vrControlCache.Add(new VRControlCover(VRControlNames.Cover_Engine_Left));
                _vrControlCache.Add(new VRControlLeverWithCover(VRControlNames.Switch_Engine_Left, VRControlNames.Cover_Engine_Left));

                _vrControlCache.Add(new VRControlCover(VRControlNames.Cover_Fuel_Dump));
                _vrControlCache.Add(new VRControlLeverWithCover(VRControlNames.Switch_Fuel_Dump, VRControlNames.Cover_Fuel_Dump));

                _vrControlCache.Add(new VRControlCover(VRControlNames.Cover_Jettison));
                _vrControlCache.Add(new VRControlInteractable(VRControlNames.Button_Jettison_Clear));
                _vrControlCache.Add(new VRControlInteractable(VRControlNames.Button_Jettison_Empty));
                _vrControlCache.Add(new VRControlInteractable(VRControlNames.Button_Jettison_All));
                _vrControlCache.Add(new VRControlInteractable(VRControlNames.Button_Jettison_Master));

                _vrControlCache.Add(new VRControlInteractable(VRControlNames.Helm_Visor));
                _vrControlCache.Add(new VRControlInteractable(VRControlNames.Helm_NightVision));
                _vrControlCache.Add(new VRControlInteractable(VRControlNames.Helm_Visor2));
                _vrControlCache.Add(new VRControlInteractable(VRControlNames.Button_Visor));
                _vrControlCache.Add(new VRControlInteractable(VRControlNames.Rear_View_Mirror));

                _vrControlCache.Add(new VRControlInteractable(VRControlNames.Button_AP_Off));
                _vrControlCache.Add(new VRControlInteractable(VRControlNames.Button_Clear_Waypoint));
                _vrControlCache.Add(new VRControlInteractable(VRControlNames.Button_Master_Caution));
                _vrControlCache.Add(new VRControlInteractable(VRControlNames.Button_MFD_Swap));
                _vrControlCache.Add(new VRControlInteractable(VRControlNames.Button_Altitude_Mode));
                _vrControlCache.Add(new VRControlInteractable(VRControlNames.Button_AP_Altitude));
                _vrControlCache.Add(new VRControlInteractable(VRControlNames.Button_AP_Heading));
                _vrControlCache.Add(new VRControlInteractable(VRControlNames.Button_AP_Nav));
                _vrControlCache.Add(new VRControlInteractable(VRControlNames.Button_AP_Speed));

                _vrControlCache.Add(new VRControlSwitchKnob(VRControlNames.Knob_Power_MFD1));
                _vrControlCache.Add(new VRControlSwitchKnob(VRControlNames.Knob_Power_MFD2));
                _vrControlCache.Add(new VRControlSwitchKnob(VRControlNames.Knob_Power_Radar));
                _vrControlCache.Add(new VRControlSwitchKnob(VRControlNames.Knob_RWR));

                _vrControlCache.Add(new VRControlInteractable(VRControlNames.Button_MMFD_Right_Power));
                _vrControlCache.Add(new VRControlInteractable(VRControlNames.Button_MMFD_Left_Power));
                _vrControlCache.Add(new VRControlInteractable(VRControlNames.Button_MMFD_Left_RWR));
                _vrControlCache.Add(new VRControlInteractable(VRControlNames.Button_MMFD_Right_Fuel));
                _vrControlCache.Add(new VRControlInteractable(VRControlNames.Button_MMFD_Right_Fuel_Drain));

                _vrControlCache.Add(new VRControlInteractable(VRControlNames.Lever_Eject_Right));
                _vrControlCache.Add(new VRControlInteractable(VRControlNames.Lever_Eject_Left));
                _vrControlCache.Add(new VRControlInteractable(VRControlNames.Button_Seat_Higher));
                _vrControlCache.Add(new VRControlInteractable(VRControlNames.Button_Seat_Lower));

                _vrControlCache.Add(new VRControlLever(VRControlNames.Switch_Battery));
                _vrControlCache.Add(new VRControlLever(VRControlNames.Switch_APU));
                _vrControlCache.Add(new VRControlLever(VRControlNames.Lever_Canopy));
                _vrControlCache.Add(new VRControlLever(VRControlNames.Switch_CATOTrim));
                _vrControlCache.Add(new VRControlLever(VRControlNames.Switch_Glimit));
                _vrControlCache.Add(new VRControlLever(VRControlNames.Switch_SAS_Roll));
                _vrControlCache.Add(new VRControlLever(VRControlNames.Switch_SAS_Yaw));
                _vrControlCache.Add(new VRControlLever(VRControlNames.Switch_SAS_Pitch));
                _vrControlCache.Add(new VRControlLever(VRControlNames.Switch_SAS_Master));
                _vrControlCache.Add(new VRControlLever(VRControlNames.Swtich_Flare));
                _vrControlCache.Add(new VRControlLever(VRControlNames.Swtich_Chaff));
                _vrControlCache.Add(new VRControlLever(VRControlNames.Switch_Brake_Lock));
                _vrControlCache.Add(new VRControlLever(VRControlNames.Switch_Wing));
                _vrControlCache.Add(new VRControlLever(VRControlNames.Ligth_Nav));
                _vrControlCache.Add(new VRControlLever(VRControlNames.Ligth_Strobe));
                _vrControlCache.Add(new VRControlLever(VRControlNames.Ligth_Landing));
                _vrControlCache.Add(new VRControlLever(VRControlNames.Ligth_Instrument));
                _vrControlCache.Add(new VRControlLever(VRControlNames.Switch_Power_HUD));
                _vrControlCache.Add(new VRControlLever(VRControlNames.Switch_Power_HMCS));
            }
        }
    }
}
