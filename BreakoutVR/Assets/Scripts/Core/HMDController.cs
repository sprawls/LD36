using UnityEngine;
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.VR;
using Valve.VR;

public class HMDController : Singleton<HMDController>
{
    [Header("Parameter"), SerializeField]
    private float m_gridFadeInTime = 0.5f;

    [LargeHeader("Tracked Object"), SerializeField]
    private SteamVR_TrackedObject m_leftControllerObject = null;

    [SerializeField]
    private SteamVR_TrackedObject m_rightControllerObject = null;

    //====================================================================================================================

    public enum ControllerIndex
    {
        Left,
        Right
    }

    public enum ControllerButton
    { 
        ApplicationMenu,
        Trigger,
    }

    public class Controller
    {
        public int Index;
    }

    //====================================================================================================================

    //public static event Action OnDashboardOpen;
    //public static event Action OnDashboardClose;
    public static event Action OnVrRoomEnter;
    public static event Action OnVrRoomExit;

    //====================================================================================================================
    //------------------------------------------------------------------------------
    public Quaternion HeadsetRotation
    {
        get { return InputTracking.GetLocalRotation(VRNode.Head); }
    }

    //------------------------------------------------------------------------------
    public Vector3 HeadsetPosition
    {
        get { return InputTracking.GetLocalPosition(VRNode.Head); }
    }

    //------------------------------------------------------------------------------
    public Quaternion GetControllerGlobalRotation(ControllerIndex index)
    {
        return m_controllers[index].transform.rotation;
    }

    //------------------------------------------------------------------------------
    public Quaternion GetControllerLocalRotation(ControllerIndex index)
    {
        return m_controllers[index].transform.localRotation;
    }

    //------------------------------------------------------------------------------
    public Vector3 ControllerGlobalPosition(ControllerIndex index)
    {
        return m_controllers[index].transform.position;
    }

    //------------------------------------------------------------------------------
    public Vector3 ControllerLocalPosition(ControllerIndex index)
    {
        return m_controllers[index].transform.localPosition;
    }

    //------------------------------------------------------------------------------
    public bool GetButton(ControllerIndex index, ControllerButton button)
    {
        return GetDevice(index).GetPress(ControllerButtonToButtonMask(button));
    }

    //------------------------------------------------------------------------------
    public bool GetButtonDown(ControllerIndex index, ControllerButton button)
    {
        return GetDevice(index).GetPressDown(ControllerButtonToButtonMask(button));
    }

    //------------------------------------------------------------------------------
    public bool GetButtonUp(ControllerIndex index, ControllerButton button)
    {
        return GetDevice(index).GetPressUp(ControllerButtonToButtonMask(button));
    }

    //------------------------------------------------------------------------------
    public void TriggerHapticPulse(ControllerIndex index, ushort durationMicroSecond = 500)
    {
        GetDevice(index).TriggerHapticPulse(durationMicroSecond);
    }

    //====================================================================================================================

    private bool m_vrRoomEnterRequested = false;
    private bool m_vrRoomExitRequested = false;
    private bool m_isInVrRoom = false;
    private Dictionary<ControllerIndex, SteamVR_TrackedObject> m_controllers = new Dictionary<ControllerIndex, SteamVR_TrackedObject>();
    private CVRCompositor m_compositor = null;

    //====================================================================================================================
    //------------------------------------------------------------------------------
    [UsedImplicitly]
    private void Awake()
    {
        m_compositor = OpenVR.Compositor;
        m_controllers.Add(ControllerIndex.Left, m_leftControllerObject);
        m_controllers.Add(ControllerIndex.Right, m_rightControllerObject);
    }

    //------------------------------------------------------------------------------
    [UsedImplicitly]
    private void Update()
    {
        SteamVR_Controller.Update();

        if (m_vrRoomEnterRequested && !m_isInVrRoom)
        {
            EnterVrRoom();
        }
        else if (m_vrRoomExitRequested && m_isInVrRoom)
        {
            ExitVrRoom();    
        }
    }

    //------------------------------------------------------------------------------
    public void RequestVrRoomEnter()
    {
        m_vrRoomEnterRequested = true;
    }

    //------------------------------------------------------------------------------
    public void RequestVrRoomExit()
    {
        m_vrRoomExitRequested = true;
    }

    //------------------------------------------------------------------------------
    private void EnterVrRoom()
    {
        m_compositor.FadeGrid(m_gridFadeInTime, true);
        m_isInVrRoom = true;

        if (OnVrRoomEnter != null)
        {
            OnVrRoomEnter();
        }
    }

    //------------------------------------------------------------------------------
    private void ExitVrRoom()
    {
        m_compositor.FadeGrid(m_gridFadeInTime, false);
        m_isInVrRoom = false;

        if (OnVrRoomExit != null)
        {
            OnVrRoomExit();
        }
    }

    //------------------------------------------------------------------------------
    private SteamVR_Controller.Device GetDevice(ControllerIndex index)
    {
        return SteamVR_Controller.Input((int) m_controllers[index].index);
    }

    //------------------------------------------------------------------------------
    private ulong ControllerButtonToButtonMask(ControllerButton button)
    {
        switch (button)
        {
             case ControllerButton.ApplicationMenu:
                return SteamVR_Controller.ButtonMask.ApplicationMenu;

            //case ControllerButton.Dashboard:
            //    return SteamVR_Controller.ButtonMask.System;

            //case ControllerButton.Grip:
            //    return SteamVR_Controller.ButtonMask.Grip;

            //case ControllerButton.Touchpad:
            //    return SteamVR_Controller.ButtonMask.Touchpad;

            case ControllerButton.Trigger:
                return SteamVR_Controller.ButtonMask.Trigger;

            default:
                Debug.LogErrorFormat("The button {0} wasn't mapped in the converter ControllerButtonToButtonMask. Using fallback system.", button.ToString());
                return SteamVR_Controller.ButtonMask.System;
        }
    }
}
