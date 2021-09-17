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
                    VRControlHelper.InvokeControl(Control.altitudeAPButton);
                }
                if (Input.GetKeyDown("h"))
                {
                    VRControlHelper.InvokeControl(Control.headingAPButton);
                }
                if (Input.GetKeyDown("q"))
                {
                    VRControlHelper.InvokeControl(Control.HookInteractable);
                }
                if (Input.GetKeyDown("z"))
                {
                    VRControlHelper.InvokeControl(Control.CatHookInteractable);
                }
                if (Input.GetKeyDown("e"))
                {
                    VRControlHelper.InvokeControl(Control.coverSwitchInteractable_rightEngine);
                }
                if (Input.GetKeyDown("r"))
                {
                    VRControlHelper.InvokeControl(Control.rightEngineSwitchInteractable);
                }
                if (Input.GetKeyDown("t"))
                {
                    VRControlHelper.InvokeControl(Control.coverSwitchInteractable_leftEngine);
                }
                if (Input.GetKeyDown("y"))
                {
                    VRControlHelper.InvokeControl(Control.leftEngineSwitchInteractable);
                }
                if (Input.GetKeyDown("u"))
                {
                    VRControlHelper.InvokeControl(Control.mainBattSwitchInteractable);
                }
                if (Input.GetKeyDown("i"))
                {
                    VRControlHelper.InvokeControl(Control.CanopyInteractable);
                }
                if (Input.GetKeyDown("o"))
                {
                    VRControlHelper.InvokeControl(Control.hmcsPowerInteractable);
                }
                if (Input.GetKeyDown("p"))
                {
                    VRControlHelper.InvokeControl(Control.hudPowerInteractable);
                }
                if (Input.GetKeyDown("s"))
                {
                    VRControlHelper.InvokeControl(Control.coverSwitchInteractable_masterArm);
                }
                if (Input.GetKeyDown("d"))
                {
                    VRControlHelper.InvokeControl(Control.masterArmSwitchInteractable);
                }
                if (Input.GetKeyDown("f"))
                {
                    VRControlHelper.InvokeControl(Control.apuSwitchInteractable);
                }
                if (Input.GetKeyDown("g"))
                {
                    VRControlHelper.InvokeControl(Control.coverSwitchInteractable_fuelDump);
                }
                if (Input.GetKeyDown("j"))
                {
                    VRControlHelper.InvokeControl(Control.fuelDumpSwitchInteractable);
                }
                if (Input.GetKeyDown("k"))
                {
                    VRControlHelper.InvokeControl(Control.ClearJettisonInteractable);
                }
                if (Input.GetKeyDown("l"))
                {
                    VRControlHelper.InvokeControl(Control.JettisonEmptyInteractable);
                }
                if (Input.GetKeyDown("m"))
                {
                    VRControlHelper.InvokeControl(Control.JettisonAllInteractable);
                }
                if (Input.GetKeyDown("w"))
                {
                    VRControlHelper.InvokeControl(Control.MasterJettisonButtonInteractable);
                }
                if (Input.GetKeyDown("x"))
                {
                    VRControlHelper.InvokeControl(Control.coverSwitchInteractable_jettisonButton);
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