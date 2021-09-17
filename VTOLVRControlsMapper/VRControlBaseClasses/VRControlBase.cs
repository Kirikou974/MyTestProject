using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace VTOLVRControlsMapper
{


    public interface IVRControl
    {
        string ControlName { get; }
    }
    public interface IVRControl<T1> : IVRControl
        where T1 : UnityEngine.Object
    {
        T1 UnityControl { get; }
    }

    public abstract class VRControlBase<T> : IVRControl<T>
        where T : UnityEngine.Object
    {
        private readonly T _unityControl;
        public T UnityControl
        {
            get
            {
                return _unityControl;
            }
        }
        public VRControlBase(T unityControl)
        {
            _unityControl = unityControl;
            _controlName = _unityControl.name;
        }

        private readonly string _controlName;
        public string ControlName
        {
            get
            {
                return _controlName;
            }
        }
    }
}
