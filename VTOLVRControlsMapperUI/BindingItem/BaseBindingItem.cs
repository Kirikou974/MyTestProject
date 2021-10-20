using VTOLVRControlsMapperUI.GridItem;

namespace VTOLVRControlsMapperUI.BindingItem
{
    public interface IBindingItem
    {
        public DeviceItem Device { get; set; }
    }
    public abstract class BaseBindingItem : IBindingItem
    {
        public DeviceItem Device { get; set; }
    }
}
