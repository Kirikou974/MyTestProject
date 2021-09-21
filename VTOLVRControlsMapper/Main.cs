using SharpDX.DirectInput;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Valve.Newtonsoft.Json;
using VTOLVRControlsMapper.Controls;
using VTOLVRControlsMapper.Core;

namespace VTOLVRControlsMapper
{
    public class Main : VTOLMOD
    {
        //string settingsFileFolder = @"VTOLVR_ModLoader\Mods\";
        string settingsFileFolder = @"VTOLVR_ModLoader\dev\My Mods\";
        public enum ControlType
        {
            VRInteractable,
            VRLever,
            VRSwitchCover,
            VRTwistKnob,
            VRTwistKnobInt,
            VRButton,
            None
        }
        public bool MappingsLoaded
        {
            get
            {
                return Mappings != null && Mappings.Count > 0;
            }
        }
        bool _devicesAcquired;
        DirectInput _directInput;
        List<DeviceInstance> _deviceInstances;
        List<Device> _devices;
        KeyboardUpdate[] _keyboardUpdates;
        KeyboardState _keyboardState;
        JoystickUpdate[] _joystickUpdates;
        JoystickState _joystickState;
        List<ControlMapping> Mappings;
        public static VRInteractable[] _vrInteractables;
        public static VRTwistKnob[] _vrTwistKnobs;
        public static VRTwistKnobInt[] _vrTwistKnobsInt;
        public static VRButton[] _vrButtons;
        public static VRSwitchCover[] _vrSwitchCovers;
        public static VRLever[] _vrLevers;
        public static Dictionary<string, string> _vrLeversWithCover;
        public static VRJoystick _vrJoystick;
        public static VRThrottle _vrThrottle;
        public override void ModLoaded()
        {
            Log("Mod Loaded");
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
            StartCoroutine(LoadControlsMapping());
        }
        public void OnApplicationFocus(bool hasFocus)
        {
            Log("Game focus is : " + hasFocus);
            foreach (Device device in _devices)
            {
                //Unacquire keyboard when not focusing game
                if (device is Keyboard)
                {
                    if (hasFocus)
                    {
                        LoadController(device);
                    }
                    else
                    {
                        UnloadController(device);
                    }
                }
            }
            _devicesAcquired = hasFocus;
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
                string filePath = GetMappingFilePath(name, currentVehicle.ToString());
                CreateMappingFile(filePath);
            }
            if (Input.GetKeyDown(KeyCode.F5))
            {
                Log("Reloading mappings");
                _ = StartCoroutine(LoadControlsMapping());
            }
            if (MappingsLoaded)
            {
                StartCoroutine(UpdateControllers());
            }
        }
        public IEnumerator UpdateControllers()
        {
            if (_devicesAcquired)
            {
                //Get controller data
                foreach (Device device in _devices)
                {
                    if (device is Keyboard)
                    {
                        _keyboardUpdates = (device as Keyboard).GetBufferedData();
                        _keyboardState = (device as Keyboard).GetCurrentState();
                    }
                    if (device is Joystick)
                    {
                        _joystickUpdates = (device as Joystick).GetBufferedData();
                        _joystickState = (device as Joystick).GetCurrentState();
                    }
                }

                //Send controller data to game control
                foreach (ControlMapping controlMapping in Mappings)
                {
                    if (controlMapping != null && controlMapping.KeyboardActions != null)
                    {
                        yield return UpdateKeyboard(controlMapping, controlMapping.KeyboardActions);
                    }
                    if (controlMapping != null && controlMapping.JoystickActions != null)
                    {
                        UpdateJoystick(controlMapping, controlMapping.JoystickActions);
                    }
                }
            }
            yield return null;
        }
        public IEnumerator UpdateKeyboard(ControlMapping mapping, List<GameAction> actions)
        {
            foreach (GameAction action in actions)
            {
                if (!(action is null))
                {
                    Keyboard device = _devices.Find(d => d.Information.InstanceGuid == action.ControllerInstanceGuid) as Keyboard;
                    KeyboardUpdate update = _keyboardUpdates.ToList().Find(k => k.Key.ToString() == action.ControllerActionName);
                    if (update.Key != Key.Unknown)
                    {
                        yield return ExecuteKeyboard(update, mapping, action);
                    }
                }
            }
        }
        public void UpdateJoystick(ControlMapping mapping, List<GameAction> actions)
        {
            foreach (GameAction action in actions)
            {
                if (!(action is null))
                {
                    Joystick device = _devices.Find(d => d.Information.InstanceGuid == action.ControllerInstanceGuid) as Joystick;
                    foreach (JoystickUpdate joystickUpdate in _joystickUpdates)
                    {
                        ExecuteJoystick(joystickUpdate, mapping, action);
                    }
                }
            }
        }
        public void ExecuteJoystick(JoystickUpdate update, ControlMapping mapping, GameAction action)
        {
            //TODO
        }
        public IEnumerator ExecuteKeyboard(KeyboardUpdate update, ControlMapping mapping, GameAction action)
        {
            if (update.Key.ToString() == action.ControllerActionName)
            {
                if (update.IsPressed)
                {
                    //Create custom control instance
                    Type customControlType = GetCustomControlType(mapping);
                    object instance;
                    if (mapping.HasCover)
                    {
                        instance = Activator.CreateInstance(customControlType, mapping.GameControlName, mapping.CoverName);
                    }
                    else
                    {
                        instance = Activator.CreateInstance(customControlType, mapping.GameControlName);
                    }

                    //Get methods for related behavior
                    List<MethodInfo> methodsInfo = customControlType.GetMethods(BindingFlags.Public | BindingFlags.Instance).ToList();
                    MethodInfo methodInfo = methodsInfo.Find(m =>
                        m.GetCustomAttribute<ControlAttribute>() != null &&
                        m.GetCustomAttribute<ControlAttribute>().SupportedBehavior == action.ControllerActionBehavior
                    );
                    methodInfo.Invoke(instance, null);
                    if (action.ControllerActionBehavior == ControllerActionBehavior.HoldOn)
                    {
                        MethodInfo offMethodInfo = methodsInfo.Find(m =>
                            m.GetCustomAttribute<ControlAttribute>().SupportedBehavior == ControllerActionBehavior.HoldOff
                        );
                        yield return new WaitUntil(() => _keyboardState.PressedKeys.Where(k => k == update.Key).Count() == 0);
                        offMethodInfo.Invoke(instance, null);
                    }
                }
            }
            yield return null;
        }
        public IEnumerator LoadDeviceInstances()
        {
            Log("Devices list loading");
            _directInput = new DirectInput();
            IList<DeviceInstance> devices = _directInput.GetDevices();
            _deviceInstances = devices.Where(
                d =>
                d.Type == SharpDX.DirectInput.DeviceType.Joystick ||
                d.Type == SharpDX.DirectInput.DeviceType.Gamepad ||
                d.Type == SharpDX.DirectInput.DeviceType.FirstPerson ||
                d.Type == SharpDX.DirectInput.DeviceType.Flight ||
                d.Type == SharpDX.DirectInput.DeviceType.Driving ||
                d.Type == SharpDX.DirectInput.DeviceType.Supplemental
            ).ToList();
            Log("Devices list loaded");
            yield return null;
        }
        private void LoadJoystick(Guid joystickGuid)
        {
            DeviceInstance instance = _deviceInstances.Find(d => d.InstanceGuid == joystickGuid);
            Joystick joystick = new Joystick(_directInput, joystickGuid);
            LoadController(joystick);
            _devices.Add(joystick);
        }
        private void LoadKeyboard(Guid joystickGuid)
        {
            Keyboard keyboard = new Keyboard(_directInput);
            LoadController(keyboard);
            _devices.Add(keyboard);
        }
        private void LoadController(Device device)
        {
            device.Properties.BufferSize = 128;
            device.Acquire();
        }
        private void UnloadController(Device device)
        {
            device.Unacquire();
        }
        private IEnumerator LoadControlsMapping()
        {
            VTOLVehicles vehicle = VTOLAPI.GetPlayersVehicleEnum();
            while (!ControlsLoaded(vehicle))
            {
                LoadControls();
                yield return new WaitForSeconds(2);
            }
            Log("Controls loaded for " + vehicle);
            LoadMappings(name, vehicle.ToString());
            Log("Mapping loaded for " + vehicle);
            yield return null;
        }
        public Type GetCustomControlType(ControlMapping mapping)
        {
            List<Type> types = mapping.Types;
            if (types.Contains(typeof(VRSwitchCover)))
            {
                return typeof(Cover);
            }
            if (types.Contains(typeof(VRLever)))
            {
                if (mapping.HasCover)
                {
                    return typeof(LeverWithCover);
                }
                return typeof(Lever);
            }
            if (types.Contains(typeof(VRTwistKnob)) || types.Contains(typeof(VRTwistKnobInt)))
            {
                return typeof(SwitchKnob);
            }
            if (types.Contains(typeof(VRButton)) || types.Contains(typeof(VRInteractable)))
            {
                return typeof(Interactable);
            }
            throw new Exception("Cannot determine control type");
        }
        public void LoadMappings(string modName, string vehicleName, string forceMappingFilePath = "")
        {
            string mappingFilePath = GetMappingFilePath(modName, vehicleName, forceMappingFilePath);
            if (!File.Exists(mappingFilePath))
                CreateMappingFile(mappingFilePath);
            Mappings = GetMappingsFromFile(mappingFilePath);
            _devices = new List<Device>();
            foreach (ControlMapping mapping in Mappings)
            {
                if (mapping.JoystickActions != null)
                {
                    foreach (GameAction action in mapping.JoystickActions)
                    {
                        Guid instanceGuid = action.ControllerInstanceGuid;
                        if (_devices.Find(d => d.Information.InstanceGuid == instanceGuid) == null)
                        {
                            LoadJoystick(instanceGuid);
                        }
                    }
                }
                if (mapping.KeyboardActions != null)
                {
                    foreach (GameAction action in mapping.KeyboardActions)
                    {
                        Guid instanceGuid = action.ControllerInstanceGuid;
                        if (_devices.Find(d => d.Information.InstanceGuid == action.ControllerInstanceGuid) == null)
                        {
                            LoadKeyboard(instanceGuid);
                        }
                    }
                }
            }
            _devicesAcquired = true;
        }
        public string GetMappingFilePath(string modName, string vehicleName, string forceMappingFilePath = "")
        {
            string mappingFilePath = Path.Combine(Directory.GetCurrentDirectory(), string.Format(@"{0}\{1}\mapping.{2}.json", settingsFileFolder, modName, vehicleName));
            if (!string.IsNullOrEmpty(forceMappingFilePath))
            {
                mappingFilePath = forceMappingFilePath;
            }
            return mappingFilePath;
        }
        private List<ControlMapping> GetMappingsFromFile(string mappingFilePath)
        {
            string jsonContent;
            using (FileStream fs = File.OpenRead(mappingFilePath))
            {
                using (var sr = new StreamReader(fs))
                {
                    jsonContent = sr.ReadToEnd();
                }
            }
            List<ControlMapping> mappings = JsonConvert.DeserializeObject<List<ControlMapping>>(jsonContent);
            return mappings;
        }
        public void CreateMappingFile(string mappingFilePath)
        {
            using (FileStream fs = File.Create(mappingFilePath))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    using (JsonTextWriter writer = new JsonTextWriter(sw))
                    {
                        List<ControlMapping> mappings = new List<ControlMapping>();
                        foreach (VRInteractable vrInteractable in _vrInteractables)
                        {
                            ControlMapping controlMapping = BuildControlMapping(vrInteractable.name, _vrButtons, _vrLevers, _vrSwitchCovers, _vrTwistKnobs, _vrTwistKnobsInt);
                            controlMapping.Types.Add(vrInteractable.GetType());
                            mappings.Add(controlMapping);
                        }
                        writer.WriteRaw(JsonConvert.SerializeObject(mappings.ToArray(), Formatting.Indented));
                    }
                }
            }
        }
        private ControlMapping BuildControlMapping(string vrInteractableName, params UnityEngine.Object[][] lists)
        {
            List<Type> types = new List<Type>();
            foreach (UnityEngine.Object[] unityObjectsArray in lists)
            {
                foreach (UnityEngine.Object unityObject in unityObjectsArray)
                {
                    if (unityObject.name == vrInteractableName)
                    {
                        types.Add(unityObject.GetType());
                    }
                }
            }
            KeyValuePair<string, string> leverWithCover = _vrLeversWithCover.ToList().Find(l => l.Key == vrInteractableName);
            if (leverWithCover.Equals(default(KeyValuePair<string, string>)))
            {
                return new ControlMapping(vrInteractableName, types);
            }
            else
            {
                return new ControlMapping(vrInteractableName, types, leverWithCover.Value);
            }
        }
        public static T GetGameControl<T>(string controlName)
            where T : UnityEngine.Object
        {
            T returnValue = null;
            ControlType controlType = ControlType.None;
            Enum.TryParse(typeof(T).Name, out controlType);
            switch (controlType)
            {
                case ControlType.VRInteractable:
                    returnValue = _vrInteractables.ToList().Find(x => x.name == controlName) as T;
                    break;
                case ControlType.VRLever:
                    returnValue = _vrLevers.ToList().Find(x => x.name == controlName) as T;
                    break;
                case ControlType.VRSwitchCover:
                    returnValue = _vrSwitchCovers.ToList().Find(x => x.name == controlName) as T;
                    break;
                case ControlType.VRTwistKnob:
                    returnValue = _vrTwistKnobs.ToList().Find(x => x.name == controlName) as T;
                    break;
                case ControlType.VRTwistKnobInt:
                    returnValue = _vrTwistKnobsInt.ToList().Find(x => x.name == controlName) as T;
                    break;
                case ControlType.VRButton:
                    returnValue = _vrButtons.ToList().Find(x => x.name == controlName) as T;
                    break;
                case ControlType.None:
                default:
                    throw new NullReferenceException("GetControl failed to get a returnValue for " + controlName);
            }
            return returnValue;
        }
        public static bool ControlsLoaded(VTOLVehicles vehicle)
        {
            int interactableCount;
            switch (vehicle)
            {
                case VTOLVehicles.FA26B:
                    interactableCount = 126;
                    break;
                case VTOLVehicles.None:
                case VTOLVehicles.AV42C:
                case VTOLVehicles.F45A:
                default:
                    throw new NotImplementedException("Controls not implemented for plane : " + vehicle);
            }
            return
                _vrJoystick != null && _vrThrottle != null &&
                _vrInteractables != null && _vrInteractables.Count() >= interactableCount &&
                _vrTwistKnobs != null && _vrTwistKnobs.Count() > 0 &&
                _vrTwistKnobsInt != null && _vrTwistKnobsInt.Count() > 0 &&
                _vrSwitchCovers != null && _vrSwitchCovers.Count() > 0 &&
                _vrLevers != null && _vrLevers.Count() > 0 &&
                _vrLeversWithCover != null && _vrLeversWithCover.Count() > 0;
        }
        public static void LoadControls()
        {
            _vrInteractables = FindObjectsOfType<VRInteractable>();
            _vrJoystick = FindObjectOfType<VRJoystick>();
            _vrThrottle = FindObjectOfType<VRThrottle>();
            _vrButtons = FindObjectsOfType<VRButton>();
            _vrTwistKnobs = FindObjectsOfType<VRTwistKnob>();
            _vrTwistKnobsInt = FindObjectsOfType<VRTwistKnobInt>();
            _vrSwitchCovers = FindObjectsOfType<VRSwitchCover>();
            _vrLevers = FindObjectsOfType<VRLever>();
            _vrLeversWithCover = new Dictionary<string, string>();
            foreach (VRLever lever in _vrLevers)
            {
                foreach (VRSwitchCover cover in _vrSwitchCovers)
                {
                    if (cover.coveredSwitch.name == lever.name)
                    {
                        //Fix for a weird behavior with covers and their repective levers
                        //Cover customCover = new Cover(cover.name);
                        LeverWithCover customLever = new LeverWithCover(lever.name, cover.name);
                        customLever.Cover.Lever.SetState(0);
                        _vrLeversWithCover.Add(lever.name, cover.name);
                    }
                }
            }
        }
    }
}