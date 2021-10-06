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
            DirectInput di = new DirectInput();
            IList<DeviceInstance> devices = di.GetDevices();
            Joystick joystick = null;
            foreach (var item in devices)
            {
                if(item.InstanceGuid == new Guid("8e0fdc40-f559-11ea-8002-444553540000") && item.Type == SharpDX.DirectInput.DeviceType.FirstPerson)
                {
                    joystick = new Joystick(di, item.InstanceGuid);
                    joystick.Properties.BufferSize = 128;
                    joystick.Acquire();
                }
            }
            JoystickOffset previousOffset = JoystickOffset.AngularAccelerationX;
            while(true)
            {
                JoystickUpdate[] updates = joystick.GetBufferedData();
                foreach (var update in updates)
                {
                    if(update.Offset != previousOffset)
                    {
                        Console.WriteLine();
                        previousOffset = update.Offset;
                        Console.WriteLine(update.Offset);
                    }
                    Console.Write("\r{0}           ", ControlsHelper.ConvertAxisValue(update.Value, true, MappingRange.High));
                }
            }
        }
    }

}