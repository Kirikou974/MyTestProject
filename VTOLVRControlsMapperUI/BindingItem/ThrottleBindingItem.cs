using System.Collections.Generic;
using VTOLVRControlsMapper.Core;
using VTOLVRControlsMapperUI.GridItem;

namespace VTOLVRControlsMapperUI.BindingItem
{
    public class ThrottleBindingItem : JoystickBindingItem<ThrottleAction>
    {
        public override ThrottleAction MappingAction
        {
            get
            {
                ThrottleAction action = base.MappingAction;
                if (Throttle != null && Throttle.IsValid())
                {
                    if (action.Power == null)
                    {
                        action.Power = new Axis();
                    }
                    action.Power.Invert = Throttle.Invert;
                    action.Power.MappingRange = Throttle.Range.Value;
                    action.Power.Name = Throttle.ControlName;
                }
                return action;
            }
            set
            {
                base.MappingAction = value;
                if (value.Power != null)
                {
                    Throttle.ControlName = value.Power.Name;
                    Throttle.Invert = value.Power.Invert;
                    Throttle.Range = value.Power.MappingRange;
                }
            }
        }
        public AxisItem Throttle { get; set; }
        public override string TriggerName => "Brake axis";
        public override string MenuName => "Chaff/Flare";
        public ThrottleBindingItem(DeviceItem device) : base(device)
        {
            Throttle = new AxisItem("Throttle Axis");
            ThumbstickX.Visible = false;
            ThumbstickY.Name = "Tilt";
            Actions.Insert(0, Throttle);
        }
    }
}
