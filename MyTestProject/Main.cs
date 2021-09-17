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
                    VRControlHelper.GetVRControl<VRControlInteractable>(Control.raiseSeatInter).StartInteract();
                }
                if (Input.GetKeyUp("z"))
                {
                    VRControlHelper.GetVRControl<VRControlInteractable>(Control.raiseSeatInter).StopInteract();
                }
                if (Input.GetKeyDown("s"))
                {
                    VRControlHelper.GetVRControl<VRControlInteractable>(Control.lowerSeatInter).StartInteract();
                }
                if (Input.GetKeyUp("s"))
                {
                    VRControlHelper.GetVRControl<VRControlInteractable>(Control.lowerSeatInter).StopInteract();
                }
                if (Input.GetKeyDown("e"))
                {
                    VRControlHelper.GetVRControl<VRControlInteractable>(Control.rightSideEjectInteractable).Invoke();
                }
                if (Input.GetKeyDown("d"))
                {
                    VRControlHelper.GetVRControl<VRControlInteractable>(Control.sideEjectInteractable).Invoke();
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