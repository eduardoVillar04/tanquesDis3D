using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TankCameraController : MonoBehaviour
{
    public List<Camera> cameraList = new List<Camera>();

    public PlayerInput playerInput;

    private int m_CurrentCamIndex = 0;
    private bool m_ChangeCameraPressed = false;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        foreach(Camera cam in cameraList)
        {
            cam.gameObject.SetActive(false);
        }
        cameraList[0].gameObject.SetActive(true);
    }

    private void Update()
    {
        Inputs();
        ChangeCamera();
    }

    private void Inputs()
    {
        m_ChangeCameraPressed = playerInput.actions["ChangeCamera"].WasPressedThisFrame();
    }

    private void ChangeCamera()
    {
        if(m_ChangeCameraPressed)
        {
            cameraList[m_CurrentCamIndex].gameObject.SetActive(false);
            if (m_CurrentCamIndex == cameraList.Count-1)
            {
                m_CurrentCamIndex = 0;
            }
            else
            {
                m_CurrentCamIndex++;
            }
            cameraList[m_CurrentCamIndex].gameObject.SetActive(true);
        }
    }
}
