using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenMainDoor : MonoBehaviour
{
    [SerializeField] private string EventIdentifier;
    [SerializeField] private bool WestDoor;

    void OnEnable()
    {
        EventManager.StartListening(EventIdentifier, OpenDoor);
    }

    void OnDisable()
    {
        EventManager.StopListening(EventIdentifier, OpenDoor);
    }

    private void OpenDoor()
    {
        if (WestDoor)
        {
            LeanTween.moveLocalZ(gameObject, -52f, 40f).setEaseOutQuad();
        }
        else
        {
            LeanTween.moveLocalZ(gameObject, 52f, 40f).setEaseOutQuad();
            AlertAll();
        }
        
    }

    private void AlertAll()
    {
        GameObject[] zombies = GameObject.FindGameObjectsWithTag("Hostile");
        foreach (GameObject zombie in zombies)
        {
            ZombieBehaviour zb = zombie.GetComponent<ZombieBehaviour>();
            zb.Alert();
        }
    }
}
