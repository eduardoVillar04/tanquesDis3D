using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TurretShoot : MonoBehaviour
{
    public Transform cannonTransform;
    public GameObject bulletGameObject;

    private bool m_CanShoot = true;
    private bool m_ShootPressed = false;
    private float m_ShootCooldown = 0.5f;
    private float m_ShootCooldownTimer = 0.0f;

    private PlayerInput playerInput;

    private ShellPoolManager shellPoolManager;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        shellPoolManager = FindObjectOfType<ShellPoolManager>();
    }


    // Update is called once per frame
    void Update()
    {
        Input();
        CooldownController();
        Shoot();
    }

    private void Input()
    {
        m_ShootPressed = playerInput.actions["Shoot"].WasPressedThisFrame();
    }

    private void CooldownController()
    {
        if(m_ShootCooldownTimer > 0)
        {
            if(m_CanShoot)
            {
                m_CanShoot = false;
            }
            m_ShootCooldownTimer -= Time.deltaTime;
        }
        if(m_ShootCooldownTimer <=0)
        {
            m_CanShoot = true;
        }
    }

    private void Shoot()
    {
        if(m_CanShoot && m_ShootPressed)
        {
            m_ShootCooldownTimer = m_ShootCooldown;
            SpawnShell();
        }
    }

    private void SpawnShell()
    {
        GameObject shell = shellPoolManager.TakeShell();

        shell.transform.position = cannonTransform.position;
        shell.transform.rotation = cannonTransform.rotation;

        Rigidbody shellRB = shell.GetComponent<Rigidbody>();
        shellRB.velocity = Vector3.zero;
        shellRB.angularVelocity = Vector3.zero;

        shell.SetActive(true);

        shell.GetComponent<Rigidbody>().AddForce(shell.transform.forward * 1000);
    }
}
