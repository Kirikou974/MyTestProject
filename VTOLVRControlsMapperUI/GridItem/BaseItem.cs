using VTOLVRControlsMapper.Core;

namespace VTOLVRControlsMapperUI.GridItem
{
    public abstract class BaseItem
    {
        private readonly string _name;
        public virtual bool Visible { get; set; }
        public virtual string Name { get => _name; }
        public BaseItem() : this(string.Empty) { }
        public BaseItem(string name)
        {
            _name = name;
            Visible = true;
        }
    }
}
