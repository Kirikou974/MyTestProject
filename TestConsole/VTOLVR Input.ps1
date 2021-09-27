[void][system.reflection.assembly]::LoadFile("C:\Users\Thierry\Source\Repos\VTOLVRPhysicalInput\packages\SharpDX.4.2.0\lib\net40\SharpDX.dll");
[void][system.reflection.assembly]::LoadFile("C:\Users\Thierry\Source\Repos\VTOLVRPhysicalInput\VTOLVRPhysicalInput\bin\Debug\SharpDX.DirectInput.dll");
$directInput = New-Object -TypeName SharpDx.DirectInput.directinput;
$devices = $directInput.GetDevices();

$devices | foreach {
    $currentDevice = $_;
    $currentDevice.InstanceName;
    if($currentDevice.InstanceName -eq "VKBsim Gunfighter Modern Combat Pro Twist ") {
        $joystick = New-Object -TypeName SharpDX.DirectInput.Joystick -ArgumentList ([SharpDX.DirectInput.DirectInput]$directInput, [system.guid]$currentDevice.InstanceGuid);
        $joystick.Properties.BufferSize = 128;
        $joystick.Acquire();
        while($true) {
            $deviceUpdates = $joystick.GetBufferedData
            $deviceUpdates | foreach {
                $_;
            }
        }
    }
}
