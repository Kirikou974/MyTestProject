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
            ControlsHelper.LoadMappings("F:\\Steam\\SteamApps\\common\\VTOL VR\\VTOLVR_ModLoader\\mods\\", "VTOLVRControlsMapper", "FA26B");
            ControlsHelper.LoadMappingInstances();
        }
    }
}