using Harmony;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyTestProject
{
    public class Main : VTOLMOD
    {
        VRInteractable[] _vrInteractables;
        VRButton[] _vrButtons;
        VRTwistKnob[] _vrKnobs;
        VRTwistKnobInt[] _vrKnobsInt;
        VRSwitchCover[] _vrSwitchCovers;
        VRLever[] _vrLevers;

        List<KeyValuePair<string, Type>> _vrControlsMap = new List<KeyValuePair<string, Type>>() {
            new KeyValuePair<string, Type>("altitudeAPButton", typeof(VRButton)),
            new KeyValuePair<string, Type>("helmVisorInteractable", typeof(VRInteractable)),
            new KeyValuePair<string, Type>("MFD2PowerInteractable", typeof(VRInteractable)),
            new KeyValuePair<string, Type>("coverSwitchInteractable_masterArm", typeof(VRSwitchCover)),
            new KeyValuePair<string, Type>("GearInteractable", typeof(VRLever))
        };

        bool _vrControlsFetched = false;
        bool _coroutineStarted = false;

        public void Start()
        {
            DontDestroyOnLoad(gameObject); // Required, else mod stops working when you leave opening scene and enter ready room
            Log("MyTestProject has started");
            base.ModLoaded();
            InitKeyboard();

            HarmonyInstance instance = HarmonyInstance.Create("me.mymod");
            instance.PatchAll(Assembly.GetExecutingAssembly());
        }

        private void InitKeyboard()
        {
            Log("Keyboard init");

        }

        private IEnumerator FindScripts()
        {
            while (!VrControlsAvailable())
            {

                _vrInteractables = FindObjectsOfType<VRInteractable>();
                _vrButtons = FindObjectsOfType<VRButton>();
                _vrKnobs = FindObjectsOfType<VRTwistKnob>();
                _vrKnobsInt = FindObjectsOfType<VRTwistKnobInt>();
                _vrSwitchCovers = FindObjectsOfType<VRSwitchCover>();
                _vrLevers = FindObjectsOfType<VRLever>();

                Log("Waiting for controls...");
                yield return new WaitForSeconds(10);
            }
            _vrControlsFetched = true;
            Log("Got VR Controls");
            Log("- VRInteractables : " + _vrInteractables.Count());
            Log("- VRButtons : " + _vrButtons.Count());
            Log("- VRTwistKnob : " + _vrKnobs.Count());
            Log("- VRTwistKnobInt : " + _vrKnobsInt.Count());
            Log("- VRSwitchCovers : " + _vrSwitchCovers.Count());
            Log("- VRLevers : " + _vrLevers.Count());

            //LogControlNames<VRTwistKnob>(_vrKnobs);
            //ClickControlNames();
        }

        private void LogControlNames<T>(T[] vrControls)
        {
            foreach (T vrControl in vrControls)
            {
                if (vrControl != null)
                {
                    Log(string.Format("[{0}] {1}", typeof(T).Name, vrControl));
                }
            }
        }

        private void InvokeControl(KeyValuePair<string, Type> vrControl)
        {
            Log(string.Format("Trying to interact with : {0} ({1})", vrControl.Key, vrControl.Value.Name));
            switch (vrControl.Value.Name)
            {
                case "VRButton":
                    Log(string.Format("[VRInteractable] Interacting with {0}", vrControl.Key));
                    _vrInteractables.ToList().Find(x => x.name == vrControl.Key).OnInteract.Invoke();
                    break;
                case "VRTwistKnob":
                    Log(string.Format("[VRTwistKnob] Interacting with {0}", vrControl.Key));
                    VRTwistKnob knob = _vrKnobs.ToList().Find(x => x.name == vrControl.Key);
                    VRTwistKnobInt knobInt = _vrKnobsInt.ToList().Find(x => x.name == vrControl.Key);
                    Log(string.Format("[VRTwistKnob] {0} current value is {1}", vrControl.Key, knob.currentValue));
                    knob.SetKnobValue(1);
                    Log(string.Format("[VRTwistKnob] {0} new value is {1}", vrControl.Key, knob.currentValue));
                    knob.FireEvent();
                    knobInt.onPushButton.Invoke();
                    VRInteractable temp = _vrInteractables.ToList().Find(x => x.name == vrControl.Key);
                    temp.OnInteract.Invoke();
                    break; ;
                case "VRSwitchCover":
                    Log(string.Format("[VRSwitchCover] Interacting with {0}", vrControl.Key));
                    VRSwitchCover cover = _vrSwitchCovers.ToList().Find(x => x.name == vrControl.Key);
                    Log(string.Format("[VRSwitchCover] {0} covered : {1}", vrControl.Key, cover.covered));
                    cover.OnSetState(0);
                    cover.coveredSwitch.OnInteract.Invoke();
                    break;
                case "VRLever":
                    Log(string.Format("[VRLever] Interacting with {0}", vrControl.Key));
                    VRLever lever = _vrLevers.ToList().Find(x => x.name == vrControl.Key);
                    lever.SetState(0);
                    lever.OnSetState.Invoke(0);
                    break;
                default:
                    Log(string.Format("[VRInteractable] Interacting with {0}", vrControl.Key));
                    _vrInteractables.ToList().Find(x => x.name == vrControl.Key).OnInteract.Invoke();
                    break;
            }
        }

        private bool VrControlsAvailable()
        {
            return (_vrInteractables != null && _vrInteractables.Count() > 0) &&
                    (_vrButtons != null && _vrButtons.Count() > 0) &&
                    (_vrKnobs != null && _vrKnobs.Count() > 0) &&
                    (_vrKnobsInt != null && _vrKnobsInt.Count() > 0) &&
                    (_vrSwitchCovers != null && _vrSwitchCovers.Count() > 0) &&
                    (_vrLevers != null && _vrLevers.Count() > 0);

        }
        /// <summary>
        /// Called by Unity each frame
        /// </summary>
        public void Update()
        {
            if (!VrControlsAvailable() && InCockpit() && !_vrControlsFetched && !_coroutineStarted)
            {
                StartCoroutine(FindScripts());
                _coroutineStarted = true;
            }
            if (Input.GetKeyDown("g"))
            {
                Log("G key down");
                InvokeControl(_vrControlsMap.Find(x => x.Key == "GearInteractable"));
            }
        }
        private static bool InCockpit()
        {
            return SceneManager.GetActiveScene().buildIndex == 7 || SceneManager.GetActiveScene().buildIndex == 11;
        }

        //This method is like update but it's framerate independent. This means it gets called at a set time interval instead of every frame. This is useful for physics calculations
        void FixedUpdate()
        {
        }
    }
}