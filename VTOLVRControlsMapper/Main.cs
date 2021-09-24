using SharpDX.DirectInput;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VTOLVRControlsMapper
{
    public class Main : VTOLMOD
    {
        public override void ModLoaded()
        {
            Log("Mod Loaded");
            StartCoroutine(ControlsHelper.LoadDeviceInstances());

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
                    _ = StartCoroutine(LoadControlsMapping());
                    break;
                case VTOLScenes.LoadingScene:
                    break;
            }
        }
        private void MissionReloaded()
        {
            _ = StartCoroutine(LoadControlsMapping());
        }
        public void OnApplicationFocus(bool hasFocus)
        {
            Log("Game focus is : " + hasFocus);
            ControlsHelper.OnApplicationFocus(hasFocus);
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
                _ = StartCoroutine(LoadControlsMapping());
            }
            if (ControlsHelper.MappingsLoaded)
            {
                StartCoroutine(ControlsHelper.UpdateControllers());
            }
        }
        private IEnumerator LoadControlsMapping()
        {
            VTOLVehicles vehicle = VTOLAPI.GetPlayersVehicleEnum();
            Log("Loading controls for " + vehicle);
            while (!ControlsHelper.ControlsLoaded(vehicle, this))
            {
                ControlsHelper.UnityObjects = FindObjectsOfType<UnityEngine.Object>().ToList();
                yield return new WaitForSeconds(1);
            }
            Log("Controls loaded for " + vehicle);
            ControlsHelper.LoadMappings(name, vehicle.ToString());
            Log("Mapping loaded for " + vehicle);
            yield return null;
        }
    }
}