$source1 = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using SharpDX.DirectInput;
namespace VTOLVRPhysicalInput
{
    public class Mappings
    {
        [XmlElement("StickMappings")]
        public List<StickMappingList> MappingsList = new List<StickMappingList>();
    }
    
    public class StickMappingList
    {
        public string StickName { get; set; }
        [XmlElement("AxisToVectorComponent")]
        public List<AxisToVectorComponentMapping> AxisToVectorComponentMappings = new List<AxisToVectorComponentMapping>();
        [XmlElement("ButtonToVectorComponent")]
        public List<ButtonToVectorComponent> ButtonToVectorComponentMappings = new List<ButtonToVectorComponent>();
        [XmlElement("ButtonToButton")]
        public List<ButtonToButton> ButtonToButtonMappings = new List<ButtonToButton>();
        [XmlElement("PovToTouchpad")]
        public List<PovToTouchpad> PovToTouchpadMappings = new List<PovToTouchpad>();
    }
    public class MappingsDictionary
    {
        public Dictionary<string, StickMappings> Sticks = new Dictionary<string, StickMappings>();
    }

    public class StickMappings
    {
        public string StickName { get; set; }
        public Joystick Stick { get; set; }
        public Dictionary<JoystickOffset, AxisToVectorComponentMapping> AxisToVectorComponentMappings = new Dictionary<JoystickOffset, AxisToVectorComponentMapping>();
        public Dictionary<JoystickOffset, ButtonToVectorComponent> ButtonToVectorComponentMappings = new Dictionary<JoystickOffset, ButtonToVectorComponent>();
        public Dictionary<JoystickOffset, ButtonToButton> ButtonToButtonMappings = new Dictionary<JoystickOffset, ButtonToButton>();
        public Dictionary<JoystickOffset, PovToTouchpad> PovToTouchpadMappings = new Dictionary<JoystickOffset, PovToTouchpad>();
    }

    public class AxisToVectorComponentMapping
    {
        public string InputAxis { get; set; }
        public bool Invert { get; set; }
        public string OutputDevice { get; set; }
        public string OutputSet { get; set; }
        public string MappingRange { get; set; }
        public string OutputComponent { get; set; }
    }

    public class ButtonToVectorComponent
    {
        public int InputButton { get; set; }
        public string OutputDevice { get; set; }
        public string OutputSet { get; set; }
        public string OutputComponent { get; set; }
        public float PressValue { get; set; }
        public float ReleaseValue { get; set; }
    }

    public class ButtonToButton
    {
        public int InputButton { get; set; }
        public string OutputDevice { get; set; }
        public string OutputButton { get; set; }
    }

    public class PovToTouchpad
    {
        public int InputPov { get; set; }
        public string OutputDevice { get; set; }
        public string OutputSet { get; set; }
    }
}
"@

Add-Type -TypeDefinition $source1 -ReferencedAssemblies System.Xml, "C:\Users\Thierry\Source\Repos\VTOLVRPhysicalInput\packages\SharpDX.4.2.0\lib\net40\SharpDX.dll","C:\Users\Thierry\Source\Repos\VTOLVRPhysicalInput\VTOLVRPhysicalInput\bin\Debug\SharpDX.DirectInput.dll"
[void][System.Reflection.Assembly]::LoadWithPartialName("System.Xml.Serialization");
[void][system.reflection.assembly]::LoadFile("C:\Users\Thierry\Source\Repos\VTOLVRPhysicalInput\packages\SharpDX.4.2.0\lib\net40\SharpDX.dll");
[void][system.reflection.assembly]::LoadFile("C:\Users\Thierry\Source\Repos\VTOLVRPhysicalInput\VTOLVRPhysicalInput\bin\Debug\SharpDX.DirectInput.dll");

$_stickMappings = New-Object VTOLVRPhysicalInput.MappingsDictionary;

function ProcessSettingsFile {
    $mappings = New-Object VTOLVRPhysicalInput.Mappings;
    $settingsFile = "F:\Steam\SteamApps\common\VTOL VR\VTOLVR_ModLoader\mods\VTOLVRPhysicalInput\VTOLVRPhysicalInputSettings.xml";
    $deserializer = New-Object System.Xml.Serialization.XmlSerializer($mappings.GetType());
    $reader = New-Object System.IO.StreamReader($settingsFile);
    $obj = $deserializer.Deserialize($reader);
    $stickMappings = [VTOLVRPhysicalInput.Mappings]$obj;
    $stickMappings.MappingsList | ForEach-Object {
        $stick = $_;
        if (-not $_stickMappings.Sticks.ContainsKey($stick.StickName))
        {
            $tempStickMappingss = New-Object VTOLVRPhysicalInput.StickMappings;
            $tempStickMappingss.StickName = $stick.StickName;
            $_stickMappings.Sticks.Add($stick.StickName, $tempStickMappingss);
        }

        $mapping = $_stickMappings.Sticks[$stick.StickName];
        $stick.AxisToVectorComponentMappings | ForEach-Object {
            $axisToVectorComponentMapping = $_;
            $mapping.AxisToVectorComponentMappings.Add([SharpDX.DirectInput.JoystickOffset]$axisToVectorComponentMapping.InputAxis, $axisToVectorComponentMapping);
        }
    }
    $reader.Close();
    $reader.Dispose();
}
ProcessSettingsFile;

$directInput = New-Object -TypeName SharpDx.DirectInput.directinput;
$devices = $directInput.GetDevices();
$joysticks = @{};
$devices | ForEach-Object {
    $currentDevice = $_;
    if($currentDevice.Type -eq "FirstPerson") {
        $joystick = New-Object -TypeName SharpDX.DirectInput.Joystick -ArgumentList ([SharpDX.DirectInput.DirectInput]$directInput, [system.guid]$currentDevice.InstanceGuid);
        $joysticks.Add($joystick.Information.ProductName, $joystick);

        #foreach($polledStick in $_stickMappings.Sticks.GetEnumerator()) {
            #$joysticks.ContainsKey($polledStick.Key);
        #}
    }
}
$joyname = "VKBsim Gunfighter Modern Combat Pro Twist ";
#$joyname = "Saitek Pro Flight X-56 Rhino Throttle";
$state = new-object SharpDX.DirectInput.JoystickState;
$joysticks[$joyname].Acquire();
sleep -Seconds 1;
$joysticks[$joyname].Poll();
$joysticks[$joyname].GetCurrentState([ref]$state);
$i = 0; 
$state
$state.Buttons | foreach { 
    if($_) {
        $i;
    }
    $i++;    
}
