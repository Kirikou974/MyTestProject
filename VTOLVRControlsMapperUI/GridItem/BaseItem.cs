using System;
using VTOLVRControlsMapper.Core;

namespace VTOLVRControlsMapperUI.GridItem
{
    public abstract class BaseItem
    {
        public virtual bool Visible { get; set; }
        public virtual string Name { get; set; }
        public abstract string ControlName { get; set; }
        public BaseItem() : this(string.Empty) { }
        public BaseItem(string name)
        {
            Name = name;
            Visible = true;
        }
    }
}
