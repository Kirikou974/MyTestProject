using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Valve.Newtonsoft.Json;

namespace VTOLVRControlsMapper
{
    public static class ControlsHelper
    {
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
        private static VRInteractable[] _vrInteractables;
        private static VRTwistKnob[] _vrTwistKnobs;
        private static VRTwistKnobInt[] _vrTwistKnobsInt;
        private static VRButton[] _vrButtons;
        private static VRSwitchCover[] _vrSwitchCovers;
        private static VRLever[] _vrLevers;
        private static VRJoystick _vrJoystick;
        private static VRThrottle _vrThrottle;
        private static List<DeviceInstance> _deviceInstances;
        private static DirectInput _directInput;
        private static KeyboardUpdate[] _keyboardUpdates;
        private static JoystickUpdate[] _joystickUpdates;
        
        private static readonly List<Device> _devices = new List<Device>();

        public static List<ControlMapping> Mappings
        {
            get;
            internal set;
        }
        public static bool MappingsLoaded
        {
            get
            {
                return Mappings != null && Mappings.Count > 0;
            }
        }
        public static Type GetCustomControlType(List<Type> types)
        {
            if (types.Contains(typeof(VRButton)))
                return typeof(Interactable);
            if (types.Contains(typeof(VRSwitchCover)))
                return typeof(Cover);
            if (types.Contains(typeof(VRLever)))
                return typeof(Lever);
            if (types.Contains(typeof(VRTwistKnob)) || types.Contains(typeof(VRTwistKnobInt)))
                return typeof(SwitchKnob);
            throw new Exception("Cannot determine control type");
        }
        public static T GetGameControl<T>(string controlName)
            where T : class
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
                _vrLevers != null && _vrLevers.Count() > 0;
        }
        public static void Update()
        {
            //Get controller data
            foreach (Device device in _devices)
            {
                if (device is Keyboard)
                    _keyboardUpdates = (device as Keyboard).GetBufferedData();
                if (device is Joystick)
                    _joystickUpdates = (device as Joystick).GetBufferedData();
            }

            //Send controller data to game control
            foreach (ControlMapping controlMapping in Mappings)
            {
                if (controlMapping != null && controlMapping.KeyboardActions != null)
                    Update(controlMapping, controlMapping.KeyboardActions);
                if (controlMapping != null && controlMapping.JoystickActions != null) 
                    Update(controlMapping, controlMapping.JoystickActions);
            }
        }
        private static void Update(ControlMapping mapping, List<GameAction> actions)
        {
            foreach (GameAction action in actions)
            {
                if (!(action is null))
                {
                    Device device = _devices.Find(d => d.Information.InstanceGuid == action.ControllerInstanceGuid);
                    if (device is Keyboard)
                    {
                        foreach (KeyboardUpdate keyboardUpdate in _keyboardUpdates)
                            ExecuteControl(keyboardUpdate, mapping, action);
                    }
                    if (device is Joystick)
                    {
                        foreach (JoystickUpdate joystickUpdate in _joystickUpdates)
                            ExecuteControl(joystickUpdate, mapping, action);
                    }
                }
            }
        }
        public static void ExecuteControl(JoystickUpdate update, ControlMapping mapping, GameAction action)
        {
            //TODO
        }
        public static void ExecuteControl(KeyboardUpdate update, ControlMapping mapping, GameAction action)
        {
            if (update.Key.ToString() == action.ControllerActionName && update.IsPressed)
            {
                Type customType = GetCustomControlType(mapping.Types);
                MethodInfo methodInfo = customType.GetMethod(action.ActionName);
                object instance = Activator.CreateInstance(customType, mapping.ControlName);
                methodInfo.Invoke(instance, null);
            }
        }
        public static void LoadControls()
        {
            _vrInteractables = UnityEngine.Object.FindObjectsOfType<VRInteractable>();
            _vrJoystick = UnityEngine.Object.FindObjectOfType<VRJoystick>();
            _vrThrottle = UnityEngine.Object.FindObjectOfType<VRThrottle>();
            _vrButtons = UnityEngine.Object.FindObjectsOfType<VRButton>();
            _vrTwistKnobs = UnityEngine.Object.FindObjectsOfType<VRTwistKnob>();
            _vrTwistKnobsInt = UnityEngine.Object.FindObjectsOfType<VRTwistKnobInt>();
            _vrSwitchCovers = UnityEngine.Object.FindObjectsOfType<VRSwitchCover>();
            _vrLevers = UnityEngine.Object.FindObjectsOfType<VRLever>();
        }
        public static void LoadDeviceInstances()
        {
            _directInput = new DirectInput();
            IList<DeviceInstance> devices = _directInput.GetDevices();
            _deviceInstances = devices.Where(
                d =>
                d.Type == DeviceType.Joystick ||
                d.Type == DeviceType.Gamepad ||
                d.Type == DeviceType.FirstPerson ||
                d.Type == DeviceType.Flight ||
                d.Type == DeviceType.Driving ||
                d.Type == DeviceType.Supplemental
            ).ToList();
        }
        private static void LoadJoystick(Guid joystickGuid)
        {
            DeviceInstance instance = _deviceInstances.Find(d => d.InstanceGuid == joystickGuid);
            Joystick joystick = new Joystick(_directInput, joystickGuid);
            joystick.Properties.BufferSize = 128;
            joystick.Acquire();
            _devices.Add(joystick);
        }
        private static void LoadKeyboard(Guid joystickGuid)
        {
            Keyboard keyboard = new Keyboard(_directInput);
            keyboard.Properties.BufferSize = 128;
            keyboard.Acquire();
            _devices.Add(keyboard);
        }
        public static void LoadMappings(string modName, string vehicleName, string forceMappingFilePath = "")
        {
            string mappingFilePath = GetMappingFilePath(modName, vehicleName, forceMappingFilePath);
            if (!File.Exists(mappingFilePath))
                CreateMappingFile(mappingFilePath);
            Mappings = GetMappingsFromFile(mappingFilePath);
            foreach (ControlMapping mapping in Mappings)
            {
                if (mapping.JoystickActions != null)
                {
                    foreach (GameAction action in mapping.JoystickActions)
                    {
                        Guid instanceGuid = action.ControllerInstanceGuid;
                        if (_devices.Find(d => d.Information.InstanceGuid == instanceGuid) == null)
                            LoadJoystick(instanceGuid);
                    }
                }
                if (mapping.KeyboardActions != null)
                {
                    foreach (GameAction action in mapping.KeyboardActions)
                    {
                        Guid instanceGuid = action.ControllerInstanceGuid;
                        if (_devices.Find(d => d.Information.InstanceGuid == action.ControllerInstanceGuid) == null)
                            LoadKeyboard(instanceGuid);
                    }
                }
            }
        }
        public static string GetMappingFilePath(string modName, string vehicleName, string forceMappingFilePath = "")
        {
            //string mappingFilePath = Path.Combine(Directory.GetCurrentDirectory(), string.Format(@"VTOLVR_ModLoader\Mods\{0}\mapping.{1}.json", modName, vehicleName));
            string mappingFilePath = Path.Combine(Directory.GetCurrentDirectory(), string.Format(@"VTOLVR_ModLoader\dev\My Mods\{0}\mapping.{1}.json", modName, vehicleName));
            if (!string.IsNullOrEmpty(forceMappingFilePath))
                mappingFilePath = forceMappingFilePath;
            return mappingFilePath;
        }
        private static List<ControlMapping> GetMappingsFromFile(string mappingFilePath)
        {
            List<ControlMapping> mappings = new List<ControlMapping>();
            string jsonContent;
            using (FileStream fs = File.OpenRead(mappingFilePath))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    jsonContent = sr.ReadToEnd();
                }
            }
            mappings = JsonConvert.DeserializeObject<List<ControlMapping>>(jsonContent);
            return mappings;
        }
        public static void CreateMappingFile(string mappingFilePath)
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
                            ControlMapping controlMapping = GetControlMapping(vrInteractable.name, _vrButtons, _vrLevers, _vrSwitchCovers, _vrTwistKnobs, _vrTwistKnobsInt);
                            controlMapping.Types.Add(vrInteractable.GetType());
                            mappings.Add(controlMapping);
                        }
                        writer.WriteRaw(JsonConvert.SerializeObject(mappings.ToArray(), Formatting.Indented));
                    }
                }
            }
        }
        private static ControlMapping GetControlMapping(string vrInteractableName, params UnityEngine.Object[][] lists)
        {
            List<Type> types = new List<Type>();
            foreach (UnityEngine.Object[] unityObjectsArray in lists)
            {
                foreach (UnityEngine.Object unityObject in unityObjectsArray)
                {
                    if (unityObject.name == vrInteractableName)
                        types.Add(unityObject.GetType());
                }
            }
            return new ControlMapping(vrInteractableName, types);
        }
    }
}
