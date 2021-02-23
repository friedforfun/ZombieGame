using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles Movement of projectile, collisions, and damage
/// </summary>
public class ProjectileBehaviour : MonoBehaviour
{
    [SerializeField] private int projectileDamage;
    [SerializeField] private ParticleSystem ImpactEffect;
    [SerializeField] protected float projectileForce;
    [SerializeField] private Rigidbody bulletRigidBody;
    [SerializeField] protected float fullDamageRange;// range at which full projectile damage is applied to target
    [SerializeField] protected float projectileDamageRange; // point at which projectile damage is 0

    private bool hasImpacted = false;
    private float startTime;
    private Vector3 startPosition;
    private int trueDamage;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
        startPosition = transform.position;
        trueDamage = projectileDamage;
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasImpacted)
        {
            float now = Time.time;
            if (now - startTime > 5f)
            {
                hasImpacted = true;
                StartCoroutine("destroySelf");
            }
        }

        float travelDist = Vector3.Distance(startPosition, transform.position);

        if (travelDist > fullDamageRange)
        {
            //Debug.Log("")
            trueDamage = (int)Mathf.Lerp(projectileDamage, 0, (travelDist - fullDamageRange) / projectileDamageRange);
        }
    }

    public void Fire(Vector3 targetDirection)
    {
        bulletRigidBody.AddForce(targetDirection * projectileForce, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        IDamagable<int> damageTest = collision.gameObject.GetComponent<IDamagable<int>>();
        if (damageTest != null)
        {
            damageTest.Damage(trueDamage);
            //Debug.Log($"Damage out: {trueDamage}");
            projectileDamage = projectileDamage / 2;
            trueDamage = projectileDamage;
        }
        ContactPoint hit = collision.GetContact(0);
        Instantiate<ParticleSystem>(ImpactEffect, hit.point, Quaternion.LookRotation(hit.normal));

        if (!hasImpacted)
        {
            hasImpacted = true;
            StartCoroutine(destroySelf());
        }
    }

    private IEnumerator destroySelf()
    {
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }

}
