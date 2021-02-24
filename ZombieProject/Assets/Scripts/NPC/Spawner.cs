using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class Spawner : MonoBehaviour
{

    [SerializeField] private int spawnCount;
    [Range(1, 100)]
    [SerializeField] private int spawnAreaSize = 1;
    //[SerializeField] private float minionOffset;
    [SerializeField] private GameObject minion;


    private float minionOffset;
    private void Start()
    {
        minionOffset = transform.position.y;

    }

    void OnEnable()
    {
        EventManager.StartListening("Spawn", Spawn);
    }

    void OnDisable()
    {
        EventManager.StopListening("Spawn", Spawn);
    }

    public void SpawnTrigger()
    {
        EventManager.TriggerEvent("Spawn");
    }

    void Spawn()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 spawnPosition = GetSpawnPosition();

            Quaternion spawnRotation = new Quaternion();
            spawnRotation.eulerAngles = new Vector3(0.0f, Random.Range(0.0f, 360.0f));
            if (spawnPosition != Vector3.zero)
            {
                Instantiate(minion, spawnPosition, spawnRotation);
            }
        }
    }

    Vector3 GetSpawnPosition()
    {
        Vector3 spawnPosition = transform.position;
        float startTime = Time.realtimeSinceStartup;
        bool test = false;
        while (test == false)
        {
            Vector2 spawnPositionRaw = Random.insideUnitCircle * spawnAreaSize;
            spawnPosition = new Vector3(spawnPosition.x + spawnPositionRaw.x, minionOffset, spawnPosition.z + spawnPositionRaw.y);
            test = !Physics.CheckSphere(spawnPosition, 0.75f);
            if (Time.realtimeSinceStartup - startTime > 0.5f)
            {
                Debug.Log("Time out placing Minion!");
                return Vector3.zero;
            }
        }
        return spawnPosition;
    }
}
