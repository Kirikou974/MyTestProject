using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VTOLVRControlsMapper.Core;

namespace VTOLVRControlsMapper.Controls
{
    public class Handle : ControlBase<EjectHandle>
    {
        public Handle(string ejectHandleName) : base(ejectHandleName) { }
        [Control(SupportedBehavior = ControllerActionBehavior.Toggle)]
        public void Pull()
        {
            UnityControl.OnHandlePull.Invoke();
        }
    }
}
