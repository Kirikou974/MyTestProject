using System;
using VTOLVRControlsMapper.Core;

namespace VTOLVRControlsMapperUI.GridItem
{
    public class ActionItem : BaseItem
    {
        public GenericGameAction Action { get; set; }
        public override string Name => Action.ControllerActionBehavior.ToString();
        public override string ControlName
        {
            get => Action.ControllerButtonName;
            set => Action.ControllerButtonName = value;
        }
        public ActionItem(ControllerActionBehavior behavior) : base()
        {
            Action = new GenericGameAction
            {
                ControllerActionBehavior = behavior
            };
        }
    }
}
