using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VTOLVRControlsMapper;
using VTOLVRControlsMapper.Core;

namespace VTOLVRControlsMapperUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool handle = true;
        private const string DEFAULT_TEXT_SEARCH = "Search control";
        public MainWindow()
        {
            InitializeComponent();
            LoadPlaneComboBox();
            LoadSearchBox();
        }
        private void LoadSearchBox()
        {
            SearchBox.IsEnabled = false;
            SearchBox.Text = DEFAULT_TEXT_SEARCH;
            SearchBox.TextChanged += SearchBox_TextChanged;
            SearchBox.GotFocus += SearchBox_GotFocus;
            SearchBox.LostFocus += SearchBox_LostFocus;
        }
        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(SearchBox.Text) || SearchBox.Text == DEFAULT_TEXT_SEARCH)
            {
                ResetSearchBox();
            }
        }
        private void ResetSearchBox()
        {
            SearchBox.Text = DEFAULT_TEXT_SEARCH;
            SearchBox.Foreground = Brushes.LightGray;
        }
        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchBox.Foreground = Brushes.Black;
            if (SearchBox.Text == DEFAULT_TEXT_SEARCH)
            {
                SearchBox.Text = string.Empty;
            }
        }
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SearchBox.Text != DEFAULT_TEXT_SEARCH)
            {
                UpdateDataSource(SearchBox.Text);
            }
            else
            {
                UpdateDataSource();
            }
        }
        private void LoadPlaneComboBox()
        {
            PlaneComboBox.SelectionChanged += PlaneComboBox_SelectionChanged;
            PlaneComboBox.DropDownClosed += PlaneComboBox_DropDownClosed;
            //TODO variable for path
            string[] files = ControlsHelper.GetMappingFiles("F:\\Steam\\SteamApps\\common\\VTOL VR\\VTOLVR_ModLoader\\mods\\", "VTOLVRControlsMapper");

            List<PlaneItem> planeList = new List<PlaneItem>();
            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                PlaneItem item = new PlaneItem
                {
                    Text = $"{ fileInfo.Name.Split(".")[1] } ({ fileInfo.Name })",
                    Value = file
                };
                planeList.Add(item);
            }
            PlaneComboBox.ItemsSource = planeList;
        }
        private void PlaneComboBox_DropDownClosed(object sender, EventArgs e)
        {
            if (handle && PlaneComboBox.SelectedItem != null)
            {
                LoadSelectedPlane();
            }
        }
        private void PlaneComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            handle = !comboBox.IsDropDownOpen;
            LoadSelectedPlane();
        }
        private void LoadSelectedPlane()
        {
            SearchBox.IsEnabled = true;
            ResetSearchBox();

            PlaneItem selectedPlane = PlaneComboBox.SelectedItem as PlaneItem;
            ControlsHelper.LoadMappings(selectedPlane.Value);
            UpdateDataSource();
        }
        private void UpdateDataSource()
        {
            UpdateDataSource(string.Empty);
        }
        private void UpdateDataSource(string search)
        {
            MappingDataGrid.ItemsSource = ControlsHelper.Mappings.Where(m => m.GameControlName.Contains(search, StringComparison.OrdinalIgnoreCase)).OrderBy(m => m.GameControlName);
        }
        private void ViewButton_Click(object sender, RoutedEventArgs e)
        {
            //TODO detect which window to open (generic or complex with axises)
            ControlMapping mapping = MappingDataGrid.SelectedItem as ControlMapping;
            List<DeviceItem> availabeDevices = GetAvailableDevices();
            List<ControllerActionBehavior> supportedBehaviors = GetSupportedBehaviors(mapping);
            GenericBinding bindingWindow = new GenericBinding(this, mapping, availabeDevices, supportedBehaviors);

            bool? dialogResult = bindingWindow.ShowDialog();
            if (dialogResult.HasValue && dialogResult.Value)
            {
                //List<GameAction> actions = new List<GameAction>();
                //List<ActionItem> actionItems = bindingWindow.Actions.Where(a => a.SelectedDevice != null && !string.IsNullOrEmpty(a.ControlName)).ToList();
                //foreach (ActionItem actionItem in actionItems)
                //{
                //    GenericGameAction genericAction = new GenericGameAction()
                //    {
                //        ControllerActionBehavior = actionItem.Behavior,
                //        ControllerButtonName = actionItem.ControlName,
                //        ControllerInstanceGuid = actionItem.SelectedDevice.ID
                //    };
                //    actions.Add(genericAction);
                //}
                //ControlsHelper.Mappings.Find(m => m.GameControlName == mapping.GameControlName).GameActions = actions;
                ////TODO save in json mapping file
                ////ControlsHelper.CreateMappingFileFromMappings();
                //UpdateDataSource(SearchBox.Text);
            }
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
        private List<DeviceItem> GetAvailableDevices()
        {
            //Get list of available devices
            using DirectInput di = new DirectInput();
            IList<DeviceInstance> deviceInstances = di.GetDevices();
            List<DeviceItem> availableDevices = new List<DeviceItem>();

            foreach (DeviceInstance deviceInstance in deviceInstances)
            {
                switch (ControlsHelper.GetDeviceType(deviceInstance))
                {
                    case SimpleDeviceType.Keyboard:
                    case SimpleDeviceType.Joystick:
                        availableDevices.Add(new DeviceItem(deviceInstance.InstanceGuid, deviceInstance.InstanceName));
                        break;
                    case SimpleDeviceType.None:
                    default:
                        break;
                }
            }
            return availableDevices;
        }
    }
}
