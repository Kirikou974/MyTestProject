using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VTOLVRControlsMapper;
using VTOLVRControlsMapper.Core;
using Rewired;
using UnityEngine;
using Joystick = SharpDX.DirectInput.Joystick;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Test test = new Test();
            Type testType = test.GetType();
            Console.WriteLine(nameof(test.MyProperty));
            PropertyInfo propInfo = testType.GetProperty(nameof(test.MyProperty));
            propInfo.SetValue(test, 2);
            Console.WriteLine(test.MyProperty);
            Console.ReadLine();
        }
    }
    public class Test
    {
        public int MyProperty { get; private set; }
    }
}