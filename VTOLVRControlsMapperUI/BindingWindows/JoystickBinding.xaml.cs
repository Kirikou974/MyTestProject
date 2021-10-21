using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using VTOLVRControlsMapper;
using VTOLVRControlsMapper.Core;
using VTOLVRControlsMapperUI.BindingItem;
using VTOLVRControlsMapperUI.GridItem;

namespace VTOLVRControlsMapperUI.BindingWindows
{
    /// <summary>
    /// Interaction logic for ThrottleBinding.xaml
    /// </summary>
    public partial class JoystickBinding : Window
    {
        public List<IBindingItem> BindingItems;
        public JoystickBinding(Window sender, ControlMapping mapping)
        {
            InitializeComponent();
            Owner = sender;
            Title = mapping.GameControlName;
            //LoadDevicesTab(mapping);
        }
        public void LoadDevicesTab<BindingType, ActionType>(ControlMapping mapping) 
            where BindingType : JoystickBindingItem
            where ActionType : JoystickAction
        {
            Dictionary<Guid, string> availableDevices = ControlsHelper.GetAvailableDevices();
            BindingItems = new List<IBindingItem>();
            foreach (Guid deviceID in availableDevices.Keys)
            {
                DeviceItem device = new DeviceItem(deviceID, availableDevices[deviceID]);
                BindingType bindingItem = (BindingType)Activator.CreateInstance(typeof(BindingType), device);
                if (mapping.GameActions != null &&
                    mapping.GameActions.Find(g =>
                        g != null &&
                        g is ActionType &&
                        g.ControllerInstanceGuid == deviceID) is ActionType action)
                {
                    bindingItem.GameAction = action;
                }
                BindingItems.Add(bindingItem);
            }
            DevicesTab.ItemsSource = BindingItems;
        }
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            BaseItem item = ((FrameworkElement)sender).DataContext as BaseItem;
            Helper.EditBinding(DevicesTab, item, WaitMessage, WaitMessageRectangle);
        }
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            BaseItem item = ((FrameworkElement)sender).DataContext as BaseItem;
            Helper.ClearBinding(DevicesTab, item);
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedItem is MappingRange mappingRange &&
                ((FrameworkElement)sender).DataContext is AxisItem item)
            {
                item.Range = mappingRange;
            }
        }
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            HandleCheckBox(sender);
        }
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            HandleCheckBox(sender);
        }
        private void HandleCheckBox(object sender)
        {
            if (((FrameworkElement)sender).DataContext is AxisItem item && sender is CheckBox checkBox)
            {
                item.Invert = checkBox.IsChecked.Value;
            }
        }
    }
}
