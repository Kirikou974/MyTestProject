using System;
using VTOLVRControlsMapper.Core;

namespace VTOLVRControlsMapperUI.GridItem
{
    public class ActionItem : BaseItem
    {
        public GenericGameAction GameAction { get; set; }
        public override string Name => GameAction.ControllerActionBehavior.ToString();
        public override string ControlName
        {
            get => GameAction.ControllerButtonName;
            set => GameAction.ControllerButtonName = value;
        }
        public ActionItem(ControllerActionBehavior behavior) : base()
        {
            GameAction = new GenericGameAction
            {
                ControllerActionBehavior = behavior
            };
        }
    }
}
