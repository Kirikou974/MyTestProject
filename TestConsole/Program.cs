using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VTOLVRControlsMapper;
using VTOLVRControlsMapper.Core;
using UnityEngine;
using Joystick = SharpDX.DirectInput.Joystick;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            TestJoystick();
        }
        static void TestJoystick()
        {
            DirectInput di = new DirectInput();
            IList<DeviceInstance> devices = di.GetDevices();
            Joystick joystick = null;
            foreach (var item in devices)
            {
                if (item.InstanceGuid == new Guid("8e0fdc40-f559-11ea-8002-444553540000") && item.Type == SharpDX.DirectInput.DeviceType.FirstPerson)
                {
                    using (joystick = new Joystick(di, item.InstanceGuid))
                    {
                        joystick.Properties.BufferSize = 128;
                        joystick.Acquire();
                    }
                }
            }
            Console.WriteLine("default(JoystickUpdate)");
            Console.WriteLine(default(JoystickOffset));
            string controlName = "Y";
            while (true)
            {
                JoystickUpdate[] updates = joystick.GetBufferedData();

                if (updates.Where(u => u.Offset.ToString() == controlName).Count() > 0)
                {
                    JoystickUpdate update = updates.First(u => u.Offset.ToString() == controlName);
                    Console.Write("\r{0}: {1}           ", update.Offset, ControlsHelper.ConvertAxisValue(update.Value, true, MappingRange.High));
                }
            }
        }
    }

}