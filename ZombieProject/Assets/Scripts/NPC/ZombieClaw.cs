using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieClaw : MonoBehaviour
{
    [SerializeField] private ZombieBehaviour zombie;
    private LayerMask playerLayer = 9;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == playerLayer)
        {
            //Debug.Log("Hit player");
            zombie.ApplyDamage(other.gameObject);
        }
    }
}
