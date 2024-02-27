using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyTankImproved : MonoBehaviour
{
    [Header("Patrol Components")]
    public Transform m_Patrol;
    public Transform[] m_AllTargetPoints;
    public int m_CurrentTargetPointIndex = 0;
    public Transform m_CurrentTargetPoint;

    public Transform m_PlayerTransform;

    public enum EnemyTankSates
    {
        NONE = -1,
        STANDBY,
        SELECT_AND_GO_TO_POINT,
        CHECK_ARRIVAL,
        PURSUIT,
        FIRING
    }

    public EnemyTankSates m_CurrentState = EnemyTankSates.STANDBY;

    //Navmesh variables
    public NavMeshAgent m_NavMeshAgent;

    //Standby variables
    public float m_StandbyTime = 2.0f;
    public float m_RemainingStandbyTime = 0.0f;

    //Firing variables
    public Transform m_Turret;
    public Transform m_BulletSpawnPoint;
    public float m_FireRate = 3f;
    public float m_RemainingFireRate = 0f;
    public float m_TurretRotationSpeed = 20.0f;
    public Transform m_WhereToAim;
    
    private ShellPoolManager m_ShellPoolManager;

    // Start is called before the first frame update
    void Start()
    {
        m_ShellPoolManager = FindObjectOfType<ShellPoolManager>();
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
        m_PlayerTransform = FindObjectOfType<TankMovement>().transform;

        m_AllTargetPoints = new Transform[m_Patrol.childCount];
        for (int i = 0; i < m_AllTargetPoints.Length; i++)
        {
            m_AllTargetPoints[i] = m_Patrol.GetChild(i);
        }
        m_CurrentTargetPoint = m_AllTargetPoints[m_CurrentTargetPointIndex];

        m_RemainingStandbyTime = m_StandbyTime;
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;

        //Pursuit();

        switch (m_CurrentState)
        {
            case EnemyTankSates.STANDBY:
                ReturnTurretToNormal(dt);
                StandbyBehaviour(dt);
                break;
            case EnemyTankSates.SELECT_AND_GO_TO_POINT:
                ReturnTurretToNormal(dt);
                SelectAndGoToPoint();
                break;
            case EnemyTankSates.CHECK_ARRIVAL:
                CheckArrival();
                break;
            case EnemyTankSates.PURSUIT:
                Pursuit(dt);
                break;
            case EnemyTankSates.FIRING:
                Firing(dt);
                break;

            default:
                break;
        }
    }

    private void StandbyBehaviour(float dt)
    {
        if(m_RemainingStandbyTime > 0)
        {
            m_RemainingStandbyTime -= dt;
        }
        else
        {
            m_RemainingStandbyTime = m_StandbyTime;
            m_CurrentState = EnemyTankSates.SELECT_AND_GO_TO_POINT;
        }
        //Si se hace con otro if entonces se hara en el mismo frame
    }

    private void SelectAndGoToPoint()
    {
        m_CurrentTargetPointIndex++;
        if(m_CurrentTargetPointIndex >= m_AllTargetPoints.Length)
        {
            m_CurrentTargetPointIndex = 0;
        }
        m_CurrentTargetPoint = m_AllTargetPoints[m_CurrentTargetPointIndex];
        
        m_NavMeshAgent.SetDestination(m_CurrentTargetPoint.position);
        m_NavMeshAgent.isStopped = false;
        
        m_CurrentState = EnemyTankSates.CHECK_ARRIVAL;
    }

    private void CheckArrival()
    {
        if(m_NavMeshAgent.path != null)
        {
            if(m_NavMeshAgent.remainingDistance < 0.1f)
            {
                m_CurrentState = EnemyTankSates.STANDBY;
            }
        }
    }
    
    private void Pursuit(float dt)
    {
        TurretLooksAtPlayer(dt);
        m_NavMeshAgent.SetDestination(m_PlayerTransform.position);

        if(m_NavMeshAgent.path != null)
        {
            if(m_NavMeshAgent.remainingDistance < 5.0f)
            {
                m_NavMeshAgent.isStopped = true;
                m_CurrentState = EnemyTankSates.FIRING;
            }
            else
            {
                m_NavMeshAgent.isStopped = false;
                m_NavMeshAgent.SetDestination(m_PlayerTransform.position);
            }
        }
        else
        {
            m_NavMeshAgent.SetDestination(m_PlayerTransform.position);
            m_NavMeshAgent.isStopped = false;
        }

    }

    private void Firing(float dt)
    {
        TurretLooksAtPlayer(dt);

        if(m_RemainingFireRate <=0)
        {
            m_RemainingFireRate = m_FireRate;
            SpawnShell();
            m_CurrentState = EnemyTankSates.PURSUIT;
        }
        else
        {
            m_RemainingFireRate -= dt;
        }

    }

    private void TurretLooksAtPlayer(float dt)
    {
        //Look at player
        Vector3 lookPos = m_WhereToAim.position - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        m_Turret.rotation = Quaternion.Slerp(m_Turret.rotation, rotation, dt * m_TurretRotationSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            m_NavMeshAgent.SetDestination(m_PlayerTransform.position);
            m_NavMeshAgent.isStopped = false;
            m_CurrentState = EnemyTankSates.PURSUIT;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            m_NavMeshAgent.isStopped = true;
            m_CurrentState = EnemyTankSates.STANDBY;
        }
    }

    private void ReturnTurretToNormal(float dt)
    {
        if(m_Turret.localEulerAngles != Vector3.zero)
        {
            m_Turret.localEulerAngles = Vector3.Slerp(m_Turret.localEulerAngles, Vector3.zero, dt * m_TurretRotationSpeed);
        }
    }

    private void SpawnShell()
    {
        GameObject shell = m_ShellPoolManager.TakeShell();

        shell.transform.position = m_BulletSpawnPoint.position;
        shell.transform.rotation = m_BulletSpawnPoint.rotation;

        Rigidbody shellRB = shell.GetComponent<Rigidbody>();
        shellRB.velocity = Vector3.zero;
        shellRB.angularVelocity = Vector3.zero;

        shell.SetActive(true);

        shell.GetComponent<Rigidbody>().AddForce(shell.transform.forward * 1000);
    }

}
