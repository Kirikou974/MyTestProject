using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTOLVRControlsMapper
{
    public static class VRControlHelper
    {
        private static List<VRInteractable> _vrInteractables;
        private static List<VRButton> _vrButtons;
        private static List<VRTwistKnob> _vrTwistKnobs;
        private static List<VRTwistKnobInt> _vrTwistKnobsInt;
        private static List<VRSwitchCover> _vrSwitchCovers;
        private static List<VRLever> _vrLevers;
        private static VTOLMOD _mod;
        private static List<IVRControl> _vrControlCache;
        public static VTOLMOD Mod
        {
            get { 
                if(_mod == null)
                {
                    throw new NullReferenceException("_mod variable not loaded");
                }
                return _mod; 
            }
        }
        public static void ToggleControl(Control control)
        {
            IVRControlToggle controlToggle = GetVRControl<IVRControlToggle>(control);
            if(controlToggle == null)
            {
                throw new NullReferenceException("controlToggle is null");
            }
            else
            {
                controlToggle.Toggle();
            }
        }
        public static T GetVRControl<T>(Control control)
            where T : class
        {
            if (!ControlsLoaded)
            {
                throw new Exception("Controls not loaded");
            }
            T returnValue = null;
            switch (typeof(T).Name)
            {
                case "VRLever":
                    returnValue = _vrLevers.Find(x => x.name == control.ToString()) as T;
                    break;
                case "VRInteractable":
                    returnValue = _vrInteractables.Find(x => x.name == control.ToString()) as T;
                    break;
                case "VRButton":
                    returnValue = _vrButtons.Find(x => x.name == control.ToString()) as T;
                    break;
                case "VRSwitchCover":
                    returnValue = _vrSwitchCovers.Find(x => x.name == control.ToString()) as T;
                    break;
                case "VRTwistKnob":
                    returnValue = _vrTwistKnobs.Find(x => x.name == control.ToString()) as T;
                    break;
                case "VRTwistKnobInt":
                    returnValue = _vrTwistKnobsInt.Find(x => x.name == control.ToString()) as T;
                    break;
                default:
                    returnValue = _vrControlCache.Find(x => x.Control == control && x is T) as T;
                    break;
            }
            return returnValue;
        }

        public static bool ControlsLoaded
        {
            get
            {
                return
                    _vrInteractables != null && _vrInteractables.Count() > 0 &&
                    _vrButtons != null && _vrButtons.Count() > 0 &&
                    _vrTwistKnobs != null && _vrTwistKnobs.Count() > 0 &&
                    _vrTwistKnobsInt != null && _vrTwistKnobsInt.Count() > 0 &&
                    _vrSwitchCovers != null && _vrSwitchCovers.Count() > 0 &&
                    _vrLevers != null && _vrLevers.Count() > 0;
            }
        }
        public static void LoadControls(VTOLMOD mod, VTOLVehicles vehicle)
        {
            _mod = mod;
            Mod.Log("Start LoadControls for " + vehicle);

            _vrInteractables = UnityEngine.Object.FindObjectsOfType<VRInteractable>().ToList();
            _vrButtons = UnityEngine.Object.FindObjectsOfType<VRButton>().ToList();
            _vrTwistKnobs = UnityEngine.Object.FindObjectsOfType<VRTwistKnob>().ToList();
            _vrTwistKnobsInt = UnityEngine.Object.FindObjectsOfType<VRTwistKnobInt>().ToList();
            _vrSwitchCovers = UnityEngine.Object.FindObjectsOfType<VRSwitchCover>().ToList();
            _vrLevers = UnityEngine.Object.FindObjectsOfType<VRLever>().ToList();

            if (ControlsLoaded)
            {
                Mod.Log(" - _vrInteractables count : " + _vrInteractables.Count());
                Mod.Log(" - _vrButtons count : " + _vrButtons.Count());
                Mod.Log(" - _vrTwistKnobs count : " + _vrTwistKnobs.Count());
                Mod.Log(" - _vrTwistKnobsInt count : " + _vrTwistKnobsInt.Count());
                Mod.Log(" - _vrSwitchCovers count : " + _vrSwitchCovers.Count());
                Mod.Log(" - _vrLevers count : " + _vrLevers.Count());
                _vrControlCache = new List<IVRControl>();
                //TODO : link action with hands movement ? 
                switch (vehicle)
                {
                    case VTOLVehicles.FA26B:
                        _vrControlCache.Add(new VRControlLever(Control.GearInteractable));
                        _vrControlCache.Add(new VRControlCover(Control.coverSwitchInteractable_masterArm));
                        _vrControlCache.Add(new VRControlLeverWithCover(Control.masterArmSwitchInteractable, Control.coverSwitchInteractable_masterArm));
                        _vrControlCache.Add(new VRControlLever(Control.FuelPort));
                        _vrControlCache.Add(new VRControlLever(Control.HookInteractable));
                        _vrControlCache.Add(new VRControlLever(Control.CatHookInteractable));

                        _vrControlCache.Add(new VRControlCover(Control.coverSwitchInteractable_rightEngine));
                        _vrControlCache.Add(new VRControlLeverWithCover(Control.rightEngineSwitchInteractable, Control.coverSwitchInteractable_rightEngine));

                        _vrControlCache.Add(new VRControlCover(Control.coverSwitchInteractable_leftEngine));
                        _vrControlCache.Add(new VRControlLeverWithCover(Control.leftEngineSwitchInteractable, Control.coverSwitchInteractable_leftEngine));

                        _vrControlCache.Add(new VRControlCover(Control.coverSwitchInteractable_fuelDump));
                        _vrControlCache.Add(new VRControlLeverWithCover(Control.fuelDumpSwitchInteractable, Control.coverSwitchInteractable_fuelDump));

                        _vrControlCache.Add(new VRControlCover(Control.coverSwitchInteractable_jettisonButton));
                        _vrControlCache.Add(new VRControlInteractable(Control.ClearJettisonInteractable));
                        _vrControlCache.Add(new VRControlInteractable(Control.JettisonEmptyInteractable));
                        _vrControlCache.Add(new VRControlInteractable(Control.JettisonAllInteractable));
                        _vrControlCache.Add(new VRControlInteractable(Control.MasterJettisonButtonInteractable));

                        _vrControlCache.Add(new VRControlInteractable(Control.helmVisorInteractable));
                        _vrControlCache.Add(new VRControlInteractable(Control.helmNVGInteractable));

                        _vrControlCache.Add(new VRControlInteractable(Control.offAPButton));
                        _vrControlCache.Add(new VRControlInteractable(Control.ClrWptButton));
                        _vrControlCache.Add(new VRControlInteractable(Control.MasterCautionInteractable));
                        _vrControlCache.Add(new VRControlInteractable(Control.mfdSwapButton));
                        _vrControlCache.Add(new VRControlInteractable(Control.AltitudeModeInteractable));
                        _vrControlCache.Add(new VRControlInteractable(Control.altitudeAPButton));
                        _vrControlCache.Add(new VRControlInteractable(Control.headingAPButton));
                        _vrControlCache.Add(new VRControlInteractable(Control.navAPButton));
                        _vrControlCache.Add(new VRControlInteractable(Control.speedAPButton));

                        _vrControlCache.Add(new VRControlLever(Control.mainBattSwitchInteractable));
                        _vrControlCache.Add(new VRControlLever(Control.apuSwitchInteractable));
                        _vrControlCache.Add(new VRControlLever(Control.CanopyInteractable));
                        _vrControlCache.Add(new VRControlLever(Control.CATOTrimInteractable));
                        _vrControlCache.Add(new VRControlLever(Control.GLimitInteractable));
                        _vrControlCache.Add(new VRControlLever(Control.RollSASInteractable));
                        _vrControlCache.Add(new VRControlLever(Control.YawSASInteractable));
                        _vrControlCache.Add(new VRControlLever(Control.PitchSASInteractable));
                        _vrControlCache.Add(new VRControlLever(Control.AssistMasterInteractable));
                        _vrControlCache.Add(new VRControlLever(Control.FlareInteractable));
                        _vrControlCache.Add(new VRControlLever(Control.ChaffInteractable));
                        _vrControlCache.Add(new VRControlLever(Control.BrakeLockInteractable));
                        _vrControlCache.Add(new VRControlLever(Control.WingSwitchInteractable));
                        _vrControlCache.Add(new VRControlLever(Control.NavLightInteractable));
                        _vrControlCache.Add(new VRControlLever(Control.StrobLightInteractable));
                        _vrControlCache.Add(new VRControlLever(Control.LandingLightInteractable));
                        _vrControlCache.Add(new VRControlLever(Control.InsturmentLightInteractable));
                        _vrControlCache.Add(new VRControlLever(Control.hudPowerInteractable));
                        _vrControlCache.Add(new VRControlLever(Control.hmcsPowerInteractable));
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
}
