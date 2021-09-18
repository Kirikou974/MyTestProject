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
                    if (Input.GetKeyDown(KeyCode.Backspace))
                    {
                        VRControlHelper.LoadControls_FA26B(this);
                    }
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
            if (Input.GetKeyDown("w"))
            {
                VRControlHelper.GetVRControl<VRControlInteractable>(VRControlNames.Button_AP_Altitude).Invoke();
            }
            if (Input.GetKeyDown("x"))
            {
                VRControlHelper.GetVRControl<VRControlInteractable>(VRControlNames.Button_AP_Heading).Invoke();
            }
            if (Input.GetKeyDown("c"))
            {
                VRControlHelper.GetVRControl<IVRControlToggle>(VRControlNames.Lever_LandingGear).Toggle();
            }
            if (Input.GetKeyDown("a")) { VRControlHelper.GetVRControl<VRControlInteractable>(VRControlNames.Button_MFD2_L1).Invoke(); }
            if (Input.GetKeyDown("z")) { VRControlHelper.GetVRControl<VRControlInteractable>(VRControlNames.Button_MFD2_L2).Invoke(); }
            if (Input.GetKeyDown("e")) { VRControlHelper.GetVRControl<VRControlInteractable>(VRControlNames.Button_MFD2_L3).Invoke(); }
            if (Input.GetKeyDown("r")) { VRControlHelper.GetVRControl<VRControlInteractable>(VRControlNames.Button_MFD2_L4).Invoke(); }
            if (Input.GetKeyDown("t")) { VRControlHelper.GetVRControl<VRControlInteractable>(VRControlNames.Button_MFD2_L5).Invoke(); }

            if (Input.GetKeyDown("y")) { VRControlHelper.GetVRControl<VRControlInteractable>(VRControlNames.Button_MFD2_R1).Invoke(); }
            if (Input.GetKeyDown("u")) { VRControlHelper.GetVRControl<VRControlInteractable>(VRControlNames.Button_MFD2_R2).Invoke(); }
            if (Input.GetKeyDown("i")) { VRControlHelper.GetVRControl<VRControlInteractable>(VRControlNames.Button_MFD2_R3).Invoke(); }
            if (Input.GetKeyDown("o")) { VRControlHelper.GetVRControl<VRControlInteractable>(VRControlNames.Button_MFD2_R4).Invoke(); }
            if (Input.GetKeyDown("p")) { VRControlHelper.GetVRControl<VRControlInteractable>(VRControlNames.Button_MFD2_R5).Invoke(); }

            if (Input.GetKeyDown("q")) { VRControlHelper.GetVRControl<VRControlInteractable>(VRControlNames.Button_MFD2_T1).Invoke(); }
            if (Input.GetKeyDown("s")) { VRControlHelper.GetVRControl<VRControlInteractable>(VRControlNames.Button_MFD2_T2).Invoke(); }
            if (Input.GetKeyDown("d")) { VRControlHelper.GetVRControl<VRControlInteractable>(VRControlNames.Button_MFD2_T3).Invoke(); }
            if (Input.GetKeyDown("f")) { VRControlHelper.GetVRControl<VRControlInteractable>(VRControlNames.Button_MFD2_T4).Invoke(); }
            if (Input.GetKeyDown("g")) { VRControlHelper.GetVRControl<VRControlInteractable>(VRControlNames.Button_MFD2_T5).Invoke(); }

            if (Input.GetKeyDown("h")) { VRControlHelper.GetVRControl<VRControlInteractable>(VRControlNames.Button_MFD2_B1).Invoke(); }
            if (Input.GetKeyDown("j")) { VRControlHelper.GetVRControl<VRControlInteractable>(VRControlNames.Button_MFD2_B2).Invoke(); }
            if (Input.GetKeyDown("k")) { VRControlHelper.GetVRControl<VRControlInteractable>(VRControlNames.Button_MFD2_B3).Invoke(); }
            if (Input.GetKeyDown("l")) { VRControlHelper.GetVRControl<VRControlInteractable>(VRControlNames.Button_MFD2_B4).Invoke(); }
            if (Input.GetKeyDown("m")) { VRControlHelper.GetVRControl<VRControlInteractable>(VRControlNames.Button_MFD2_B5).Invoke(); }
        }

        private IEnumerator LoadControls()
        {
            VTOLVehicles currentVehicle = VTOLAPI.GetPlayersVehicleEnum();
            Log("Loading controls for " + currentVehicle);

            switch (currentVehicle)
            {
                case VTOLVehicles.FA26B:
                    while (!VRControlHelper.ControlsLoaded_FA26B)
                    {
                        VRControlHelper.LoadControls_FA26B(this);
                        yield return new WaitForSeconds(2);
                    }
                    break;
                case VTOLVehicles.None:
                case VTOLVehicles.AV42C:
                case VTOLVehicles.F45A:
                default:
                    break;
            }
        }

        //This method is like update but it's framerate independent. This means it gets called at a set time interval instead of every frame. This is useful for physics calculations
        void FixedUpdate()
        {
        }
    }
}