using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellScript : MonoBehaviour
{
    public float lifeTime = 10f;

    public ShellPoolManager m_ShellPoolManager;

    public GameObject m_ExplossionEffect;
    public GameObject m_ExplossionEffectPrefab;
    private ParticleSystem m_ExplossionParticleSystem;

    private void Awake()
    {
        m_ShellPoolManager = FindObjectOfType<ShellPoolManager>();
        m_ExplossionEffect = GameObject.Instantiate(m_ExplossionEffectPrefab);
        m_ExplossionParticleSystem = m_ExplossionEffect.GetComponent<ParticleSystem>();
    }


    // Update is called once per frame
    void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            m_ShellPoolManager.ReturnShell(gameObject);
        }
    }

    private void OnEnable()
    {
        lifeTime = 10f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<EnemyTank>())
        {
            collision.gameObject.GetComponent<EnemyTank>().TakeDamage(10);
        }

        m_ExplossionEffect.transform.position = transform.position;
        m_ExplossionParticleSystem.Play();
        m_ShellPoolManager.ReturnShell(gameObject);
    }
}
