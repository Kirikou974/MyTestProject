using SharpDX.DirectInput;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace VTOLVRControlsMapper
{
    public class Main : VTOLMOD
    {
        public override void ModLoaded()
        {
            Log("Mod Loaded");
            ControlsHelper.LoadDeviceInstances();
            Log("Devices list loaded");

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
                    StartCoroutine(LoadControlsMapping());
                    break;
                case VTOLScenes.LoadingScene:
                    break;
            }
        }
        private void MissionReloaded()
        {
            StartCoroutine(LoadControlsMapping());
        }
        /// <summary>
        /// Called by Unity each frame
        /// </summary
        public void Update()
        {
            VTOLVehicles currentVehicle = VTOLAPI.GetPlayersVehicleEnum();
            if (Input.GetKeyDown(KeyCode.F8))
            {
                Log("Recreating mappings file");
                string filePath = ControlsHelper.GetMappingFilePath(name, currentVehicle.ToString());
                ControlsHelper.CreateMappingFile(filePath);
            }
            if (Input.GetKeyDown(KeyCode.F5))
            {
                Log("Reloading mappings");
                StartCoroutine(LoadControlsMapping());
            }

            if (ControlsHelper.MappingsLoaded)
                ControlsHelper.Update();
        }

        private IEnumerator LoadControlsMapping()
        {
            VTOLVehicles vehicle = VTOLAPI.GetPlayersVehicleEnum();
            while (!ControlsHelper.ControlsLoaded(vehicle))
            {
                ControlsHelper.LoadControls();
                yield return new WaitForSeconds(2);
            }
            Log("Controls loaded for " + vehicle);
            ControlsHelper.LoadMappings(name, vehicle.ToString());
            Log("Mapping loaded for " + vehicle);
        }
    }
}