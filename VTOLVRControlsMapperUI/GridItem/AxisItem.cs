using VTOLVRControlsMapper.Core;
using VTOLVRControlsMapperUI.GridItem;

namespace VTOLVRControlsMapperUI.GridItem
{
    public class AxisItem : JoystickItem
    {
        public bool Invert { get; set; }
        public MappingRange? Range { get; set; }
        public AxisItem(string name): base(name) { }
        public bool IsValid()
        {
            return Range.HasValue && !string.IsNullOrEmpty(Name);
        }
    }
}
