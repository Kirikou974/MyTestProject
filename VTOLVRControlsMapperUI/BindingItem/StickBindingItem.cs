using VTOLVRControlsMapper.Core;
using VTOLVRControlsMapperUI.GridItem;

namespace VTOLVRControlsMapperUI.BindingItem
{
    public class StickBindingItem : JoystickBindingItem<StickAction>
    {
        //public override StickAction MappingAction
        //{
        //    get => base.MappingAction;
        //    set
        //    {
        //        base.MappingAction = value;
                
        //    }
        //}
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
            Actions.Insert(0, Roll);
            Actions.Insert(1, Pitch);
            Actions.Insert(2, Yaw);
        }
    }
}
