using System;
using System.Linq;

namespace MyTestProject
{
    public class VRControl<T> where T: UnityEngine.Object
    {
        #region Fields
        private static T[] _vrControls;
        private static T[] VRControls
        {
            get
            {
                if (_vrInteractables == null)
                {
                    _vrControls = UnityEngine.Object.FindObjectsOfType<T>();
                }
                return _vrControls;
            }
        }
        //private static VRInteractable[] _vrInteractables;
        //private static VRButton[] _vrButtons;
        //private static VRTwistKnob[] _vrTwistKnobs;
        //private static VRTwistKnobInt[] _vrTwistKnobInts;
        //private static VRSwitchCover[] _vrSwitchCovers;
        //private static VRLever[] _vrLevers;
        //private static VRInteractable[] VRInteractables
        //{
        //    get
        //    {
        //        if (_vrInteractables == null)
        //        {
        //            _vrInteractables = UnityEngine.Object.FindObjectsOfType<VRInteractable>();
        //        }
        //        return _vrInteractables;
        //    }
        //}
        //private static VRButton[] VRButtons
        //{
        //    get
        //    {
        //        if (_vrButtons == null)
        //        {
        //            _vrButtons = UnityEngine.Object.FindObjectsOfType<VRButton>();
        //        }
        //        return _vrButtons;
        //    }
        //}
        //private static VRTwistKnob[] VRTwistKnobs
        //{
        //    get
        //    {
        //        if (_vrTwistKnobs == null)
        //        {
        //            _vrTwistKnobs = UnityEngine.Object.FindObjectsOfType<VRTwistKnob>();
        //        }
        //        return _vrTwistKnobs;
        //    }
        //}
        //private static VRTwistKnobInt[] VRTwistKnobInts
        //{
        //    get
        //    {
        //        if (_vrTwistKnobInts == null)
        //        {
        //            _vrTwistKnobInts = UnityEngine.Object.FindObjectsOfType<VRTwistKnobInt>();
        //        }
        //        return _vrTwistKnobInts;
        //    }
        //}
        //private static VRSwitchCover[] VRSwitchCovers
        //{
        //    get
        //    {
        //        if (_vrSwitchCovers == null)
        //        {
        //            _vrSwitchCovers = UnityEngine.Object.FindObjectsOfType<VRSwitchCover>();
        //        }
        //        return _vrSwitchCovers;
        //    }
        //}
        //private static VRLever[] VRLevers
        //{
        //    get
        //    {
        //        if (_vrLevers == null)
        //        {
        //            _vrLevers = UnityEngine.Object.FindObjectsOfType<VRLever>();
        //        }
        //        return _vrLevers;
        //    }
        //}
        public string Name
        {
            get;
            set;
        }
        private ControlType _controlType;
        public ControlType ControlType
        {
            get
            {
                _controlType = ControlType.None;
                bool parseResult = Enum.TryParse<ControlType>(typeof(T).Name, out _controlType);
                if (!parseResult)
                {
                    throw new Exception("Control type is not valid");
                }
                else
                {
                    return _controlType;
                }
            }
        }

        private T _unityControl;
        public T UnityControl
        {
            get
            {
                if(_unityControl == null)
                {
                    _unityControl = VRControls.ToList().Find(x => x.name == Name);
                    switch (ControlType)
                    {
                        case ControlType.VRInteractable:
                            _unityControl = VRInteractables.ToList().Find(x => x.name == Name);
                            break;
                        case ControlType.VRSwitchCover:
                            break;
                        case ControlType.VRLever:
                            break;
                        case ControlType.VRButton:
                            break;
                        case ControlType.VRTwistKnob:
                            break;
                        case ControlType.VRTwistKnobInt:
                            break;
                        case ControlType.VRJoystick:
                            break;
                        case ControlType.VRThrottle:
                            break;
                        case ControlType.None:
                            break;
                        default:
                            break;
                    }
                }

                return _unityControl;
            }
        }
        #endregion
    }
    public enum ControlType
    {
        VRInteractable,
        VRSwitchCover,
        VRLever,
        VRButton,
        VRTwistKnob,
        VRTwistKnobInt,
        VRJoystick,
        VRThrottle,
        None
    }
}
