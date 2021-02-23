using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages what camera is selected
/// </summary>
public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private PlayerStateManager playerState;
    [SerializeField] private GameObject MainCameraR;
    [SerializeField] private GameObject MainCameraL;
    [SerializeField] private GameObject AimCameraR;
    [SerializeField] private GameObject AimCameraL;

    private GameObject MainCamera;
    private GameObject AimCamera;
    public bool rightShoulderCam;

    // Start is called before the first frame update
    void Start()
    {
        rightShoulderCam = true;
        MainCamera = MainCameraR;
        AimCamera = AimCameraR;
        MainCamera.SetActive(true);
        AimCameraR.SetActive(false);
        MainCameraL.SetActive(false);
        AimCameraL.SetActive(false);
    }

    public void EnterAim()
    {
        if (!(playerState.GetState() is PlayerJumping))
        {
            MainCamera.SetActive(false);
            AimCamera.SetActive(true);
        }
    }

    public void LeaveAim()
    {
        AimCamera.SetActive(false);
        MainCamera.SetActive(true);
    }

    public void SwapShoulder()
    {
        //Debug.Log("Swap Shoulder");
        MainCamera.SetActive(false);
        AimCamera.SetActive(false);

        if (rightShoulderCam)
        {
            MainCamera = MainCameraL;
            AimCamera = AimCameraL;
            rightShoulderCam = false;
        }
        else
        {
            MainCamera = MainCameraR;
            AimCamera = AimCameraR;
            rightShoulderCam = true;
        }

        if (playerState.aimHeld)
        {
            AimCamera.SetActive(true);
        }
        else
        {
            MainCamera.SetActive(true);
        }
    }

}
