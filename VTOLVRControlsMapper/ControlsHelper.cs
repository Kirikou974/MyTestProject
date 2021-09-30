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
        static DirectInput _directInput;
        static List<DeviceInstance> _deviceInstances;
        static List<Device> _devices = new List<Device>();
        static KeyboardUpdate[] _keyboardUpdates;
        static KeyboardState _keyboardState;
        static JoystickUpdate[] _joystickUpdates;
        static JoystickState _joystickState;
        static Dictionary<string, object> _customControlCache;
        static List<ControlMapping> _mappings;
        static List<UnityEngine.Object> _unityObjects = new List<UnityEngine.Object>();
        static VRJoystick _vrJoystick;
        static VRThrottle _vrThrottle;
        public static List<Device> Devices { get => _devices; }
        public static bool MappingsLoaded
        {
            get
            {
                return _mappings != null && _mappings.Count > 0 &&
                    _customControlCache != null && _customControlCache.Count > 0;
            }
        }
        #region Unity objects
        public static bool UnityObjectsLoaded(VTOLVehicles vehicle)
        {
            if (_vrJoystick is null && _vrThrottle is null)
            {
                return false;
            }
            IEnumerable<VRInteractable> controls = GetGameControls<VRInteractable>();
            switch (vehicle)
            {
                case VTOLVehicles.FA26B:
                    return controls.Count() >= 126;
                case VTOLVehicles.None:
                case VTOLVehicles.AV42C:
                case VTOLVehicles.F45A:
                default:
                    throw new NotImplementedException("Controls not implemented for plane : " + vehicle);
            }
        }
        public static void LoadUnityObjects()
        {
            _unityObjects = new List<UnityEngine.Object>();
            _vrJoystick = UnityEngine.Object.FindObjectOfType<VRJoystick>();
            _vrThrottle = UnityEngine.Object.FindObjectOfType<VRThrottle>();

            //Get IControl derived class to deduce which unity control to go fetch
            List<Type> controlTypes = GetDerivedTypes<IControl>();
            foreach (Type controlType in controlTypes)
            {
                //FindObjectsOfType<T> where T is the type deduced from the classes that implement IControl interface
                Type unityObjectType = controlType.BaseType.GenericTypeArguments[0];
                MethodInfo[] vtolModMethods = typeof(UnityEngine.Object).GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
                MethodInfo findObjectsOfTypeMethod = vtolModMethods.Single(m => m.IsGenericMethod && m.Name == nameof(UnityEngine.Object.FindObjectsOfType));
                MethodInfo findObjectsOfTypeMethodGeneric = findObjectsOfTypeMethod.MakeGenericMethod(new Type[] { unityObjectType });
                UnityEngine.Object[] objects = findObjectsOfTypeMethodGeneric.Invoke(typeof(UnityEngine.Object), null) as UnityEngine.Object[];
                _unityObjects.AddRange(objects);
            }
        }
        public static T GetGameControl<T>(string controlName) where T : UnityEngine.Object
        {
            IEnumerable<T> results = GetGameControls<T>();
            return results.Single(o => o.name == controlName);
        }
        public static IEnumerable<T> GetGameControls<T>() where T : UnityEngine.Object
        {
            return _unityObjects.OfType<T>();
        }
        private static IEnumerable<T> GetGameControls<T>(string controlName) where T : UnityEngine.Object
        {
            return GetGameControls<T>().Where(g => g.name == controlName);
        }
        #endregion
        #region Mappings and mapping file
        public static void CreateMappingFile(string mappingFilePath)
        {
            string directoryName = Path.GetDirectoryName(mappingFilePath);
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
            using (FileStream fs = File.Create(mappingFilePath))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    using (JsonTextWriter writer = new JsonTextWriter(sw))
                    {
                        List<ControlMapping> mappings = new List<ControlMapping>();
                        IEnumerable<VRInteractable> vrInteractables = GetGameControls<VRInteractable>();
                        foreach (VRInteractable vrInteractable in vrInteractables)
                        {
                            IEnumerable<UnityEngine.Object> linkedObjects = GetGameControls<UnityEngine.Object>(vrInteractable.name);
                            ControlMapping controlMapping = BuildControlMapping(vrInteractable.name, linkedObjects);
                            mappings.Add(controlMapping);
                        }
                        writer.WriteRaw(JsonConvert.SerializeObject(mappings.ToArray(), Formatting.Indented));
                    }
                }
            }
        }
        public static string GetMappingFilePath(string settingsFileFolder, string modName, string vehicleName, string forceMappingFilePath = "")
        {
            string mappingFilePath = Path.Combine(Directory.GetCurrentDirectory(), settingsFileFolder, modName, string.Format(@"mapping.{0}.json", vehicleName));
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
        private static ControlMapping BuildControlMapping(string vrInteractableName, IEnumerable<UnityEngine.Object> linkedObjects)
        {
            List<Type> types = new List<Type>();
            foreach (UnityEngine.Object unityObject in linkedObjects)
            {
                types.Add(unityObject.GetType());
            }
            return new ControlMapping(vrInteractableName, types);
        }
        public static void LoadMappings(string settingsFileFolder, string modName, string vehicleName, string forceMappingFilePath = "")
        {
            //Create JSON file if it does not exist
            string mappingFilePath = GetMappingFilePath(settingsFileFolder, modName, vehicleName, forceMappingFilePath);
            if (!File.Exists(mappingFilePath))
            {
                CreateMappingFile(mappingFilePath);
            }
            //Generate mappings from JSON file parsing
            _mappings = GetMappingsFromFile(mappingFilePath);
        }
        public static void LoadMappingInstances()
        {
            _customControlCache = new Dictionary<string, object>();
            foreach (ControlMapping mapping in _mappings)
            {
                CreateMappingInstances(mapping);
            }
        }
        private static void CreateMappingInstances(ControlMapping mapping)
        {
            //Create custom control instance
            Type customControlType = GetMappingType(mapping.Types);
            object instance = Activator.CreateInstance(customControlType, mapping.GameControlName);
            _customControlCache.Add(mapping.GameControlName, instance);
        }
        private static Type GetMappingType(List<Type> types)
        {
            IEnumerable<Type> controlTypes = GetDerivedTypes<IControl>();
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
        private static object GetMappingInstance(ControlMapping mapping)
        {
            return _customControlCache[mapping.GameControlName];
        }
        #endregion
        #region Controllers
        public static IEnumerator UpdateControllers()
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
            foreach (ControlMapping controlMapping in _mappings)
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
            yield return null;
        }
        private static IEnumerator UpdateKeyboard(ControlMapping mapping, List<GameAction> actions)
        {
            foreach (GameAction action in actions)
            {
                if (!(action is null))
                {
                    Keyboard device = _devices.Find(d => d.Information.InstanceGuid == action.ControllerInstanceGuid) as Keyboard;
                    KeyboardUpdate update = _keyboardUpdates.FirstOrDefault(k => k.Key.ToString() == action.ControllerActionName);
                    if (update.Key != Key.Unknown)
                    {
                        yield return ExecuteKeyboard(update, mapping, action);
                    }
                }
            }
        }
        private static void UpdateJoystick(ControlMapping mapping, List<GameAction> actions)
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
        private static void ExecuteJoystick(JoystickUpdate update, ControlMapping mapping, GameAction action)
        {
            //TODO
        }
        private static IEnumerator ExecuteKeyboard(KeyboardUpdate update, ControlMapping mapping, GameAction action)
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
                    yield return methodInfo.Invoke(instance, null);
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
        public static void LoadControllers<T>()
        {
            foreach (ControlMapping mapping in _mappings)
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
        }
        private static void AcquireController(Device device)
        {
            device.Properties.BufferSize = 128;
            device.Acquire();
        }
        #endregion
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
        public static List<Type> GetDerivedTypes<T>()
        {
            return GetDerivedTypes<T>(Assembly.GetAssembly(typeof(T)));
        }
        private static List<Type> GetDerivedTypes<T>(Assembly assembly)
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
