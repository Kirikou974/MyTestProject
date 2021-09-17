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
        VTOLVehicles currentVehicle = VTOLVehicles.None;
        public override void ModLoaded()
        {
            Log("Mod Loaded");
            if (VTOLAPI.SceneLoaded == null)
            {
                VTOLAPI.SceneLoaded += Sceneloaded;
            }
            if (VTOLAPI.MissionReloaded == null)
            {
                VTOLAPI.MissionReloaded += MissionReloaded;
            }
            base.ModLoaded();
        }

        private void Sceneloaded(VTOLScenes scene)
        {
            switch (scene)
            {
                case VTOLScenes.ReadyRoom:
                    break;
                case VTOLScenes.Akutan:
                case VTOLScenes.CustomMapBase:
                    currentVehicle = VTOLAPI.GetPlayersVehicleEnum();
                    StartCoroutine(LoadControls());
                    break;
                case VTOLScenes.LoadingScene:
                    break;
            }
        }
        private void MissionReloaded()
        {
            StartCoroutine(LoadControls());
        }
        /// <summary>
        /// Called by Unity each frame
        /// </summary>
        public void Update()
        {
            switch (currentVehicle)
            {
                case VTOLVehicles.FA26B:
                    if (VRControlHelper.ControlsLoaded_FA26B)
                    {
                        FA26B_ControlMapping();
                    }
                    break;
                case VTOLVehicles.None:
                case VTOLVehicles.AV42C:
                case VTOLVehicles.F45A:
                default:
                    break;
            }
        }

        public void FA26B_ControlMapping()
        {
            if (Input.GetKeyDown("a"))
            {
                VRControlHelper.GetVRControl<VRControlInteractable>(VRControlNames.Button_AP_Altitude).Invoke();
            }
            if (Input.GetKeyDown("q"))
            {
                VRControlHelper.GetVRControl<VRControlInteractable>(VRControlNames.Button_AP_Heading).Invoke();
            }
            if (Input.GetKeyDown("w"))
            {
                VRControlHelper.GetVRControl<IVRControlToggle>(VRControlNames.Lever_LandingGear).Toggle();
            }
            if (Input.GetKeyDown("z"))
            {
                VRControlHelper.GetVRControl<VRControlInteractable>(VRControlNames.Button_Seat_Higher).StartInteract();
            }
            if (Input.GetKeyUp("z"))
            {
                VRControlHelper.GetVRControl<VRControlInteractable>(VRControlNames.Button_Seat_Higher).StopInteract();
            }
            if (Input.GetKeyDown("s"))
            {
                VRControlHelper.GetVRControl<VRControlInteractable>(VRControlNames.Button_Seat_Lower).StartInteract();
            }
            if (Input.GetKeyUp("s"))
            {
                VRControlHelper.GetVRControl<VRControlInteractable>(VRControlNames.Button_Seat_Lower).StopInteract();
            }
            if (Input.GetKeyDown("e"))
            {
                VRControlHelper.GetVRControl<VRControlInteractable>(VRControlNames.Lever_Eject_Right).Invoke();
            }
            if (Input.GetKeyDown("d"))
            {
                VRControlHelper.GetVRControl<VRControlInteractable>(VRControlNames.Lever_Eject_Left).Invoke();
            }
            if (Input.GetKeyDown("v"))
            {
                VRControlHelper.GetVRControl<VRControlInteractable>(VRControlNames.Rear_View_Mirror).Invoke();
            }
        }

        private IEnumerator LoadControls()
        {
            VTOLVehicles currentVehicle = VTOLAPI.GetPlayersVehicleEnum();
            switch (currentVehicle)
            {
                case VTOLVehicles.FA26B:
                    Log("Loading controls for " + currentVehicle);
                    VRControlHelper.LoadFA26BControls(this);
                    break;
                case VTOLVehicles.None:
                case VTOLVehicles.AV42C:
                case VTOLVehicles.F45A:
                default:
                    break;
            }
            yield return new WaitForSeconds(2);
        }

        //This method is like update but it's framerate independent. This means it gets called at a set time interval instead of every frame. This is useful for physics calculations
        void FixedUpdate()
        {
        }
    }
}