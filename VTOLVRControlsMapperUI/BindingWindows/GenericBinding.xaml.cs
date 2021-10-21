using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using VTOLVRControlsMapper;
using VTOLVRControlsMapper.Core;
using VTOLVRControlsMapperUI.BindingItem;
using VTOLVRControlsMapperUI.GridItem;

namespace VTOLVRControlsMapperUI.BindingWindows
{
    /// <summary>
    /// Logique d'interaction pour Binding.xaml
    /// </summary>
    public partial class GenericBinding
    {
        public List<IBindingItem> BindingItems;
        public GenericBinding(Window sender, ControlMapping mapping)
        {
            InitializeComponent();

            Owner = sender;
            Title = mapping.GameControlName;

            LoadDevicesTab(mapping);
        }
        private void LoadDevicesTab(ControlMapping mapping)
        {
            Dictionary<Guid, string> availableDevices = ControlsHelper.GetAvailableDevices();
            List<ControllerActionBehavior> supportedBehaviors = GetSupportedBehaviors(mapping);
            BindingItems = new List<IBindingItem>();

            foreach (Guid deviceID in availableDevices.Keys)
            {
                DeviceItem deviceItem = new DeviceItem(deviceID, availableDevices[deviceID]);
                List<ActionItem> actionItems = new List<ActionItem>();
                foreach (ControllerActionBehavior behavior in supportedBehaviors)
                {
                    actionItems.Add(GetActionItem(mapping, behavior, deviceID));
                }
                BindingItems.Add(new GenericBindingItem { Actions = actionItems, Device = deviceItem });
                DevicesTab.ItemsSource = BindingItems;
            }
        }
        private ActionItem GetActionItem(ControlMapping mapping, ControllerActionBehavior behavior, Guid deviceID)
        {
            ActionItem actionItem = new ActionItem(behavior);
            if (behavior == ControllerActionBehavior.HoldOff)
            {
                actionItem.Visible = false;
            }
            if (mapping.GameActions != null &&
                mapping.GameActions.Find(g =>
                g != null &&
                g is GenericGameAction &&
                (g as GenericGameAction).ControllerActionBehavior == behavior &&
                g.ControllerInstanceGuid == deviceID) is GenericGameAction gameAction)
            {
                actionItem.Action = gameAction;
            }
            return actionItem;
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
        private List<ControllerActionBehavior> GetSupportedBehaviors(ControlMapping mapping)
        {
            //Get the list of possible actions (On, Off etc...)
            Type customControlType = ControlsHelper.GetCustomControlType(mapping.Types);
            MethodInfo[] methodsInfo = ControlsHelper.GetExecuteMethods(customControlType);
            List<ControllerActionBehavior> returnList = new List<ControllerActionBehavior>();
            foreach (MethodInfo methodInfo in methodsInfo)
            {
                ControlMethodAttribute attribute = methodInfo.GetCustomAttribute<ControlMethodAttribute>();
                returnList.Add(attribute.SupportedBehavior);
            }
            return returnList;
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
