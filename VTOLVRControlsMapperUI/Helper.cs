using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using VTOLVRControlsMapper;
using VTOLVRControlsMapper.Core;
using VTOLVRControlsMapperUI.BindingItem;
using VTOLVRControlsMapperUI.GridItem;
using MenuItem = VTOLVRControlsMapperUI.GridItem.MenuItem;

namespace VTOLVRControlsMapperUI
{
    public static class Helper
    {
        public static async Task<string> StartAsyncListening(Guid deviceID, bool isAxis = false)
        {
            return await Task<string>.Factory.StartNew(() =>
            {
                using DirectInput di = new DirectInput();
                IList<DeviceInstance> deviceInstances = di.GetDevices();
                DeviceInstance instance = deviceInstances.First(d => d.InstanceGuid == deviceID);
                string result = string.Empty;
                switch (ControlsHelper.GetDeviceType(instance))
                {
                    case SimpleDeviceType.Keyboard:
                        Keyboard keyboard = new Keyboard(di);
                        result = GetDeviceInput<Keyboard, KeyboardState, RawKeyboardState, KeyboardUpdate>(keyboard);
                        break;
                    case SimpleDeviceType.Joystick:
                        Joystick joystick = new Joystick(di, instance.InstanceGuid);
                        result = GetDeviceInput<Joystick, JoystickState, RawJoystickState, JoystickUpdate>(joystick, isAxis);
                        break;
                    case SimpleDeviceType.None:
                    default:
                        break;
                }
                return result;
            });
        }
        public static async void EditBinding(TabControl devicesTab, BaseItem item, TextBlock waitMessage, Rectangle waitMessageRectangle)
        {
            //Listen to device
            waitMessage.Visibility = Visibility.Visible;
            waitMessageRectangle.Visibility = Visibility.Visible;

            if (devicesTab.SelectedItem is GenericBindingItem genericBindingItem && item is ActionItem actionItem)
            {
                string newControlName = await StartAsyncListening(genericBindingItem.Device.ID);
                actionItem.Action.ControllerButtonName = newControlName;
                actionItem.Action.ControllerInstanceGuid = genericBindingItem.Device.ID;
            }
            else if (devicesTab.SelectedItem is ThrottleBindingItem throttleBindingItem && item is JoystickItem joystickItem)
            {
                bool isAxis = joystickItem is AxisItem;
                string newControlName = await StartAsyncListening(throttleBindingItem.Device.ID, isAxis);
                joystickItem.ControlName = newControlName;
            }
            devicesTab.Items.Refresh();

            waitMessage.Visibility = Visibility.Hidden;
            waitMessageRectangle.Visibility = Visibility.Hidden;
        }
        public static void ClearBinding(TabControl devicesTab, BaseItem item)
        {
            if (item is BaseItem actionItem)
            {
                actionItem.ControlName = string.Empty;
                devicesTab.Items.Refresh();
            }
        }
        private static string GetDeviceInput<DevType, StateType, RawStateType, UpdateType>(DevType device, bool isAxis = false)
            where DevType : CustomDevice<StateType, RawStateType, UpdateType>
            where StateType : class, IDeviceState<RawStateType, UpdateType>, new()
            where RawStateType : struct
            where UpdateType : struct, IStateUpdate
        {
            ControlsHelper.AcquireDevice(device);
            UpdateType[] updates = null;
            static bool loopPredicate(UpdateType[] u)
            {
                return u != null && u.Count() > 0;
            }
            string returnValue = string.Empty;
            bool updateCurrentState = true;
            StateType state = null;
            int axisListeningRange = 65535 / 4;
            while (!loopPredicate(updates) || string.IsNullOrEmpty(returnValue))
            {
                updates = device.GetBufferedData();
                if (loopPredicate(updates))
                {
                    foreach (UpdateType update in updates)
                    {
                        Type updateType = update.GetType();
                        if (update is KeyboardUpdate)
                        {
                            KeyboardUpdate? kbUpdate = update as KeyboardUpdate?;
                            if (kbUpdate.Value.IsReleased)
                            {
                                returnValue = kbUpdate.Value.Key.ToString();
                            }
                        }
                        else if (update is JoystickUpdate)
                        {
                            JoystickUpdate? joystickUpdate = update as JoystickUpdate?;

                            //When it's a button and value is equal 0 it means a button was pushed (value goes up do 128) and released (values comes back down to 0)
                            if (!isAxis && joystickUpdate.Value.Offset.ToString().StartsWith("Buttons") && joystickUpdate.Value.Value == 0)
                            {
                                returnValue = joystickUpdate.Value.Offset.ToString();
                            }
                            //When dealing with an axis and value is -1000 or +1000 around the previous of the state device catch the offset name
                            if (isAxis && !joystickUpdate.Value.Offset.ToString().StartsWith("Buttons"))
                            {
                                if (updateCurrentState)
                                {
                                    state = device.GetCurrentState();
                                    updateCurrentState = false;
                                }

                                string offsetName = joystickUpdate.Value.Offset.ToString();
                                string lastChar = offsetName.Last().ToString();
                                int updatedOffsetValue = joystickUpdate.Value.Value;
                                int currentOffsetValue = 0;
                                if (int.TryParse(lastChar, out int index))
                                {
                                    string arrayOffsetName = offsetName[0..^1];
                                    int[] offsetArray = state.GetType().GetProperty(arrayOffsetName).GetValue(state) as int[];
                                    currentOffsetValue = offsetArray[index];
                                }
                                else
                                {
                                    currentOffsetValue = (int)state.GetType().GetProperty(offsetName).GetValue(state);
                                }
                                if (currentOffsetValue - axisListeningRange > updatedOffsetValue || currentOffsetValue + axisListeningRange < updatedOffsetValue)
                                {
                                    returnValue = joystickUpdate.Value.Offset.ToString();
                                }
                            }
                        }
                    }
                }
            }
            device.Unacquire();
            return returnValue;
        }
    }
}
