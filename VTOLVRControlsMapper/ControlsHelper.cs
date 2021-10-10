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
    public enum SimpleDeviceType
    {
        None,
        Keyboard,
        Joystick
    }
    public class ControlsHelper
    {
        static DirectInput _directInput;
        static List<DeviceInstance> _deviceInstances;
        static Dictionary<Device, SimpleDeviceType> _devices;
        static Dictionary<string, object> _customControlCache;
        static List<ControlMapping> _mappings;
        static List<UnityEngine.Object> _unityObjects = new List<UnityEngine.Object>();
        static Dictionary<Guid, KeyboardUpdate[]> _keyboardUpdates;
        static Dictionary<Guid, KeyboardState> _keyboardStates;
        static Dictionary<Guid, JoystickUpdate[]> _joystickUpdates;
        static Dictionary<Guid, JoystickState> _joystickStates;
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
        public static void Reset()
        {
            _unityObjects = new List<UnityEngine.Object>();
            Main.instance.StopCoroutine(nameof(GenericRoutine));
            Main.instance.StopCoroutine(nameof(AxisRoutine));
            Main.instance.StopCoroutine(nameof(MenuRoutine));
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
        public static void StartGameControlsRoutines()
        {
            //Poll controllers routine
            IEnumerable<Guid> controllersToPoll = GetUniqueDeviceGuids();
            _keyboardUpdates = new Dictionary<Guid, KeyboardUpdate[]>();
            _joystickUpdates = new Dictionary<Guid, JoystickUpdate[]>();
            foreach (Guid controllerGuid in controllersToPoll)
            {
                Main.instance.StartCoroutine(PollRoutine(controllerGuid));
            }
            StartGenericGameActionRoutines();
            StartThrottleActionRoutines();
            StartStickActionRoutines();
            //foreach (ControlMapping controlMapping in _mappings)
            //{
            //    if (controlMapping != null && controlMapping.GameActions != null)
            //    {
            //        foreach (GameAction action in controlMapping.GameActions)
            //        {
            //            if (action != null)
            //            {
            //                Guid controllerGuid = action.ControllerInstanceGuid;
            //                if ((action is GenericGameAction) && (action as GenericGameAction) != null)
            //                {
            //                    GenericGameAction genericAction = action as GenericGameAction;
            //                    Keyboard keyboard = _devices.Find(d => d.Information.InstanceGuid == controllerGuid) as Keyboard;
            //                    Main.instance.StopCoroutine(GenericRoutine(keyboard, controlMapping, genericAction));
            //                    Main.instance.StartCoroutine(GenericRoutine(keyboard, controlMapping, genericAction));
            //                }
            //                else if ((action is ThrottleAction) && (action as ThrottleAction) != null)
            //                {
            //                    ThrottleAction throttleAction = action as ThrottleAction;
            //                    Joystick joystick = _devices.Find(d => d.Information.InstanceGuid == controllerGuid) as Joystick;
            //                    Main.instance.StopCoroutine(AxisRoutine(joystick, throttleAction.PowerAxis, controlMapping);
            //                    Main.instance.StopCoroutine(AxisRoutine(joystick, throttleAction.TriggerAxis, controlMapping);
            //                    //TODO handle Menu and Thumbstick axis
            //                    //yield return ExecuteButton(throttleAction.Menu, joystickUpdate.Offset.ToString(), ControllerActionBehavior.Toggle);
            //                }
            //                else if (action is StickAction)
            //                {
            //                    //TODO implement this
            //                    //StickRoutine(controlMapping, action as StickAction);
            //                }
            //            }
            //        }
            //    }
            //}
        }
        private static void StartStickActionRoutines()
        {
            //TODO implement
        }
        private static void StartThrottleActionRoutines()
        {
            //TODO implement
        }
        public static void StartGenericGameActionRoutines()
        {
            var mappings = GetGameActions<GenericGameAction>();
            foreach (var mapping in mappings)
            {
                Guid controllerInstanceGuid = mapping.Action.ControllerInstanceGuid;
                SimpleDeviceType deviceType = GetDeviceType(controllerInstanceGuid);
                switch (deviceType)
                {
                    case SimpleDeviceType.Keyboard:
                        Func<KeyboardUpdate, bool> kbUpdatePredicate = (update) =>
                        {
                            return update.Key != Key.Unknown &&
                                update.Key.ToString() == mapping.Action.ControllerButtonName &&
                                update.IsPressed;
                        };
                        Func<KeyboardState, bool> kbStatePredicate = (state) =>
                        {
                            return state.PressedKeys.Where(k => k.ToString() == mapping.Action.ControllerButtonName).Count() == 0;
                        };
                        IEnumerator kbRoutine = GenericRoutine(
                            _keyboardUpdates, _keyboardStates, 
                            mapping.GameControlName, mapping.Action,
                            kbUpdatePredicate, kbStatePredicate
                        );
                        Main.instance.StartCoroutine(kbRoutine);
                        break;
                    case SimpleDeviceType.Joystick:
                        Predicate<JoystickUpdate> joyUpdatePredicate = (update) =>
                        {
                            return update.Offset.ToString() == mapping.Action.ControllerButtonName && update.Value == 128;
                        };
                        Predicate<JoystickState> joyStatePredicate = (state) =>
                        {
                            //TODO implement
                            //return state.PressedKeys.Where(k => k.ToString() == mapping.Action.ControllerButtonName).Count() == 0;
                            return false;
                        };
                        IEnumerator joyRoutine = GenericRoutine(
                            _keyboardUpdates, _keyboardStates,
                            mapping.GameControlName, mapping.Action,
                            joyUpdatePredicate, joyStatePredicate
                        );
                        Main.instance.StartCoroutine(joyRoutine);
                        break;
                    case SimpleDeviceType.None:
                    default:
                        break;
                }
            }
        }
        public static dynamic GetGameActions<T>() where T : GameAction
        {
            var retValue = _mappings
                .Where(g => g.GameActions != null)
                .SelectMany(m => m.GameActions, (mapping, Action) => new { mapping.GameControlName, Action })
                .Where(a => a.Action != null && a.Action is T);
            return retValue;
        }
        private static IEnumerator PollRoutine(Guid controllerGuid)
        {
            while (true)
            {
                SimpleDeviceType devType = GetDeviceType(controllerGuid);
                switch (devType)
                {
                    case SimpleDeviceType.Keyboard:
                        Keyboard keyboard = GetDevice<Keyboard>(controllerGuid);
                        KeyboardUpdate[] kbUpdates = keyboard.GetBufferedData();
                        KeyboardState kbState = keyboard.GetCurrentState();
                        _keyboardUpdates[controllerGuid] = kbUpdates;
                        _keyboardStates[controllerGuid] = kbState;
                        break;
                    case SimpleDeviceType.Joystick:
                        Joystick joystick = GetDevice<Joystick>(controllerGuid);
                        JoystickUpdate[] joystickUpdates = joystick.GetBufferedData();
                        JoystickState joystickState = joystick.GetCurrentState();
                        _joystickUpdates[controllerGuid] = joystickUpdates;
                        _joystickStates[controllerGuid] = joystickState;
                        break;
                    case SimpleDeviceType.None:
                    default:
                        break;
                }
                yield return null;
            }
        }
        private static IEnumerator GenericRoutine<Update, State>(
            Dictionary<Guid, Update[]> updates,
            Dictionary<Guid, State> states,
            string gameControlName,
            GenericGameAction action,
            Func<Update, bool> updatePredicate,
            Func<State, bool> statePredicate)
            where Update : IStateUpdate
        {
            while (true)
            {
                if (updates != null &&
                    updates[action.ControllerInstanceGuid] != null &&
                    updates[action.ControllerInstanceGuid].Where(updatePredicate).Count() > 0)
                {
                    Update update = updates[action.ControllerInstanceGuid].First(updatePredicate);

                    //Get custom control instance and methods
                    object instance = _customControlCache[gameControlName];
                    MethodInfo methodInfo = GetExecuteMethod(instance, gameControlName, action.ControllerActionBehavior);
                    yield return methodInfo.Invoke(instance, null);

                    if (action.ControllerActionBehavior == ControllerActionBehavior.HoldOn)
                    {
                        bool isReleased = statePredicate(states[action.ControllerInstanceGuid]);
                        MethodInfo offMethodInfo = GetExecuteMethod(instance, gameControlName, ControllerActionBehavior.HoldOff);
                        yield return new WaitUntil(() => isReleased);
                        offMethodInfo.Invoke(instance, null);
                    }
                }
                yield return null;
            }
        }
        private static IEnumerator AxisRoutine(Joystick joystick, Axis axis, ControlMapping mapping)
        {
            while (true)
            {
                //if (axis != null)
                //{
                //    //Get custom control instance and methods
                //    object instance = _customControlCache[mapping.GameControlName];
                //    MethodInfo methodInfo = GetExecuteMethod(instance, mapping.GameControlName, ControllerActionBehavior.Axis);
                //    //Get controller update
                //    Func<KeyValuePair<Guid, JoystickUpdate>, bool> predicate = delegate (KeyValuePair<Guid, JoystickUpdate> keyValuePair)
                //    {
                //        return keyValuePair.Value.Offset.ToString() == axis.Name;
                //    };
                //    if (_joystickUpdates != null && _joystickUpdates.Where(predicate).Count() > 0)
                //    {
                //        JoystickUpdate update = _joystickUpdates.First(predicate);
                //        float axisValue = ConvertAxisValue(update.Value, axis.Invert, axis.MappingRange);
                //        methodInfo.Invoke(instance, new object[] { axisValue });
                //    }
                //}
                yield return null;
            }
        }
        private static IEnumerator MenuRoutine(Joystick joystick, string menu, ControlMapping mapping, JoystickUpdate[] updates)
        {
            //TODO implement
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
        public static void LoadControllers()
        {
            _directInput = new DirectInput();
            IList<DeviceInstance> devices = _directInput.GetDevices();
            _deviceInstances = devices.ToList();
            _devices = new Dictionary<Device, SimpleDeviceType>();
            _keyboardUpdates = new Dictionary<Guid, KeyboardUpdate[]>();
            _keyboardStates = new Dictionary<Guid, KeyboardState>();
            _joystickUpdates = new Dictionary<Guid, JoystickUpdate[]>();
            _joystickStates = new Dictionary<Guid, JoystickState>();

            IEnumerable<Guid> controllerGuids = GetUniqueDeviceGuids();
            foreach (Guid controllerGuid in controllerGuids)
            {
                Device device = null;
                SimpleDeviceType deviceType = GetDeviceType(controllerGuid);
                switch (deviceType)
                {
                    case SimpleDeviceType.Keyboard:
                        device = new Keyboard(_directInput);
                        _keyboardUpdates.Add(controllerGuid, null);
                        _keyboardStates.Add(controllerGuid, null);
                        break;
                    case SimpleDeviceType.Joystick:
                        device = new Joystick(_directInput, controllerGuid);
                        _joystickUpdates.Add(controllerGuid, null);
                        _joystickStates.Add(controllerGuid, null);
                        break;
                    case SimpleDeviceType.None:
                    default:
                        break;
                }
                AcquireController(device);
                _devices.Add(device, deviceType);
            }
        }
        private static SimpleDeviceType GetDeviceType(Guid guid)
        {
            DeviceInstance instance = _deviceInstances.Find(d => d.InstanceGuid == guid);
            SimpleDeviceType devType = SimpleDeviceType.None;
            switch (instance.Type)
            {
                case DeviceType.Keyboard:
                    devType = SimpleDeviceType.Keyboard;
                    break;
                case DeviceType.Joystick:
                case DeviceType.Gamepad:
                case DeviceType.Driving:
                case DeviceType.Flight:
                case DeviceType.FirstPerson:
                case DeviceType.Supplemental:
                    devType = SimpleDeviceType.Joystick;
                    break;
                case DeviceType.Device:
                case DeviceType.Mouse:
                case DeviceType.ControlDevice:
                case DeviceType.ScreenPointer:
                case DeviceType.Remote:
                default:
                    break;
                    //Not supported device types
            }
            return devType;
        }
        private static IEnumerable<Guid> GetUniqueDeviceGuids()
        {
            return _mappings
               .Where(mapping => mapping.GameActions != null && mapping.GameActions.Count != 0)
               .SelectMany(mapping => mapping.GameActions)
               .Select(gameAction => gameAction.ControllerInstanceGuid)
               .Distinct();
        }
        public static T GetDevice<T>(Guid guid) where T : Device
        {
            return _devices.Single(d => d.Key is T && d.Key.Information.InstanceGuid == guid).Key as T;
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
        //Got this from VTOLVRPhysicalInput mod : https://github.com/solidshadow1126/VTOLVRPhysicalInput
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
