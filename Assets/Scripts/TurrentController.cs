using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TurrentController : MonoBehaviour
{
    public float turretXRotation;

    public Transform m_Turret;

    public float m_RotationTurret = 10f;
    public float m_RotationCannon = 10f;

    private PlayerInput m_PlayerInput;
    private Vector2 m_RotationInput;

    // Start is called before the first frame update
    void Start()
    {
        m_PlayerInput = GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;
        Inputs();
        RotateTurret(dt);
    }

    private void Inputs()
    {
        m_RotationInput = m_PlayerInput.actions["TurretControl"].ReadValue<Vector2>();
    }

    private void RotateTurret(float dt)
    {
        float rotateHorizontal = m_RotationInput.x;
        float rotateVertical = m_RotationInput.y;

        float rotationY = rotateHorizontal * m_RotationTurret * dt;
        float rotationX = rotateVertical * m_RotationCannon * dt;

        m_Turret.Rotate(rotationX, rotationY, 0);

        Vector3 currentRotation = m_Turret.localEulerAngles;
        if (currentRotation.x > 180f)
        {
            currentRotation.x -= 360f;
        }
        currentRotation.x = Mathf.Clamp(currentRotation.x, -20f, 20f);
        turretXRotation = currentRotation.x;
        m_Turret.localEulerAngles = currentRotation;
    }
}
