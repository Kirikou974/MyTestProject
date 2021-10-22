using System.Collections.Generic;
using VTOLVRControlsMapper.Core;
using VTOLVRControlsMapperUI.GridItem;

namespace VTOLVRControlsMapperUI.BindingItem
{
    public abstract class JoystickBindingItem : BaseBindingItem
    {
        public List<BaseItem> Actions { get; set; }
        public AxisItem ThumbstickX { get; set; }
        public AxisItem ThumbstickY { get; set; }
        //TODO implement thumbstickclick
        public MenuItem ThumbstickClick { get; set; }
        public AxisItem TriggerAxis { get; set; }
        public MenuItem TriggerButton { get; set; }
        public virtual string TriggerName { get; }
        public MenuItem Menu { get; set; }
        public virtual string MenuName { get; }
        public abstract GameAction GameAction { get; set; }
        public JoystickBindingItem(DeviceItem device)
        {
            Device = device;
            ThumbstickX = new AxisItem("Thumbstick X Axis");
            ThumbstickY = new AxisItem("Thumbstick Y Axis");
            string triggerName = TriggerName;
            string menuName = MenuName;
            if (string.IsNullOrEmpty(triggerName)) triggerName = "Trigger";
            if (string.IsNullOrEmpty(menuName)) menuName = "Menu";
            TriggerAxis = new AxisItem(triggerName + " axis");
            TriggerButton = new MenuItem(triggerName + " button");
            Menu = new MenuItem(menuName);
            Actions = new List<BaseItem>
            {
                TriggerAxis,
                TriggerButton,
                ThumbstickX,
                ThumbstickY,
                Menu
            };
        }
        public abstract bool IsValid();
    }
    public class JoystickBindingItem<T> : JoystickBindingItem where T : JoystickAction, new()
    {
        public override GameAction GameAction
        {
            get => MappingAction;
            set => MappingAction = (T)value;
        }
        public virtual T MappingAction
        {
            get
            {
                T action = new T
                {
                    ControllerInstanceGuid = Device.ID
                };
                if (Menu != null)
                {
                    action.Menu = Menu.ControlName;
                }
                if (TriggerButton != null)
                {
                    action.TriggerButton = TriggerButton.ControlName;
                }
                if (ThumbstickX != null && ThumbstickX.IsValid())
                {
                    if (action.Thumbstick == null)
                    {
                        action.Thumbstick = new Thumbstick();
                    }
                    action.Thumbstick.X = new Axis
                    {
                        Name = ThumbstickX.ControlName,
                        Invert = ThumbstickX.Invert,
                        MappingRange = ThumbstickX.Range.Value
                    };
                }
                if (ThumbstickY != null && ThumbstickY.IsValid())
                {
                    if (action.Thumbstick == null)
                    {
                        action.Thumbstick = new Thumbstick();
                    }
                    action.Thumbstick.Y = new Axis
                    {
                        Name = ThumbstickY.ControlName,
                        Invert = ThumbstickY.Invert,
                        MappingRange = ThumbstickY.Range.Value
                    };
                }
                if (TriggerAxis != null && TriggerAxis.IsValid())
                {
                    if (action.TriggerAxis == null)
                    {
                        action.TriggerAxis = new Axis();
                    }
                    action.TriggerAxis.Invert = TriggerAxis.Invert;
                    action.TriggerAxis.MappingRange = TriggerAxis.Range.Value;
                    action.TriggerAxis.Name = TriggerAxis.ControlName;
                }

                return action;
            }
            set
            {
                if (value.Thumbstick != null)
                {
                    if (value.Thumbstick.X != null)
                    {
                        ThumbstickX.ControlName = value.Thumbstick.X.Name;
                        ThumbstickX.Invert = value.Thumbstick.X.Invert;
                        ThumbstickX.Range = value.Thumbstick.X.MappingRange;
                    }
                    if (value.Thumbstick.X != null)
                    {
                        ThumbstickY.ControlName = value.Thumbstick.Y.Name;
                        ThumbstickY.Invert = value.Thumbstick.Y.Invert;
                        ThumbstickY.Range = value.Thumbstick.Y.MappingRange;
                    }
                }
                if (value.TriggerAxis != null)
                {
                    TriggerAxis.ControlName = value.TriggerAxis.Name;
                    TriggerAxis.Invert = value.TriggerAxis.Invert;
                    TriggerAxis.Range = value.TriggerAxis.MappingRange;
                }

                Menu.ControlName = value.Menu;
                TriggerButton.ControlName = value.TriggerButton;
            }
        }
        public JoystickBindingItem(DeviceItem device) : base(device) { }
        public override bool IsValid()
        {
            return MappingAction.IsValid();
        }
    }
}
