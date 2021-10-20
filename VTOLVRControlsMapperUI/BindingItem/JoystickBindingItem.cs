using VTOLVRControlsMapper.Core;
using VTOLVRControlsMapperUI.GridItem;

namespace VTOLVRControlsMapperUI.BindingItem
{
    public class JoystickBindingItem<T> : BaseBindingItem where T : JoystickAction
    {
        public virtual T MappingAction { get; set; }
    }
}
