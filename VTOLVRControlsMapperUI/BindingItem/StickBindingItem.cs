using VTOLVRControlsMapper.Core;
using VTOLVRControlsMapperUI.GridItem;

namespace VTOLVRControlsMapperUI.BindingItem
{
    public class StickBindingItem : JoystickBindingItem<StickAction>
    {
        public override StickAction MappingAction
        {
            get
            {
                StickAction action = base.MappingAction;
                if (Roll != null && Roll.IsValid())
                {
                    if (action.Roll == null)
                    {
                        action.Roll = new Axis();
                    }
                    action.Roll.Invert = Roll.Invert;
                    action.Roll.MappingRange = Roll.Range.Value;
                    action.Roll.Name = Roll.ControlName;
                }
                if (Pitch != null && Pitch.IsValid())
                {
                    if (action.Pitch == null)
                    {
                        action.Pitch = new Axis();
                    }
                    action.Pitch.Invert = Pitch.Invert;
                    action.Pitch.MappingRange = Pitch.Range.Value;
                    action.Pitch.Name = Pitch.ControlName;
                }
                if (Yaw != null && Yaw.IsValid())
                {
                    if (action.Yaw == null)
                    {
                        action.Yaw = new Axis();
                    }
                    action.Yaw.Invert = Yaw.Invert;
                    action.Yaw.MappingRange = Yaw.Range.Value;
                    action.Yaw.Name = Yaw.ControlName;
                }
                return action;
            }
            set
            {
                base.MappingAction = value;
                if (value.Roll != null)
                {
                    Roll.ControlName = value.Roll.Name;
                    Roll.Invert = value.Roll.Invert;
                    Roll.Range = value.Roll.MappingRange;
                }
                if (value.Pitch != null)
                {
                    Pitch.ControlName = value.Pitch.Name;
                    Pitch.Invert = value.Pitch.Invert;
                    Pitch.Range = value.Pitch.MappingRange;
                }
                if (value.Yaw != null)
                {
                    Yaw.ControlName = value.Yaw.Name;
                    Yaw.Invert = value.Yaw.Invert;
                    Yaw.Range = value.Yaw.MappingRange;
                }
            }
        }
        public AxisItem Roll { get; set; }
        public AxisItem Yaw { get; set; }
        public AxisItem Pitch { get; set; }
        public override string TriggerName => "Fire";
        public override string MenuName => "Menu";
        public StickBindingItem(DeviceItem device) : base(device)
        {
            Roll = new AxisItem("Roll Axis");
            Pitch = new AxisItem("Pitch Axis");
            Yaw = new AxisItem("Yaw Axis");
            Trigger.IsAxis = false;
            Actions.Insert(0, Roll);
            Actions.Insert(1, Pitch);
            Actions.Insert(2, Yaw);
        }
    }
}
