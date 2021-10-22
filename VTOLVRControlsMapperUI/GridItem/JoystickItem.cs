namespace VTOLVRControlsMapperUI.GridItem
{
    public class JoystickItem : BaseItem
    {
        public override string ControlName { get; set; }
        public bool IsAxis { get; set; }
        public JoystickItem(string name) : base(name)
        {
            IsAxis = true;
        }
    }
}
