using UnityEngine;
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.VR;
using Valve.VR;

public class HMDController : Singleton<HMDController>
{
    [SerializeField]
    private float m_gridFadeInTime = 0.5f;

    //====================================================================================================================

    public enum ControllerIndex
    {
        Left,
        Right
    }

    public enum ControllerButton : ulong
    { 
        Dashboard               = 0,
        ApplicationMenu         = 1,
        Grip                    = 2,
        Trigger                 = 33,
        Touchpad                = 32,
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
    public Vector3 HeadsetLocation
    {
        get { return InputTracking.GetLocalPosition(VRNode.Head); }
    }

    //------------------------------------------------------------------------------
    public Quaternion ControllerRotation(ControllerIndex index)
    {
        return m_controllers[index].transform.rot;
    }

    //------------------------------------------------------------------------------
    public Vector3 ControllerLocation(ControllerIndex index)
    {
        return m_controllers[index].transform.pos;
    }

    //------------------------------------------------------------------------------
    public bool GetButton(ControllerIndex index, ControllerButton button)
    {
        return m_controllers[index].GetPressDown((ulong)button);
    }

    //------------------------------------------------------------------------------
    public bool GetButtonDown(ControllerIndex index, ControllerButton button)
    {
        return m_controllers[index].GetPress((ulong) button);
    }

    //------------------------------------------------------------------------------
    public bool GetButtonUp(ControllerIndex index, ControllerButton button)
    {
        return m_controllers[index].GetPressUp((ulong)button);
    }

    //------------------------------------------------------------------------------
    public void TriggerHapticPulse(ControllerIndex index, ushort durationMicroSecond = 500)
    {
        m_controllers[index].TriggerHapticPulse(durationMicroSecond);
    }

    //====================================================================================================================

    private bool m_vrRoomEnterRequested = false;
    private bool m_vrRoomExitRequested = false;
    private bool m_isInVrRoom = false;
    private Dictionary<ControllerIndex, SteamVR_Controller.Device> m_controllers = new Dictionary<ControllerIndex, SteamVR_Controller.Device>();
    private CVRCompositor m_compositor = null;

    //====================================================================================================================
    //------------------------------------------------------------------------------
    [UsedImplicitly]
    private void Awake()
    {
        m_controllers.Add
        (
            ControllerIndex.Left, 
            SteamVR_Controller.Input(SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost
        )));

        m_controllers.Add
       (
           ControllerIndex.Right,
           SteamVR_Controller.Input(SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost
       )));
    }

    //------------------------------------------------------------------------------
    [UsedImplicitly]
    private void Update()
    {
        m_compositor = OpenVR.Compositor;
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
}
