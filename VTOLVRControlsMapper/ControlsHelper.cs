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
        static Type _joystickStateType = typeof(JoystickState);
        public static List<ControlMapping> Mappings => _mappings;
        public static Dictionary<Device, SimpleDeviceType> Devices => _devices;

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
                            List<Type> types = new List<Type>();
                            foreach (UnityEngine.Object unityObject in linkedObjects)
                            {
                                types.Add(unityObject.GetType());
                            }
                            ControlMapping controlMapping = new ControlMapping(vrInteractable.name, types);
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
        public static void CreateMappingFileFromMappings(string mappingFilePath)
        {
            //TODO implement
        }
        private static JsonSerializerSettings GetJSONSerializerSettings()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            };
            return settings;
        }
        public static string GetMappingFilePath(string settingsFileFolder, string modName, string vehicleName)
        {
            string mappingFilePath = Path.Combine(Directory.GetCurrentDirectory(), settingsFileFolder, modName, string.Format(@"mapping.{0}.json", vehicleName));
            return mappingFilePath;
        }
        public static string[] GetMappingFiles(string settingsFileFolder, string modName)
        {
            string modFolder = Path.Combine(Directory.GetCurrentDirectory(), settingsFileFolder, modName);
            string[] files = Directory.GetFiles(modFolder, "mapping.*.json");
            return files;
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
        public static void LoadMappings(string settingsFileFolder, string modName, string vehicleName)
        {
            string mappingFilePath = GetMappingFilePath(settingsFileFolder, modName, vehicleName);
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
                Type customControlType = GetCustomControlType(mapping.Types);

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
        public static Type GetCustomControlType(List<Type> types)
        {
            IEnumerable<Type> controlTypes = GetDerivedTypes<IControl>();
            Type customControlType = controlTypes.FirstOrDefault(t =>
                t.GetCustomAttribute<ControlClassAttribute>() != null &&
                t.GetCustomAttribute<ControlClassAttribute>().UnityTypes.SequenceEqual(types)
            );
            return customControlType;
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
            StartJoystickActionRoutines();
        }
        private static void StartJoystickActionRoutines()
        {
            var mappings = GetGameActions<JoystickAction>();
            foreach (var mapping in mappings)
            {
                JoystickAction action = mapping.Action as JoystickAction;
                Guid controllerGuid = action.ControllerInstanceGuid;
                if (mapping.Action is StickAction)
                {
                    StickAction stickAction = mapping.Action;
                    Main.instance.StartCoroutine(AxisRoutine(controllerGuid, nameof(Stick.UpdateMainAxis), mapping.GameControlName, stickAction.Pitch, stickAction.Roll, stickAction.Yaw));
                }
                else if (mapping.Action is ThrottleAction)
                {
                    ThrottleAction throttleAction = mapping.Action;
                    Main.instance.StartCoroutine(AxisRoutine(controllerGuid, nameof(Throttle.UpdateMainAxis), mapping.GameControlName, throttleAction.Power));
                }
                if (action.Thumbstick != null)
                {
                    Main.instance.StartCoroutine(AxisRoutine(controllerGuid, nameof(Stick.UpdateThumbstickAxis), mapping.GameControlName, action.Thumbstick.X, action.Thumbstick.Y));
                }
                Main.instance.StartCoroutine(MenuRoutine(controllerGuid, mapping.GameControlName, action.Menu));
                Main.instance.StartCoroutine(AxisRoutine(controllerGuid, nameof(Stick.UpdateTriggerAxis), mapping.GameControlName, action.Trigger));
            }
        }
        private static void StartGenericGameActionRoutines()
        {
            var mappings = GetGameActions<GenericGameAction>();
            foreach (var mapping in mappings)
            {
                GenericGameAction action = mapping.Action;
                Guid controllerInstanceGuid = action.ControllerInstanceGuid;
                DeviceInstance instance = GetDeviceInstance(controllerInstanceGuid);
                string controllerButtonName = action.ControllerButtonName;
                SimpleDeviceType deviceType = GetDeviceType(instance);
                switch (deviceType)
                {
                    case SimpleDeviceType.Keyboard:
                        //Detect when key is pressed
                        Func<KeyboardUpdate, bool> kbUpdatePredicate = (update) =>
                        {
                            return update.Key != Key.Unknown &&
                                update.Key.ToString() == controllerButtonName &&
                                update.IsPressed;
                        };
                        //Detect when key is released
                        Func<bool> kbReleaseStatePredicate = () =>
                        {
                            return !_keyboardStates[controllerInstanceGuid].IsPressed((Key)Enum.Parse(typeof(Key), controllerButtonName, true));
                        };
                        Main.instance.StartCoroutine(GenericRoutine(_keyboardUpdates, mapping.GameControlName, action,
                            kbUpdatePredicate, kbReleaseStatePredicate));
                        break;
                    case SimpleDeviceType.Joystick:
                        Func<JoystickUpdate, bool> joyUpdatePredicate = (update) =>
                        {
                            return update.Offset.ToString() == controllerButtonName && update.Value == 128;
                        };
                        Func<bool> joyReleaseStatePredicate = () =>
                        {
                            //TODO implement
                            //return state.PressedKeys.Where(k => k.ToString() == mapping.Action.ControllerButtonName).Count() == 0;
                            return false;
                        };
                        Main.instance.StartCoroutine(GenericRoutine(_joystickUpdates, mapping.GameControlName, mapping.Action,
                            joyUpdatePredicate, joyReleaseStatePredicate));
                        break;
                    case SimpleDeviceType.None:
                    default:
                        break;
                }
            }
        }

        private static DeviceInstance GetDeviceInstance(Guid controllerInstanceGuid)
        {
            DeviceInstance instance = _deviceInstances.Find(d => d.InstanceGuid == controllerInstanceGuid);
            return instance;
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
                DeviceInstance instance = GetDeviceInstance(controllerGuid);
                SimpleDeviceType devType = GetDeviceType(instance);
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
        private static IEnumerator GenericRoutine<Update>(
            Dictionary<Guid, Update[]> updates,
            string gameControlName,
            GenericGameAction action,
            Func<Update, bool> updatePredicate,
            Func<bool> releasePredicate)
            where Update : IStateUpdate
        {
            while (true)
            {
                if (updates != null &&
                    updates[action.ControllerInstanceGuid] != null &&
                    updates[action.ControllerInstanceGuid].Where(updatePredicate).Count() > 0)
                {
                    Update update = updates[action.ControllerInstanceGuid].First(updatePredicate);
                    object instance = _customControlCache[gameControlName];
                    MethodInfo methodInfo = GetExecuteMethod(instance.GetType(), action.ControllerActionBehavior);
                    yield return methodInfo.Invoke(instance, null);
                    if (action.ControllerActionBehavior == ControllerActionBehavior.HoldOn)
                    {
                        MethodInfo offMethodInfo = GetExecuteMethod(instance.GetType(), ControllerActionBehavior.HoldOff);
                        yield return new WaitUntil(releasePredicate);
                        yield return offMethodInfo.Invoke(instance, null);
                    }
                }
                yield return null;
            }
        }
        private static IEnumerator AxisRoutine(Guid controllerGuid, string methodName, string gameControlName, Axis X, Axis Y = null, Axis Z = null)
        {
            //If first axis is null don't start the routine
            if (X != null)
            {
                while (true)
                {
                    if (_joystickUpdates != null &&
                        _joystickUpdates[controllerGuid] != null &&
                        _joystickStates != null &&
                        _joystickStates[controllerGuid] != null)
                    {
                        //Get custom control instance and methods
                        object instance = _customControlCache[gameControlName];
                        MethodInfo methodInfo = GetExecuteMethod(instance, methodName);
                        PropertyInfo vectorProperty = instance.GetType().GetProperty(nameof(ControlJoystick<MonoBehaviour>.VectorUpdate));

                        //Get axis values from update or state
                        float xValue = GetAxisValue(controllerGuid, X);
                        float yValue = GetAxisValue(controllerGuid, Y);
                        float zValue = GetAxisValue(controllerGuid, Z);

                        //If axis values didn't change do not update
                        Vector3 vector = new Vector3(xValue, yValue, zValue);
                        Vector3 currentVector = (Vector3)vectorProperty.GetValue(instance);
                        if (currentVector != vector)
                        {
                            methodInfo.Invoke(instance, new object[] { vector });
                        }
                    }
                    yield return null;
                }
            }
        }
        private static float GetAxisValue(Guid controllerGuid, Axis axis)
        {
            if (axis != null)
            {
                bool predicate(JoystickUpdate update)
                {
                    return update.Offset.ToString() == axis.Name;
                }
                JoystickState currentState = _joystickStates[controllerGuid];
                if (ShouldExecuteJoystick(predicate, controllerGuid))
                {
                    JoystickUpdate update = _joystickUpdates[controllerGuid].First(predicate);
                    return ConvertAxisValue(update.Value, axis.Invert, axis.MappingRange);
                }
                else
                {
                    return ConvertAxisValue((int)_joystickStateType.GetProperty(axis.Name).GetValue(currentState), axis.Invert, axis.MappingRange);
                }
            }
            else
            {
                return 0.0f;
            }
        }
        private static bool TryGetPredicate(Axis axis, out Func<JoystickUpdate, bool> predicate)
        {
            if (axis != null)
            {
                bool retpredicate(JoystickUpdate update)
                {
                    return update.Offset.ToString() == axis.Name;
                }
                predicate = retpredicate;
                return true;
            }
            predicate = null;
            return false;
        }
        private static IEnumerator MenuRoutine(Guid controllerInstanceGuid, string gameControlName, string menu)
        {
            if (!string.IsNullOrEmpty(menu))
            {
                while (true)
                {
                    //Get custom control instance and methods
                    object instance = _customControlCache[gameControlName];
                    MethodInfo methodInfo = GetExecuteMethod(instance, nameof(ControlJoystick<MonoBehaviour>.ClickMenu));

                    bool predicate(JoystickUpdate update)
                    {
                        //128 = button pressed
                        return update.Offset.ToString() == menu && update.Value == 128;
                    }
                    if (ShouldExecuteJoystick(predicate, controllerInstanceGuid))
                    {
                        JoystickUpdate update = _joystickUpdates[controllerInstanceGuid].First(predicate);
                        methodInfo.Invoke(instance, null);
                    }
                    yield return null;
                }
            }
        }
        private static bool ShouldExecuteJoystick(Func<JoystickUpdate, bool> predicate, Guid controllerInstanceGuid)
        {
            return _joystickUpdates != null && _joystickUpdates[controllerInstanceGuid].Where(predicate).Count() > 0;
        }
        public static MethodInfo GetExecuteMethod(Type type, ControllerActionBehavior behavior)
        {
            List<MethodInfo> methodsInfo = type.GetMethods(BindingFlags.Public | BindingFlags.Instance).ToList();
            MethodInfo methodInfo = methodsInfo.Find(m =>
                m.GetCustomAttribute<ControlMethodAttribute>() != null &&
                m.GetCustomAttribute<ControlMethodAttribute>().SupportedBehavior == behavior
            );
            return methodInfo;
        }
        public static MethodInfo[] GetExecuteMethods(Type type)
        {
            IEnumerable<MethodInfo> methodsInfo = type
                .GetMethods(BindingFlags.Public | BindingFlags.Instance) 
                .Where(m => m.GetCustomAttribute<ControlMethodAttribute>() != null
            );
            return methodsInfo.ToArray();
        }
        private static MethodInfo GetExecuteMethod(object instance, string methodName)
        {
            MethodInfo methodInfo = instance.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);
            return methodInfo;
        }
        public static void LoadDevices()
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
                DeviceInstance instance = GetDeviceInstance(controllerGuid);
                SimpleDeviceType deviceType = GetDeviceType(instance);
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
                if (device != null)
                {
                    AcquireDevice(device);
                    _devices.Add(device, deviceType);
                }
            }
        }
        public static SimpleDeviceType GetDeviceType(DeviceInstance instance)
        {
            SimpleDeviceType devType = SimpleDeviceType.None;
            if (instance != null)
            {
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
            try
            {
                return _devices.Single(d => d.Key is T && d.Key.Information.InstanceGuid == guid).Key as T;
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }
        public static void AcquireDevice(Device device)
        {
            device.Properties.BufferSize = 128;
            device.Acquire();
        }
        public static void UnacquireDevice(Device device)
        {
            device.Unacquire();
        }
        #endregion
        private static List<Type> GetDerivedTypes<T>()
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
        private static float ConvertAxisValue(int value, bool invert, MappingRange mappingRange = MappingRange.Full)
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
