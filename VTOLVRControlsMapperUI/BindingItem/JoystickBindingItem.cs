using System.Collections.Generic;
using VTOLVRControlsMapper.Core;
using VTOLVRControlsMapperUI.GridItem;

namespace VTOLVRControlsMapperUI.BindingItem
{
    public abstract class JoystickBindingItem<T> : BaseBindingItem where T : JoystickAction, new()
    {
        public List<BaseItem> Actions { get; set; }
        public AxisItem ThumbstickX { get; set; }
        public AxisItem ThumbstickY { get; set; }
        //TODO implement thumbstickclick
        public MenuItem ThumbstickClick { get; set; }
        public AxisItem Trigger { get; set; }
        public abstract string TriggerName { get; }
        public MenuItem Menu { get; set; }
        public abstract string MenuName { get; }
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
                if (ThumbstickX != null && ThumbstickX.IsValid())
                {
                    if (action.Thumbstick == null)
                    {
                        action.Thumbstick = new Thumbstick();
                    }
                    action.Thumbstick.X = new Axis(ThumbstickX.ControlName, ThumbstickX.Invert, ThumbstickX.Range.Value);
                }
                if (ThumbstickY != null && ThumbstickY.IsValid())
                {
                    if (action.Thumbstick == null)
                    {
                        action.Thumbstick = new Thumbstick();
                    }
                    action.Thumbstick.Y = new Axis(ThumbstickY.ControlName, ThumbstickY.Invert, ThumbstickY.Range.Value);
                }
                if (Trigger != null && Trigger.IsValid())
                {
                    if (action.Trigger == null)
                    {
                        action.Trigger = new Axis();
                    }
                    action.Trigger.Invert = Trigger.Invert;
                    action.Trigger.MappingRange = Trigger.Range.Value;
                    action.Trigger.Name = Trigger.ControlName;
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
                if (value.Trigger != null)
                {
                    Trigger.ControlName = value.Trigger.Name;
                    Trigger.Invert = value.Trigger.Invert;
                    Trigger.Range = value.Trigger.MappingRange;
                }

                Menu.ControlName = value.Menu;
            }
        }
        public JoystickBindingItem(DeviceItem device)
        {
            Device = device;
            ThumbstickX = new AxisItem("Thumbstick X Axis");
            ThumbstickY = new AxisItem("Thumbstick Y Axis");
            Trigger = new AxisItem(TriggerName);
            Menu = new MenuItem(MenuName);
            Actions = new List<BaseItem>
            {
                Trigger,
                ThumbstickX,
                ThumbstickY,
                Menu
            };
        }
    }
}
