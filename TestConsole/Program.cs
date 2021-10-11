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
            ControlsHelper.LoadControllers();
            Joystick joystick = ControlsHelper.GetDevice<Joystick>(new Guid("8e0fdc40-f559-11ea-8002-444553540000"));
            Keyboard kb = ControlsHelper.GetDevice<Keyboard>(new Guid("6f1d2b61-d5a0-11cf-bfc7-444553540000"));
            var test = ControlsHelper.GetGameActions<GenericGameAction>();
            foreach (var item in test)
            {
                Console.WriteLine(item);
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
                if (item.InstanceGuid == new Guid("8e0fdc40-f559-11ea-8002-444553540000") && item.Type == SharpDX.DirectInput.DeviceType.FirstPerson)
                {
                    joystick = new Joystick(di, item.InstanceGuid);
                    joystick.Properties.BufferSize = 128;
                    joystick.Acquire();
                }
            }
            string controlName = "Buttons4";
            while (true)
            {
                JoystickUpdate[] updates = joystick.GetBufferedData();

                //foreach (var item in updates)
                //{
                //    Console.WriteLine(item.RawOffset);
                //}
                if (updates.Where(u => u.Offset.ToString() == controlName).Count() > 0)
                {
                    JoystickUpdate update = updates.First(u => u.Offset.ToString() == controlName);
                    //Console.Write("\r{0}: {1}           ", update.Offset, ControlsHelper.ConvertAxisValue(update.Value, true, MappingRange.High));
                    Console.Write("\r{0}: {1}           ", update.Offset, update.Value);
                }
            }
        }
    }

}