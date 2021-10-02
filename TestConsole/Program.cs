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

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Test test = new Test();
            FieldInfo testProp = test.GetType().GetField("Toto", BindingFlags.NonPublic | BindingFlags.Instance);
            Console.WriteLine(testProp.GetValue(test));
            Console.ReadLine();
        }
    }
    class Test
    {
        protected string Toto = "OK";
    }
}