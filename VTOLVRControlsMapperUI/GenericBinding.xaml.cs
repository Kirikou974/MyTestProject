using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using VTOLVRControlsMapper;
using VTOLVRControlsMapper.Core;
using VTOLVRControlsMapperUI.CustomItem;

namespace VTOLVRControlsMapperUI
{
    /// <summary>
    /// Logique d'interaction pour Binding.xaml
    /// </summary>
    public partial class GenericBinding : Window
    {
        private readonly string controlName;
        public List<BindingItem> BindingItems;
        public GenericBinding(Window sender, ControlMapping mapping, List<DeviceItem> availableDevices, List<ControllerActionBehavior> supportedBehaviors)
        {
            InitializeComponent();

            Owner = sender;
            controlName = mapping.GameControlName;
            Title = controlName;

            LoadDevicesTab(mapping, availableDevices, supportedBehaviors);
        }
        private void LoadDevicesTab(ControlMapping mapping, List<DeviceItem> availableDevices, List<ControllerActionBehavior> supportedBehaviors)
        {
            BindingItems = new List<BindingItem>();
            foreach (DeviceItem deviceItem in availableDevices)
            {
                List<ActionItem> actionItems = new List<ActionItem>();
                foreach (ControllerActionBehavior behavior in supportedBehaviors)
                {
                    ControllerActionBehavior currentBehavior = behavior;
                    ActionItem actionItem = new ActionItem(behavior);
                    actionItem.Enabled = true;
                    if (behavior == ControllerActionBehavior.HoldOff)
                    {
                        actionItem.Enabled = false;
                        currentBehavior = ControllerActionBehavior.HoldOn;
                    }
                    if (mapping.GameActions != null &&
                        mapping.GameActions.Find(g =>
                            g != null &&
                            g is GenericGameAction &&
                            (g as GenericGameAction).ControllerActionBehavior == currentBehavior &&
                            g.ControllerInstanceGuid == deviceItem.ID) is GenericGameAction gameAction)
                    {
                        actionItem.ControlName = gameAction.ControllerButtonName;
                    }
                    actionItems.Add(actionItem);
                }
                BindingItems.Add(new BindingItem { Actions = actionItems, Device = deviceItem });
                DevicesTab.ItemsSource = BindingItems;
            }
        }
        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            //Listen to device
            WaitMessage.Visibility = Visibility.Visible;
            WaitMessageRectangle.Visibility = Visibility.Visible;

            if (DevicesTab.SelectedItem is BindingItem bindingItem && ((FrameworkElement)sender).DataContext is ActionItem actionItem)
            {
                string newControlName = await StartAsyncListening(bindingItem.Device.ID);
                BindingItem bindingItemToModify = BindingItems[BindingItems.FindIndex(b => b.Device == bindingItem.Device)];
                ActionItem actionItemToModify = bindingItemToModify.Actions.Find(a => a.Behavior == actionItem.Behavior);
                if (actionItem.Behavior == ControllerActionBehavior.HoldOn)
                {
                    ActionItem actionItemToModifyOff = bindingItemToModify.Actions.Find(a => a.Behavior == ControllerActionBehavior.HoldOff);
                    actionItemToModifyOff.ControlName = newControlName;
                }
                actionItemToModify.ControlName = newControlName;

                DevicesTab.Items.Refresh();
            }

            WaitMessage.Visibility = Visibility.Hidden;
            WaitMessageRectangle.Visibility = Visibility.Hidden;
        }
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            if (DevicesTab.SelectedItem is BindingItem bindingItem && ((FrameworkElement)sender).DataContext is ActionItem actionItem)
            {
                BindingItem bindingItemToModify = BindingItems[BindingItems.FindIndex(b => b.Device == bindingItem.Device)];
                ActionItem actionItemToClear = bindingItemToModify.Actions.Find(a => a.Behavior == actionItem.Behavior);
                actionItemToClear.ControlName = string.Empty;
                if (actionItem.Behavior == ControllerActionBehavior.HoldOn)
                {
                    ActionItem actionItemToClearOff = bindingItemToModify.Actions.Find(a => a.Behavior == ControllerActionBehavior.HoldOff);
                    actionItemToClearOff.ControlName = string.Empty;
                }
                DevicesTab.Items.Refresh();
            }
        }
        private async Task<string> StartAsyncListening(Guid deviceID)
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
                        //TODO listen only buttons or axis
                        Joystick joystick = new Joystick(di, instance.InstanceGuid);
                        result = GetDeviceInput<Joystick, JoystickState, RawJoystickState, JoystickUpdate>(joystick);
                        break;
                    case SimpleDeviceType.None:
                    default:
                        break;
                }
                return result;
            });
        }
        private string GetDeviceInput<DevType, StateType, RawStateType, UpdateType>(DevType device)
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
                            returnValue = kbUpdate.Value.Key.ToString();
                        }
                        else if (update is JoystickUpdate)
                        {
                            JoystickUpdate? joystickUpdate = update as JoystickUpdate?;
                            if (joystickUpdate.Value.Offset.ToString().StartsWith("Buttons"))
                            {
                                returnValue = joystickUpdate.Value.Offset.ToString();
                            }
                        }
                    }
                }
            }
            device.Unacquire();
            return returnValue;
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
