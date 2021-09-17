using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VTOLVRControlsMapper
{
    public enum Control
    {
        GearInteractable,
        helmVisorInteractable,
        helmNVGInteractable,
        altitudeAPButton,
        speedAPButton,
        headingAPButton,
        navAPButton,
        ClrWptButton,
        offAPButton,
        MasterCautionInteractable,
        AltitudeModeInteractable,
        mfdSwapButton,
        hmcsPowerInteractable,
        hudPowerInteractable,
        FuelPort,
        CATOTrimInteractable,
        GLimitInteractable,
        RollSASInteractable,
        YawSASInteractable,
        PitchSASInteractable,
        AssistMasterInteractable,
        HookInteractable,
        CatHookInteractable,
        coverSwitchInteractable_rightEngine,
        rightEngineSwitchInteractable,
        coverSwitchInteractable_leftEngine,
        leftEngineSwitchInteractable,
        mainBattSwitchInteractable,
        apuSwitchInteractable,
        CanopyInteractable,
        FlareInteractable,
        ChaffInteractable,
        coverSwitchInteractable_masterArm,
        masterArmSwitchInteractable,
        BrakeLockInteractable,
        WingSwitchInteractable,
        NavLightInteractable,
        StrobLightInteractable,
        LandingLightInteractable,
        InsturmentLightInteractable,
        fuelDumpSwitchInteractable,
        coverSwitchInteractable_fuelDump,
        coverSwitchInteractable_jettisonButton,
        ClearJettisonInteractable,
        JettisonEmptyInteractable,
        JettisonAllInteractable,
        MasterJettisonButtonInteractable,
        powButtonMMFDLeft,
        powButtonMMFDRight,
        RWRButton,
        fuelButton,
        fuelDrainButton,
        MFD1PowerInteractable,
        MFD2PowerInteractable,
        RadarPowerInteractable,
        RWRModeInteractable,
        rightSideEjectInteractable,
        sideEjectInteractable,
        lowerSeatInter,
        raiseSeatInter
    }

    public enum Plane
    {
        FA26B,
        F45A,
        AV42C
    }
    public interface IVRControl
    {
        Control Control { get; }
    }
    public interface IVRControl<T1> : IVRControl
        where T1 : UnityEngine.Object
    {
        T1 UnityControl { get; }
    }

    public abstract class VRControlBase<T> : IVRControl<T>
        where T : UnityEngine.Object
    {
        private readonly T _unityControl;
        public T UnityControl
        {
            get
            {
                return _unityControl;
            }
        }
        public VRControlBase(T unityControl)
        {
            _unityControl = unityControl;
            if (!Enum.TryParse(_unityControl.name, out _control))
            {
                throw new Exception("Error in VRControl build");
            }
        }

        private readonly Control _control;
        public Control Control
        {
            get
            {
                return _control;
            }
        }
    }
}
