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
        private static List<IVRControl> _vrControls;
        public static VTOLMOD Mod { get { return _mod; } }

        public static IVRControl GetControl(Control control)
        {
            if (!ControlsLoaded)
            {
                throw new Exception("Controls not loaded");
            }
            return _vrControls.Find(x => x.Control == control);
        }
        public static void InvokeControl(Control control)
        {
            GetControl(control).Toggle();
        }

        public static VRLever GetVRLever(Control control)
        {
            if (!ControlsLoaded)
            {
                throw new Exception("Controls not loaded");
            }
            return _vrLevers.Find(x => x.name == control.ToString());
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
                _vrControls = new List<IVRControl>();
                //TODO : link action with hands movement ? 
                switch (vehicle)
                {
                    case VTOLVehicles.FA26B:
                        _vrControls.Add(new VRControlLever(_vrLevers.Find(x => x.name == Control.GearInteractable.ToString())));
                        VRControlCover masterArmCover = new VRControlCover(
                            _vrSwitchCovers.Find(x => x.name == Control.coverSwitchInteractable_masterArm.ToString()),
                            new VRControlLever(_vrLevers.Find(x => x.name == Control.coverSwitchInteractable_masterArm.ToString())));
                        _vrControls.Add(masterArmCover);
                        _vrControls.Add(new VRControlLeverWithCover(_vrLevers.Find(x => x.name == Control.masterArmSwitchInteractable.ToString()), masterArmCover));
                        _vrControls.Add(new VRControlLever(_vrLevers.Find(x => x.name == Control.FuelPort.ToString())));
                        _vrControls.Add(new VRControlLever(_vrLevers.Find(x => x.name == Control.HookInteractable.ToString())));
                        _vrControls.Add(new VRControlLever(_vrLevers.Find(x => x.name == Control.CatHookInteractable.ToString())));

                        VRControlCover rightEngineCover = new VRControlCover(
                            _vrSwitchCovers.Find(x => x.name == Control.coverSwitchInteractable_rightEngine.ToString()),
                            new VRControlLever(_vrLevers.Find(x => x.name == Control.coverSwitchInteractable_rightEngine.ToString())));
                        _vrControls.Add(rightEngineCover);
                        _vrControls.Add(new VRControlLeverWithCover(_vrLevers.Find(x => x.name == Control.rightEngineSwitchInteractable.ToString()), rightEngineCover));

                        VRControlCover leftEngineCover = new VRControlCover(
                            _vrSwitchCovers.Find(x => x.name == Control.coverSwitchInteractable_leftEngine.ToString()),
                            new VRControlLever(_vrLevers.Find(x => x.name == Control.coverSwitchInteractable_leftEngine.ToString())));
                        _vrControls.Add(leftEngineCover);
                        _vrControls.Add(new VRControlLeverWithCover(_vrLevers.Find(x => x.name == Control.leftEngineSwitchInteractable.ToString()), leftEngineCover));

                        VRControlCover fuelDumpCover = new VRControlCover(
                            _vrSwitchCovers.Find(x => x.name == Control.coverSwitchInteractable_fuelDump.ToString()),
                            new VRControlLever(_vrLevers.Find(x => x.name == Control.coverSwitchInteractable_fuelDump.ToString())));
                        _vrControls.Add(fuelDumpCover);
                        _vrControls.Add(new VRControlLeverWithCover(_vrLevers.Find(x => x.name == Control.fuelDumpSwitchInteractable.ToString()), fuelDumpCover));

                        VRControlCover jettisonCover = new VRControlCover(
                            _vrSwitchCovers.Find(x => x.name == Control.coverSwitchInteractable_jettisonButton.ToString()),
                            new VRControlLever(_vrLevers.Find(x => x.name == Control.coverSwitchInteractable_jettisonButton.ToString())));
                        _vrControls.Add(jettisonCover);
                        _vrControls.Add(new VRControlInteractable(_vrInteractables.Find(x => x.name == Control.ClearJettisonInteractable.ToString())));
                        _vrControls.Add(new VRControlInteractable(_vrInteractables.Find(x => x.name == Control.JettisonEmptyInteractable.ToString())));
                        _vrControls.Add(new VRControlInteractable(_vrInteractables.Find(x => x.name == Control.JettisonAllInteractable.ToString())));
                        _vrControls.Add(new VRControlInteractable(_vrInteractables.Find(x => x.name == Control.MasterJettisonButtonInteractable.ToString())));

                        _vrControls.Add(new VRControlLever(_vrLevers.Find(x => x.name == Control.mainBattSwitchInteractable.ToString())));
                        _vrControls.Add(new VRControlLever(_vrLevers.Find(x => x.name == Control.apuSwitchInteractable.ToString())));
                        _vrControls.Add(new VRControlLever(_vrLevers.Find(x => x.name == Control.CanopyInteractable.ToString())));
                        _vrControls.Add(new VRControlLever(_vrLevers.Find(x => x.name == Control.CATOTrimInteractable.ToString())));
                        _vrControls.Add(new VRControlLever(_vrLevers.Find(x => x.name == Control.GLimitInteractable.ToString())));
                        _vrControls.Add(new VRControlLever(_vrLevers.Find(x => x.name == Control.RollSASInteractable.ToString())));
                        _vrControls.Add(new VRControlLever(_vrLevers.Find(x => x.name == Control.YawSASInteractable.ToString())));
                        _vrControls.Add(new VRControlLever(_vrLevers.Find(x => x.name == Control.PitchSASInteractable.ToString())));
                        _vrControls.Add(new VRControlLever(_vrLevers.Find(x => x.name == Control.AssistMasterInteractable.ToString())));
                        _vrControls.Add(new VRControlLever(_vrLevers.Find(x => x.name == Control.FlareInteractable.ToString())));
                        _vrControls.Add(new VRControlLever(_vrLevers.Find(x => x.name == Control.ChaffInteractable.ToString())));
                        _vrControls.Add(new VRControlLever(_vrLevers.Find(x => x.name == Control.BrakeLockInteractable.ToString())));
                        _vrControls.Add(new VRControlLever(_vrLevers.Find(x => x.name == Control.WingSwitchInteractable.ToString())));
                        _vrControls.Add(new VRControlLever(_vrLevers.Find(x => x.name == Control.NavLightInteractable.ToString())));
                        _vrControls.Add(new VRControlLever(_vrLevers.Find(x => x.name == Control.StrobLightInteractable.ToString())));
                        _vrControls.Add(new VRControlLever(_vrLevers.Find(x => x.name == Control.LandingLightInteractable.ToString())));
                        _vrControls.Add(new VRControlLever(_vrLevers.Find(x => x.name == Control.InsturmentLightInteractable.ToString())));
                        _vrControls.Add(new VRControlLever(_vrLevers.Find(x => x.name == Control.hudPowerInteractable.ToString())));
                        _vrControls.Add(new VRControlLever(_vrLevers.Find(x => x.name == Control.hmcsPowerInteractable.ToString())));

                        _vrControls.Add(new VRControlInteractable(_vrInteractables.Find(x => x.name == Control.helmVisorInteractable.ToString())));
                        _vrControls.Add(new VRControlInteractable(_vrInteractables.Find(x => x.name == Control.helmNVGInteractable.ToString())));
                        _vrControls.Add(new VRControlInteractable(_vrInteractables.Find(x => x.name == Control.altitudeAPButton.ToString())));
                        _vrControls.Add(new VRControlInteractable(_vrInteractables.Find(x => x.name == Control.headingAPButton.ToString())));
                        _vrControls.Add(new VRControlInteractable(_vrInteractables.Find(x => x.name == Control.navAPButton.ToString())));
                        _vrControls.Add(new VRControlInteractable(_vrInteractables.Find(x => x.name == Control.speedAPButton.ToString())));
                        _vrControls.Add(new VRControlInteractable(_vrInteractables.Find(x => x.name == Control.offAPButton.ToString())));
                        _vrControls.Add(new VRControlInteractable(_vrInteractables.Find(x => x.name == Control.ClrWptButton.ToString())));
                        _vrControls.Add(new VRControlInteractable(_vrInteractables.Find(x => x.name == Control.MasterCautionInteractable.ToString())));
                        _vrControls.Add(new VRControlInteractable(_vrInteractables.Find(x => x.name == Control.AltitudeModeInteractable.ToString())));
                        _vrControls.Add(new VRControlInteractable(_vrInteractables.Find(x => x.name == Control.mfdSwapButton.ToString())));

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
