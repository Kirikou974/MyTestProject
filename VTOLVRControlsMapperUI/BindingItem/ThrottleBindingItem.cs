using System.Collections.Generic;
using VTOLVRControlsMapper.Core;
using VTOLVRControlsMapperUI.GridItem;

namespace VTOLVRControlsMapperUI.BindingItem
{
    public class ThrottleBindingItem : JoystickBindingItem<ThrottleAction>
    {
        public List<BaseItem> Actions { get; set; }
        public ThrottleAction _mappingAction;
        public override ThrottleAction MappingAction
        {
            get => _mappingAction;
            set
            {
                _mappingAction = value;
                if (_mappingAction.Power != null)
                {
                    Throttle.ControlName = _mappingAction.Power.Name;
                    Throttle.Invert = _mappingAction.Power.Invert;
                    Throttle.Range = _mappingAction.Power.MappingRange;
                }
                if (_mappingAction.Thumbstick != null)
                {
                    if (_mappingAction.Thumbstick.X != null)
                    {
                        ThumbstickX.ControlName = _mappingAction.Thumbstick.X.Name;
                        ThumbstickX.Invert = _mappingAction.Thumbstick.X.Invert;
                        ThumbstickX.Range = _mappingAction.Thumbstick.X.MappingRange;
                    }
                    if (_mappingAction.Thumbstick.X != null)
                    {
                        ThumbstickY.ControlName = _mappingAction.Thumbstick.Y.Name;
                        ThumbstickY.Invert = _mappingAction.Thumbstick.Y.Invert;
                        ThumbstickY.Range = _mappingAction.Thumbstick.Y.MappingRange;
                    }
                }
                if (_mappingAction.Trigger != null)
                {
                    Brake.ControlName = _mappingAction.Trigger.Name;
                    Brake.Invert = _mappingAction.Trigger.Invert;
                    Brake.Range = _mappingAction.Trigger.MappingRange;
                }

                ChaffFlare.ControlName = _mappingAction.Menu;
            }
        }
        public AxisItem Throttle { get; set; }
        public AxisItem ThumbstickX { get; set; }
        public AxisItem ThumbstickY { get; set; }
        public AxisItem Brake { get; set; }
        public MenuItem ChaffFlare { get; set; }
        public ThrottleBindingItem(DeviceItem device)
        {
            Device = device;
            Throttle = new AxisItem("Throttle Axis");
            ThumbstickX = new AxisItem("Thumbstick X Axis");
            ThumbstickY = new AxisItem("Thumbstick Y Axis");
            Brake = new AxisItem("Brake axis");
            ChaffFlare = new MenuItem("Chaff/Flare");
            Actions = new List<BaseItem>
            {
                Throttle,
                Brake,
                ThumbstickX,
                ThumbstickY,
                ChaffFlare
            };
        }
    }
}
