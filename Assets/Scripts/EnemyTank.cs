using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTank : MonoBehaviour
{
    public Transform m_Patrol;
    public Transform[] m_AllTargetPoints;
    public int m_CurrentTargetPointIndex = 0;
    public Transform m_CurrentTargetPoint;

    public float m_Speed = 5.0f;
    public float m_RotationSpeed = 5.0f;

    public int lifeHitPoints = 1;
    public GameObject explossionEffect;

    // Start is called before the first frame update
    void Start()
    {
        m_AllTargetPoints = new Transform[m_Patrol.childCount];
        for(int i = 0; i < m_AllTargetPoints.Length; i++)
        {
            m_AllTargetPoints[i] = m_Patrol.GetChild(i);
        }
        m_CurrentTargetPoint = m_AllTargetPoints[m_CurrentTargetPointIndex];
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;
        Move(dt);
        Rotate(dt);
    }

    private void Move(float dt)
    {
        float step = m_Speed * dt;
        Vector3 targetPointAtOurHeight = m_CurrentTargetPoint.position;
        targetPointAtOurHeight.y = transform.position.y;
        transform.position = Vector3.MoveTowards(transform.position, targetPointAtOurHeight, step);

        if (Vector3.Distance(transform.position,targetPointAtOurHeight) < 0.1f)
        {
            m_CurrentTargetPointIndex++;
            if(m_CurrentTargetPointIndex >= m_AllTargetPoints.Length)
            {
                m_CurrentTargetPointIndex = 0;
            }
            m_CurrentTargetPoint = m_AllTargetPoints[m_CurrentTargetPointIndex];
        }
    }

    private void Rotate(float dt)
    {
        Vector3 targetPointAtOurHeight = m_CurrentTargetPoint.position;
        targetPointAtOurHeight.y = transform.position.y;

        var targetRotation = Quaternion.LookRotation(targetPointAtOurHeight - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,m_RotationSpeed * dt);
    }

    public void TakeDamage(int howMuchDamage)
    {
        lifeHitPoints -= howMuchDamage;
        if(lifeHitPoints<=0)
        {
            GameObject.Instantiate(explossionEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

}
