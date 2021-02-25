using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerIdentifier : MonoBehaviour
{
    [SerializeField] private string SpawnID;
    //private string desiredString = $"{gameObject.GetInstanceID()}Spawner";

    private void Start()
    {
        if (SpawnID is null)
        {
            throw new UnassignedReferenceException();
        }
    }

    public string GetSpawnID()
    {
        return SpawnID;
    }

}
