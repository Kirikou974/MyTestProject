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
            var devices = di.GetDevices();
            Joystick joystick = null;
            foreach (var device in devices)
            {
                if(device.InstanceName == "Saitek Pro Flight X-56 Rhino Throttle" && device.Type == SharpDX.DirectInput.DeviceType.FirstPerson)
                {
                    Console.WriteLine(device.Type);
                    Console.WriteLine(device.InstanceGuid);
                    Console.WriteLine(device.InstanceName);
                    joystick = new Joystick(di, device.InstanceGuid);
                    joystick.Properties.BufferSize = 128;
                    joystick.Acquire();
                }
            }
            JoystickOffset currentOffset = JoystickOffset.TorqueZ;
            while(true)
            {
                JoystickUpdate[] joystickUpdates = joystick.GetBufferedData();
                foreach (var joystickUpdate in joystickUpdates)
                {
                    if(joystickUpdate.Offset != JoystickOffset.Sliders1)
                    {
                        if (currentOffset != joystickUpdate.Offset)
                        {
                            currentOffset = joystickUpdate.Offset;
                            Console.WriteLine();
                            Console.WriteLine(joystickUpdate.Offset);
                        }
                        else
                        {
                            Console.Write("\r{0}       ", joystickUpdate.Value);
                        }
                    }
                }
            }

            //Console.ReadLine();
        }
    }
}