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
        private static DirectInput _directInput;
        private static List<DeviceInstance> _deviceInstances;
        private static Dictionary<string, object> _customControlCache;
        private static List<UnityEngine.Object> _unityObjects = new List<UnityEngine.Object>();
        private static Dictionary<Guid, KeyboardUpdate[]> _keyboardUpdates;
        private static Dictionary<Guid, JoystickUpdate[]> _joystickUpdates;
        private static Dictionary<Guid, JoystickState> _joystickStates;
        private static Dictionary<Guid, KeyboardState> _keyboardStates;
        public static List<ControlMapping> Mappings { get; private set; }
        public static Dictionary<Device, SimpleDeviceType> Devices { get; private set; }

        public static bool MappingsLoaded
        {
            get
            {
                return Mappings != null && Mappings.Count > 0 &&
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
            Main.Instance.StopCoroutine(nameof(GenericRoutine));
            Main.Instance.StopCoroutine(nameof(AxisRoutine));
            Main.Instance.StopCoroutine(nameof(MenuRoutine));
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
                _ = Directory.CreateDirectory(directoryName);
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
        public static JsonSerializerSettings GetJSONSerializerSettings()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            };
            return settings;
        }
        public static string GetMappingFilePath(string settingsFileFolder, string modName, string vehicleName)
        {
            string mappingFilePath = Path.Combine(GetModFilePath(settingsFileFolder, modName), string.Format(@"mapping.{0}.json", vehicleName));
            return mappingFilePath;
        }
        public static string[] GetMappingFiles(string settingsFileFolder, string modName)
        {
            string modFolder = GetModFilePath(settingsFileFolder, modName);
            string[] files = Directory.GetFiles(modFolder, "mapping.*.json");
            return files;
        }
        public static string GetModFilePath(string folder, string modName)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), folder, modName);
        }
        private static List<ControlMapping> GetMappingsFromFile(string mappingFilePath)
        {
            string jsonContent;
            using (FileStream fs = File.OpenRead(mappingFilePath))
            {
                using (StreamReader sr = new StreamReader(fs))
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
            Mappings = GetMappingsFromFile(mappingFilePath);
        }
        public static void LoadMappingInstances()
        {
            _customControlCache = new Dictionary<string, object>();
            foreach (ControlMapping mapping in Mappings)
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
                    Main.Instance.Log("Cannot create a custom control isntance for : " + mapping.GameControlName);
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
                Main.Instance.StartCoroutine(PollRoutine(controllerGuid));
            }
            StartGenericGameActionRoutines();
            StartJoystickActionRoutines();
        }
        private static void StartJoystickActionRoutines()
        {
            dynamic mappings = GetGameActions<JoystickAction>();
            foreach (dynamic mapping in mappings)
            {
                //Get custom control instance and methods
                object instance = _customControlCache[mapping.GameControlName];
                MethodInfo mainAxisMethodInfo = GetExecuteMethod(instance, nameof(ControlJoystick<MonoBehaviour>.UpdateMainAxis));
                MethodInfo thumbstickAxisMethodInfo = GetExecuteMethod(instance, nameof(ControlJoystick<MonoBehaviour>.UpdateThumbstickAxis));
                MethodInfo triggerAxisMethodInfo = GetExecuteMethod(instance, nameof(ControlJoystick<MonoBehaviour>.UpdateTriggerAxis));
                MethodInfo menuPressedMethodInfo = GetExecuteMethod(instance, nameof(ControlJoystick<MonoBehaviour>.PressMenu));
                MethodInfo menuReleasedMethodInfo = GetExecuteMethod(instance, nameof(ControlJoystick<MonoBehaviour>.ReleaseMenu));
                MethodInfo triggerPressedMethodInfo = GetExecuteMethod(instance, nameof(ControlJoystick<MonoBehaviour>.PressTriggerButton));
                MethodInfo triggerReleasedMethodInfo = GetExecuteMethod(instance, nameof(ControlJoystick<MonoBehaviour>.ReleaseTriggerButton));

                JoystickAction action = mapping.Action as JoystickAction;
                Guid controllerGuid = action.ControllerInstanceGuid;

                //Start routine for each axis : Roll, pitch, yaw, thumbstick
                if (mapping.Action is StickAction)
                {
                    StickAction stickAction = mapping.Action;
                    _ = Main.Instance.StartCoroutine(AxisRoutine(controllerGuid, instance, mainAxisMethodInfo, stickAction.Pitch, stickAction.Yaw, stickAction.Roll));
                }
                else if (mapping.Action is ThrottleAction)
                {
                    ThrottleAction throttleAction = mapping.Action;
                    _ = Main.Instance.StartCoroutine(AxisRoutine(controllerGuid, instance, mainAxisMethodInfo, throttleAction.Power));
                }
                if (action.Thumbstick != null)
                {
                    _ = Main.Instance.StartCoroutine(AxisRoutine(controllerGuid, instance, thumbstickAxisMethodInfo, mapping.GameControlName, action.Thumbstick.X, action.Thumbstick.Y));
                }

                //Start a routine for menu button
                Func<JoystickUpdate, bool> menuPressedPredicate = GetJoystickPressedPredicate(action.Menu);
                Func<JoystickState, bool> menuReleasedPredicate = GetJoystickReleasedPredicate(action.Menu);
                _ = Main.Instance.StartCoroutine(
                    MenuRoutine(controllerGuid, instance, menuPressedMethodInfo, menuReleasedMethodInfo, menuPressedPredicate, menuReleasedPredicate, action.Menu));

                //Start a routine for trigger axis (brake) and trigger button (fire)
                Func<JoystickUpdate, bool> triggerPressedPredicate = GetJoystickPressedPredicate(action.TriggerButton);
                Func<JoystickState, bool> triggerReleasedPredicate = GetJoystickReleasedPredicate(action.TriggerButton);
                _ = Main.Instance.StartCoroutine(
                    MenuRoutine(controllerGuid, instance, triggerPressedMethodInfo, triggerReleasedMethodInfo, triggerPressedPredicate, triggerReleasedPredicate, action.TriggerButton));
                _ = Main.Instance.StartCoroutine(AxisRoutine(controllerGuid, instance, triggerAxisMethodInfo, action.TriggerAxis));
            }
        }
        private static void StartGenericGameActionRoutines()
        {
            dynamic mappings = GetGameActions<GenericGameAction>();
            foreach (dynamic mapping in mappings)
            {
                GenericGameAction action = mapping.Action;
                Guid controllerInstanceGuid = action.ControllerInstanceGuid;
                DeviceInstance deviceInstance = GetDeviceInstance(controllerInstanceGuid);
                string controllerButtonName = action.ControllerButtonName;
                object instance = _customControlCache[mapping.GameControlName];
                MethodInfo methodInfo = GetExecuteMethod(instance.GetType(), action.ControllerActionBehavior);
                SimpleDeviceType deviceType = GetDeviceType(deviceInstance);

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
                        Func<KeyboardState, bool> kbReleaseStatePredicate = (state) =>
                        {
                            return !state.IsPressed((Key)Enum.Parse(typeof(Key), controllerButtonName, true));
                        };
                        _ = Main.Instance.StartCoroutine(GenericRoutine<KeyboardUpdate, KeyboardState, RawKeyboardState>(
                            _keyboardUpdates, _keyboardStates, instance, methodInfo, action, kbUpdatePredicate, kbReleaseStatePredicate
                        ));
                        break;
                    case SimpleDeviceType.Joystick:
                        //Detect when button is pressed
                        Func<JoystickUpdate, bool> joyUpdatePredicate = GetJoystickPressedPredicate(controllerButtonName);
                        //Detect when button is released
                        Func<JoystickState, bool> joyReleaseStatePredicate = GetJoystickReleasedPredicate(controllerButtonName);
                        _ = Main.Instance.StartCoroutine(GenericRoutine(
                            _joystickUpdates, _joystickStates, instance, methodInfo, mapping.Action, joyUpdatePredicate, joyReleaseStatePredicate
                        ));
                        break;
                    case SimpleDeviceType.None:
                    default:
                        break;
                }
            }
        }
        public static Func<JoystickUpdate, bool> GetJoystickPressedPredicate(string buttonName)
        {
            bool joystickPressedPredicate(JoystickUpdate update)
            {
                return update.Offset.ToString() == buttonName && update.Value == 128;
            }
            return joystickPressedPredicate;
        }
        public static Func<JoystickState, bool> GetJoystickReleasedPredicate(string buttonName)
        {
            bool joystickReleasedPredicate(JoystickState state)
            {
                int offsetValue = GetOffsetValue(buttonName, state);
                return offsetValue == 0;
            }
            return joystickReleasedPredicate;
        }
        private static DeviceInstance GetDeviceInstance(Guid controllerInstanceGuid)
        {
            DeviceInstance instance = _deviceInstances.Find(d => d.InstanceGuid == controllerInstanceGuid);
            return instance;
        }
        public static Dictionary<Guid, string> GetAvailableDevices()
        {
            Dictionary<Guid, string> availableDevices = new Dictionary<Guid, string>();
            //Get list of available devices
            using (DirectInput di = new DirectInput())
            {
                IList<DeviceInstance> deviceInstances = di.GetDevices();

                foreach (DeviceInstance deviceInstance in deviceInstances)
                {
                    switch (GetDeviceType(deviceInstance))
                    {
                        case SimpleDeviceType.Keyboard:
                        case SimpleDeviceType.Joystick:
                            availableDevices.Add(deviceInstance.InstanceGuid, deviceInstance.InstanceName);
                            break;
                        case SimpleDeviceType.None:
                        default:
                            break;
                    }
                }
            }
            return availableDevices;
        }
        public static dynamic GetGameActions<T>() where T : GameAction
        {
            var retValue = Mappings
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
        private static IEnumerator GenericRoutine<UpdateType, StateType, RawStateType>(
            Dictionary<Guid, UpdateType[]> updates,
            Dictionary<Guid, StateType> states,
            object instance,
            MethodInfo methodInfo,
            GenericGameAction action,
            Func<UpdateType, bool> updatePredicate,
            Func<StateType, bool> releasePredicate)

            where StateType : class, IDeviceState<RawStateType, UpdateType>, new()
            where RawStateType : struct
            where UpdateType : struct, IStateUpdate
        {
            while (true)
            {
                if (updates != null &&
                    updates[action.ControllerInstanceGuid] != null &&
                    updates[action.ControllerInstanceGuid].Where(updatePredicate).Count() > 0)
                {
                    yield return methodInfo.Invoke(instance, null);
                    if (action.ControllerActionBehavior == ControllerActionBehavior.Hold)
                    {
                        StateType state = states[action.ControllerInstanceGuid];
                        MethodInfo offMethodInfo = GetExecuteMethod(instance.GetType(), ControllerActionBehavior.HoldOff);
                        yield return new WaitUntil(() => releasePredicate(state));
                        yield return offMethodInfo.Invoke(instance, null);
                    }
                }
                yield return null;
            }
        }
        private static IEnumerator AxisRoutine(Guid controllerGuid, object instance, MethodInfo methodInfo, Axis X, Axis Y = null, Axis Z = null)
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
                        //Get axis values from update or state
                        float xValue = GetAxisValue(controllerGuid, X);
                        float yValue = GetAxisValue(controllerGuid, Y);
                        float zValue = GetAxisValue(controllerGuid, Z);

                        Vector3 vector = new Vector3(xValue, yValue, zValue);
                        _ = methodInfo.Invoke(instance, new object[] { vector });
                    }
                    yield return null;
                }
            }
        }
        private static IEnumerator MenuRoutine(
            Guid controllerInstanceGuid,
            object instance,
            MethodInfo pressedMethodInfo,
            MethodInfo releaseMethodInfo,
            Func<JoystickUpdate, bool> pressedPredicate,
            Func<JoystickState, bool> releasedPredicate,
            string menu)
        {
            if (!string.IsNullOrEmpty(menu))
            {
                while (true)
                {
                    //TODO handle keyboard too
                    if (ShouldExecuteJoystick(pressedPredicate, controllerInstanceGuid))
                    {
                        _ = pressedMethodInfo.Invoke(instance, null);
                        JoystickState state = _joystickStates[controllerInstanceGuid];
                        yield return new WaitUntil(() => releasedPredicate(state));
                        _ = releaseMethodInfo.Invoke(instance, null);
                    }
                    yield return null;
                }
            }
        }
        private static float GetAxisValue(Guid controllerGuid, Axis axis)
        {
            if (axis != null && _joystickStates != null && _joystickUpdates != null)
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
                else if (currentState != null)
                {
                    Type joystickStateType = typeof(JoystickState);
                    int offsetValue = GetOffsetValue(axis.Name, currentState);
                    return ConvertAxisValue(offsetValue, axis.Invert, axis.MappingRange);
                }
            }
            return 0.0f;
        }
        public static int GetOffsetValue(string offsetName, object state)
        {
            string lastChar = offsetName.Last().ToString();
            int currentOffsetValue;
            if (int.TryParse(lastChar, out int index))
            {
                string arrayOffsetName = offsetName.Substring(0, offsetName.Length - 1);
                int[] offsetArray = state.GetType().GetProperty(arrayOffsetName).GetValue(state) as int[];
                currentOffsetValue = offsetArray[index];
            }
            else
            {
                currentOffsetValue = (int)state.GetType().GetProperty(offsetName).GetValue(state);
            }
            return currentOffsetValue;
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
            Devices = new Dictionary<Device, SimpleDeviceType>();

            _deviceInstances = devices.ToList();
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
                    Devices.Add(device, deviceType);
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
            return Mappings
               .Where(mapping => mapping.GameActions != null && mapping.GameActions.Count != 0)
               .SelectMany(mapping => mapping.GameActions)
               .Select(gameAction => gameAction.ControllerInstanceGuid)
               .Distinct();
        }
        public static T GetDevice<T>(Guid guid) where T : Device
        {
            try
            {
                return Devices.Single(d => d.Key is T && d.Key.Information.InstanceGuid == guid).Key as T;
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
            try
            {
                Assembly assembly = Assembly.GetAssembly(typeof(T));
                Type derivedType = typeof(T);
                return assembly
                    .GetTypes()
                    .Where(t =>
                        t != derivedType &&
                        t.IsClass &&
                        !t.IsAbstract &&
                        derivedType.IsAssignableFrom(t))
                    .ToList();
            }
            catch (ReflectionTypeLoadException ex)
            {
                foreach (Exception inner in ex.LoaderExceptions)
                {
                    Main.Instance.Log(inner);
                }
                throw ex;
            }
        }
        //Got this from VTOLVRPhysicalInput mod : https://github.com/solidshadow1126/VTOLVRPhysicalInput
        private static float ConvertAxisValue(int value, bool invert, MappingRange mappingRange = MappingRange.Full)
        {
            float retVal = value == 65535 ? 1 : ((float)value / 32767) - 1;
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
