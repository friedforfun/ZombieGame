using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggeredSpawner : Spawner
{

    void Start()
    {
        SpawnIdentifier = GetComponentInParent<SpawnerIdentifier>().GetSpawnID();
        StartListening();
    }

}
