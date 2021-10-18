using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Valve.Newtonsoft.Json;
using VTOLVRControlsMapper;
using VTOLVRControlsMapper.Core;
using VTOLVRControlsMapperUI.CustomItem;

namespace VTOLVRControlsMapperUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool handle = true;
        private const string DEFAULT_TEXT_SEARCH = "Search control";
        //TODO variable for path, appsettings ? 
        private readonly string mappingFilePath = "F:\\Steam\\SteamApps\\common\\VTOL VR\\VTOLVR_ModLoader\\mods\\";
        private readonly string modName = "VTOLVRControlsMapper";
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
            UpdateDataSource(SearchBox.Text);
        }
        private void LoadPlaneComboBox()
        {
            PlaneComboBox.SelectionChanged += PlaneComboBox_SelectionChanged;
            PlaneComboBox.DropDownClosed += PlaneComboBox_DropDownClosed;
            string[] files = ControlsHelper.GetMappingFiles(mappingFilePath, modName);

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
            string calculatedSearch = search;
            if (calculatedSearch == DEFAULT_TEXT_SEARCH)
            {
                calculatedSearch = string.Empty;
            }
            MappingDataGrid.ItemsSource = ControlsHelper.Mappings.Where(m => m.GameControlName.Contains(calculatedSearch, StringComparison.OrdinalIgnoreCase)).OrderBy(m => m.GameControlName);
        }
        private void ViewButton_Click(object sender, RoutedEventArgs e)
        {
            ControlMapping mapping = MappingDataGrid.SelectedItem as ControlMapping;
            List<DeviceItem> availabeDevices = GetAvailableDevices();
            List<ControllerActionBehavior> supportedBehaviors = GetSupportedBehaviors(mapping);
            GenericBinding bindingWindow = new GenericBinding(this, mapping, availabeDevices, supportedBehaviors);

            bool? dialogResult = bindingWindow.ShowDialog();
            if (dialogResult.HasValue && dialogResult.Value)
            {
                List<GameAction> actions = new List<GameAction>();
                List<BindingItem> bindingItems = bindingWindow.BindingItems.Where(b => b.Device != null && b.Actions != null).ToList();

                //TODO handle Throttle and Joystick
                foreach (BindingItem bindingItem in bindingItems)
                {
                    foreach (ActionItem actionItem in bindingItem.Actions)
                    {
                        if (!string.IsNullOrEmpty(actionItem.ControlName) && actionItem.Behavior != ControllerActionBehavior.HoldOff)
                        {
                            GenericGameAction genericAction = new GenericGameAction()
                            {
                                ControllerActionBehavior = actionItem.Behavior,
                                ControllerButtonName = actionItem.ControlName,
                                ControllerInstanceGuid = bindingItem.Device.ID
                            };
                            actions.Add(genericAction);
                        }
                    }
                }
                ControlsHelper.Mappings.Find(m => m.GameControlName == mapping.GameControlName).GameActions = actions;
                SaveMappings((PlaneComboBox.SelectedValue as PlaneItem).Value);
                UpdateDataSource(SearchBox.Text);
            }
        }
        private void SaveMappings(string jsonFilePath)
        {
            using FileStream fs = File.OpenWrite(jsonFilePath);
            using StreamWriter sw = new StreamWriter(fs);
            using JsonTextWriter writer = new JsonTextWriter(sw);
            writer.WriteRawValue("");
            writer.WriteRaw(JsonConvert.SerializeObject(ControlsHelper.Mappings.ToArray(), Formatting.Indented, ControlsHelper.GetJSONSerializerSettings()));
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
