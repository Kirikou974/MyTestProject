using VTOLVRControlsMapper.Core;

namespace VTOLVRControlsMapperUI.CustomItem
{
    public class ActionItem
    {
        public ControllerActionBehavior Behavior { get; set; }
        public string ControlName { get; set; }
        public ActionItem(ControllerActionBehavior behavior)
        {
            Behavior = behavior;
        }
        public bool Enabled { get; set; }
    }
}
