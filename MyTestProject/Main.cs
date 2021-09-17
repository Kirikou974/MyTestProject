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
                    VRControlHelper.GetVRControl<VRControlInteractable>(Control.altitudeAPButton).Invoke();
                }
                if (Input.GetKeyDown("q"))
                {
                    VRControlHelper.GetVRControl<VRControlInteractable>(Control.headingAPButton).Invoke();
                }
                if (Input.GetKeyDown("w"))
                {
                    VRControlHelper.GetVRControl<IVRControlToggle>(Control.GearInteractable).Toggle();
                }
                if (Input.GetKeyDown("z"))
                {
                    VRControlHelper.GetVRControl<IVRControlKnob>(Control.MFD1PowerInteractable).Increase();
                }
                if (Input.GetKeyDown("e"))
                {
                    VRControlHelper.GetVRControl<IVRControlKnob>(Control.MFD1PowerInteractable).Decrease();
                }
                if (Input.GetKeyDown("s"))
                {
                    VRControlHelper.GetVRControl<VRControlSwitchKnob>(Control.MFD1PowerInteractable).On();
                }
                if (Input.GetKeyDown("d"))
                {
                    VRControlHelper.GetVRControl<VRControlSwitchKnob>(Control.MFD1PowerInteractable).Off();
                }
                if (Input.GetKeyDown("x"))
                {
                    VRControlHelper.GetVRControl<VRControlSwitchKnob>(Control.MFD1PowerInteractable).Toggle();
                }
                if (Input.GetKeyDown("r"))
                {
                    VRControlHelper.GetVRControl<IVRControlKnob>(Control.MFD2PowerInteractable).Increase();
                }
                if (Input.GetKeyDown("t"))
                {
                    VRControlHelper.GetVRControl<IVRControlKnob>(Control.MFD2PowerInteractable).Decrease();
                }
                if (Input.GetKeyDown("f"))
                {
                    VRControlHelper.GetVRControl<VRControlSwitchKnob>(Control.MFD2PowerInteractable).On();
                }
                if (Input.GetKeyDown("g"))
                {
                    VRControlHelper.GetVRControl<VRControlSwitchKnob>(Control.MFD2PowerInteractable).Off();
                }
                if (Input.GetKeyDown("v"))
                {
                    VRControlHelper.GetVRControl<VRControlSwitchKnob>(Control.MFD2PowerInteractable).Toggle();
                }
                if (Input.GetKeyDown("y"))
                {
                    VRControlHelper.GetVRControl<IVRControlKnob>(Control.RWRModeInteractable).Increase();
                }
                if (Input.GetKeyDown("u"))
                {
                    VRControlHelper.GetVRControl<IVRControlKnob>(Control.RWRModeInteractable).Decrease();
                }
                if (Input.GetKeyDown("h"))
                {
                    VRControlHelper.GetVRControl<VRControlSwitchKnob>(Control.RWRModeInteractable).On();
                }
                if (Input.GetKeyDown("j"))
                {
                    VRControlHelper.GetVRControl<VRControlSwitchKnob>(Control.RWRModeInteractable).Off();
                }
                if (Input.GetKeyDown("n"))
                {
                    VRControlHelper.GetVRControl<VRControlSwitchKnob>(Control.RWRModeInteractable).Toggle();
                }
                if (Input.GetKeyDown("i"))
                {
                    VRControlHelper.GetVRControl<IVRControlKnob>(Control.RadarPowerInteractable).Increase();
                }
                if (Input.GetKeyDown("o"))
                {
                    VRControlHelper.GetVRControl<IVRControlKnob>(Control.RadarPowerInteractable).Decrease();
                }
                if (Input.GetKeyDown("k"))
                {
                    VRControlHelper.GetVRControl<VRControlSwitchKnob>(Control.RadarPowerInteractable).On();
                }
                if (Input.GetKeyDown("l"))
                {
                    VRControlHelper.GetVRControl<VRControlSwitchKnob>(Control.RadarPowerInteractable).Off();
                }
                if (Input.GetKeyDown("m"))
                {
                    VRControlHelper.GetVRControl<VRControlSwitchKnob>(Control.RadarPowerInteractable).Toggle();
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