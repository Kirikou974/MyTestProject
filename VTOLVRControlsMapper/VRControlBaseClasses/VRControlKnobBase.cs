using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTOLVRControlsMapper
{
    public interface IVRControlKnob
    {
        void Increase();
        void Decrease();
        void SetState(int state);
    }
    public abstract class VRControlKnobBase<T> : VRControlBase<T>, IVRControlKnob
        where T : UnityEngine.Object
    {
        protected VRControlKnobBase(T unityControl) : base(unityControl) { }
        public abstract void Increase();
        public abstract void Decrease();
        public abstract void SetState(int state);
    }
}
