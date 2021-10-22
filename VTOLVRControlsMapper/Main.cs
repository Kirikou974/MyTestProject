using System.Collections;
using UnityEngine;

namespace VTOLVRControlsMapper
{
    public class Main : VTOLMOD
    {
        private const string SETTINGS_FOLDER = @"VTOLVR_ModLoader\Mods\";
        public static Main Instance { get; private set; }
        public override void ModLoaded()
        {
            Log("Mod Loaded");
            if (Instance == null)
            {
                Instance = this;
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
                case VTOLScenes.Akutan:
                case VTOLScenes.CustomMapBase:
                    Log("Scene Loaded");
                    _ = StartCoroutine(StartMod());
                    break;
                case VTOLScenes.ReadyRoom:
                case VTOLScenes.LoadingScene:
                case VTOLScenes.SplashScene:
                case VTOLScenes.SamplerScene:
                case VTOLScenes.VehicleConfiguration:
                case VTOLScenes.MeshTerrain:
                case VTOLScenes.OpenWater:
                case VTOLScenes.VTEditMenu:
                case VTOLScenes.VTEditLoadingScene:
                case VTOLScenes.VTMapEditMenu:
                case VTOLScenes.CommRadioTest:
                case VTOLScenes.ShaderVariantsScene:
                case VTOLScenes.CustomMapBase_OverCloud:
                default:
                    break;
            }
        }
        private void MissionReloaded()
        {
            _ = StartCoroutine(StartMod());
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
                string filePath = ControlsHelper.GetMappingFilePath(SETTINGS_FOLDER, name, currentVehicle.ToString());
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
            ControlsHelper.LoadMappings(SETTINGS_FOLDER, name, vehicle.ToString());
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