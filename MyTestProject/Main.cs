using Harmony;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VTOLVRControlsMapper
{
    public class Main : VTOLMOD
    {
        public override void ModLoaded()
        {
            Log("Mod Loaded");
            if(VTOLAPI.SceneLoaded == null)
            {
                VTOLAPI.SceneLoaded += Sceneloaded;
            }
            if(VTOLAPI.MissionReloaded == null)
            {
                VTOLAPI.MissionReloaded += MissionReloaded;
            }
            base.ModLoaded();
        }

        private void Sceneloaded(VTOLScenes scene)
        {
            Log("Scene Loaded");
            switch (scene)
            {
                case VTOLScenes.ReadyRoom:
                    break;
                case VTOLScenes.Akutan:
                case VTOLScenes.CustomMapBase:
                    Log("Map Loaded");
                    StartCoroutine(LoadControls());
                    break;
                case VTOLScenes.LoadingScene:
                    break;
            }
        }
        private void MissionReloaded()
        {
            Log("MissionReloaded");
            StartCoroutine(LoadControls());
        }
        /// <summary>
        /// Called by Unity each frame
        /// </summary>
        public void Update()
        {
            if(VRControlHelper.ControlsLoaded)
            {
                //TODO : read mapping from file
                if (Input.GetKeyDown("a"))
                {
                    VRControlHelper.GetVRControl<VRControlButton>(Control.altitudeAPButton).Click();
                }
                if (Input.GetKeyDown("h"))
                {
                    VRControlHelper.GetVRControl<VRControlButton>(Control.headingAPButton).Click();
                }
                if (Input.GetKeyDown("q"))
                {
                    VRControlHelper.GetVRControl<IVRControlToggle>(Control.HookInteractable).Toggle();
                }
                if (Input.GetKeyDown("z"))
                {
                    VRControlHelper.GetVRControl<IVRControlToggle>(Control.CatHookInteractable).Toggle();
                }
                if (Input.GetKeyDown("e"))
                {
                    VRControlHelper.GetVRControl<IVRControlToggle>(Control.coverSwitchInteractable_rightEngine).Toggle();
                }
                if (Input.GetKeyDown("r"))
                {
                    VRControlHelper.GetVRControl<IVRControlToggle>(Control.rightEngineSwitchInteractable).Toggle();
                }
                if (Input.GetKeyDown("t"))
                {
                    VRControlHelper.GetVRControl<IVRControlToggle>(Control.coverSwitchInteractable_leftEngine).Toggle();
                }
                if (Input.GetKeyDown("y"))
                {
                    VRControlHelper.GetVRControl<IVRControlToggle>(Control.leftEngineSwitchInteractable).Toggle();
                }
                if (Input.GetKeyDown("u"))
                {
                    VRControlHelper.GetVRControl<IVRControlToggle>(Control.mainBattSwitchInteractable).Toggle();
                }
                if (Input.GetKeyDown("i"))
                {
                    VRControlHelper.GetVRControl<IVRControlToggle>(Control.CanopyInteractable).Toggle();
                }
                if (Input.GetKeyDown("o"))
                {
                    VRControlHelper.GetVRControl<VRControlInteractable>(Control.hmcsPowerInteractable).Invoke();
                }
                if (Input.GetKeyDown("p"))
                {
                    VRControlHelper.GetVRControl<VRControlInteractable>(Control.hudPowerInteractable).Invoke();
                }
                if (Input.GetKeyDown("s"))
                {
                    VRControlHelper.GetVRControl<IVRControlToggle>(Control.coverSwitchInteractable_masterArm).Toggle();
                }
                if (Input.GetKeyDown("d"))
                {
                    VRControlHelper.GetVRControl<IVRControlToggle>(Control.masterArmSwitchInteractable).Toggle();
                }
                if (Input.GetKeyDown("f"))
                {
                    VRControlHelper.GetVRControl<IVRControlToggle>(Control.apuSwitchInteractable).Toggle();
                }
                if (Input.GetKeyDown("g"))
                {
                    VRControlHelper.GetVRControl<IVRControlToggle>(Control.coverSwitchInteractable_fuelDump).Toggle();
                }
                if (Input.GetKeyDown("j"))
                {
                    VRControlHelper.GetVRControl<IVRControlToggle>(Control.fuelDumpSwitchInteractable).Toggle();
                }
                if (Input.GetKeyDown("k"))
                {
                    VRControlHelper.GetVRControl<VRControlButton>(Control.ClearJettisonInteractable).Click();
                }
                if (Input.GetKeyDown("l"))
                {
                    VRControlHelper.GetVRControl<VRControlButton>(Control.JettisonEmptyInteractable).Click();
                }
                if (Input.GetKeyDown("m"))
                {
                    VRControlHelper.GetVRControl<VRControlButton>(Control.JettisonAllInteractable).Click();
                }
                if (Input.GetKeyDown("w"))
                {
                    VRControlHelper.GetVRControl<VRControlButton>(Control.MasterJettisonButtonInteractable).Click();
                }
                if (Input.GetKeyDown("x"))
                {
                    VRControlHelper.GetVRControl<IVRControlToggle>(Control.coverSwitchInteractable_jettisonButton).Toggle();
                }
                if (Input.GetKeyDown("c"))
                {
                    VRControlHelper.GetVRControl<VRControlLever>(Control.GearInteractable).Toggle();
                }
            }
        }

        private IEnumerator LoadControls()
        {
            VTOLVehicles currentVehicle = VTOLAPI.GetPlayersVehicleEnum();
            VRControlHelper.LoadControls(this, currentVehicle);
            yield return new WaitForSeconds(2);
        }

        //This method is like update but it's framerate independent. This means it gets called at a set time interval instead of every frame. This is useful for physics calculations
        void FixedUpdate()
        {
        }
    }
}