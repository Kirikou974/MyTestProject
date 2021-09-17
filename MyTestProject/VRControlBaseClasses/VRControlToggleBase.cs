using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTOLVRControlsMapper
{
    public interface IVRControlToggle
    {
        void Toggle();
    }
    public abstract class VRControlToggleBase<T> : VRControlBase<T>, IVRControlToggle
        where T: UnityEngine.Object
    {
        protected VRControlToggleBase(T unityControl) : base(unityControl) { }

        public abstract void Toggle();
    }
}
