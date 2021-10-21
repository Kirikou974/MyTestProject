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
        public AxisItem Trigger { get; set; }
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
            Trigger = new AxisItem(triggerName);
            Menu = new MenuItem(menuName);
            Actions = new List<BaseItem>
            {
                Trigger,
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
        public JoystickBindingItem(DeviceItem device) : base(device) { }
        public override bool IsValid()
        {
            return MappingAction.IsValid();
        }
    }
}
