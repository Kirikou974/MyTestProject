using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VTOLVRControlsMapper.Controls;
using VTOLVRControlsMapper.Core;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            List<MethodInfo> methodsInfo = typeof(Interactable).GetMethods().ToList();
            DirectInput di = new DirectInput();
            Keyboard kb = new Keyboard(di);

            kb.Properties.BufferSize = 128;
            kb.Acquire();
            bool keepGoing = true;
            while(keepGoing)
            {
                var kbUpdates = kb.GetBufferedData();
                foreach (var kbUpdate in kbUpdates)
                {
                    Console.WriteLine(kbUpdate.Key);
                    KeyboardState kbState = kb.GetCurrentState();
                    foreach (var item in kbState.PressedKeys)
                    {
                        Console.WriteLine(item);
                    }
                    if (kbUpdate.Key == Key.Escape)
                    {
                        keepGoing = false;
                    }

                }
            }
        }
    }
}
