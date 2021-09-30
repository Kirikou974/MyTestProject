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
            KeyboardUpdate[] _keyboardUpdates = new KeyboardUpdate[] { new KeyboardUpdate() { RawOffset = 1, Sequence = 1, Timestamp = 1, Value = 1 } };
            KeyboardUpdate update = _keyboardUpdates.SingleOrDefault(k => k.Key.ToString() == "a");
            Console.WriteLine(update);
            Console.ReadLine();
        }
    }
}