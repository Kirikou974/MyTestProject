using System.Collections.Generic;
using VTOLVRControlsMapper.Core;

namespace VTOLVRControlsMapperUI.CustomItem
{
    public class BindingItem
    {
        public DeviceItem Device { get; set; }
        public List<ActionItem> Actions { get; set; }
    }
}
