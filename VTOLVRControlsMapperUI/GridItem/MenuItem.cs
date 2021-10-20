namespace VTOLVRControlsMapperUI.GridItem
{
    public class MenuItem : JoystickItem
    {
        public override bool Visible => false;
        public MenuItem(string name): base(name) { }
    }
}
