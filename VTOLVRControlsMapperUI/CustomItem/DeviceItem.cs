using System;

namespace VTOLVRControlsMapperUI.CustomItem
{
    public class DeviceItem
    {
        public string Name { get; set; }
        public Guid ID { get; set; }
        public DeviceItem(Guid id, string name)
        {
            ID = id;
            Name = name;
        }
    }
}