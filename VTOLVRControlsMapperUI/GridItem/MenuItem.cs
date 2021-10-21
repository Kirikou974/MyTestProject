namespace VTOLVRControlsMapperUI.GridItem
{
    public class MenuItem : JoystickItem
    {
        public override bool IsAxis => false;
        public MenuItem(string name): base(name) { }
    }
}
