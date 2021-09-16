using Harmony;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyTestProject
{
    public class Main : VTOLMOD
    {
        public override void ModLoaded()
        {
            Log("Mod Loaded");
            if(VTOLAPI.SceneLoaded == null)
            {
                VTOLAPI.SceneLoaded += Sceneloaded;
            }
            if(VTOLAPI.MissionReloaded == null)
            {
                VTOLAPI.MissionReloaded += MissionReloaded;
            }
            base.ModLoaded();
        }

        private void Sceneloaded(VTOLScenes scene)
        {
            Log("Scene Loaded");
            switch (scene)
            {
                case VTOLScenes.ReadyRoom:
                    break;
                case VTOLScenes.Akutan:
                case VTOLScenes.CustomMapBase:
                    Log("Map Loaded");
                    StartCoroutine(LoadControls());
                    break;
                case VTOLScenes.LoadingScene:
                    break;
            }
        }
        private void MissionReloaded()
        {
            Log("MissionReloaded");
            StartCoroutine(LoadControls());
        }
        /// <summary>
        /// Called by Unity each frame
        /// </summary>
        public void Update()
        {
            if(VRControl.ControlsLoaded)
            {
                //TODO : read mapping from file
                if (Input.GetKeyDown("a"))
                {
                    VRControl.Get(Control.altitudeAPButton).Invoke();
                }
                if (Input.GetKeyDown("h"))
                {
                    VRControl.Get(Control.headingAPButton).Invoke();
                }
                if (Input.GetKeyDown("q"))
                {
                    VRControl.Get(Control.NavLightInteractable).Invoke();
                }
                if (Input.GetKeyDown("z"))
                {
                    VRControl.Get(Control.StrobLightInteractable).Invoke();
                }
                if (Input.GetKeyDown("e"))
                {
                    VRControl.Get(Control.LandingLightInteractable).Invoke();
                }
                if (Input.GetKeyDown("r"))
                {
                    VRControl.Get(Control.InsturmentLightInteractable).Invoke();
                }
                if (Input.GetKeyDown("t"))
                {
                    VRControl.Get(Control.PitchSASInteractable).Invoke();
                }
                if (Input.GetKeyDown("y"))
                {
                    VRControl.Get(Control.AssistMasterInteractable).Invoke();
                }
                if (Input.GetKeyDown("u"))
                {
                    VRControl.Get(Control.FlareInteractable).Invoke();
                }
                if (Input.GetKeyDown("i"))
                {
                    VRControl.Get(Control.ChaffInteractable).Invoke();
                }
                if (Input.GetKeyDown("o"))
                {
                    VRControl.Get(Control.BrakeLockInteractable).Invoke();
                }
                if (Input.GetKeyDown("p"))
                {
                    VRControl.Get(Control.WingSwitchInteractable).Invoke();
                }

                //if (Input.GetKeyDown("g"))
                //{
                //    VRControl.Get(Control.GearInteractable).Invoke();
                //}
                //if (Input.GetKeyDown("b"))
                //{
                //    VRControl.Get(Control.helmVisorInteractable).Invoke();
                //}
                //if (Input.GetKeyDown("n"))
                //{
                //    VRControl.Get(Control.helmNVGInteractable).Invoke();
                //}
                //if (Input.GetKeyDown("s"))
                //{
                //    VRControl.Get(Control.speedAPButton).Invoke();
                //}
                //if (Input.GetKeyDown("o"))
                //{
                //    VRControl.Get(Control.offAPButton).Invoke();
                //}
                //if (Input.GetKeyDown("v"))
                //{
                //    VRControl.Get(Control.navAPButton).Invoke();
                //}
                //if (Input.GetKeyDown("c"))
                //{
                //    VRControl.Get(Control.ClrWptButton).Invoke();
                //}
                //if (Input.GetKeyDown("z"))
                //{
                //    VRControl.Get(Control.AltitudeModeInteractable).Invoke();
                //}
                //if (Input.GetKeyDown("m"))
                //{
                //    VRControl.Get(Control.MasterCautionInteractable).Invoke();
                //}
                //if (Input.GetKeyDown("w"))
                //{
                //    VRControl.Get(Control.mfdSwapButton).Invoke();
                //}
                //if (Input.GetKeyDown("k"))
                //{
                //    VRControl.Get(Control.FuelPort).Invoke();
                //}
            }
        }

        private IEnumerator LoadControls()
        {
            VTOLVehicles currentVehicle = VTOLAPI.GetPlayersVehicleEnum();
            VRControl.LoadControls(this, currentVehicle);
            yield return new WaitForSeconds(2);
        }

        //This method is like update but it's framerate independent. This means it gets called at a set time interval instead of every frame. This is useful for physics calculations
        void FixedUpdate()
        {
        }
    }
}