using SharpDX.DirectInput;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Valve.Newtonsoft.Json;
using VTOLVRControlsMapper.Core;
using DeviceType = SharpDX.DirectInput.DeviceType;

namespace VTOLVRControlsMapper
{
    public class ControlsHelper
    {
        //string settingsFileFolder = @"VTOLVR_ModLoader\Mods\";
        static readonly string settingsFileFolder = @"VTOLVR_ModLoader\dev\My Mods\";
        static bool _devicesAcquired;
        static DirectInput _directInput;
        static List<DeviceInstance> _deviceInstances;
        static List<Device> _devices;
        static KeyboardUpdate[] _keyboardUpdates;
        static KeyboardState _keyboardState;
        static JoystickUpdate[] _joystickUpdates;
        static JoystickState _joystickState;
        static Dictionary<string, object> _customControlCache;
        static List<ControlMapping> Mappings;
        //public static VRJoystick _vrJoystick;
        //public static VRThrottle _vrThrottle;
        public static List<UnityEngine.Object> UnityObjects { get; set; }
        public static List<Device> Devices { get => _devices; }
        public static bool MappingsLoaded { get => Mappings != null && Mappings.Count > 0; }
        public static bool ControlsLoaded(VTOLVehicles vehicle, VTOLMOD mod)
        {
            mod.Log("ControlsLoaded");
            VRInteractable[] controls = GetGameControls<VRInteractable>();
            mod.Log("controls: " + controls);

            if (controls is null)
            {
                mod.Log("controls is null");
                return false;
            }
            switch (vehicle)
            {
                case VTOLVehicles.FA26B:
                    mod.Log("controls count: " + controls.Count());
                    return controls.Count() > 126;
                case VTOLVehicles.None:
                case VTOLVehicles.AV42C:
                case VTOLVehicles.F45A:
                default:
                    throw new NotImplementedException("Controls not implemented for plane : " + vehicle);
            }
        }
        public static void OnApplicationFocus(bool hasFocus)
        {
            foreach (Device device in Devices)
            {
                //Unacquire keyboard when not focusing game
                if (!(device is null) && device is Keyboard)
                {
                    if (hasFocus)
                    {
                        AcquireController(device);
                    }
                    else
                    {
                        UnacquireController(device);
                    }
                }
            }
            _devicesAcquired = hasFocus;
        }
        public static T[] GetGameControls<T>()
            where T : UnityEngine.Object
        {
            if (UnityObjects is null)
            {
                return null;
            }
            IEnumerable<UnityEngine.Object> result = UnityObjects.Where(o => o is T);
            return result as T[];
        }
        public static T GetGameControl<T>(string controlName)
            where T : UnityEngine.Object
        {
            IEnumerable<T> results = GetGameControls<T>();
            if (results is null) { return null; }
            return results.ToList().Find(o => o.name == controlName);
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
                        VRInteractable[] vrInteractables = GetGameControls<VRInteractable>();
                        foreach (VRInteractable vrInteractable in vrInteractables)
                        {
                            IEnumerable<UnityEngine.Object> linkedObjects = UnityObjects.Where(o => o.name == vrInteractable.name);
                            ControlMapping controlMapping = BuildControlMapping(vrInteractable.name, linkedObjects);
                            mappings.Add(controlMapping);
                        }
                        writer.WriteRaw(JsonConvert.SerializeObject(mappings.ToArray(), Formatting.Indented));
                    }
                }
            }
        }
        private static ControlMapping BuildControlMapping(string vrInteractableName, IEnumerable<UnityEngine.Object> linkedObjects)
        {
            List<Type> types = new List<Type>();
            foreach (UnityEngine.Object unityObject in linkedObjects)
            {
                types.Add(unityObject.GetType());
            }
            return new ControlMapping(vrInteractableName, types);
        }
        public static void LoadMappings(string modName, string vehicleName, string forceMappingFilePath = "")
        {
            string mappingFilePath = GetMappingFilePath(modName, vehicleName, forceMappingFilePath);
            if (!File.Exists(mappingFilePath))
            {
                CreateMappingFile(mappingFilePath);
            }
            Mappings = GetMappingsFromFile(mappingFilePath);
            _devices = new List<Device>();
            _customControlCache = new Dictionary<string, object>();
            foreach (ControlMapping mapping in Mappings)
            {
                CreateMappingInstances(mapping);
                LoadControllers<Joystick>(mapping);
                LoadControllers<Keyboard>(mapping);
            }
            _devicesAcquired = true;
        }
        public static void CreateMappingInstances(ControlMapping mapping)
        {
            //Create custom control instance
            Type customControlType = GetMappingType(mapping.Types);
            object instance;
            instance = Activator.CreateInstance(customControlType, mapping.GameControlName);
            _customControlCache.Add(mapping.GameControlName, instance);
        }

        public static Type GetMappingType(List<Type> types)
        {
            IEnumerable<Type> controlTypes = FindAllDerivedTypes<IControl>();
            Type returnType = null;
            foreach (Type type in controlTypes)
            {
                Type genericType = GetBaseTypeGeneric(type.BaseType);
                if (types.Contains(genericType))
                {
                    returnType = type;
                }
            }
            return returnType;
        }
        public static Type GetBaseTypeGeneric(Type baseType)
        {
            if (!baseType.IsGenericType)
            {
                return GetBaseTypeGeneric(baseType.BaseType);
            }
            else
            {
                return baseType.GenericTypeArguments[0];
            }
        }
        public static object GetMappingInstance(ControlMapping mapping)
        {
            return _customControlCache[mapping.GameControlName];
        }
        public static string GetMappingFilePath(string modName, string vehicleName, string forceMappingFilePath = "")
        {
            string mappingFilePath = Path.Combine(Directory.GetCurrentDirectory(), string.Format(@"{0}\{1}\mapping.{2}.json", settingsFileFolder, modName, vehicleName));
            if (!string.IsNullOrEmpty(forceMappingFilePath))
            {
                mappingFilePath = forceMappingFilePath;
            }
            return mappingFilePath;
        }
        private static List<ControlMapping> GetMappingsFromFile(string mappingFilePath)
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

        public static IEnumerator UpdateControllers()
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
        public static IEnumerator UpdateKeyboard(ControlMapping mapping, List<GameAction> actions)
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
        public static void UpdateJoystick(ControlMapping mapping, List<GameAction> actions)
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
        public static void ExecuteJoystick(JoystickUpdate update, ControlMapping mapping, GameAction action)
        {
            //TODO
        }
        public static IEnumerator ExecuteKeyboard(KeyboardUpdate update, ControlMapping mapping, GameAction action)
        {
            if (update.Key.ToString() == action.ControllerActionName)
            {
                if (update.IsPressed)
                {
                    //Get custom control instance
                    object instance = GetMappingInstance(mapping);

                    //Get methods for related behavior
                    List<MethodInfo> methodsInfo = instance.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance).ToList();
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
        public static IEnumerator LoadDeviceInstances()
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
            yield return null;
        }
        private static void LoadControllers<T>(ControlMapping mapping)
        {
            List<GameAction> actions = new List<GameAction>();
            bool isKeyboard = false;
            if (typeof(T) == typeof(Joystick))
            {
                actions = mapping.JoystickActions;
            }
            else if (typeof(Keyboard) == typeof(Keyboard))
            {
                actions = mapping.KeyboardActions;
                isKeyboard = true;
            }
            if (actions != null)
            {
                foreach (GameAction action in actions)
                {
                    Guid instanceGuid = action.ControllerInstanceGuid;
                    if (_devices.Find(d => d.Information.InstanceGuid == instanceGuid) == null)
                    {
                        DeviceInstance instance = _deviceInstances.Find(d => d.InstanceGuid == instanceGuid);
                        Device device = null;
                        if (isKeyboard)
                        {
                            device = new Keyboard(_directInput);
                        }
                        else
                        {
                            device = new Joystick(_directInput, instanceGuid);
                        }
                        AcquireController(device);
                        _devices.Add(device);
                    }
                }
            }
        }
        private static void AcquireController(Device device)
        {
            device.Properties.BufferSize = 128;
            device.Acquire();
        }
        private static void UnacquireController(Device device)
        {
            device.Unacquire();
        }
        public static List<Type> FindAllDerivedTypes<T>()
        {
            return FindAllDerivedTypes<T>(Assembly.GetAssembly(typeof(T)));
        }
        public static List<Type> FindAllDerivedTypes<T>(Assembly assembly)
        {
            var derivedType = typeof(T);
            return assembly
                .GetTypes()
                .Where(t =>
                    t != derivedType &&
                    t.IsClass &&
                    !t.IsAbstract &&
                    derivedType.IsAssignableFrom(t)
                    ).ToList();
        }
    }
}
