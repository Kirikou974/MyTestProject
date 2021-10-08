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
            TestJSON();
            //TestJoystick();
        }
        static void TestJSON()
        {
            ControlsHelper.LoadDeviceInstances();
            ControlsHelper.LoadMappings("F:\\Steam\\SteamApps\\common\\VTOL VR\\VTOLVR_ModLoader\\mods\\VTOLVRControlsMapper\\mapping.FA26B2.json");
            foreach (var mapping in ControlsHelper.Mappings)
            {
                if(mapping.GameActions != null)
                {
                    foreach (var gameAction in mapping.GameActions)
                    {
                        if(gameAction is ThrottleAction)
                        {
                            ThrottleAction ta = gameAction as ThrottleAction;
                            Console.WriteLine(gameAction.GetType());
                            Console.WriteLine(ta.PowerAxis);
                            Console.WriteLine(ta.TriggerAxis);
                            Console.WriteLine(ta.Menu);
                        }
                    }
                }
            }
            ControlsHelper.LoadControllers();
            Console.ReadLine();
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
                    joystick = new Joystick(di, item.InstanceGuid);
                    joystick.Properties.BufferSize = 128;
                    joystick.Acquire();
                }
            }
            JoystickOffset previousOffset = JoystickOffset.AngularAccelerationX;
            while (true)
            {
                JoystickUpdate[] updates = joystick.GetBufferedData();
                foreach (var update in updates)
                {
                    if (update.Offset != previousOffset)
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