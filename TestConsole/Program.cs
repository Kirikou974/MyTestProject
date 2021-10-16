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
            ControlsHelper.LoadMappings("F:\\Steam\\SteamApps\\common\\VTOL VR\\VTOLVR_ModLoader\\mods\\VTOLVRControlsMapper\\mapping.FA26B.json");
            //ControlsHelper.LoadControllers();
            //Joystick joystick = ControlsHelper.GetDevice<Joystick>(new Guid("8e0fdc40-f559-11ea-8002-444553540000"));
            //Keyboard kb = ControlsHelper.GetDevice<Keyboard>(new Guid("6f1d2b61-d5a0-11cf-bfc7-444553540000"));
            //var test = ControlsHelper.GetGameActions<ThrottleAction>();
            foreach (var item in ControlsHelper.Mappings)
            {
                if (item.GameActions != null)
                    Console.WriteLine(item.GameActions.ToString());
            }
            Console.ReadLine();
            //TestJoystick();
        }
        static void TestJoystick()
        {
            DirectInput di = new DirectInput();
            IList<DeviceInstance> devices = di.GetDevices();
            Joystick joystick = null;
            foreach (var item in devices)
            {
                if (item.Type == SharpDX.DirectInput.DeviceType.FirstPerson)
                {
                    Console.WriteLine(item.InstanceName);
                    Console.WriteLine(item.InstanceGuid);
                }
                if (item.InstanceGuid == new Guid("ccb75030-fce8-11eb-8001-444553540000") && item.Type == SharpDX.DirectInput.DeviceType.FirstPerson)
                {
                    joystick = new Joystick(di, item.InstanceGuid);
                    joystick.Properties.BufferSize = 128;
                    joystick.Acquire();
                }
            }
            string previousOffset = string.Empty;
            Type joystickStateType = typeof(JoystickState);

            while (true)
            {
                JoystickUpdate[] updates = joystick.GetBufferedData();
                JoystickState state = joystick.GetCurrentState();

                foreach (var item in updates)
                {
                    //if (previousOffset != item.Offset.ToString())
                    //{
                    //    previousOffset = item.Offset.ToString();
                    //}
                    if (item.Offset.ToString() == "Y")
                    {
                        Console.Write("\r{0}: {1}       | {2} : {3}           ", item.Offset, item.Value, "X", joystickStateType.GetProperty("X").GetValue(state));
                    }
                }
            }
        }
    }

}