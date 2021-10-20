using VTOLVRControlsMapper.Core;

namespace VTOLVRControlsMapperUI.GridItem
{
    public class ActionItem : BaseItem
    {
        public GenericGameAction Action { get; set; }
        public override string Name => Action.ControllerActionBehavior.ToString();
        public ActionItem(ControllerActionBehavior behavior) : base()
        {
            Action = new GenericGameAction
            {
                ControllerActionBehavior = behavior
            };
        }
    }
}
