namespace VTOLVRControlsMapper
{
    public class Cover : ControlToggleBase<VRSwitchCover>
    {
        public Lever Lever
        {
            get;
            internal set;
        }
        public Cover(string coverName) :
            base(ControlsHelper.GetGameControl<VRSwitchCover>(coverName))
        {
            Lever = new Lever(coverName);
            //Do a toggle when class is instanciated, either way the switch cover is initialized to false and the covered lever can be activated without visually lifting the cover
            //Not sure why this is necessary except for coverSwitchInteractable_jettisonButton
            //if (coverName != VRControlNames.Cover_Jettison)
            //{
            //    Toggle();
            //}
        }
        public override void Toggle()
        {
            Lever.Toggle();
        }
    }
}
