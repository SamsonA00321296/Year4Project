using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Valve.VR;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;


public class WatchOverlay : MonoBehaviour
{
    private ulong overlayHandle = OpenVR.k_ulOverlayHandleInvalid;
    // Start is called before the first frame update
    void Start()
    {
        InitOpenVR();

        overlayHandle = CreateOverlay("WatchOverlayKey", "WatchOverlay");

        var position = new Vector3(0, 0, 2);
        var rotation = Quaternion.Euler(0, 0, 0);


        SetOverlayTransformRelative(overlayHandle, OpenVR.k_unTrackedDeviceIndex_Hmd, position, rotation);

        var filePath = Application.streamingAssetsPath + "/icon.png";
        SetOverlayFromFile(overlayHandle, filePath);

        SetOverlaySize(overlayHandle, 0.5f);


        ShowOverlay(overlayHandle);

    }

    private void SetOverlaySize(ulong handle, float size)
    {
        var error = OpenVR.Overlay.SetOverlayWidthInMeters(handle, size);
        if (error != EVROverlayError.None)
        {
            throw new Exception("Failed to set overlay size: " + error);
        }
    }

    private void SetOverlayTransformRelative(ulong handle, uint deviceIndex, Vector3 position, Quaternion rotation)
    {
        var rigidTransform = new SteamVR_Utils.RigidTransform(position, rotation);
        var matrix = rigidTransform.ToHmdMatrix34();

        var error = OpenVR.Overlay.SetOverlayTransformTrackedDeviceRelative(overlayHandle, OpenVR.k_unTrackedDeviceIndex_Hmd, ref matrix);
        if (error != EVROverlayError.None)
        {
            throw new Exception("Failed to set overlay position: " + error);
        }
    }
    private void SetOverlayTransformAbsolute(ulong handle, Vector3 position , Quaternion rotation)
    {
        var rigidTransform = new SteamVR_Utils.RigidTransform(position, rotation);
        var matrix = rigidTransform.ToHmdMatrix34();

        var error = OpenVR.Overlay.SetOverlayTransformAbsolute(overlayHandle, ETrackingUniverseOrigin.TrackingUniverseStanding, ref matrix);
        if (error != EVROverlayError.None)
        {
            throw new Exception("Failed to set overlay position: " + error);
        }
    }

    private void ShowOverlay(ulong handle)
    {
        var error = OpenVR.Overlay.ShowOverlay(overlayHandle);
        if (error != EVROverlayError.None)
        {
            throw new Exception("Failed to show overlay: " + error);
        }

    }
    private void SetOverlayFromFile(ulong overlay, string file)
    {
        var error = OpenVR.Overlay.SetOverlayFromFile(overlay, file);
        if (error != EVROverlayError.None)
        {
            throw new Exception("Failed to draw image file: " + error);
        }
    }

    private void OnDestroy()
    {
        ShutdownOpenVR();
    }

    private void InitOpenVR()
    {
        if (OpenVR.System != null) return;


        var error = EVRInitError.None;
        OpenVR.Init(ref error, EVRApplicationType.VRApplication_Overlay);
        if (error != EVRInitError.None)
        {
            throw new Exception("Failed to initialize OpenVR: " + error);
        }
    }

    private ulong CreateOverlay(string key, string name)
    {

        var handle = OpenVR.k_ulOverlayHandleInvalid;
        var error = OpenVR.Overlay.CreateOverlay(key, name, ref handle);
        if (error != EVROverlayError.None)
        {
            throw new Exception("Failed to create overlay: " + error);
        }
        return handle;

    }

    private void OnApplicationQuit()
    {
        DestroyOverlay(overlayHandle);
    }

    private void DestroyOverlay(ulong handle)
    {
        if (handle != OpenVR.k_ulOverlayHandleInvalid)
        {
            var error = OpenVR.Overlay.DestroyOverlay(handle);
            if(error != EVROverlayError.None)
            {
                throw new Exception("Failed to dispose overlay: " + error);
            }
        }
    }

    private void ShutdownOpenVR()
    {
        if (OpenVR.System != null)
        {
            OpenVR.Shutdown();
        }
    }
}
