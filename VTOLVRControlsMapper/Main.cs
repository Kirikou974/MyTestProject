using SharpDX.DirectInput;
using System;
using System.Collections;
using UnityEngine;

namespace VTOLVRControlsMapper
{
    public class Main : VTOLMOD
    {
        static readonly string _settingsFileFolder = @"VTOLVR_ModLoader\Mods\";
        static bool _updateControllers = true;
        private static VTOLMOD _instance;
        public static VTOLMOD instance { get => _instance; }
        public override void ModLoaded()
        {
            Log("Mod Loaded");
            if(_instance == null)
            {
                _instance = this;
            }
            StartCoroutine(LoadDeviceInstances());
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
        private IEnumerator LoadDeviceInstances()
        {
            ControlsHelper.LoadDeviceInstances();
            yield return null;
        }
        private void Sceneloaded(VTOLScenes scene)
        {
            switch (scene)
            {
                case VTOLScenes.ReadyRoom:
                    break;
                case VTOLScenes.Akutan:
                case VTOLScenes.CustomMapBase:
                    Log("Scene Loaded");
                    StartCoroutine(LoadModObjects());
                    break;
                case VTOLScenes.LoadingScene:
                    break;
            }
        }
        private void MissionReloaded()
        {
            StartCoroutine(LoadModObjects());
        }
        public void OnApplicationFocus(bool hasFocus)
        {
            //Log("Game focus is : " + hasFocus);
            //_updateControllers = hasFocus;
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
                string filePath = ControlsHelper.GetMappingFilePath(_settingsFileFolder, name, currentVehicle.ToString());
                ControlsHelper.CreateMappingFile(filePath);
            }
            if (Input.GetKeyDown(KeyCode.F5))
            {
                Log("Reloading mappings");
                StartCoroutine(LoadModObjects());
            }
            if (ControlsHelper.MappingsLoaded && _updateControllers)
            {
                StartCoroutine(ControlsHelper.UpdateGameControls());
            }
        }
        private IEnumerator LoadModObjects()
        {
            VTOLVehicles vehicle = VTOLAPI.GetPlayersVehicleEnum();
            Log("Controls loading for " + vehicle);
            while (!ControlsHelper.UnityObjectsLoaded(vehicle))
            {
                //Load unity objects (buttons, levers, covers...) to access later
                ControlsHelper.LoadUnityObjects();
                yield return new WaitForSeconds(2);
            }
            Log("Controls loaded for " + vehicle);

            //Load mappings from json file
            Log("Mapping loading for " + vehicle);
            ControlsHelper.LoadMappings(_settingsFileFolder, name, vehicle.ToString());
            Log("Mapping loaded for " + vehicle);

            //Load controllers
            Log("Loading controllers");
            ControlsHelper.LoadControllers();
            Log("Controllers loaded");

            //Load VRHands to have interactions
            while (VRHandController.controllers.Count != 2)
            {
                Log("Waiting for hands...");
                yield return new WaitForSeconds(2);
            }

            //Load custom control instances
            Log("Loading custom control instances");
            ControlsHelper.LoadMappingInstances();
            Log("Custom control instances loaded");
            yield return null;
        }
    }
}