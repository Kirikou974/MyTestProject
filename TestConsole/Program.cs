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
                if (item.Type == SharpDX.DirectInput.DeviceType.FirstPerson)
                {
                    Console.WriteLine(item.InstanceName);
                    Console.WriteLine(item.InstanceGuid);
                }
                if (item.InstanceGuid == new Guid("ccb75030-fce8-11eb-8001-444553540000") && item.Type == SharpDX.DirectInput.DeviceType.FirstPerson)
                {
                    joystick = new Joystick(di, item.InstanceGuid);
                    joystick.Properties.BufferSize = ControlsHelper.BUFFER_SIZE;
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
                    if (previousOffset != item.Offset.ToString())
                    {
                        previousOffset = item.Offset.ToString();
                    }
                    string offsetName = item.Offset.ToString();
                    int currentOffsetValue;

                    //detect if it is an array of values 
                    if (int.TryParse(offsetName.Last().ToString(), out _))
                    {
                        string offsetArrayName = offsetName;
                        while (int.TryParse(offsetArrayName.Last().ToString(), out _))
                        {
                            offsetArrayName = offsetArrayName.Substring(0, offsetArrayName.Length - 1);
                        }
                        int offsetIndex = int.Parse(offsetName.Substring(offsetArrayName.Length, offsetName.Length - offsetArrayName.Length));
                        //string arrayOffsetName = offsetName.Substring(0, offsetName.Length - 1);

                        if (offsetArrayName == nameof(JoystickState.Buttons))
                        {
                            bool[] offsetArray = state.GetType().GetProperty(offsetArrayName).GetValue(state) as bool[];
                            currentOffsetValue = offsetArray[offsetIndex] ? ControlsHelper.BUFFER_SIZE : default;
                        }
                        else
                        {
                            int[] offsetArray = state.GetType().GetProperty(offsetArrayName).GetValue(state) as int[];
                            currentOffsetValue = offsetArray[offsetIndex];
                        }
                    }
                    else
                    {
                        currentOffsetValue = (int)state.GetType().GetProperty(offsetName).GetValue(state);
                    }
                    Console.Write("\rOffset {0}: {1}    | State {2} : {3}           ", item.Offset, item.Value, item.Offset, currentOffsetValue);
                }
            }
        }
    }

}