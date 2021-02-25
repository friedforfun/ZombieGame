using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTrigger : MonoBehaviour
{
    [SerializeField] protected string SpawnIdentifier = "Spawn";

    private LayerMask playerLayer = 9;
    private bool hasFired = false;

    private void Start()
    {
        SpawnIdentifier = GetComponentInParent<SpawnerIdentifier>().GetSpawnID();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == playerLayer)
        {
            if (!hasFired)
            {
                EventManager.TriggerEvent(SpawnIdentifier);
            }

            hasFired = true;
        }
        
    }
}
