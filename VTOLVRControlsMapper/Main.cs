using System.Collections;
using UnityEngine;

namespace VTOLVRControlsMapper
{
    public class Main : VTOLMOD
    {
        static readonly string _settingsFileFolder = @"VTOLVR_ModLoader\Mods\";
        private static Main _instance;
        public static Main instance { get => _instance; }
        public override void ModLoaded()
        {
            Log("Mod Loaded");
            if (_instance == null)
            {
                _instance = this;
            }
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
                    Log("Scene Loaded");
                    StartCoroutine(StartMod());
                    break;
                case VTOLScenes.LoadingScene:
                    break;
            }
        }
        private void MissionReloaded()
        {
            StartCoroutine(StartMod());
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
                _ = StartCoroutine(StartMod());
            }
        }
        private IEnumerator StartMod()
        {
            ControlsHelper.Reset();

            VTOLVehicles vehicle = VTOLAPI.GetPlayersVehicleEnum();

            Log("Controls loading for " + vehicle);
            while (!ControlsHelper.UnityObjectsLoaded(vehicle))
            {
                //Load unity objects (buttons, levers, covers...) to access later
                Log("Waiting for controls loading...");
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
            ControlsHelper.LoadDevices();
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

            Log("Start game controls update routines");
            //Start game controls update routines
            ControlsHelper.StartGameControlsRoutines();
            Log("Game controls update routines started");
            yield return null;
        }
    }
}