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
        public static List<ControlMapping> Mappings { get => _mappings; }
        static List<UnityEngine.Object> _unityObjects = new List<UnityEngine.Object>();
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

                        JsonSerializerSettings settings = new JsonSerializerSettings()
                        {
                            TypeNameHandling = TypeNameHandling.All
                        };
                        writer.WriteRaw(JsonConvert.SerializeObject(mappings.ToArray(), Formatting.Indented, GetJSONSerializerSettings()));
                    }
                }
            }
        }
        private static JsonSerializerSettings GetJSONSerializerSettings()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            };
            return settings;
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
            ControlMapping[] mappings = JsonConvert.DeserializeObject<ControlMapping[]>(jsonContent, GetJSONSerializerSettings());
            return mappings.ToList();
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
            string mappingFilePath = GetMappingFilePath(settingsFileFolder, modName, vehicleName, forceMappingFilePath);
            LoadMappings(mappingFilePath);
        }
        public static void LoadMappings(string mappingFilePath)
        {
            //Create JSON file if it does not exist
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
                //Determine custom control type from class attributes
                IEnumerable<Type> controlTypes = GetDerivedTypes<IControl>();
                Type customControlType = controlTypes.FirstOrDefault(t =>
                    t.GetCustomAttribute<ControlClassAttribute>() != null &&
                    t.GetCustomAttribute<ControlClassAttribute>().UnityTypes.SequenceEqual(mapping.Types)
                );
                //Create custom control instance
                if (customControlType != null)
                {
                    object instance = Activator.CreateInstance(customControlType, mapping.GameControlName);
                    _customControlCache.Add(mapping.GameControlName, instance);
                }
                else
                {
                    Main.instance.Log("Cannot create a custom control isntance for : " + mapping.GameControlName);
                }
            }
        }
        #endregion
        #region Controllers
        public static IEnumerator UpdateGameControls()
        {
            //Get controllers update
            foreach (Device device in _devices)
            {
                if (device is Keyboard)
                {
                    _keyboardUpdates = (device as Keyboard).GetBufferedData();
                    _keyboardState = (device as Keyboard).GetCurrentState();
                }
                if (device is Joystick)
                {
                    //TODO: this is a bug, need to handle multiple joysticks
                    _joystickUpdates = (device as Joystick).GetBufferedData();
                    _joystickState = (device as Joystick).GetCurrentState();
                }
            }

            //Send controller data to game control
            foreach (ControlMapping controlMapping in _mappings)
            {
                if (controlMapping != null && controlMapping.GameActions != null)
                {
                    foreach (GameAction action in controlMapping.GameActions)
                    {
                        if (action is GenericGameAction)
                        {
                            yield return UpdateGeneric(controlMapping, action as GenericGameAction);
                        }
                        else if (action is ThrottleAction)
                        {
                            yield return UpdateThrottle(controlMapping, action as ThrottleAction);
                        }
                        else if (action is StickAction)
                        {
                            yield return UpdateStick(controlMapping, action as StickAction);
                        }
                    }
                }
            }
            yield return null;
        }
        private static IEnumerator UpdateGeneric(ControlMapping mapping, GenericGameAction action)
        {
            if (!(action is null) && !(_keyboardUpdates is null))
            {
                //TODO handle other update types
                KeyboardUpdate update = _keyboardUpdates.FirstOrDefault(k => k.Key.ToString() == action.ControllerButtonName);
                if (update.Key != Key.Unknown && update.Key.ToString() == action.ControllerButtonName && update.IsPressed)
                {
                    bool keyIsReleased = _keyboardState.PressedKeys.Where(k => k == update.Key).Count() == 0;
                    yield return ExecuteButton(mapping.GameControlName, update.Key.ToString(), action.ControllerActionBehavior, keyIsReleased);
                }
            }
            yield return null;
        }
        private static IEnumerator UpdateThrottle(ControlMapping mapping, ThrottleAction throttleAction)
        {
            if (!(throttleAction is null) && !(_joystickUpdates is null))
            {
                foreach (JoystickUpdate joystickUpdate in _joystickUpdates)
                {
                    ExecuteAxis(throttleAction.PowerAxis, mapping, joystickUpdate);
                    ExecuteAxis(throttleAction.TriggerAxis, mapping, joystickUpdate);
                    //TODO handle Menu and Thumbstick axis
                    //yield return ExecuteButton(throttleAction.Menu, joystickUpdate.Offset.ToString(), ControllerActionBehavior.Toggle);
                }
            }
            yield return null;
        }
        private static IEnumerator UpdateStick(ControlMapping mapping, StickAction throttleAction)
        {
            yield return null;
        }
        private static void ExecuteAxis(Axis axis, ControlMapping mapping, JoystickUpdate joystickUpdate)
        {
            if (axis != null && axis.Name == joystickUpdate.Offset.ToString())
            {
                object instance = _customControlCache[mapping.GameControlName];
                MethodInfo methodInfo = GetExecuteMethod(instance, mapping.GameControlName, ControllerActionBehavior.Axis);
                float axisValue = ConvertAxisValue(joystickUpdate.Value, axis.Invert, axis.MappingRange);
                methodInfo.Invoke(instance, new object[] { axisValue });
            }
        }

        private static IEnumerator ExecuteButton(string gameControlName, string controllerButtonName, ControllerActionBehavior behavior, bool buttonIsReleased = false)
        {
            //Get custom control instance
            object instance = _customControlCache[gameControlName];

            //Get methods for related behavior
            MethodInfo methodInfo = GetExecuteMethod(instance, gameControlName, behavior);
            yield return methodInfo.Invoke(instance, null);
            if (behavior == ControllerActionBehavior.HoldOn)
            {
                MethodInfo offMethodInfo = GetExecuteMethod(instance, gameControlName, ControllerActionBehavior.HoldOff);
                yield return new WaitUntil(() => buttonIsReleased);
                offMethodInfo.Invoke(instance, null);
            }
            yield return null;
        }
        private static MethodInfo GetExecuteMethod(object instance, string gameControlName, ControllerActionBehavior behavior)
        {
            List<MethodInfo> methodsInfo = instance.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance).ToList();
            MethodInfo methodInfo = methodsInfo.Find(m =>
                m.GetCustomAttribute<ControlMethodAttribute>() != null &&
                m.GetCustomAttribute<ControlMethodAttribute>().SupportedBehavior == behavior
            );
            return methodInfo;
        }
        public static void LoadDeviceInstances()
        {
            _directInput = new DirectInput();
            IList<DeviceInstance> devices = _directInput.GetDevices();
            _deviceInstances = devices.ToList();
        }
        public static void LoadControllers()
        {
            IEnumerable<Guid> instanceGuids = _mappings
                .Where(mapping => mapping.GameActions != null && mapping.GameActions.Count != 0)
                .SelectMany(mapping => mapping.GameActions)
                .Select(gameAction => gameAction.ControllerInstanceGuid)
                .Distinct();

            foreach (Guid instanceGuid in instanceGuids)
            {
                if (_devices.Find(d => d.Information.InstanceGuid == instanceGuid) == null)
                {
                    DeviceInstance instance = _deviceInstances.Find(d => d.InstanceGuid == instanceGuid);
                    Device device = null;
                    switch (instance.Type)
                    {
                        case DeviceType.Keyboard:
                            device = new Keyboard(_directInput);
                            break;
                        case DeviceType.Joystick:
                        case DeviceType.Gamepad:
                        case DeviceType.Driving:
                        case DeviceType.Flight:
                        case DeviceType.FirstPerson:
                        case DeviceType.Supplemental:
                            device = new Joystick(_directInput, instanceGuid);
                            break;
                        case DeviceType.Device:
                        case DeviceType.Mouse:
                        case DeviceType.ControlDevice:
                        case DeviceType.ScreenPointer:
                        case DeviceType.Remote:
                        default:
                            //Not supported device types
                            break;
                    }
                    AcquireController(device);
                    _devices.Add(device);
                }
            }
        }
        private static void AcquireController(Device device)
        {
            device.Properties.BufferSize = 128;
            device.Acquire();
        }
        #endregion
        public static List<Type> GetDerivedTypes<T>()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(T));
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
        public static float ConvertAxisValue(int value, bool invert, MappingRange mappingRange = MappingRange.Full)
        {
            float retVal;
            if (value == 65535)
            {
                retVal = 1;
            }
            else
            {
                retVal = (((float)value / 32767) - 1);
            }
            if (invert)
            {
                retVal *= -1;
            }
            switch (mappingRange)
            {
                case MappingRange.Low:
                    retVal /= 2;
                    retVal -= 0.5f;
                    break;
                case MappingRange.High:
                    retVal /= 2;
                    retVal += 0.5f;
                    break;
                case MappingRange.Full:
                default:
                    break;
            }
            return (float)Math.Round(retVal, 2);
        }
    }
}
